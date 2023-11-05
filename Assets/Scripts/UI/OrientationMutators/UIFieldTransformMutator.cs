using UnityEngine;

namespace Core.UI.OrientationMutators
{
    public class UIFieldTransformMutator : UITransformMutator
    {
        [SerializeField] private UIGameScreen _owner;
        
        public override void Apply(ScreenOrientation orientation)
        {
            base.Apply(orientation);
            
            if(_owner.Data == null) return;
             
            var derived = orientation == ScreenOrientation.Portrait ? _portrait : _landscape;

            var derivedCorners = new Vector3[4];
            derived.GetWorldCorners(derivedCorners);
            var leftBottom = derivedCorners[0];
            var rightTop = derivedCorners[2];
            
            _owner.Data.GameProcessor.Scene.Field.AdaptSize(leftBottom, rightTop, derived.rect.size);
        }
    }
}