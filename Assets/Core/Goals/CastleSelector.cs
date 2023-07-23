using System;
using Core.Goals;
using UnityEngine;

public class CastleSelector : MonoBehaviour
{
    public event Action OnCastleCompleted;
    
    [SerializeField] private GameProcessor _gameProcessor;
    [SerializeField] private CastleLibrary _library;
    [SerializeField] private Transform _castleRoot;
    
    private Castle _castleInstance;
    
    public CastleLibrary Library => _library;
    public Castle ActiveCastle => _castleInstance;
    
    private void PlayerInfo_OnCastleChanged()
    {
        if (_castleInstance != null)
        {
            _castleInstance.OnCompleted -= CastleInstance_OnCompleted;
            Destroy(_castleInstance.gameObject);
            _castleInstance = null;
        }
        
        var lastSelectedCastle = _gameProcessor.PlayerInfo.GetLastSelectedCastle();
        var castlePrefab = _library.GetCastle(lastSelectedCastle);
 
        _castleInstance = Instantiate(castlePrefab, _castleRoot);
        _castleInstance.gameObject.name = castlePrefab.Name;
        _castleInstance.View.Root.anchorMin = Vector2.zero;
        _castleInstance.View.Root.anchorMax = Vector2.one;
        _castleInstance.View.Root.offsetMin = Vector2.zero;
        _castleInstance.View.Root.offsetMax = Vector2.zero;
        _castleInstance.View.Root.localScale = Vector3.one;
        
        _castleInstance.Init(_gameProcessor);
        _castleInstance.OnCompleted += CastleInstance_OnCompleted;
    }

    private void CastleInstance_OnCompleted()
    {
        OnCastleCompleted?.Invoke();
    }

    public void Init()
    {
        _gameProcessor.PlayerInfo.OnCastleChanged += PlayerInfo_OnCastleChanged;
        PlayerInfo_OnCastleChanged();
    }
}