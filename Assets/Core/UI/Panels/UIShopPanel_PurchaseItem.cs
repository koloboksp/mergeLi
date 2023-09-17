using System;
using Core.Steps.CustomOperations;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Core
{
    public class UIShopPanel_PurchaseItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private AssetImage _offerIcon;
        [SerializeField] private Text _count;
       
        private Model _model;
        private void Awake()
        {
            _button.onClick.AddListener(OnClick);
        }
        
        public void SetModel(Model model)
        {
            _model = model;
            
            _count.text = _model.CurrencyAmount.ToString();
            _offerIcon.SpriteName = _model.BackgroundName;
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
            private UIShopPanel.Model _owner;
            private string _name;
            private bool _selected;
            private string _inAppId;
            private PurchaseType _purchaseType;
            private string _backgroundName;
            private int _currencyAmount;

            public Model(UIShopPanel.Model owner)
            {
                _owner = owner;
            }

            public string Name => _name;
            public string InAppId => _inAppId;
            public int CurrencyAmount => _currencyAmount;
            public PurchaseType PurchaseType => _purchaseType;
            public string BackgroundName => _backgroundName;

            public Model Init(string name, string inAppId, int currencyAmount, PurchaseType purchaseType)
            {
                _name = name;
                _inAppId = inAppId;
                _currencyAmount = currencyAmount;
                _purchaseType = purchaseType;
                
                return this;
            }

            public Model SetBackgroundName(string name)
            {
                _backgroundName = name;
                return this;
            }
            public async void BuyMe()
            {
                var result = await _owner.Buy(this);
            }
        }
    }
}