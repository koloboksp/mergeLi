using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIShopPanel_ItemAds : UIShopPanel_Item
    {
        [SerializeField] private Text _currencyCount;

        private ShopPanelAdsItem _item;
        
        protected override void Awake()
        {
            base.Awake();
        }
        
        public override void SetModel(UIShopPanel_ItemModel model)
        {
            base.SetModel(model);

            _item = model.Item as ShopPanelAdsItem;
            _currencyCount.text = Model.Item.CurrencyAmount.ToString();
        }

        protected override async void OnClick()
        {
            Model.Owner.ChangeLockInputState(true);
            var result = await _item.AdsViewer.Show(_item.Name, Application.exitCancellationToken);
            await Model.Owner.SomethingBoughtAsync(this.Model, result.success, result.amount);
            Model.Owner.ChangeLockInputState(false);
        }
    }
}