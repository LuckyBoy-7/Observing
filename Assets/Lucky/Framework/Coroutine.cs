using System;
using System.Collections;
using System.Collections.Generic;
using Lucky.Utilities;
using UnityEngine;

namespace Lucky.Framework
{
    public class Coroutine : LuckyCompoennt
    {
        public bool RemoveOnComplete = true;
        public bool IsRealtime;
        private Stack<IEnumerator> enumerators;
        private float waitTimer;
        private bool ended;
        public bool Finished { get; private set; }
        public Action OnFinish;

        public IEnumerator Current
        {
            get
            {
                if (enumerators.Count <= 0)
                {
                    return null;
                }

                return enumerators.Peek();
            }
        }

        public Coroutine(IEnumerator functionCall, bool removeOnComplete = true)
        {
            enumerators = new();
            enumerators.Push(functionCall);
            RemoveOnComplete = removeOnComplete;
        }

        public Coroutine(bool removeOnComplete = true)
        {
            RemoveOnComplete = removeOnComplete;
            enumerators = new();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            while (true)
            {
                orig_FixedUpdate();
                IEnumerator enumerator = Current;
                // IEnumerator还可以返回一个Action类型的函数
                if (enumerator?.Current is Action<Coroutine> action)
                    action(this);
                else
                {
                    if (Current is SwapImmediately swapImmediately)
                    {
                        // 因为原来要等一帧再pop的
                        enumerators.Pop();
                        enumerators.Push(swapImmediately.Inner);
                    }
                    else if (Current?.Current is not SwapImmediately)
                        break;
                }
            }
        }

        private void orig_FixedUpdate()
        {
            ended = false;
            if (waitTimer > 0f)
            {
                waitTimer -= Timer.FixedDeltaTime(IsRealtime);
                return;
            }

            if (enumerators.Count > 0)
            {
                IEnumerator enumerator = enumerators.Peek();
                if (enumerator.MoveNext() && !ended)
                {
                    // 如果是wait时间, 那么就是等那么久, 如果都不是, 那就自然地浪费了一个生命周期, 相当于yielf return null
                    if (enumerator.Current is int)
                    {
                        waitTimer = (int)enumerator.Current;
                    }
                    else if (enumerator.Current is float)
                    {
                        waitTimer = (float)enumerator.Current;
                        return;
                    }

                    if (enumerator.Current is IEnumerator)
                    {
                        enumerators.Push(enumerator.Current as IEnumerator);
                    }
                }
                else if (!ended)
                {
                    enumerators.Pop();
                    if (enumerators.Count == 0)
                    {
                        Finished = true;
                        Active = false;
                        if (RemoveOnComplete)
                        {
                            base.RemoveSelf();
                        }
                        OnFinish?.Invoke();
                    }
                }
            }
        }

        public void Cancel()
        {
            Active = false;
            Finished = true;
            waitTimer = 0f;
            enumerators.Clear();
            ended = true;
        }

        public void Replace(IEnumerator functionCall)
        {
            Active = true;
            Finished = false;
            waitTimer = 0f;
            enumerators.Clear();
            enumerators.Push(functionCall);
            ended = true;
        }

    }
}