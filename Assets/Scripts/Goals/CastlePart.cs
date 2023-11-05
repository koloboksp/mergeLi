using System;
using Core.Goals;
using UnityEngine;

public class CastlePart : MonoBehaviour
{
    public event Action OnIconChanged;
    public event Action OnCostChanged;
    public event Action<int, bool> OnPointsChanged;
    public event Action<bool, bool> OnUnlockedStateChanged;
    public event Action OnSelectedStateChanged;

    [SerializeField] private Castle _owner;
    [SerializeField] private int _index;
    [SerializeField] private int _cost;
    [SerializeField] private CastlePartView _view;

    private bool _unlocked;
    private int _points;
    private bool _selected;
    
    public CastlePartView View => _view;

    public Castle Owner
    {
        get => _owner;
        set => _owner = value;
    }
    
    public int Cost
    {
        get => _cost;
        set
        {
            _cost = value;
            OnCostChanged?.Invoke();
        }
    }

    public bool Unlocked => _unlocked;
    public int Points => _points;
    public bool Selected => _selected;
    public int Index => _index;
    
    public void Select(bool state)
    {
        if (_selected != state)
        {
            _selected = state;
            OnSelectedStateChanged?.Invoke();
        }
    }

    public void ChangeUnlockState(bool unlocked, bool instant)
    {
        if (_unlocked != unlocked)
        {
            var oldUnlocked = _unlocked;
            _unlocked = unlocked;
            
            OnUnlockedStateChanged?.Invoke(oldUnlocked, instant);
        } 
    }

    public void SetPoints(int points, bool instant)
    {
        if (_points != points)
        {
            var oldPoints = _points;
            _points = points;
            OnPointsChanged?.Invoke(oldPoints, instant);
        }
    }
}