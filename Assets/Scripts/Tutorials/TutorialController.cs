using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Gameplay;
using Core.Utils;
using UI.Common;
using UnityEngine;

namespace Core.Tutorials
{
    public class TutorialController : MonoBehaviour
    {
        [SerializeField] private UITutorialFocuser _tutorialFocuser;
        [SerializeField] private UITutorialFinger _tutorialFinger;
        [SerializeField] private UITutorialDialog _tutorialDialog;
        [SerializeField] private UIExtendedButton _settingsBtn;

        [SerializeField] private GameProcessor _gameProcessor;
        
        private readonly List<Tutorial> _availableTutorials = new List<Tutorial>();
        private DependencyHolder<UIPanelController> _panelController;
        
        public GameProcessor GameProcessor => _gameProcessor;

        public IReadOnlyList<Tutorial> AvailableTutorials => _availableTutorials;
        public UITutorialFocuser Focuser => _tutorialFocuser;
        public UITutorialFinger Finger => _tutorialFinger;
        public UITutorialDialog Dialog => _tutorialDialog;
        public UIExtendedButton SettingsBtn => _settingsBtn;

        
        public void Awake()
        {
            gameObject.GetComponentsInChildren(_availableTutorials);
            
            _settingsBtn.onClick.AddListener(SettingsBtn_OnClick);
        }

        public async Task TryStartTutorial(bool forceStart, CancellationToken cancellationToken)
        {
            foreach (var tutorial in _availableTutorials)
            {
                if(tutorial.CanStart(forceStart))
                    await tutorial.Execute(cancellationToken);
            }
        }

        public bool CanStartTutorial(bool forceStart)
        {
            return _availableTutorials.Any(tutorial => tutorial.CanStart(forceStart));
        }
        
        private void SettingsBtn_OnClick()
        {
            var panelData = new UISettingsPanelData();
            panelData.GameProcessor = _gameProcessor;
            _ = _panelController.Value.PushPopupScreenAsync<UISettingsPanel>(
                panelData,
                Application.exitCancellationToken);
        }
    }
}