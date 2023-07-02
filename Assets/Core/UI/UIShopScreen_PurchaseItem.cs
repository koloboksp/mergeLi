using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIShopScreen_PurchaseItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;
        [SerializeField] private Text _name;
       
        private Model _model;
        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }
        
        public void SetModel(Model model)
        {
            _model = model;
            
            _name.text = _model.Name;
        }
        
        private void OnClick()
        {
            _model.BuyMe();
        }
        
        private void OnSelectionChanged()
        {
            
        }
        
        public class Model
        {
            private UIShopScreen.Model _owner;
            private string _name;
            private bool _selected;
            private string _inAppId;

            public Model(UIShopScreen.Model owner)
            {
                _owner = owner;
            }

            public string Name => _name;
            public string InAppId => _inAppId;

            public Model SetName(string name)
            {
                _name = name;
                return this;
            }
            
            public Model SetInAppId(string inAppId)
            {
                _inAppId = inAppId;
                return this;
            }

            public async void BuyMe()
            {
                var buy = await _owner.Buy(this);
                
            }
        }
    }
}