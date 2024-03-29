﻿using System;
using Core.Effects;
using Core.Goals;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CastleSelector : MonoBehaviour
{
    public event Action OnCastleCompleted;
    public event Action<Castle> OnCastleChanged;
    
    [SerializeField] private GameProcessor _gameProcessor;
    [SerializeField] private CastleLibrary _library;
    [SerializeField] private Transform _castleRoot;
    
    [SerializeField] private Material _selectionMaterial;

    private Castle _castleInstance;
    
    public CastleLibrary Library => _library;
    public Castle ActiveCastle => _castleInstance;
    
    //private void PlayerInfo_OnCastleChanged()
    //{
    //    if (_castleInstance != null)
    //    {
    //        _castleInstance.OnCompleted -= CastleInstance_OnCompleted;
    //        _castleInstance.OnPartSelected -= CastleInstance_OnPartSelected;
    //        Destroy(_castleInstance.gameObject);
    //        _castleInstance = null;
    //    }
    //    
    //    var lastSelectedCastle = _gameProcessor.PlayerInfo.GetLastSelectedCastle();
    //    var castlePrefab = _library.GetCastle(lastSelectedCastle);
 //
    //    _castleInstance = Instantiate(castlePrefab, _castleRoot);
    //    _castleInstance.gameObject.name = castlePrefab.Id;
    //    _castleInstance.View.Root.anchorMin = Vector2.zero;
    //    _castleInstance.View.Root.anchorMax = Vector2.one;
    //    _castleInstance.View.Root.offsetMin = Vector2.zero;
    //    _castleInstance.View.Root.offsetMax = Vector2.zero;
    //    _castleInstance.View.Root.localScale = Vector3.one;
    //    
    //    _castleInstance.Init(_gameProcessor);
    //    
    //    _castleInstance.OnCompleted += CastleInstance_OnCompleted;
    //    _castleInstance.OnPartSelected += CastleInstance_OnPartSelected;
    //    CastleInstance_OnPartSelected();
    //}

    private CoinsEffectReceiver _coinsEffectReceiver;

    private void CastleInstance_OnPartSelected()
    {
        if (_coinsEffectReceiver != null)
        {
            Destroy(_coinsEffectReceiver);
            _coinsEffectReceiver = null;
        }

        var selectedCastlePart = _castleInstance.GetSelectedCastlePart();
        if (selectedCastlePart != null)
        {
            _coinsEffectReceiver = selectedCastlePart.AddComponent<CoinsEffectReceiver>();
        }
    }
    
    private void CastleInstance_OnCompleted()
    {
        OnCastleCompleted?.Invoke();
        OnCastleChanged?.Invoke(null);
    }

    public void Init()
    {
       // _gameProcessor.PlayerInfo.OnCastleChanged += PlayerInfo_OnCastleChanged;
       // PlayerInfo_OnCastleChanged();
    }

    public void SelectActiveCastle(string id)
    {
        if (_castleInstance != null)
        {
            _castleInstance.OnCompleted -= CastleInstance_OnCompleted;
            _castleInstance.OnPartSelected -= CastleInstance_OnPartSelected;
            Destroy(_castleInstance.gameObject);
            _castleInstance = null;
        }
        
        var castlePrefab = _library.GetCastle(id);
 
        _castleInstance = Instantiate(castlePrefab, _castleRoot);
        _castleInstance.gameObject.name = castlePrefab.Id;
        _castleInstance.View.Root.anchorMin = Vector2.zero;
        _castleInstance.View.Root.anchorMax = Vector2.one;
        _castleInstance.View.Root.offsetMin = Vector2.zero;
        _castleInstance.View.Root.offsetMax = Vector2.zero;
        _castleInstance.View.Root.localScale = Vector3.one;
        
        _castleInstance.Init(_gameProcessor);
        
        _castleInstance.OnCompleted += CastleInstance_OnCompleted;
        _castleInstance.OnPartSelected += CastleInstance_OnPartSelected;
        CastleInstance_OnPartSelected();
        
        OnCastleChanged?.Invoke(null);
    }

    public void ForceCompleteCastle()
    {
        _castleInstance.ForceComplete();
    }   
}