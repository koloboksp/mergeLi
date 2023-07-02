using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIBuff : MonoBehaviour
{
    protected Action _onClick;
    protected Action<PointerEventData> _onBeginDrag;
    protected Action<PointerEventData> _onEndDrag;
    protected Action<PointerEventData> _onDrag;

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
}

