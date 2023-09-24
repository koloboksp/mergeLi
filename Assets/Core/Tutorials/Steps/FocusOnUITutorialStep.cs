using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class FocusOnUITutorialStep : TutorialStep, IFocusedOnSomething
    {
        [SerializeField] private string _tag;

        private UITutorialElement _target;
        
        protected override async Task<bool> InnerInit(CancellationToken cancellationToken)
        {
            var tutorialElements = FindObjectsOfType<UITutorialElement>();
            _target = tutorialElements.First(i => i.Tag == _tag);

            return true;
        }

        public Rect GetFocusedRect()
        {
            var corners = new Vector3[4];
            _target.Root.GetWorldCorners(corners);

            var lb = corners[0];
            var rt = corners[2];
            var min = Vector3.Min(lb, rt);
            var max = Vector3.Max(lb, rt);

            var rect = new Rect(
                new Vector2(min.x, min.y),
                new Vector2(max.x - min.x, max.y - min.y));
            
            return rect;
        }
    }
}