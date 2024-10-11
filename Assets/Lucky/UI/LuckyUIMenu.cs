using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lucky.Framework.Extensions;
using Lucky.Framework;
using Lucky.Framework.Utilities;
using UnityEngine;
using Object = System.Object;
using Input = Lucky.Framework.Inputs.Input;

namespace Lucky.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class LuckyUIMenu : ManagedBehaviour
    {
        /// <summary>
        /// 两个可交互条目之间的间隔
        /// </summary>
        public float itemSpace = 72;

        /// <summary>
        /// 两个块之间的间隔, 块以Text顶部为界限分割, 也就是发现Text就把它当最这个块的头
        /// </summary>
        public float blockSpace = 82;

        /// <summary>
        /// 菜单的宽度, 决定了子项的左右边界
        /// </summary>
        public static float width = 1450;

        public List<LuckyUI> items = new();
        public List<LuckyUIInteract> interactableItems = new();

        /// <summary>
        /// 当前指向的可交互条目的索引
        /// </summary>
        public int idx = -1;

        public LuckyUIMenu openedFromMenu = null;

        private RectTransform RectTransform;

        /// 更新焦点后自身的anchoredPosition应该在哪儿
        private float targetPosY;

        private float lerpK = 0.05f;

        // 整个menu顶部到窗口顶部的最大距离, 这决定了窗口的移动方式
        private float maxPaddingY = 200;

        /// 为Menu添加一个条目
        public void AddItem(LuckyUI item)
        {
            items.Add(item);
            if (item is LuckyUIInteract interact)
                interactableItems.Add(interact);
        }


        protected void Awake()
        {
            // 默认关闭
            transform.gameObject.SetActive(false);
            RectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            if (items.All(item => item is LuckyUIText))
                throw new Exception("你这菜单只能看是什么是意思?");
            ChangeIdx(0);
        }

        protected override void ManagedFixedUpdate()
        {
            base.ManagedFixedUpdate();
            if (Input.MenuCancel.Pressed && openedFromMenu)
                ReturnLastMenu();

            if (Input.MenuConfirm.Pressed && this[idx] is LuckyUIButton button)
            {
                button.callback?.Invoke();
            }
            else if ((Input.MenuLeft.Pressed || Input.MenuRight.Pressed) && this[idx] is LuckyUISelect select)
            {
                select.OnChoose(Input.MenuLeft.Pressed ? -1 : 1);
            }

            if (Input.MenuDown.Pressed)
                MoveDown();
            else if (Input.MenuUp.Pressed)
                MoveUp();


            RectTransform.anchoredPosition = RectTransform.anchoredPosition.WithY(MathUtils.Lerp(RectTransform.anchoredPosition.y, targetPosY, lerpK));
        }

        private void UpdateFocus()
        {
            // 因为默认锚点都在中间, 然后我们一开始把menu放在屏幕正中心
            Vector2 pivotPos = new Vector2(-width / 2, 0);
            float dist = 0;
            // 整个菜单的高, 由于可能有的项可以展开, 所以每次都得重新算
            float menuHeight = GetDistTo(-1);
            bool isFixed = menuHeight + 2 * maxPaddingY < Screen.height; // 说明项很少, 无需移动, 直接按中心固定即可
            if (isFixed)
            {
                targetPosY = menuHeight / 2;
                orig_UpdateFocus(pivotPos);
                return;
            }

            // 如果走到这, 则说明项够多, 但仍需判断若把当前项放到中间后, 上下会不会太空
            dist = GetDistTo(idx);
            bool isEmptyAbove = dist + maxPaddingY / 2 < (float)Screen.height / 2;
            bool isEmptyBeneath = menuHeight - dist + maxPaddingY / 2 < (float)Screen.height / 2;
            if (isEmptyAbove || isEmptyBeneath) // 说明太空了 
            {
                if (isEmptyAbove)
                    targetPosY = (float)Screen.height / 2 - maxPaddingY;
                else if (isEmptyBeneath)
                    targetPosY = menuHeight - ((float)Screen.height / 2 - maxPaddingY);

                orig_UpdateFocus(pivotPos);
                return;
            }

            // 说明不是很空, 可以直接将当前项放到屏幕中心
            targetPosY = dist;
            orig_UpdateFocus(pivotPos);
        }

        private void orig_UpdateFocus(Vector2 pivotPos)
        {
            LuckyUI preItem = null;
            float dist;
            // 把每一项放好位置(感觉以后做了展开之类的功能这里才有用, 不然更新一次就行了)
            dist = 0;
            foreach (var item in items)
            {
                if (preItem != null)
                {
                    if (item is LuckyUIText)
                        dist += blockSpace;
                    else if (item is LuckyUIInteract)
                        dist += itemSpace;
                }

                item.GetComponent<RectTransform>().anchoredPosition = pivotPos.WithY(-dist);
                preItem = item;
            }
        }

        private float GetDistTo(int i)
        {
            if (i < 0)
                i += interactableItems.Count;
            // 因为默认锚点都在中间, 然后我们一开始把menu放在屏幕正中心
            LuckyUI preItem = null;
            LuckyUI targetItem = this[i];
            // 拿到从开头到dist的距离
            float dist = 0;
            foreach (var item in items)
            {
                if (preItem != null)
                {
                    if (item is LuckyUIText)
                        dist += blockSpace;
                    else if (item is LuckyUIInteract)
                        dist += itemSpace;
                }

                if (ReferenceEquals(item, targetItem))
                    break;
                preItem = item;
            }

            return dist;
        }

        private void MoveUp() => ChangeIdx((idx - 1 + interactableItems.Count) % interactableItems.Count);
        private void MoveDown() => ChangeIdx((idx + 1 + interactableItems.Count) % interactableItems.Count);

        /// <summary>
        /// 改变idx到对应位置, 并调用相关函数, 并重新聚焦
        /// </summary>
        /// <param name="to"></param>
        private void ChangeIdx(int to)
        {
            if (idx != -1) // 还没初始化
                this[idx].OnExit();
            idx = to;
            UpdateFocus();
            this[idx].OnEnter();
        }

        public LuckyUIInteract this[int i] => interactableItems[i];

        #region 常用函数部分

        public void OpenMenu(LuckyUIMenu menu)
        {
            gameObject.SetActive(false);
            menu.gameObject.SetActive(true);
            menu.openedFromMenu = this;
        }

        public void ReturnLastMenu()
        {
            gameObject.SetActive(false);
            openedFromMenu.gameObject.SetActive(true);
            openedFromMenu = null;
        }

        public void Quit()
        {
            Application.Quit();
        }

        #endregion

    }
}