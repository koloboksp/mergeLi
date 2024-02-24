using System;
using System.Threading;
using System.Threading.Tasks;
using Assets.Scripts.Core;
using Atom;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core
{
    public enum UIStartPanelChoice
    {
        Continue,
        New
    }

    public static class Locale
    {
        public static GuidEx MessageBoxAuthenticateError = new GuidEx("1f7d307ffdd4405ba7972eb018186ddf");

    }
    
    public class UIStartPanel : UIPanel
    {
        [SerializeField] private Button _playBtn;
        [SerializeField] private UIStaticTextLocalizator _playLabelBtn;
        [SerializeField] private GuidEx _playLocKey;
        [SerializeField] private GuidEx _continueLocKey;

        [SerializeField] private Button _castlesBtn;
        [SerializeField] private Button _hatsBtn;

        [SerializeField] private Button _settingsBtn;

        [SerializeField] private Button _loginSocialBtn;
        [SerializeField] private Button _achievementsBtn;

        [SerializeField] private SpawnAnimator _panelAnimator;
        private Model _model;
        private UIStartPanelData _data;
        private UIStartPanelChoice _choice;
        private CancellationTokenSource _cancellationTokenSource;

        public UIStartPanelChoice Choice => _choice;
        
        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            _playBtn.onClick.AddListener(PlayBtn_OnClick);
            _castlesBtn.onClick.AddListener(CastleBtn_OnClick);
            _hatsBtn.onClick.AddListener(HatsBtn_OnClick);
            
            _settingsBtn.onClick.AddListener(SettingsBtn_OnClick);
            
            _loginSocialBtn.onClick.AddListener(LoginSocialBtn_OnClick);
            _achievementsBtn.onClick.AddListener(ShowAchievementsBtn_OnClick);
        }
        
        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
        
        private void CloseBtn_OnClick()
        {
            
        }
        
        private void PlayBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
        
        private void CastleBtn_OnClick()
        {
            var data = new UICastlesLibraryPanel.UICastleLibraryPanelData();
            data.Selected = _data.GameProcessor.GetFirstUncompletedCastle();
            data.Castles = _data.GameProcessor.CastleSelector.Library.Castles;
            data.GameProcessor = _data.GameProcessor;
            ApplicationController.Instance.UIPanelController.PushPopupScreenAsync(typeof(UICastlesLibraryPanel), data, _cancellationTokenSource.Token);
        }
        
        private void HatsBtn_OnClick()
        {
           
        }
        
        private void SettingsBtn_OnClick()
        {
            var panelData = new UISettingsPanelData();
            panelData.GameProcessor = _data.GameProcessor;
            ApplicationController.Instance.UIPanelController.PushPopupScreenAsync(typeof(UISettingsPanel), panelData, _cancellationTokenSource.Token);
        }
        
        
        private void LoginSocialBtn_OnClick()
        {
            ApplicationController.Instance.ISocialService.AuthenticateAsync(_cancellationTokenSource.Token);
        }
        
        private async void ShowAchievementsBtn_OnClick()
        {
            if (!ApplicationController.Instance.ISocialService.IsAuthenticated())
            {
                var authenticated = await ApplicationController.Instance.ISocialService.AuthenticateAsync(_cancellationTokenSource.Token);
                if (!authenticated)
                {
                    var errorMessageBox = await ApplicationController.Instance.UIPanelController.PushScreenAsync(
                        typeof(UIMessageBoxPanel),
                        new UIMessageBoxPanelData()
                        {
                            MessageKey = Locale.MessageBoxAuthenticateError,
                        }, 
                        _cancellationTokenSource.Token) as UIMessageBoxPanel;
                    
                    await errorMessageBox.ShowAsync(_cancellationTokenSource.Token);
                    return;
                }
            }

            await ApplicationController.Instance.ISocialService.ShowAchievementsUIAsync(_cancellationTokenSource.Token);
        }
        
        private void ContinueBtn_OnClick()
        {
            _choice = UIStartPanelChoice.Continue;

            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }
        
        public override void SetData(UIScreenData undefinedData)
        {
            _data = undefinedData as UIStartPanelData;
            _model = new Model();

            if (_data.GameProcessor.HasPreviousSessionGame)
            {
                _playLabelBtn.Id = _continueLocKey;
            }
            else
            {
                _playLabelBtn.Id = _playLocKey;
            }

            _panelAnimator.Play(_data.Instant);
            if (_data.Instant)
            {
                
            }
            else
            {
                
            }
            
            SetupLoginSocialBtn();
        }

        private void SetupLoginSocialBtn()
        {
            if (ApplicationController.Instance.ISocialService.IsAuthenticated())
            {
                _loginSocialBtn.gameObject.SetActive(false);
            }
            else
            {
                _loginSocialBtn.gameObject.SetActive(true);
            }
        }
        
        public class Model
        {
         
        }
    }
    
    public class UIStartPanelData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
        public bool Instant { get; set; }
    }
}