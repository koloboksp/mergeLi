using System;
using System.Collections.Generic;
using System.Threading;
using Core;
using Core.Steps.CustomOperations;
using Core.Tutorials;
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
    [SerializeField] protected GameObject _iconPanel;
    [SerializeField] protected Text _costLabel;
    [SerializeField] protected CanvasGroup _commonCanvasGroup;
    [SerializeField] protected Image _cooldownImage;
    [SerializeField] protected UITutorialElement _tutorialElement;

    public GameObject IconPanel => _iconPanel;
    
    public UIBuff SetModel(Buff model)
    {
        _model = model;
        _model
            .OnAvailableStateChanged(AvailableStateChanged)
            .OnRestCooldownChanged(RestCooldownChanged);

        SetCost();
        AvailableStateChanged();
        RestCooldownChanged();

        _tutorialElement.Tag = _model.Id;
        return this;
    }

    protected virtual void AvailableStateChanged()
    {
        UpdateInteractableAndFillAmount();
    }

    private void UpdateInteractableAndFillAmount()
    {
        var interactable = false;
        var fillAmount = 1.0f;
        if (_model.Available && _model.RestCooldown == 0)
        {
            interactable = true;
            fillAmount = 0.0f;
        }
        else
        {
            interactable = false;
            fillAmount = (float)_model.RestCooldown / (float)_model.Cooldown;
        }

        _commonCanvasGroup.interactable = interactable;
        _cooldownImage.fillAmount = fillAmount;
    }

    protected virtual void RestCooldownChanged()
    {
        UpdateInteractableAndFillAmount();
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
        if (!_model.IsCurrencyEnough)
        {
            ApplicationController.Instance.UIPanelController.PushPopupScreenAsync<UIShopPanel>(
                new UIShopPanelData()
                {
                    GameProcessor =  _model.GameProcessor,
                    Market = _model.GameProcessor.Market,
                    Items = UIShopPanel.FillShopItems(_model.GameProcessor)
                }, 
                Application.exitCancellationToken);
            return true;
        }

        return false;
    }
}

