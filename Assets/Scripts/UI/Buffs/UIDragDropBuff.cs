using Core;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDragDropBuff : UIBuff, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private AudioClip _onBeginDragClip;
    
    private bool _showShopScreenRequired;
    private DependencyHolder<SoundsPlayer> _soundPlayer;

    public void OnBeginDrag(PointerEventData eventData)
    {
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
        if(!_model.Available)
            return;
        if(_showShopScreenRequired) 
            return;
        
        _onEndDrag?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!_model.Available) return;
        if(_showShopScreenRequired) return;
        
        _onDrag?.Invoke(eventData);
    }
}