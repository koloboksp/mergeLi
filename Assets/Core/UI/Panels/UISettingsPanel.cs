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

        private Model _model;
        private UISettingsPanelData _data;

        private void Awake()
        {
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
            
            _changeSkinBtn.onClick.AddListener(ChangeSkinBtn_OnClick);
        }

        private void ChangeSkinBtn_OnClick()
        {
            var skinScreenData = new UISkinPanel.UISkinScreenData();
            skinScreenData.SelectedSkin = _data.GameProcessor.Scene.ActiveSkin.Name;
            skinScreenData.Skins = _data.GameProcessor.Scene.Library.Containers.Select(i => i.Name);
            skinScreenData.SkinChanger = _data.GameProcessor.Scene;
            ApplicationController.Instance.UIPanelController.PushPopupScreen(typeof(UISkinPanel), skinScreenData);
        }

        private void CloseBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
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