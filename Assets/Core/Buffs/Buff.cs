using System;
using Core.Steps;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Buff : MonoBehaviour
{
    private Action AvailableStateChanged;
    private Action RestCooldownChanged;

    [SerializeField] protected GameProcessor _gameProcessor;
    [SerializeField] private UIBuff _controlPrefab;
    [SerializeField] private int _cost = 1;
    [SerializeField] private int _cooldown = 3;

    private UIBuff _control;
    private bool _available = true;
    private int _restCooldown;

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

        ProcessUsing();
    }
    
    protected void OnBeginDrag(PointerEventData eventData)
    {
        if(!IsAvailable) return;

        InnerOnBeginDrag(eventData);
    }
    
    protected void OnEndDrag(PointerEventData eventData)
    {
        if(!IsAvailable) return;

        InnerOnEndDrag(eventData);

        ProcessUsing();
    }
    
    protected void OnDrag(PointerEventData eventData)
    {
        if(!IsAvailable) return;

        InnerOnDrag(eventData);
    }

    void ProcessUsing()
    {
        _restCooldown = _cooldown;
        RestCooldownChanged?.Invoke();
        
        if (_restCooldown != 0)
        {
            Available = false;
            _gameProcessor.OnStepCompleted += GameProcessor_OnStepCompleted;
        }
        else
        {
            Available = true;
        }
        _gameProcessor.PlayerInfo.ConsumeCoins(_cost);
    }
    protected virtual void InnerOnClick() { }
    protected virtual void InnerOnEndDrag(PointerEventData eventData) { }
    protected virtual void InnerOnBeginDrag(PointerEventData eventData) { }
    protected virtual void InnerOnDrag(PointerEventData eventData) { }

    public bool Available
    {
        get => _available;
        set
        {
            if (_available != value)
            {
                _available = value;
                AvailableStateChanged?.Invoke();
            }
        }
    }
    public virtual bool IsAvailable => _gameProcessor.PlayerInfo.GetAvailableCoins() >= _cost;
    public int Cooldown => _cooldown;
    public int RestCooldown => _restCooldown;


    private void GameProcessor_OnStepCompleted(Step step)
    {
        if (step.Tag == "Move"
            || step.Tag == "Merge")
        {
            if (_restCooldown != 0)
            {
                _restCooldown--;
                Inner_OnRestCooldownChanged();
                RestCooldownChanged?.Invoke();
            }
            
            if (_restCooldown == 0)
            {
                _gameProcessor.OnStepCompleted -= GameProcessor_OnStepCompleted;
                Available = true;
            }
        }
    }

    protected virtual void Inner_OnRestCooldownChanged() { }
    
    public Buff OnAvailableStateChanged(Action onChanged)
    {
        AvailableStateChanged = onChanged;
        return this;
    }
    
    public Buff OnRestCooldownChanged(Action onChanged)
    {
        RestCooldownChanged = onChanged;
        return this;
    }
}