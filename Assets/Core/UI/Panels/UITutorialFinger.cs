using UnityEngine;

namespace Core
{
    public sealed class UITutorialFinger : MonoBehaviour
    {
        [SerializeField] private RectTransform _areaRoot;
        [SerializeField] private RectTransform _root;
        public void Show(Rect rect)
        {
            gameObject.SetActive(true);
            _root.anchoredPosition = _areaRoot.InverseTransformPoint(rect.position + new Vector2(rect.width * 0.5f, 0));
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}