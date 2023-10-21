using UnityEngine;

public class CastlePartView : MonoBehaviour
{
    [SerializeField] private CastlePart _model;
    [SerializeField] private CastleViewer _castleView;
    [SerializeField] private RectTransform _root;
    
    public RectTransform Root => _root;
    
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
                _castleView.ShowPartBorn(_model.Index, instant);
            else
                _castleView.ShowPartDeath(_model.Index, instant);
            
        }
    }
    
    private void OnPointsChanged(int oldPoints, bool instant)
    {
        if (_model.Points < _model.Cost)
        {
            _castleView.ShowPartProgress(_model.Index, (float)oldPoints/_model.Cost, (float)_model.Points/_model.Cost, instant);
        }
        else
        {
            _castleView.ShowPartComplete(_model.Index, instant);
        }
    }
}