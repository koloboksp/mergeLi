using System;
using Assets.Scripts.Core;
using Atom;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIMessageBoxPanel : UIPanel
    {
        [SerializeField] private Button _okBtn;
        [SerializeField] private UIStaticTextLocalizator _messageLoc;

        private void Awake()
        {
            _okBtn.onClick.AddListener(OkBtn_OnClick);
        }

        private void OkBtn_OnClick()
        {
            ApplicationController.Instance.UIPanelController.PopScreen(this);
        }

        public override void SetData(UIScreenData undefinedData)
        {
            base.SetData(undefinedData);

            var data = undefinedData as UIMessageBoxPanelData;
            _messageLoc.Id = data.MessageKey;
        }
    }

    public class UIMessageBoxPanelData : UIScreenData
    {
        public GuidEx MessageKey { get; }
        
        public UIMessageBoxPanelData(GuidEx messageKey)
        {
            MessageKey = messageKey;
        }
    }
}