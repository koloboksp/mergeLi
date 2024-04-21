using System;
using System.Collections.Generic;
using Core;
using Core.Steps.CustomOperations;
using Skins;
using UnityEngine;

public interface IScene
{
    SkinContainer ActiveSkin { get; }
    GameProcessor GameProcessor { get; }
    HatsLibrary HatsLibrary { get; }
    int[] ActiveHats { get; }
    public int[] UserInactiveHatsFilter { set; }
    public bool IsHatActive(int hatI);
}

public class Scene : MonoBehaviour, IScene, IHatsChanger
{
    [SerializeField] private SkinsLibrary _skinsLibrary;
    [SerializeField] private HatsLibrary _hatsLibrary;
    [SerializeField] private GameProcessor _gameProcessor;
    [SerializeField] private Field _field;
    [SerializeField] private Transform _sceneRoot;

    private readonly List<int> _userInactiveHatsFilter = new List<int>();
    private SkinContainer _activeSkin;
    private Hat _activeHat;
    
    public GameProcessor GameProcessor => _gameProcessor;
    public IField Field => _field;
    public Transform SceneRoot => _sceneRoot;
    
    public SkinsLibrary SkinLibrary => _skinsLibrary;
    public SkinContainer ActiveSkin => _activeSkin;
    public HatsLibrary HatsLibrary => _hatsLibrary;

    public int[] UserInactiveHatsFilter
    {
        get => _userInactiveHatsFilter.ToArray();
        set
        {
            _userInactiveHatsFilter.Clear();
            if (value != null)
                _userInactiveHatsFilter.AddRange(value);
        }
    }

    public void SetSkin(string skinName)
    {
        _activeSkin = _skinsLibrary.GetContainer(skinName);
        if (_activeSkin == null)
            _activeSkin = _skinsLibrary.GetDefaultSkin();
        
        var skinChangeables = _field.gameObject.GetComponentsInChildren<ISkinChangeable>();
        foreach (var skinChangeable in skinChangeables)
            skinChangeable.ChangeSkin(_activeSkin);
    }
    
    public int[] ActiveHats
    {
        get
        {
            var availableHats = _hatsLibrary.Hats;
            
            var activeHatIndexes = new List<int>();
            for (var hatI = 0; hatI < availableHats.Count; hatI++)
            {
                if (!availableHats[hatI].Available)
                    continue;
                if (_userInactiveHatsFilter != null && _userInactiveHatsFilter.Contains(hatI))
                    continue;
                
                activeHatIndexes.Add(hatI);
            }

            return activeHatIndexes.ToArray();
        }
    }

    public bool IsHatActive(int hatI)
    {
        var availableHats = _hatsLibrary.Hats;

        if (hatI >= availableHats.Count)
            return false;
        if (!availableHats[hatI].Available)
            return false;
        if (_userInactiveHatsFilter != null && _userInactiveHatsFilter.Contains(hatI))
            return false;

        return true;
    }

    public void SetUserInactiveHatsFilter(int[] userInactiveHatsFilter)
    {
        _userInactiveHatsFilter.Clear();
        if (userInactiveHatsFilter != null)
            _userInactiveHatsFilter.AddRange(userInactiveHatsFilter);

        ApplicationController.Instance.SaveController.SaveSettings.UserInactiveHatsFilter = userInactiveHatsFilter;
        
        var hatChangeables = _field.gameObject.GetComponentsInChildren<IHatChangeable>();
        foreach (var hatChangeable in hatChangeables)
            hatChangeable.ChangeUserInactiveHatsFilter();
    }
}

public interface ISkinChanger
{
    public void SetSkin(string skinName);
}

public interface IHatsChanger
{
  //  public void SetHat(string hatName);
    public void SetUserInactiveHatsFilter(int[] userInactiveHatsFilter);
}