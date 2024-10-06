using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Lucky.Extensions;
using Lucky.Framework;
using Lucky.Inputs;
using Lucky.Utilities;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Input = Lucky.Inputs.Input;
using Timer = Lucky.Utilities.Timer;

namespace Lucky.Framework.Items
{
    /// <summary>
    /// menu anchor pivot都在中心, 子项anchor pivot都在左上角
    /// </summary>
    public class TextMenu : ManagedUIBehaviour
    {
        public bool Focused = true;

        /// <summary>
        /// 菜单内部的所有项
        /// </summary>
        private List<Item> items = new List<Item>();

        /// <summary>
        /// 当前指向的项的id, 只可指向可hover的项, 例如像一般的text就不可hover
        /// </summary>
        public int SelectionIdx = -1;

        /// <summary>
        /// 子项对于自身的偏移调整[-1, 1], 比如正常情况子项是渲染到menu右下角的, 如果justify设为[-0.5, 0.5], 就可以把子项渲染到正中心 
        /// </summary>
        public Vector2 Justify;

        /// <summary>
        /// 两项之间的间距, 上面的下边到下面的上边的距离
        /// </summary>
        public float ItemSpacing = 4f;

        /// <summary>
        /// Menu的宽至少要这么多
        /// </summary>
        public float MinWidth;

        public Color HighlightColor = Color.white;
        public Color DisabledColor = Color.gray;

        private static readonly Color HighlightColorA = ColorUtils.HexToColor("84FF54");

        private static readonly Color HighlightColorB = ColorUtils.HexToColor("FCFF59");

        private Image overlayImage;
        public bool ShowOverlay { get; set; } = false;

        public Action OnESC;
        public Action OnCancel;
        public Action OnUpdate;
        public Action OnPause;
        public Action OnClose;

        public bool AutoScroll = true;

        public Item Current
        {
            get => SelectionIdx == -1 ? null : items[SelectionIdx];
            set => SelectionIdx = items.IndexOf(value);
        }

        /// <summary>
        /// Menu的宽
        /// </summary>
        public float Width { get; private set; }

        /// <summary>
        /// Menu的高
        /// </summary>
        public float Height { get; private set; }

        /// <summary>
        /// 左边部分项的宽(最大的)
        /// </summary>
        public float LeftColumnWidth { get; private set; }

        /// <summary>
        /// 右边部分项的宽(最大的)
        /// </summary>
        public float RightColumnWidth { get; private set; }

        /// <summary>
        /// menu高度至少要这么多才会scroll
        /// </summary>
        public float ScrollableMinSize => Screen.height - 300;


