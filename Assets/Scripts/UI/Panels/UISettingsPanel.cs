using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Core.Gameplay;
using UI.Panels;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UISettingsPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        
        [SerializeField] private Button _soundEnableBtn;
        [SerializeField] private Image _soundEnableBtnIcon;
        [SerializeField] private Sprite _soundEnableIcon;
        [SerializeField] private Sprite _soundDisableIcon;
        [SerializeField] private Slider _soundVolumeSlider;
        
        [SerializeField] private Button _musicEnableBtn;
        [SerializeField] private Image _musicEnableBtnIcon;
        [SerializeField] private Sprite _musicEnableIcon;
        [SerializeField] private Sprite _musicDisableIcon;
        [SerializeField] private Slider _musicVolumeSlider;
        
        [SerializeField] private Button _vibrationEnableBtn;
        [SerializeField] private Image _vibrationEnableBtnIcon;
        [SerializeField] private Sprite _vibrationEnableIcon;
        [SerializeField] private Sprite _vibrationDisableIcon;

        [SerializeField] private Button _changeLanguageBtn;
        [SerializeField] private Text _changeLanguageBtnText;
        [SerializeField] private Image _changeLanguageBtnImage;

        private Model _model;
        private UISettingsPanelData _data;
        
        private void Awake()
        {
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
            
            _soundEnableBtn.onClick.AddListener(SoundEnableBtn_OnClick);
            _soundVolumeSlider.onValueChanged.AddListener(SoundVolumeSlider_OnValueChanged);
            
            _musicEnableBtn.onClick.AddListener(MusicEnableBtn_OnClick);
            _musicVolumeSlider.onValueChanged.AddListener(MusicVolumeSlider_OnValueChanged);

#if UNITY_WEBGL
            _vibrationEnableBtn.gameObject.SetActive(false);
#else
            _vibrationEnableBtn.gameObject.SetActive(true);
            _vibrationEnableBtn.onClick.AddListener(VibrationEnableBtn_OnClick);
#endif
            _changeLanguageBtn.onClick.AddListener(ChangeLanguageBtn_OnClick);
        }
        
        private void ChangeSkinBtn_OnClick()
        {
            var skinScreenData = new UISkinPanel.UISkinPanelData();
            skinScreenData.SelectedSkin = ApplicationController.Instance.SaveController.SaveSettings.ActiveSkin;
            skinScreenData.Skins = _data.GameProcessor.Scene.SkinLibrary.Containers.Select(i => i.Name);
            skinScreenData.SkinChanger = _data.GameProcessor;
            ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UISkinPanel>(
                skinScreenData, 
                Application.exitCancellationToken);
        }
        
        private void ChangeLanguageBtn_OnClick()
        {
            var screenData = new UILanguagePanelData();
            screenData.Selected = ApplicationController.Instance.LocalizationController.ActiveLanguage;
            screenData.Available = ApplicationController.Instance.LocalizationController.LanguagePackDescs
                .Select(i => new UILanguagePanelLanguageData
                {
                    Language = i.Language,
                    Icon = i.NativeFlag,
                    Label = i.NativeName,
                });
            screenData.Changer = ApplicationController.Instance;
            ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UILanguagePanel>(
                screenData,
                Application.exitCancellationToken);
        }

        private void ClearProgressBtn_OnClick()
        {
            ApplicationController.Instance.SaveController.Clear();
        }
        
        private void CloseBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }

        private void ShowCastlesBtn_OnClick()
        {
            var data = new UICastleLibraryPanelData
            {
                Selected = _data.GameProcessor.SessionProcessor.GetFirstUncompletedCastleName(),
                Castles = _data.GameProcessor.CastleSelector.Library.Castles,
                GameProcessor = _data.GameProcessor
            };
            ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UICastlesLibraryPanel>(
                data,
                Application.exitCancellationToken);
        }

        public override void SetData(UIScreenData undefinedData)
        {
            base.SetData(undefinedData);

            _data = undefinedData as UISettingsPanelData;
            _model = new Model();
            
            SetLanguageBtn();
        }
        
        protected override void InnerActivate()
        {
            base.InnerActivate();
            
            SetLanguageBtn();
            SetSound();
            SetMusic();
            UpdateVibrationEnable();
        }

        private void SetSound()
        {
            UpdateSoundEnable();
            _soundVolumeSlider.SetValueWithoutNotify(ApplicationController.Instance.SoundController.SoundVolume);
        }

        private void UpdateSoundEnable()
        {
            _soundEnableBtnIcon.sprite = ApplicationController.Instance.SoundController.SoundEnable
                ? _soundEnableIcon
                : _soundDisableIcon;
        }
        
        private void SoundEnableBtn_OnClick()
        {
            ApplicationController.Instance.SoundController.SoundEnable = !ApplicationController.Instance.SoundController.SoundEnable;
            UpdateSoundEnable();
        }
        
        private void SoundVolumeSlider_OnValueChanged(float value)
        {
            ApplicationController.Instance.SoundController.SoundEnable = true;
            UpdateSoundEnable();
            ApplicationController.Instance.SoundController.SoundVolume = value;
        }
        
        private void SetMusic()
        {
            UpdateMusicEnable();
            _musicVolumeSlider.SetValueWithoutNotify(ApplicationController.Instance.SoundController.MusicVolume);
        }
        
        private void UpdateMusicEnable()
        {
            _musicEnableBtnIcon.sprite = ApplicationController.Instance.SoundController.MusicEnable
                ? _musicEnableIcon
                : _musicDisableIcon;
        }
        
        private void UpdateVibrationEnable()
        {
            _vibrationEnableBtnIcon.sprite = ApplicationController.Instance.VibrationController.Enable
                ? _vibrationEnableIcon
                : _vibrationDisableIcon;
        }
        
        private void MusicEnableBtn_OnClick()
        {
            ApplicationController.Instance.SoundController.MusicEnable = !ApplicationController.Instance.SoundController.MusicEnable;
            UpdateMusicEnable();
        }
        
        private void VibrationEnableBtn_OnClick()
        {
            ApplicationController.Instance.VibrationController.Enable = !ApplicationController.Instance.VibrationController.Enable;
            UpdateVibrationEnable();
        }
        
        private void MusicVolumeSlider_OnValueChanged(float value)
        {
            ApplicationController.Instance.SoundController.MusicEnable = true;
            UpdateMusicEnable();
            ApplicationController.Instance.SoundController.MusicVolume = value;
        }
        
        private void SetLanguageBtn()
        {
            var languagePackDesc = ApplicationController.Instance.LocalizationController.GetActivePackDesc();
            _changeLanguageBtnText.text = languagePackDesc.NativeName;
            _changeLanguageBtnImage.sprite = languagePackDesc.NativeFlag;
        }
        
        public class Model
        {
         
        }
    }
    
    public class UISettingsPanelData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
    }
}