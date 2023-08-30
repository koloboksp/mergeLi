using System;
using Core.Buffs;
using Core.Steps;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Buff : MonoBehaviour
{
    protected Action _availableStateChanged;
    private Action _restCooldownChanged;

    [SerializeField] protected GameProcessor _gameProcessor;
    [SerializeField] private UIBuff _controlPrefab;
    [SerializeField] private int _cost = 1;
    [SerializeField] private int _cooldown = 3;

    private UIBuff _control;
    private bool _available = true;
    private int _restCooldown;

    public GameProcessor GameProcessor => _gameProcessor;
    public int Cost => _cost;

    public void Awake()
    {
        _gameProcessor.OnStepCompleted += GameProcessor_OnStepCompleted;
    }

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
        if(!IsCurrencyEnough) return;
        if(!Available) return;

        InnerOnClick();

        ProcessUsing();
    }
    
    protected void OnBeginDrag(PointerEventData eventData)
    {
        if(!IsCurrencyEnough) return;
        if(!Available) return;

        InnerOnBeginDrag(eventData);
    }
    
    protected void OnEndDrag(PointerEventData eventData)
    {
        if(!IsCurrencyEnough) return;
        if(!Available) return;

        if(InnerOnEndDrag(eventData))
            ProcessUsing();
    }
    
    protected void OnDrag(PointerEventData eventData)
    {
        if(!IsCurrencyEnough) return;
        if(!Available) return;

        InnerOnDrag(eventData);
    }

    private void ProcessUsing()
    {
        InnerProcessUsing();
        
        _restCooldown = _cooldown;
        _restCooldownChanged?.Invoke();
        
        if (_restCooldown != 0)
            Available = false;
        else
            Available = true;
    }

    protected abstract void InnerProcessUsing();
    
    protected virtual void InnerOnClick() { }

    protected virtual bool InnerOnEndDrag(PointerEventData eventData)
    {
        return true;
    }
    
    protected virtual void InnerOnBeginDrag(PointerEventData eventData) { }
    protected virtual void InnerOnDrag(PointerEventData eventData) { }

    public virtual bool Available
    {
        get => _available;
        set
        {
            if (_available != value)
            {
                _available = value;
                _availableStateChanged?.Invoke();
            }
        }
    }
    
    public virtual bool IsCurrencyEnough => _gameProcessor.PlayerInfo.GetAvailableCoins() >= _cost;

    public int Cooldown => _cooldown;
    public int RestCooldown => _restCooldown;
    
    private void GameProcessor_OnStepCompleted(Step step)
    {
        if (step.Tag == GameProcessor.MoveStepTag
            || step.Tag == GameProcessor.MergeStepTag)
        {
            if (_restCooldown != 0)
            {
                _restCooldown--;
                Inner_OnRestCooldownChanged();
                _restCooldownChanged?.Invoke();
                
                if (_restCooldown == 0)
                    Available = true;
            }
        }
        Inner_OnStepCompleted(step);
    }

    protected virtual void Inner_OnRestCooldownChanged() { }
    protected virtual void Inner_OnStepCompleted(Step step) { }

    public Buff OnAvailableStateChanged(Action onChanged)
    {
        _availableStateChanged = onChanged;
        return this;
    }
    
    public Buff OnRestCooldownChanged(Action onChanged)
    {
        _restCooldownChanged = onChanged;
        return this;
    }
}