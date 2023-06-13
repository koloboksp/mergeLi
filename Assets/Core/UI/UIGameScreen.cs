using System;
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
            ApplicationController.Instance.UIScreenController.PushScreen(typeof(UISkinScreen), new UISkinScreenData());
        }
    }

    public class UIGameScreenData : UIScreenData
    {
        public GameProcessor GameProcessor { get; set; }
    }
}