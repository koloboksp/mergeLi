using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core;
using Atom;
using UI.Panels;
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
        [SerializeField] private Button _rateUsBtn;
        [SerializeField] private Button _achievementsBtn;
        [SerializeField] private Button _leaderboardBtn;

        [SerializeField] private SpawnAnimator _panelAnimator;

        [SerializeField] private UIAnyGiftIndicator _anyGiftIndicator;
        
        [FormerlySerializedAs("_cheatsPanelBtn")] [SerializeField] private Button _cheatsBtn;

        private Model _model;
        private UIStartPanelData _data;
        private UIStartPanelChoice _choice;
        private DependencyHolder<UIPanelController> _panelController;

        public UIStartPanelChoice Choice => _choice;
        
        private void Awake()
        {
            _playBtn.onClick.AddListener(PlayBtn_OnClick);
            _castlesBtn.onClick.AddListener(CastleBtn_OnClick);
            _hatsBtn.onClick.AddListener(HatsBtn_OnClick);
            
            _settingsBtn.onClick.AddListener(SettingsBtn_OnClick);
            
            _loginSocialBtn.onClick.AddListener(LoginSocialBtn_OnClick);
            _rateUsBtn.onClick.AddListener(RateUsBtn_OnClick);
            _achievementsBtn.onClick.AddListener(ShowAchievementsBtn_OnClick);
            _leaderboardBtn.onClick.AddListener(ShowLeaderboardBtn_OnClick);
            
            _cheatsBtn.onClick.AddListener(ShowCheatsBtn_OnClick);
        }
        
        private void CloseBtn_OnClick()
        {
            
        }
        
        private void PlayBtn_OnClick()
        {
            _panelController.Value.PopScreen(this);
        }
        
        private void CastleBtn_OnClick()
        {
            var data = new UICastleLibraryPanelData
            {
                Selected = _data.GameProcessor.SessionProcessor.GetFirstUncompletedCastleName(),
                Castles = _data.GameProcessor.CastleSelector.Library.Castles,
                GameProcessor = _data.GameProcessor
            };
            _ = _panelController.Value.PushPopupScreenAsync<UICastlesLibraryPanel>(
                data,
                Application.exitCancellationToken);
        }
        
        private void HatsBtn_OnClick()
        {
            var data = new UIHatsPanelData();
            data.GameProcessor = _data.GameProcessor;
            data.Selected = _data.GameProcessor.Scene.HatsLibrary.Hats[0];
            data.UserActiveHatsFilter = _data.GameProcessor.Scene.GetUserActiveHatsFilter();
            data.Hats = _data.GameProcessor.Scene.HatsLibrary.Hats;
            data.HatsChanger = _data.GameProcessor.Scene;
            
            _ = _panelController.Value.PushPopupScreenAsync<UIHatsPanel>(
                data,
                Application.exitCancellationToken);
        }
        
        private void SettingsBtn_OnClick()
        {
            var panelData = new UISettingsPanelData();
            panelData.GameProcessor = _data.GameProcessor;
            _ = _panelController.Value.PushPopupScreenAsync<UISettingsPanel>(
                panelData,
                Application.exitCancellationToken);
        }
        
        
        private async void LoginSocialBtn_OnClick()
        {
            if (!ApplicationController.Instance.ISocialService.IsAuthenticated())
            {
                var waitingScreen = await _panelController.Value.PushPopupScreenAsync<UIWaitingPanel>(
                    null, 
                    Application.exitCancellationToken);
                
                var authenticateResult = await ApplicationController.Instance.ISocialService.AuthenticateAsync(
                    Application.exitCancellationToken);
                if (!authenticateResult)
                {
                    _panelController.Value.PopScreen(waitingScreen);
                    var errorMessageBox = await _panelController.Value.PushPopupScreenAsync<UIMessageBoxPanel>(
                        new UIMessageBoxPanelData(Locale.MessageBoxAuthenticateError), 
                        Application.exitCancellationToken);
                    
                    await errorMessageBox.ShowAsync(Application.exitCancellationToken);
                    return;
                }
                
                _panelController.Value.PopScreen(waitingScreen);
            }
        }

        private async void RateUsBtn_OnClick()
        {
            ApplicationController.RateUsInStore();
        }

        private async void ShowAchievementsBtn_OnClick()
        {
            if (!ApplicationController.Instance.ISocialService.IsAuthenticated())
            {
                var waitingScreen = await _panelController.Value.PushPopupScreenAsync<UIWaitingPanel>(
                    null,
                    Application.exitCancellationToken);
                
                var authenticateResult = await ApplicationController.Instance.ISocialService.AuthenticateAsync(
                    Application.exitCancellationToken);
                if (!authenticateResult)
                {
                    _panelController.Value.PopScreen(waitingScreen);
                    var errorMessageBox = await _panelController.Value.PushPopupScreenAsync<UIMessageBoxPanel>(
                        new UIMessageBoxPanelData(Locale.MessageBoxAuthenticateError), 
                        Application.exitCancellationToken);
                    
                    await errorMessageBox.ShowAsync(Application.exitCancellationToken);
                    return;
                }
                
                _panelController.Value.PopScreen(waitingScreen);
            }
            
            await ApplicationController.Instance.ISocialService.ShowAchievementsUIAsync(Application.exitCancellationToken);
        }
        
        private async void ShowLeaderboardBtn_OnClick()
        {
            if (!ApplicationController.Instance.ISocialService.IsAuthenticated())
            {
                var waitingScreen = await _panelController.Value.PushPopupScreenAsync<UIWaitingPanel>(
                    null,
                    Application.exitCancellationToken);
                
                var authenticateResult = await ApplicationController.Instance.ISocialService.AuthenticateAsync(
                    Application.exitCancellationToken);
                if (!authenticateResult)
                {
                    _panelController.Value.PopScreen(waitingScreen);
                    var errorMessageBox = await _panelController.Value.PushPopupScreenAsync<UIMessageBoxPanel>(
                        new UIMessageBoxPanelData(Locale.MessageBoxAuthenticateError), 
                        Application.exitCancellationToken);
                    
                    await errorMessageBox.ShowAsync(Application.exitCancellationToken);
                    return;
                }
                
                _panelController.Value.PopScreen(waitingScreen);
            }
            
            await ApplicationController.Instance.ISocialService.SetScoreForLeaderBoard(SessionProcessor.BEST_SESSION_SCORE_LEADERBOARD, ApplicationController.Instance.SaveController.SaveProgress.BestSessionScore, Application.exitCancellationToken);
            await ApplicationController.Instance.ISocialService.ShowLeaderboardUIAsync(Application.exitCancellationToken);
        }

        private async void ShowCheatsBtn_OnClick()
        {
#if DEBUG
            _ = _panelController.Value.PushPopupScreenAsync<UICheatsPanel>(
                new UICheatsPanelData()
                {
                    GameProcessor = _data.GameProcessor,
                },
                Application.exitCancellationToken);
#endif
        }

        private void ContinueBtn_OnClick()
        {
            _choice = UIStartPanelChoice.Continue;

            _panelController.Value.PopScreen(this);
        }
        
        public override void SetData(UIScreenData undefinedData)
        {
            _data = undefinedData as UIStartPanelData;
            _model = new Model();

            SetPlayButton();

            _panelAnimator.Play(_data.Instant);
            if (_data.Instant)
            {
                
            }
            else
            {
                
            }
            
            SetupLoginSocialBtn();
            
            _anyGiftIndicator.Set(_data.GameProcessor.GiftsMarket);
        }

        private void SetPlayButton()
        {
            if (_data.GameProcessor.SessionProcessor.HasPreviousSessionGame)
            {
                _playLabelBtn.Id = _continueLocKey;
            }
            else
            {
                _playLabelBtn.Id = _playLocKey;
            }
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