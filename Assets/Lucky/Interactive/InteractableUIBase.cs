using UnityEngine;

namespace Lucky.Interactive
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class InteractableUIBase : InteractableBase
    {
        private RectTransform rectTransform;

        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                    rectTransform = GetComponent<RectTransform>();
                return rectTransform;
            }
        }

        protected abstract override SortingLayerType sortingLayerType { get; }

        protected abstract override long SortingOrder { get; }

        public abstract override Vector2 BoundsCheckPos { get; }

        public override bool IsPositionInBounds(Vector2 pos, RectTransform trans = null)
        {
            if (trans == null)
                trans = RectTransform;
            float scaleX = 1, scaleY = 1;
            Transform cur = transform;
            while (cur)
            {
                scaleX *= cur.localScale.x;
                scaleY *= cur.localScale.y;
                cur = cur.parent;
            }

            float width = trans.rect.width * scaleX;
            float height = trans.rect.height * scaleY;
            Vector2 pivot = trans.pivot;
            float x = trans.position.x - pivot.x * width;
            float y = trans.position.y - pivot.y * height;
            if (pos.x <= x + width
                && pos.x >= x
                && pos.y <= y + height
                && pos.y >= y)
                return true;
            return false;
        }


        protected virtual void Start()
        {
            GameCursor.Instance?.RegisterInteractableUI(this);
        }

        protected virtual void OnEnable()
        {
            GameCursor.Instance?.RegisterInteractableUI(this);
        }

        protected virtual void OnDisable()
        {
            GameCursor.Instance?.UnregisterInteractableUI(this);
        }
    }
}