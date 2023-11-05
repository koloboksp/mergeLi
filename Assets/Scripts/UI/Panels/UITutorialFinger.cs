using UnityEngine;

namespace Core
{
    public sealed class UITutorialFinger : MonoBehaviour
    {
        [SerializeField] private RectTransform _areaRoot;
        [SerializeField] private RectTransform _root;
        public void Show(Rect rect, FingerOrientation orientation)
        {
            gameObject.SetActive(true);
            _root.anchoredPosition = GetPosition(rect, orientation);
            _root.rotation = GetRotation(orientation);
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
    }

    public enum FingerOrientation
    {
        Up,
        Down
    }
}