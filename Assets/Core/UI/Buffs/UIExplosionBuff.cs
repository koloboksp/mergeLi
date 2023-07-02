using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIExplosionBuff : UIBuff, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Button _clickArea;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _onBeginDrag?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _onEndDrag?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _onDrag?.Invoke(eventData);
    }
}