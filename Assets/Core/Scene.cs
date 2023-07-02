using System;
using Core.Steps.CustomOperations;
using UnityEngine;

public class Scene : MonoBehaviour, ISkinChanger
{
    [SerializeField] private SkinsLibrary _skinsLibrary;
    [SerializeField] private Field _field;
    [SerializeField] private Transform _sceneRoot;
    [SerializeField] private PurchasesLibrary _purchasesLibrary;

    private SkinContainer _activeSkin;
    public Field Field => _field;
    public Transform SceneRoot => _sceneRoot;
    
    public SkinsLibrary Library => _skinsLibrary;
    public SkinContainer ActiveSkin => _activeSkin;
    public PurchasesLibrary PurchasesLibrary => _purchasesLibrary;

    public void SetSkin(string skinName)
    {
        _activeSkin = _skinsLibrary.GetContainer(skinName);
        var skinChangeables = this.GetComponentsInChildren<ISkinChangeable>();
        foreach (var skinChangeable in skinChangeables)
        {
            skinChangeable.ChangeSkin(_activeSkin);
        }
    }

    public void Awake()
    {
        _activeSkin = _skinsLibrary.GetContainer("default");
    }
}

public interface ISkinChanger
{
    public void SetSkin(string skinName);
}