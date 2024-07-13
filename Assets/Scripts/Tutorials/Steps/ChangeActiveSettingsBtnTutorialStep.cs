using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Tutorials
{
    public class ChangeActiveSettingsBtnTutorialStep : TutorialStep
    {
        [SerializeField] public bool _state;
      
        protected override async Task<bool> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            Tutorial.Controller.SettingsBtn.gameObject.SetActive(_state);
            return true;
        }
    }
}