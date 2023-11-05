using UnityEngine;

namespace Core.UI.OrientationMutators
{
    public class UITransformMutator : UIMutator
    {
        [SerializeField] private RectTransform _target;
        [SerializeField] protected RectTransform _landscape;
        [SerializeField] protected RectTransform _portrait;
       
        public override void Apply(ScreenOrientation orientation)
        {
            var derived = orientation == ScreenOrientation.Portrait ? _portrait : _landscape;
            
            _target.anchoredPosition = derived.anchoredPosition;
            _target.anchorMax = derived.anchorMax;
            _target.anchorMin = derived.anchorMin;
            _target.offsetMin = derived.offsetMin;
            _target.offsetMax = derived.offsetMax;
            _target.pivot = derived.pivot;
        }
    }
}