using System.Collections.Generic;
using Core;
using Core.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDragDropBuff : UIBuff, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private AudioClip _onBeginDragClip;
    
    private bool _showShopScreenRequired;
    private bool _parentGroupAllowsInteraction;
    private DependencyHolder<SoundsPlayer> _soundPlayer;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _parentGroupAllowsInteraction = ParentGroupAllowsInteraction();
        if (!_parentGroupAllowsInteraction)
            return;
        
        if (!_model.Available)
        {
            _soundPlayer.Value.Play(UICommonSounds.Unavailable);
            return;
        }

        _showShopScreenRequired = ShowShopScreenRequired();
        if(_showShopScreenRequired)
            return;
        
        _soundPlayer.Value.Play(_onBeginDragClip);
        _onBeginDrag?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_parentGroupAllowsInteraction)
            return;
        
        if(!_model.Available)
            return;
        
        if(_showShopScreenRequired) 
            return;
        
        _onEndDrag?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_parentGroupAllowsInteraction)
            return;
        if (!_model.Available)
            return;
        if (_showShopScreenRequired)
            return;
        
        _onDrag?.Invoke(eventData);
    }
}