using UnityEngine;
using UnityEngine.EventSystems;

public class Buff : MonoBehaviour
{
    [SerializeField] protected GameProcessor _gameProcessor;
    [SerializeField] private UIBuff _controlPrefab;
    [SerializeField] private int _cost = 1;

    private UIBuff _control;
    public GameProcessor GameProcessor => _gameProcessor;
    public int Cost => _cost;
    
    public UIBuff CreateControl()
    {
        _control = Instantiate(_controlPrefab);
        _control
            .SetModel(this) 
            .OnClick(OnClick)
            .OnBeginDrag(OnBeginDrag)
            .OnEndDrag(OnEndDrag)
            .OnDrag(OnDrag);
        return _control;
    }
    
    protected void OnClick()
    {
        if(!IsAvailable) return;
        
        InnerOnClick();

        _gameProcessor.PlayerInfo.ConsumeCoins(_cost);
    }
    
    protected void OnEndDrag(PointerEventData eventData)
    {
        if(!IsAvailable) return;

        InnerOnEndDrag(eventData);
        
        _gameProcessor.PlayerInfo.ConsumeCoins(_cost);
    }

    protected void OnBeginDrag(PointerEventData eventData)
    {
        if(!IsAvailable) return;

        InnerOnBeginDrag(eventData);
    }
    
    protected void OnDrag(PointerEventData eventData)
    {
        if(!IsAvailable) return;

        InnerOnDrag(eventData);
    }
    
    protected virtual void InnerOnClick() { }
    protected virtual void InnerOnEndDrag(PointerEventData eventData) { }
    protected virtual void InnerOnBeginDrag(PointerEventData eventData) { }
    protected virtual void InnerOnDrag(PointerEventData eventData) { }

    public virtual bool IsAvailable => _gameProcessor.PlayerInfo.GetAvailableCoins() > _cost;
}