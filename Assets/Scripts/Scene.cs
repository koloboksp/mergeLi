using System;
using Core;
using Core.Steps.CustomOperations;
using Skins;
using UnityEngine;

public interface IScene
{
    SkinContainer ActiveSkin { get; }
    GameProcessor GameProcessor { get; }
    Hat ActiveHat { get; }
}

public class Scene : MonoBehaviour, IScene
{
    [SerializeField] private SkinsLibrary _skinsLibrary;
    [SerializeField] private HatsLibrary _hatsLibrary;
    [SerializeField] private GameProcessor _gameProcessor;
    [SerializeField] private Field _field;
    [SerializeField] private Transform _sceneRoot;
    
    private SkinContainer _activeSkin;
    private Hat _activeHat;
    
    public GameProcessor GameProcessor => _gameProcessor;
    public Field Field => _field;
    public Transform SceneRoot => _sceneRoot;
    
    public SkinsLibrary SkinLibrary => _skinsLibrary;
    public SkinContainer ActiveSkin => _activeSkin;
    public HatsLibrary HatsLibrary => _hatsLibrary;
    public Hat ActiveHat => _activeHat;
    
    public void SetSkin(string skinName)
    {
        _activeSkin = _skinsLibrary.GetContainer(skinName);
        if (_activeSkin == null)
            _activeSkin = _skinsLibrary.GetDefaultSkin();
        
        var skinChangeables = _field.gameObject.GetComponentsInChildren<ISkinChangeable>();
        foreach (var skinChangeable in skinChangeables)
            skinChangeable.ChangeSkin(_activeSkin);
    }
    
    public void SetHat(string hatName)
    {
        _activeHat = _hatsLibrary.GetHat(hatName);
        if (_activeHat == null)
            _activeHat = _hatsLibrary.GetDefaultHat();
        
        var hatChangeables = _field.gameObject.GetComponentsInChildren<IHatChangeable>();
        foreach (var hatChangeable in hatChangeables)
            hatChangeable.ChangeHat(_activeHat);
    }
}

public interface ISkinChanger
{
    public void SetSkin(string skinName);
}

public interface IHatsChanger
{
    public void SetHat(string hatName);
}