using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public sealed class UITutorialFinger : MonoBehaviour
    {
        [SerializeField] private RectTransform _areaRoot;
        [SerializeField] private RectTransform _root;
        
        [SerializeField] private GameObject _normal;
        [SerializeField] private Image _normalIcon;
        [SerializeField] private Sprite _default;
        [SerializeField] private Sprite _pressed;

        [SerializeField] private GameObject _pointingStyle;
        
        public void Show(Rect rect, FingerOrientation orientation, FingerStyle style)
        {
            gameObject.SetActive(true);
            _root.anchoredPosition = GetPosition(rect, orientation);
            _root.rotation = GetRotation(orientation);

            ChangeStyle(style);
        }

        private void ChangeStyle(FingerStyle style)
        {
            if (style == FingerStyle.Normal)
            {
                _normal.SetActive(true);
                _pointingStyle.SetActive(false);
            }
            else if (style == FingerStyle.Pointing)
            {
                _normal.SetActive(false);
                _pointingStyle.SetActive(true);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private Vector2 GetPosition(Rect rect, FingerOrientation orientation)
        {
            if (orientation == FingerOrientation.Down)
                return _areaRoot.InverseTransformPoint(rect.position + new Vector2(rect.width * 0.5f, rect.height));
            
            return _areaRoot.InverseTransformPoint(rect.position + new Vector2(rect.width * 0.5f, 0));
        }
        
        private Quaternion GetRotation(FingerOrientation orientation)
        {
            if (orientation == FingerOrientation.Down)
                return Quaternion.Euler(0, 0, 180);
            return Quaternion.identity;
        }

        public void ForceFocusOn(Rect rect)
        {
            _root.anchoredPosition = GetPosition(rect, FingerOrientation.Up);
        }

        public void Click()
        {
            StartCoroutine(CClick());
        }

        public IEnumerator CClick()
        {
            ChangeStyle(FingerStyle.Normal);
            _normalIcon.sprite = _pressed;
            yield return new WaitForSeconds(0.25f);
            _normalIcon.sprite = _default;
        }
    }

    public enum FingerOrientation
    {
        Up,
        Down
    }
    
    public enum FingerStyle
    {
        Pointing,
        Normal
    }
}