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

    private int _points;
    private bool _selected;
    private Sprite _icon;

    public CastlePartView View => _view; 

    public Vector2Int GridPosition => _gridPosition;
    public int Cost => _cost;
    public int Points => _points;
    public bool Selected => _selected;
    public bool IsCompleted => _points >= _cost;
    public Sprite Icon => _icon;

    private void Awake()
    {
        _view.OnClick += OnClick;
    }

    public void Init(Castle owner, Vector2Int gridPosition, Sprite icon, int cost)
    {
        _owner = owner;
        _gridPosition = gridPosition;
        _cost = cost;
        OnCostChanged?.Invoke();
        _icon = icon;
        OnIconChanged?.Invoke();
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