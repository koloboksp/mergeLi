using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class MoveToUITutorialStep : BaseMoveToTutorialStep
    {
        [SerializeField] private string _tag;

        private UITutorialElement _target;
        
        protected override async Task<bool> InnerInitAsync(CancellationToken cancellationToken)
        {
            _target = UITutorialElement.FindByTag(_tag);
            
            return true;
        }
        
        protected override (Vector2 position, Vector2 size) GetToPose()
        {
            var rect = FocusOnUITutorialStep.GetRect(_target.Root);
            return (rect.min, rect.size);
        }
    }
}