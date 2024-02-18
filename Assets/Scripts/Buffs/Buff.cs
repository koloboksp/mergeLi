using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Buffs;
using Core.Steps;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IBuff
{
    string Id { get; }
    int GetRestCooldown();
}

public abstract class Buff : MonoBehaviour, IBuff
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
        _gameProcessor.OnUndoStepsClear += GameProcessor_OnUndoStepsClear;
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

        var readyToUse = InnerOnClick();
        if(readyToUse)
            ProcessUsing(null);
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

        var readyToUse = InnerOnEndDrag(eventData);
        if(readyToUse)
            ProcessUsing(eventData);
    }
    
    protected void OnDrag(PointerEventData eventData)
    {
        if(!IsCurrencyEnough) return;
        if(!Available) return;

        InnerOnDrag(eventData);
    }

    private void ProcessUsing(PointerEventData eventData)
    {
        _restCooldown = _cooldown;
        _restCooldownChanged?.Invoke();
        
        if (_restCooldown != 0)
            Available = false;
        else
            Available = true;
        
        InnerProcessUsing(eventData);
    }

    protected abstract void InnerProcessUsing(PointerEventData pointerEventData);

    protected virtual bool InnerOnClick()
    {
        return true;
    }

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
    
    public virtual bool IsCurrencyEnough => ApplicationController.Instance.SaveController.GetAvailableCoins() >= _cost;

    public int Cooldown => _cooldown;
    public int RestCooldown => _restCooldown;
    
    private void GameProcessor_OnStepCompleted(Step step, StepExecutionType executionType)
    {
        Inner_OnStepCompleted(step);
    }

    private void GameProcessor_OnUndoStepsClear()
    {
        Inner_OnUndoStepsClear();
    }
    
    internal void ConsumeCooldown(int stepValue)
    {
        _restCooldown -= stepValue;
        if (_restCooldown > _cooldown)
            _restCooldown = 0;
        if (_restCooldown < 0)
            _restCooldown = 0;
        
        Inner_OnRestCooldownChanged();
        _restCooldownChanged?.Invoke();

        Available = _restCooldown == 0;
    }
    
    protected virtual void Inner_OnRestCooldownChanged() { }
    protected virtual void Inner_OnStepCompleted(Step step) { }
    protected virtual void Inner_OnUndoStepsClear() { }

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
    
    public abstract string Id { get; }
    
    public int GetRestCooldown()
    {
        return _restCooldown;
    }

    public void SetRestCooldown(int restCooldown)
    {
        _restCooldown = restCooldown;
        
        Inner_OnRestCooldownChanged();
        _restCooldownChanged?.Invoke();

        Available = _restCooldown == 0;
    }
}