        /// <summary>
        /// 第一个可hover的项, 没有就返回0(但理论上不会没有)
        /// </summary>
        public int FirstPossibleSelection
        {
            get
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i] != null && items[i].Hoverable)
                    {
                        return i;
                    }
                }

                return 0;
            }
        }

        /// <summary>
        /// 最后一个可hover的项, 没有就返回0(但理论上不会没有)
        /// </summary>
        public int LastPossibleSelection
        {
            get
            {
                for (int i = items.Count - 1; i >= 0; i--)
                {
                    if (items[i] != null && items[i].Hoverable)
                    {
                        return i;
                    }
                }

                return 0;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Tag = Tags.PauseUpdate;
            Justify = new Vector2(-0.5f, 0.5f);
            UpdatePositions();

            overlayImage = gameObject.AddComponent<Image>();
            overlayImage.sprite = Resources.Load<Sprite>("Primatives/Square");
            RectTransform.sizeDelta = Settings.ScreenSize;
            overlayImage.color = new Color(0, 0, 0, 0.8f);
            overlayImage.enabled = false;
        }

        public void ResetPositionImmediately()
        {
            RecalculateSize();
            if (AutoScroll && Height > ScrollableMinSize)
            {
                RectTransform.SetAnchoredPositionY(ScrollTargetY);
            }
        }

        /// <summary>
        /// 添加一个子项并初始化, 重新计算画布等
        /// </summary>
        public TextMenu Add(Item item)
        {
            items.Add(item);

            if (SelectionIdx == -1)
            {
                MoveSelection(1);
            }

            UpdatePositions();
            return this;
        }

        /// <summary>
        /// 清空item
        /// </summary>
        public void Clear() => items = new List<Item>();

        /// <summary>
        /// 找到item对应的索引
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(Item item) => items.IndexOf(item);

        /// <summary>
        /// 将当前SelectionIdx向某个方向移动直到碰到一个hoverable
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="wiggle"></param>
        public void MoveSelection(int direction, bool wiggle = false)
        {
            int idxBackup = SelectionIdx;
            direction = Math.Sign(direction);
            bool found = false;
            for (int i = 0; i < items.Count; i++)
            {
                SelectionIdx = MathUtils.Mod(SelectionIdx + direction, items.Count);
                if (Current.Hoverable)
                {
                    if (SelectionIdx == idxBackup)
                        return;
                    found = true;
                    break;
                }
            }

            // 没初始化的时候for完了可能都找不到
            if (!found)
            {
                SelectionIdx = idxBackup;
                return;
            }

            if (idxBackup != -1)
            {
                items[idxBackup].OnExit();
                Current.OnEnter(); // 如果还没初始化那么Current最好也别调用, 不然就会出现刚切换菜单就有动画
            }

            if (wiggle)
            {
                Current.SelectWiggler?.Start();
            }
        }

        /// <summary>
        /// 重新计算Menu的宽高
        /// </summary>
        public void RecalculateSize()
        {
            LeftColumnWidth = RightColumnWidth = Height = 0f;
            // 计算左边最大宽度
            foreach (Item item in items)
            {
                if (item.IncludeWidthInMeasurement)
                    LeftColumnWidth = Math.Max(LeftColumnWidth, item.LeftWidth());
            }

            // 计算右边最大宽度
            foreach (Item item2 in items)
            {
                if (item2.IncludeWidthInMeasurement)
                    RightColumnWidth = Math.Max(RightColumnWidth, item2.RightWidth());
            }

            // 计算menu高度
            foreach (Item item3 in items)
            {
                if (item3.Visible)
                    Height += item3.Height() + ItemSpacing;
            }

            // 植树问题, 最后一个ItemSpacing会多算一次
            Height -= ItemSpacing;
            // Menu的宽
            Width = Math.Max(MinWidth, LeftColumnWidth + RightColumnWidth);
        }

        /// <summary>
        /// 获取从第一项topleft到目标项midleft之间的距离(y方向)
        /// </summary>
        /// <param name="targetItem"></param>
        /// <returns></returns>
        public float GetYOffsetOf(Item targetItem)
        {
            if (targetItem == null)
                return 0f;

            float dist = 0f;
            foreach (Item item in items)
            {
                if (targetItem.Visible)
                    dist += item.Height() + ItemSpacing;
                if (item == targetItem)
                    break;
            }

            return dist - targetItem.Height() * 0.5f - ItemSpacing;
        }

        /// <summary>
        /// 关闭面板
        /// </summary>
        public void Close()
        {
            Current.OnExit();
            OnClose?.Invoke();
            Destroy(this);
        }

        /// <summary>
        /// 运行一个携程后关闭面板, 一般用于IO的时候
        /// </summary>
        /// <param name="routine"></param>
        /// <param name="onClose"></param>
        public void RunCoroutineThenClose(IEnumerator routine, Action onClose)
        {
            Focused = false;
            Visible = false;
            StartCoroutine(orig_RunCoroutineThenClose(routine, onClose));
        }

        private IEnumerator orig_RunCoroutineThenClose(IEnumerator routine, Action onClose)
        {
            yield return routine;
            onClose?.Invoke();
            Close();
        }

        protected override void ManagedUpdate()
        {
            OnUpdate?.Invoke();

            overlayImage.enabled = ShowOverlay;

            // 当有焦点时
            if (Focused)
            {
                // 上下移动
                if (Input.MenuDown.Pressed)
                {
                    // 按住下会焦点会一直向下, 除非此时是重复输入且到了最后一项, 因为防止玩家在Setting界面不小心错过最后一项(虽然再按上就行了)
                    if (!Input.MenuDown.Repeating || SelectionIdx != LastPossibleSelection)
                    {
                        MoveSelection(1, true);
                    }
                }
                else if (Input.MenuUp.Pressed && (!Input.MenuUp.Repeating || SelectionIdx != FirstPossibleSelection))
                {
                    MoveSelection(-1, true);
                }

                // 左右选择
                if (Input.MenuLeft.Pressed)
                {
                    Current.OnLeftPressed();
                }

                if (Input.MenuRight.Pressed)
                {
                    Current.OnRightPressed();
                }

                // 确认
                if (Input.MenuConfirm.Pressed)
                {
                    Current.OnConfirmPressed();
                }

                // Journal
                if (Input.MenuJournal.Pressed)
                {
                    Current.OnAltPressed();
                }

                if (!Input.MenuConfirm.Pressed)
                {
                    // 取消
                    if (Input.MenuCancel.Pressed)
                    {
                        OnCancel?.Invoke();
                    }
                    else if (Input.Esc.Pressed && OnESC != null)
                    {
                        Input.Esc.ConsumeBuffer();
                        OnESC?.Invoke();
                    }
                    else if (Input.Pause.Pressed && OnPause != null)
                    {
                        Input.Pause.ConsumeBuffer();
                        OnPause?.Invoke();
                    }
                }
            }

            // 光敏模式, 开启后只显示淡绿, 否则绿黄交替闪烁
            if (Settings.DisableFlashes)
            {
                HighlightColor = HighlightColorA;
            }
            else if (Timer.OnInterval(0.1f, true))
            {
                HighlightColor = HighlightColor == HighlightColorA ? HighlightColorB : HighlightColorA;
            }

            if (AutoScroll)
            {
                if (Height > ScrollableMinSize)
                {
                    // lerp
                    RectTransform.AddAnchoredPositionY(
                        (ScrollTargetY - RectTransform.anchoredPosition.y) * (1f - (float)Math.Pow(0.01f, Timer.DeltaTime(true)))
                    );
                    return;
                }

                RectTransform.anchoredPosition = Vector3.zero;
            }
        }

        public float ScrollTargetY
        {
            get
            {
                float min = (float)Screen.height / 2 - 150 - Height * Justify.y;
                float max = 150f + Height * Justify.y - (float)Screen.height / 2;
                return MathUtils.Clamp(GetYOffsetOf(Current) - Height * Justify.y, min, max);
            }
        }

        protected override void ManagedFixedUpdate()
        {
            UpdatePositions();
            Current?.OnStay();
        }

        private void UpdatePositions()
        {
            RecalculateSize();
            // 第一项开始位置, 锚点在左上角
            Vector2 curItemPos = Justify * new Vector2(Width, Height);
            foreach (Item item in items)
            {
                if (item.Visible)
                {
                    float itemHeight = item.Height();
                    item.UpdatePosition(curItemPos);
                    curItemPos.y -= itemHeight + ItemSpacing;
                }
            }
        }
    }
}