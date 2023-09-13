using System;
using Core.Goals;
using UnityEngine;

public class CastlePart : MonoBehaviour
{
    public event Action OnIconChanged;
    public event Action OnCostChanged;
    public event Action OnProgressChanged;
    public event Action OnSelectedStateChanged;

    [SerializeField] private Castle _owner;
    [SerializeField] private Vector2Int _gridPosition;
    [SerializeField] private int _cost;
    [SerializeField] private CastlePartView _view;
    [SerializeField] private Sprite _icon;
    private int _points;
    private bool _selected;
    
    public CastlePartView View => _view;

    public Castle Owner
    {
        get => _owner;
        set => _owner = value;
    }

    public Vector2Int GridPosition
    {
        get => _gridPosition;
        set => _gridPosition = value;
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

    public int Points => _points;
    public bool Selected => _selected;
    
    public Sprite Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            OnIconChanged?.Invoke();
        }
    }
    
    public void Select(bool state)
    {
        if (_selected != state)
        {
            _selected = state;
            OnSelectedStateChanged?.Invoke();
        }
    }

    public void SetPoints(int points)
    {
        if (_points == points) return;
        
        _points = points;
        OnProgressChanged?.Invoke();
    }
    
    public void Complete()
    {
        SetPoints(_cost);
    }
}