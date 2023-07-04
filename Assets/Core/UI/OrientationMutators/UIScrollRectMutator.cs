using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.OrientationMutators
{
    public class UIScrollRectMutator : UITransformMutator
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private GridLayoutGroup _gridLayoutGroup;
        public override void Apply(ScreenOrientation orientation)
        {
            base.Apply(orientation);

            if (orientation == ScreenOrientation.Portrait)
            {
                _scrollRect.vertical = false;
                _scrollRect.horizontal = true;
                _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                _gridLayoutGroup.constraintCount = 1;
            }
            else
            {
                _scrollRect.vertical = true;
                _scrollRect.horizontal = false;
                _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                _gridLayoutGroup.constraintCount = 1;
            }
        }
    }
}