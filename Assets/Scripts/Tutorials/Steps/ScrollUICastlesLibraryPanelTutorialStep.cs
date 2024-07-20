using System.Threading;
using System.Threading.Tasks;
using UI.Panels;
using UnityEngine;

namespace Core.Tutorials
{
    public class ScrollUICastlesLibraryPanelTutorialStep : TutorialStep
    {
        [SerializeField] private float _speed = 0.2f;
        
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        { 
            var panel = ApplicationController.Instance.UIPanelController.GetPanel<UICastlesLibraryPanel>();
            panel.StartAutoScrollContent(_speed);

            return true;
        }
    }
}