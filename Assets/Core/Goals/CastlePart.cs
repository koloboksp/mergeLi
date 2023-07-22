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
    public bool IsCompleted => _points >= _cost;
    public Sprite Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            OnIconChanged?.Invoke();
        }
    }

    private void Awake()
    {
        _view.OnClick += OnClick;
    }
    
    public void ApplyProgress(CastlePartProgress partProgress)
    {
        _points = partProgress.IsCompleted ? _cost : 0;
        OnProgressChanged?.Invoke();
    }

    public void OnClick()
    {
        _owner.SelectPart(this);
    }

    public void Select(bool state)
    {
        _selected = state;
        OnSelectedStateChanged?.Invoke();
    }

    public int AddPoints(int additionalPoints)
    {
        var restPoints = (_points + additionalPoints) - _cost;
        if (restPoints > 0)
        {
            _points = _cost;
            OnProgressChanged?.Invoke();
            return restPoints;
        }
        
        _points += additionalPoints;
        OnProgressChanged?.Invoke();
        return 0;
    }

    
}