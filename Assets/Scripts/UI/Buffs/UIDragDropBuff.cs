using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDragDropBuff : UIBuff, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private bool _showShopScreenRequired;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!_model.Available) return;
        _showShopScreenRequired = ShowShopScreenRequired();
        if(_showShopScreenRequired) return;
        
        _onBeginDrag?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(!_model.Available) return;
        if(_showShopScreenRequired) return;
        
        _onEndDrag?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(!_model.Available) return;
        if(_showShopScreenRequired) return;
        
        _onDrag?.Invoke(eventData);
    }
}