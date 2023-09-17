using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public sealed class UITutorialFocuser : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private RectTransform _right;
        [SerializeField] private RectTransform _left;
        [SerializeField] private RectTransform _top;
        [SerializeField] private RectTransform _bottom;
        [SerializeField] private RectTransform _center;
        [SerializeField] private Button _centerButton;

        private FocusMode _focusMode;
        private RectTransform _target;
        
        private void RecalculateRect(RectInt rect)
        {
            var size = _root.rect;
            _right.anchoredPosition = new Vector2(rect.xMax, rect.yMin);
            _right.sizeDelta = new Vector2(size.width - rect.xMax, size.height - rect.yMin);
            
            _left.anchoredPosition = new Vector2(0, 0);
            _left.sizeDelta = new Vector2(rect.xMin, rect.yMax);
            
            _top.anchoredPosition = new Vector2(0, rect.yMax);
            _top.sizeDelta = new Vector2(rect.xMax, size.height - rect.yMax);
            
            _bottom.anchoredPosition = new Vector2(rect.xMin, 0);
            _bottom.sizeDelta = new Vector2(size.xMax - rect.xMin, rect.yMin);
            
            _center.anchoredPosition = new Vector2(rect.xMin, rect.yMin);
            _center.sizeDelta = new Vector2(rect.size.x, rect.size.y);
        }

        public void FocusOn(RectTransform target)
        {
            _target = target;
            _focusMode = FocusMode.Transform;
        }
        
        public void FocusOn(Rect rect)
        {
            var lb = _root.InverseTransformPoint(rect.min);
            var rt = _root.InverseTransformPoint(rect.max);

            var rectInt = new RectInt(
                new Vector2Int((int)lb.x, (int)lb.y),
                new Vector2Int((int)(rt.x - lb.x), (int)(rt.y - lb.y)));
            
            _focusMode = FocusMode.Rect;
            RecalculateRect(rectInt);
        }
        
        public void Update()
        {
            if (_focusMode != FocusMode.Transform) return;
            if (_target == null) return;
            
            var corners = new Vector3[4];
            _target.GetWorldCorners(corners);

            var lb = _root.InverseTransformPoint(corners[0]);
            var rt = _root.InverseTransformPoint(corners[2]);
            var min = Vector3.Min(lb, rt);
            var max = Vector3.Max(lb, rt);

            var rect = new RectInt(
                new Vector2Int((int)min.x, (int)min.y),
                new Vector2Int((int)(max.x - min.x), (int)(max.y - min.y)));
            RecalculateRect(rect);
        }

        public async Task WaitForClick(CancellationToken cancellationToken)
        {
             _centerButton.onClick.AddListener(OnClick);
            bool buttonClicked = false;
            
            while (!buttonClicked)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new OperationCanceledException();

                await Task.Yield();
            }
            
            void OnClick()
            {
                _centerButton.onClick.RemoveListener(OnClick);
                buttonClicked = true;
            }
        }

        enum FocusMode
        {
            None,
            Transform,
            Rect
        }
    }
}