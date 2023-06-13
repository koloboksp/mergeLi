using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace Core
{
    public class UISkinScreen : UIScreen
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private ScrollView _skins;
        [SerializeField] private UISkinScreen_SkinItem _itemPrefab;
        
        private UISkinScreenData _data;

        private void Awake()
        {
            _closeBtn.onClick.AddListener(CloseBtn_OnClick);
        }

        private void CloseBtn_OnClick()
        {
            ApplicationController.Instance.UIScreenController.PopScreen(this);
        }

        public override void SetData(UIScreenData data)
        {
            _data = data as UISkinScreenData;
            
          //  var skins = _data.Skins;

          //  foreach (var VARIABLE in skins)
          //  {
          //      
          //  }
        }

        void Item_OnClick(UISkinScreen_SkinItem sender)
        {
           // _data.Scene.SetSkin(sender.SkinName);
        }
    }

    public class UISkinScreenData : UIScreenData
    {
        public Scene Scene;
    }
}