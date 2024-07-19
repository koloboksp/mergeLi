using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class FocusOnUITutorialStep : TutorialStep, IFocusedOnSomething
    {
        private static readonly Vector3[] _noAllocCorners = new Vector3[4];

        [SerializeField] private string _tag;

        private UITutorialElement _target;
        
        protected override async Task<bool> InnerInitAsync(CancellationToken cancellationToken)
        {
            _target = UITutorialElement.FindByTag(_tag);

            Tutorial.Controller.SetFocusedRect(GetFocusedRect());
            
            return true;
        }

        public Rect GetFocusedRect()
        {
            return GetRect(_target.Root);
        }

        
        public static Rect GetRect(RectTransform rectTransform)
        {
            rectTransform.GetWorldCorners(_noAllocCorners);

            var lb = _noAllocCorners[0];
            var rt = _noAllocCorners[2];
            var min = Vector3.Min(lb, rt);
            var max = Vector3.Max(lb, rt);

            var rect = new Rect(
                new Vector2(min.x, min.y),
                new Vector2(max.x - min.x, max.y - min.y));
            
            return rect;
        }
    }
}