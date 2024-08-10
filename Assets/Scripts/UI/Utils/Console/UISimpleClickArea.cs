using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UISimpleClickArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public event Action<UISimpleClickArea> OnClick;

    public void OnPointerDown(PointerEventData eventData)
    {
            
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(this);
    }
}