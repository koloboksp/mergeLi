using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIExplosionBuff : UIBuff, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Button _clickArea;
    private bool _showShopScreenRequired;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _showShopScreenRequired = ShowShopScreenRequired();
        if(_showShopScreenRequired) return;

        _onBeginDrag?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(_showShopScreenRequired) return;
        
        _onEndDrag?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(_showShopScreenRequired) return;

        _onDrag?.Invoke(eventData);
    }
}