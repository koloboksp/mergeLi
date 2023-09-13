using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UISettingsPanel : UIPanel
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _changeSkinBtn;
        [SerializeField] private Button _clearProgressBtn;
        [SerializeField] private Button _showCastlesBtn;

        private Model _model;
        private UISettingsPanelData _data;

        private void Awake()
        {
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
            
            _changeSkinBtn.onClick.AddListener(ChangeSkinBtn_OnClick);
            _clearProgressBtn.onClick.AddListener(ClearProgressBtn_OnClick);
            
            _showCastlesBtn.onClick.AddListener(ShowCastlesBtn_OnClick);
        }
        
        private void ChangeSkinBtn_OnClick()
        {
            var skinScreenData = new UISkinPanel.UISkinPanelData();
            skinScreenData.SelectedSkin = _data.GameProcessor.Scene.ActiveSkin.Name;
            skinScreenData.Skins = _data.GameProcessor.Scene.Library.Containers.Select(i => i.Name);
            skinScreenData.SkinChanger = _data.GameProcessor.Scene;
            ApplicationController.Instance.UIPanelController.PushPopupScreen(typeof(UISkinPanel), skinScreenData);
        }

        private void ClearProgressBtn_OnClick()
        {
            _data.GameProcessor.PlayerInfo.Clear();
        }
        
        private void CloseBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }

        private void ShowCastlesBtn_OnClick()
        {
            var data = new UICastlesLibraryPanel.UICastleLibraryPanelData();
            data.Selected = _data.GameProcessor.GetFirstUncompletedCastle();
            data.Castles = _data.GameProcessor.CastleSelector.Library.Castles;
            data.GameProcessor = _data.GameProcessor;
            ApplicationController.Instance.UIPanelController.PushPopupScreen(typeof(UICastlesLibraryPanel), data);
        }

        public override void SetData(UIScreenData undefinedData)
        {
            _data = undefinedData as UISettingsPanelData;
            _model = new Model();
        }
        
        public class UISettingsPanelData : UIScreenData
        {
            public GameProcessor GameProcessor { get; set; }
        }
        
        public class Model
        {
         
        }
    }
}