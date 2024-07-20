using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class SetActiveHatOnUIHatsPanelTutorialStep : TutorialStep, IClickOnSomething
    {
        [SerializeField] private string _hat;
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            var hatsPanel = ApplicationController.Instance.UIPanelController.GetPanel<UIHatsPanel>();
            hatsPanel.SetActiveHat(_hat);
            return true;
        }
    }
}