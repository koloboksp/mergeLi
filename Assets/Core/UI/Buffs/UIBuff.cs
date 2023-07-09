using System;
using System.Collections.Generic;
using Core;
using Core.Steps.CustomOperations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBuff : MonoBehaviour
{
    protected Action _onClick;
    protected Action<PointerEventData> _onBeginDrag;
    protected Action<PointerEventData> _onEndDrag;
    protected Action<PointerEventData> _onDrag;

    [SerializeField] protected Buff _model;
    [SerializeField] protected Text _costLabel;
    [SerializeField] protected CanvasGroup _commonCanvasGroup;
    [SerializeField] protected Image _cooldownImage;

    public UIBuff SetModel(Buff model)
    {
        _model = model;
        _model
            .OnAvailableStateChanged(AvailableStateChanged)
            .OnRestCooldownChanged(RestCooldownChanged);

        SetCost();
        AvailableStateChanged();
        RestCooldownChanged();
        return this;
    }

    protected virtual void AvailableStateChanged()
    {
        _commonCanvasGroup.interactable = _model.Available;
    }

    protected virtual void RestCooldownChanged()
    {
        var fillAmount = (_model.Cooldown == 0) ? 0.0f : (float)_model.RestCooldown / (float)_model.Cooldown;
        _cooldownImage.fillAmount = fillAmount;
    }
    
    private void SetCost()
    {
        _costLabel.text = _model.Cost.ToString();
    }

    public UIBuff OnClick(Action action)
    {
        _onClick = action;
        return this;
    }
    public UIBuff OnBeginDrag(Action<PointerEventData> action)
    {
        _onBeginDrag = action;
        return this;
    }
    public UIBuff OnEndDrag(Action<PointerEventData> action)
    {
        _onEndDrag = action;
        return this;
    }
    public UIBuff OnDrag(Action<PointerEventData> action)
    {
        _onDrag = action;
        return this;
    }

    protected bool ShowShopScreenRequired()
    {
        if (!_model.IsAvailable)
        {
            ApplicationController.Instance.UIPanelController.PushPopupScreen(typeof(UIShopPanel),
                new UIShopScreenData()
                {
                    Market = _model.GameProcessor.Market,
                    PurchaseItems = new List<PurchaseItem>(_model.GameProcessor.PurchasesLibrary.Items)
                });
            return true;
        }

        return false;
    }
}

