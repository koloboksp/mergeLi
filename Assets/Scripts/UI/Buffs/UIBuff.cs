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
    private static readonly List<CanvasGroup> _noAllocFoundCanvasGroups = new(); 
    
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
            .OnRestCooldownChanged(RestCooldownChanged)
            .OnDragPhaseChanged(OnDragPhaseChanged);
       
        _onClick += _model.OnClick;
        _onBeginDrag += _model.OnBeginDrag;
        _onDrag += _model.OnDrag;
        _onEndDrag += _model.OnEndDrag;

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
    
    private void OnDragPhaseChanged(DragPhase dragPhase)
    {
        _iconPanel.SetActive(dragPhase == DragPhase.End);
    }
    
    private void SetCost()
    {
        _costLabel.text = _model.Cost.ToString();
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

    public void UnsubscribeModel()
    {
        _model
            .ReleaseAvailableStateChanged(AvailableStateChanged)
            .ReleaseRestCooldownChanged(RestCooldownChanged)
            .ReleaseDragPhaseChanged(OnDragPhaseChanged);
        
        _onClick -= _model.OnClick;
        _onBeginDrag -= _model.OnBeginDrag;
        _onDrag -= _model.OnDrag;
        _onEndDrag -= _model.OnEndDrag;
    }

    protected bool ParentGroupAllowsInteraction()
    {
        return ParentGroupAllowsInteraction(transform, _noAllocFoundCanvasGroups);
    }
    
    public static bool ParentGroupAllowsInteraction(Transform transform, List<CanvasGroup> canvasGroupCache)
    {
        var t = transform;
        while (t != null)
        {
            t.GetComponents(canvasGroupCache);
            for (var i = 0; i < canvasGroupCache.Count; i++)
            {
                if (canvasGroupCache[i].enabled && !canvasGroupCache[i].interactable)
                    return false;

                if (canvasGroupCache[i].ignoreParentGroups)
                    return true;
            }

            t = t.parent;
        }

        return true;
    }
}

