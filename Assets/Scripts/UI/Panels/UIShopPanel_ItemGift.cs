using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Core
{
    public class UIShopPanel_ItemGift : UIShopPanel_Item
    {
        [SerializeField] private Animation _presentAnimation;
        [SerializeField] private AnimationClip _presentAvailableClip;
        [SerializeField] private AnimationClip _presentNotAvailableClip;
        [SerializeField] private UITimer _timer;
        [SerializeField] private Text _requiredInternetConnectionLbl;

        private ShopPanelGiftItem _item;
        
        protected override void Awake()
        {
            base.Awake();
            
            _timer.OnComplete += Timer_OnComplete;
        }
        
        private async void Timer_OnComplete(UITimer sender)
        {
            Interactable = true;

            await NetworkTimeManager.ForceUpdate(Application.exitCancellationToken);
            SetAvailability();
        }

        public override void SetModel(UIShopPanel_ItemModel model)
        {
            base.SetModel(model);
            _item = model.Item as ShopPanelGiftItem;
           
            SetAvailability();
        }

        private void SetAvailability()
        {
            if (NetworkTimeManager.TimeUpdated)
            {
                var restTimeForCollect = _item.Gift.GetRestTimeForCollect(NetworkTimeManager.NowTicks);

                _timer.gameObject.SetActive(true);
                _timer.Set(restTimeForCollect);

                var available = restTimeForCollect <= 0;
                _requiredInternetConnectionLbl.gameObject.SetActive(false);
                Interactable = available;
                _presentAnimation.Play(available ? _presentAvailableClip.name : _presentNotAvailableClip.name);
            }
            else
            {
                _timer.gameObject.SetActive(false);
                _requiredInternetConnectionLbl.gameObject.SetActive(true);
                
                Interactable = false;
                _presentAnimation.Play(_presentNotAvailableClip.name);
            }
        }

        protected override async void OnClick()
        {
            Model.Owner.ChangeLockInputState(true);
            var result = await _item.Gift.CollectAsync(Application.exitCancellationToken);
            SetAvailability();
            await Model.Owner.SomethingBoughtAsync(this.Model, result.success, result.amount);
            Model.Owner.ChangeLockInputState(false);
        }
    }
}