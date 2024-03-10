using UnityEngine;

public class CastlePartView : MonoBehaviour
{
    [SerializeField] private CastlePart _model;
    
    private void Awake()
    {
        _model.OnCostChanged += OnCostChanged;
        OnCostChanged();
        
        _model.OnUnlockedStateChanged += OnUnlockedStateChanged;
        OnUnlockedStateChanged(_model.Unlocked, true);

        _model.OnPointsChanged += OnPointsChanged;
        OnPointsChanged(_model.Points, true);
    }
    
    private void OnCostChanged()
    {
      
    }
    
    private void OnUnlockedStateChanged(bool oldUnlocked, bool instant)
    {
        if (oldUnlocked != _model.Unlocked)
        {
            if(_model.Unlocked)
                _model.Owner.View.ShowPartBorn(_model.Index, instant);
            else
                _model.Owner.View.ShowPartDeath(_model.Index, instant);
        }
    }
    
    private void OnPointsChanged(int oldPoints, bool instant)
    {
        _model.Owner.View.ShowPartProgress(_model.Index, oldPoints, _model.Points, _model.Cost, instant);
        if (_model.Points == _model.Cost)
            _model.Owner.View.ShowPartComplete(_model.Index, instant);
    }
}