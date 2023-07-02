using System;
using System.Collections.Generic;
using Core;
using Core.Steps.CustomOperations;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIBuff : MonoBehaviour
{
    protected Action _onClick;
    protected Action<PointerEventData> _onBeginDrag;
    protected Action<PointerEventData> _onEndDrag;
    protected Action<PointerEventData> _onDrag;

    [SerializeField] protected Buff _model;
    
    public UIBuff SetModel(Buff model)
    {
        _model = model;
        return this;
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
            ApplicationController.Instance.UIScreenController.PushPopupScreen(typeof(UIShopScreen),
                new UIShopScreenData()
                {
                    Market = _model.GameProcessor.Market,
                    PurchaseItems = new List<PurchaseItem>(_model.GameProcessor.Scene.PurchasesLibrary.Items)
                });
            return true;
        }

        return false;
    }
}

