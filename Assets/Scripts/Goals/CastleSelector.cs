using System;
using System.Collections.Generic;
using Core.Effects;
using Core.Goals;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CastleSelector : MonoBehaviour
{
    public event Action<Castle> OnCastleCompleted;
    public event Action<Castle> OnCastleChanged;
    
    [SerializeField] private GameProcessor _gameProcessor;
    [SerializeField] private CastleLibrary _library;
    [SerializeField] private Transform _castleRoot;
    
    [SerializeField] private Material _selectionMaterial;

    private Castle _castleInstance;
    private List<string> _completedInSessionCastles = new List<string>();
    
    public CastleLibrary Library => _library;
    public Castle ActiveCastle => _castleInstance;
    
    private void CastleInstance_OnCompleted(Castle castle)
    {
        try
        {
            _completedInSessionCastles.Add(castle.Id);
            OnCastleCompleted?.Invoke(castle);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public int GetEarnedPoints()
    {
        var earnedPoints = 0;
        
        if (ActiveCastle != null)
        {
            earnedPoints += ActiveCastle.GetPoints();
        }

        return earnedPoints;
    }
    
    public void SetData()
    {
      
    }

    public void SelectActiveCastle(string id)
    {
        if (_castleInstance != null)
        {
            _castleInstance.OnCompleted -= CastleInstance_OnCompleted;
            Destroy(_castleInstance.gameObject);
            _castleInstance = null;
        }
        
        var castlePrefab = _library.GetCastle(id);
        if (castlePrefab == null)
            castlePrefab = _library.Castles[0];
        
        _castleInstance = Instantiate(castlePrefab, _castleRoot);
        _castleInstance.gameObject.name = castlePrefab.Id;
        _castleInstance.View.Root.anchorMin = Vector2.zero;
        _castleInstance.View.Root.anchorMax = Vector2.one;
        _castleInstance.View.Root.offsetMin = Vector2.zero;
        _castleInstance.View.Root.offsetMax = Vector2.zero;
        _castleInstance.View.Root.localScale = Vector3.one;
        
        _castleInstance.SetData(_gameProcessor);
        
        _castleInstance.OnCompleted += CastleInstance_OnCompleted;
       
        OnCastleChanged?.Invoke(null);
    }

    public void ForceCompleteCastle()
    {
        _castleInstance.ForceComplete();
    }   
}