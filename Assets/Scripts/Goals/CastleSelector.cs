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

    private Castle _castle;
   
    public CastleLibrary Library => _library;
    public Castle ActiveCastle => _castle;
    
    public void SetData()
    {
      
    }

    public void SelectActiveCastle(string id)
    {
        var previousCastle = _castle;
        if (previousCastle != null)
        {
            Destroy(previousCastle.gameObject);
            previousCastle = null;
        }
        
        var castlePrefab = _library.GetCastle(id);
        if (castlePrefab != null)
        {
            _castle = Instantiate(castlePrefab, _castleRoot);
            _castle.gameObject.name = castlePrefab.Id;
            _castle.SetData(_gameProcessor);
        }
        else
        {
            _castle = null;
        }
        
        OnCastleChanged?.Invoke(previousCastle);
    }

    public void ForceCompleteCastle()
    {
        _castle.ForceComplete();
    }   
}