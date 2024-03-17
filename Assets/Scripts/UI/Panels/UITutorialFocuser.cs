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
        
        private Rect _desiredRect;
        private Rect _currentRect;
        private bool _smooth;
        private float _smoothTime = 0.5f;
        private float _smoothTimer;

        private void RecalculateRect(Rect rect)
        {
            var rectInt = new RectInt(
                new Vector2Int((int)rect.x, (int)rect.y),
                new Vector2Int((int)rect.width, (int)rect.height));
            
            var size = _root.rect;
            _right.anchoredPosition = new Vector2(rectInt.xMax, rectInt.yMin);
            _right.sizeDelta = new Vector2(size.width - rectInt.xMax, size.height - rectInt.yMin);
            
            _left.anchoredPosition = new Vector2(0, 0);
            _left.sizeDelta = new Vector2(rectInt.xMin, rectInt.yMax);
            
            _top.anchoredPosition = new Vector2(0, rectInt.yMax);
            _top.sizeDelta = new Vector2(rectInt.xMax, size.height - rectInt.yMax);
            
            _bottom.anchoredPosition = new Vector2(rectInt.xMin, 0);
            _bottom.sizeDelta = new Vector2(size.xMax - rectInt.xMin, rectInt.yMin);
            
            _center.anchoredPosition = new Vector2(rectInt.xMin, rectInt.yMin);
            _center.sizeDelta = new Vector2(rectInt.size.x, rectInt.size.y);
        }

        // public void FocusOn(RectTransform target)
        // {
        //     _target = target;
        //     _focusMode = FocusMode.Transform;
        // }
        
        // public void FocusOn(Rect rect, bool smooth)
        // {
        //     _focusMode = FocusMode.Rect;
        //  
        //     var lb = _root.InverseTransformPoint(rect.min);
        //     var rt = _root.InverseTransformPoint(rect.max);
        //     _desiredRect = new Rect(
        //         new Vector2Int((int)lb.x, (int)lb.y),
        //         new Vector2Int((int)(rt.x - lb.x), (int)(rt.y - lb.y)));
        //     _smooth = smooth;
        //     
        //     if (_smooth)
        //     {
        //         _smoothTimer = 0;
        //         enabled = true;
        //     }
        //     else
        //     {
        //         _currentRect = _desiredRect;
        //         RecalculateRect(_currentRect);
        //     }
        // }

        public void ForceFocusOn(Rect rect)
        {
            RecalculateRect(rect);
        }

        public async Task FocusOnAsync(Rect rect, bool smooth, CancellationToken cancellationToken)
        {
            _focusMode = FocusMode.Rect;
         
            var lb = _root.InverseTransformPoint(rect.min);
            var rt = _root.InverseTransformPoint(rect.max);
            _desiredRect = new Rect(
                new Vector2Int((int)lb.x, (int)lb.y),
                new Vector2Int((int)(rt.x - lb.x), (int)(rt.y - lb.y)));
            _smooth = smooth;
            
            if (_smooth)
            {
                _smoothTimer = 0;
                enabled = true;
                await UpdateAsync(cancellationToken);
                _currentRect = _desiredRect;
            }
            else
            {
                _currentRect = _desiredRect;
                RecalculateRect(_currentRect);
            }
        }
        
        // public void FocusOn(bool smooth)
        // {
        //     _focusMode = FocusMode.None;
        //     _smooth = smooth;
        //     _desiredRect = _root.rect;
        //     
        //     if (_smooth)
        //     {
        //         _smoothTimer = 0;
        //         enabled = true;
        //     }
        //     else
        //     {
        //         _currentRect = _desiredRect;
        //         RecalculateRect(_currentRect);
        //     }
        // }
        
        public async Task UnfocusOnAsync(bool smooth, CancellationToken cancellationToken)
        {
            _focusMode = FocusMode.None;
            _smooth = smooth;
            _desiredRect = _root.rect;
            
            if (_smooth)
            {
                _smoothTimer = 0;
                enabled = true;
                await UpdateAsync(cancellationToken);
                _currentRect = _desiredRect;
            }
            else
            {
                _currentRect = _desiredRect;
                RecalculateRect(_currentRect);
            }
        }

        public async Task HideAsync(bool smooth, CancellationToken cancellationToken)
        {
            _focusMode = FocusMode.None;
            _smooth = smooth;
            _desiredRect = _root.rect;
            _desiredRect.min -= new Vector2(100, 100);
            _desiredRect.max += new Vector2(100, 100);
            if (_smooth)
            {
                _smoothTimer = 0;
                enabled = true;
                await UpdateAsync(cancellationToken);
                _currentRect = _desiredRect;
            }
            else
            {
                _currentRect = _desiredRect;
                RecalculateRect(_currentRect);
            }
            
            gameObject.SetActive(false);
        }
        
        // public void Update()
        // {
        //     if (_smooth)
        //     {
        //         
        //     }
        //     
        //     if (_focusMode == FocusMode.Rect)
        //     {
        //         
        //     }
        //     
        //     if (_focusMode == FocusMode.Transform)
        //     {
        //         if (_target != null)
        //         {
        //             var corners = new Vector3[4];
        //             _target.GetWorldCorners(corners);
        //
        //             var lb = _root.InverseTransformPoint(corners[0]);
        //             var rt = _root.InverseTransformPoint(corners[2]);
        //             var min = Vector3.Min(lb, rt);
        //             var max = Vector3.Max(lb, rt);
        //
        //             _desiredRect = new Rect(
        //                 new Vector2(min.x, min.y),
        //                 new Vector2(max.x - min.x, max.y - min.y));
        //         }
        //     }
        //
        //     if (_smoothTimer < _smoothTime)
        //     {
        //         _smoothTimer += Time.deltaTime;
        //         _smoothTimer = Mathf.Clamp(_smoothTimer, 0, _smoothTime);
        //         
        //         var nSmoothTimer = _smoothTimer / _smoothTime;
        //         var rect = new Rect(
        //             Vector3.Lerp(_currentRect.position, _desiredRect.position, nSmoothTimer),
        //             Vector3.Lerp(_currentRect.size, _desiredRect.size, nSmoothTimer));
        //         RecalculateRect(rect);
        //     }
        //     else
        //     {
        //         enabled = false;
        //     }
        // }
        
        public async Task<bool> UpdateAsync(CancellationToken cancellationToken)
        {
            if (_smooth)
            {
                
            }
            
            if (_focusMode == FocusMode.Rect)
            {
                
            }
            
            if (_focusMode == FocusMode.Transform)
            {
                if (_target != null)
                {
                    var corners = new Vector3[4];
                    _target.GetWorldCorners(corners);

                    var lb = _root.InverseTransformPoint(corners[0]);
                    var rt = _root.InverseTransformPoint(corners[2]);
                    var min = Vector3.Min(lb, rt);
                    var max = Vector3.Max(lb, rt);

                    _desiredRect = new Rect(
                        new Vector2(min.x, min.y),
                        new Vector2(max.x - min.x, max.y - min.y));
                }
            }

            while (_smoothTimer < _smoothTime)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                _smoothTimer += Time.deltaTime;
                _smoothTimer = Mathf.Clamp(_smoothTimer, 0, _smoothTime);
                
                var nSmoothTimer = _smoothTimer / _smoothTime;
                var rect = new Rect(
                    Vector3.Lerp(_currentRect.position, _desiredRect.position, nSmoothTimer),
                    Vector3.Lerp(_currentRect.size, _desiredRect.size, nSmoothTimer));
                RecalculateRect(rect);

                await Task.Yield();
            }

            return true;
        }

        public async Task WaitForClick(CancellationToken cancellationToken)
        {
            await AsyncHelpers.WaitForClick(_centerButton, cancellationToken);
        }

        enum FocusMode
        {
            None,
            Transform,
            Rect
        }
    }
}