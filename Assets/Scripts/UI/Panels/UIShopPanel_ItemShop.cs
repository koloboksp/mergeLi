﻿using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIShopPanel_ItemShop : UIShopPanel_Item
    {
        [SerializeField] private Text _cost;
        [SerializeField] private Text _currencyCount;

        private ShopPanelMarketItem _item;
        
        public override void SetModel(UIShopPanel_ItemModel model)
        {
            base.SetModel(model);
            _item = model.Item as ShopPanelMarketItem;
            
            _currencyCount.text = Model.Item.CurrencyAmount.ToString();
            _cost.text = ApplicationController.Instance.PurchaseController.GetLocalizedPriceString(_item.ProductId);
        }
        
        protected override async void OnClick()
        {
            Model.Owner.ChangeLockInputState(true);
            var result = await _item.Market.BuyAsync(_item.ProductId, Application.exitCancellationToken);
            await Model.Owner.SomethingBoughtAsync(this.Model, result.success, result.amount);
            Model.Owner.ChangeLockInputState(false);
        }
    }
}