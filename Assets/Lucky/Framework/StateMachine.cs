using System;
using System.Collections;
using Lucky.Utilities;
using UnityEngine;

namespace Lucky.Framework
{
    public class StateMachine : LuckyCompoennt
    {
        private int state;
        private Action[] begins;
        private Func<int>[] updates;
        private Action[] ends;
        private Func<IEnumerator>[] coroutines;
        private Coroutine currentCoroutine;

        public bool ChangedStates;
        public bool Log;
        public int PreviousState { get; private set; }

        public StateMachine(int maxStates = 10)
        {
            PreviousState = (state = -1);
            begins = new Action[maxStates];
            updates = new Func<int>[maxStates];
            ends = new Action[maxStates];
            coroutines = new Func<IEnumerator>[maxStates];
            currentCoroutine = new Coroutine();
            currentCoroutine.RemoveOnComplete = false;
        }
        
        public override void Added()
        {
            if (state == -1)
            {
                State = 0;
            }
        }

        public int State
        {
            get => state;
            set
            {
                if (state != value)
                {
                    if (Log)
                    {
                        Debug.Log(string.Concat("Enter State ", value, " (leaving ", state, ")"));
                    }

                    ChangedStates = true;
                    PreviousState = state;
                    state = value;
                    if (PreviousState != -1 && ends[PreviousState] != null)
                    {
                        if (Log)
                        {
                            Debug.Log("Calling End " + PreviousState);
                        }

                        ends[PreviousState]();
                    }

                    if (begins[state] != null)
                    {
                        if (Log)
                        {
                            Debug.Log("Calling Begin " + state);
                        }

                        begins[state]();
                    }

                    if (coroutines[state] != null)
                    {
                        if (Log)
                        {
                            Debug.Log("Starting Coroutine " + state);
                        }

                        currentCoroutine.Replace(coroutines[state]());
                        return;
                    }

                    currentCoroutine.Cancel();
                }
            }
        }


        public void SetCallbacks(int state, Func<int> onUpdate, Func<IEnumerator> coroutine = null, Action begin = null, Action end = null)
        {
            updates[state] = onUpdate;
            begins[state] = begin;
            ends[state] = end;
            coroutines[state] = coroutine;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            ChangedStates = false;
            if (updates[state] != null)
            {
                State = updates[state]();
            }

            if (currentCoroutine.Active)
            {
                currentCoroutine.FixedUpdate();
                if (!ChangedStates && Log && currentCoroutine.Finished)
                {
                    Debug.Log("Finished Coroutine " + state);
                }
            }
        }

        public static implicit operator int(StateMachine s)
        {
            return s.state;
        }
    }
}