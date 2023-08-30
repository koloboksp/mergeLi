using UnityEngine;

namespace Core
{
    public class UIProgressBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _rect;
        [SerializeField] private RectTransform _barRect;

        public void SetProgress(float nValue)
        {
            _barRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _rect.rect.width * nValue);
        }
    }
}