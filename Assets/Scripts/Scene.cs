using System;
using Core.Steps.CustomOperations;
using UnityEngine;

public class Scene : MonoBehaviour, ISkinChanger, IHatsChanger
{
    [SerializeField] private SkinsLibrary _skinsLibrary;
    [SerializeField] private HatsLibrary _hatsLibrary;
    [SerializeField] private GameProcessor _gameProcessor;
    [SerializeField] private Field _field;
    [SerializeField] private Transform _sceneRoot;
    
    private SkinContainer _activeSkin;
    private string _activeHat;
    
    public GameProcessor GameProcessor => _gameProcessor;
    public Field Field => _field;
    public Transform SceneRoot => _sceneRoot;
    
    public SkinsLibrary SkinLibrary => _skinsLibrary;
    public SkinContainer ActiveSkin => _activeSkin;
    public HatsLibrary HatsLibrary => _hatsLibrary;
    public string ActiveHat => _activeHat;
    
    public void SetSkin(string skinName)
    {
        _activeSkin = _skinsLibrary.GetContainer(skinName);
        var skinChangeables = _field.gameObject.GetComponentsInChildren<ISkinChangeable>();
        foreach (var skinChangeable in skinChangeables)
            skinChangeable.ChangeSkin(_activeSkin);
    }

    public void Awake()
    {
        _activeSkin = _skinsLibrary.GetContainer("default");
        _activeHat = "none";
    }

    public void SetHat(string hatName)
    {
        var hat = _hatsLibrary.GetHat(hatName);
        var hatChangeables = _field.gameObject.GetComponentsInChildren<IHatChangeable>();
        foreach (var hatChangeable in hatChangeables)
            hatChangeable.ChangeHat(hat);
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