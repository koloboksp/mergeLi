using System;
using System.Collections.Generic;
using Core;
using Core.Effects;
using Core.Gameplay;
using Core.Goals;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CastleSelector : MonoBehaviour
{
    public event Action<Castle> OnCastleChanged;
    
    [SerializeField] private GameProcessor _gameProcessor;
    [SerializeField] private CastleLibrary _library;
    [SerializeField] private Transform _castleRoot;
    
    [SerializeField] private Material _selectionMaterial;

    private Castle _castleInstance;
   
    public CastleLibrary Library => _library;
    public Castle ActiveCastle => _castleInstance;
    
    public void SetData()
    {
      
    }

    public void SelectActiveCastle(string id)
    {
        if (_castleInstance != null)
        {
            Destroy(_castleInstance.gameObject);
            _castleInstance = null;
        }
        
        var castlePrefab = _library.GetCastle(id);
        if (castlePrefab == null)
            castlePrefab = _library.Castles[0];
        
        _castleInstance = Instantiate(castlePrefab, _castleRoot);
        _castleInstance.gameObject.name = castlePrefab.Id;
        // _castleInstance.View.Root.anchorMin = Vector2.zero;
        // _castleInstance.View.Root.anchorMax = Vector2.one;
        // _castleInstance.View.Root.offsetMin = Vector2.zero;
        // _castleInstance.View.Root.offsetMax = Vector2.zero;
        // _castleInstance.View.Root.localScale = Vector3.one;
        
        _castleInstance.SetData(_gameProcessor);
        
        OnCastleChanged?.Invoke(null);
    }

    public void ForceCompleteCastle()
    {
        _castleInstance.ForceComplete();
    }   
}