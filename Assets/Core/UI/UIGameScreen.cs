using System;
using System.Collections.Generic;
using System.Linq;
using Core.Steps;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIGameScreen : UIScreen
    {
        [SerializeField] private Button _undoBtn;
        [SerializeField] private Button _showSkinBtn;

        private UIGameScreenData _data;

        public void Awake()
        {
            _undoBtn.onClick.AddListener(UndoBtn_OnClick);
            _showSkinBtn.onClick.AddListener(ShowSkinBtn_OnClick);
        }

        public override void SetData(UIScreenData data)
        {
            base.SetData(data);
            
            _data = data as UIGameScreenData;
            _data.GameProcessor.OnStepCompleted += OnStepCompleted;
            _data.GameProcessor.OnStepExecute += OnStepExecute;
        }

        private void OnStepExecute(Step sender)
        {
            _undoBtn.interactable = false;
        }

        private void OnStepCompleted(Step sender)
        {
            _undoBtn.interactable = true;
        }

        private void UndoBtn_OnClick()
        {
            _data.GameProcessor.Undo();
        }
        
        private void ShowSkinBtn_OnClick()
        {
            var skinScreenData = new UISkinScreen.UISkinScreenData();
            skinScreenData.SelectedSkin = _data.GameProcessor.Scene.ActiveSkin.Name;
            skinScreenData.Skins = _data.GameProcessor.Scene.Library.Containers.Select(i => i.Name);
            skinScreenData.SkinChanger = _data.GameProcessor.Scene;
            ApplicationController.Instance.UIScreenController.PushScreen(typeof(UISkinScreen), skinScreenData);
        }
    }

    public class UIGameScreenData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
    }
}