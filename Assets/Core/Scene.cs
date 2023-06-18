using System;
using UnityEngine;

public class Scene : MonoBehaviour, ISkinChanger
{
    [SerializeField]
    private SkinsLibrary _skinsLibrary;

    private SkinContainer _activeSkin;


    public SkinsLibrary Library => _skinsLibrary;
    public SkinContainer ActiveSkin => _activeSkin;

    public void SetSkin(string skinName)
    {
        _activeSkin = _skinsLibrary.GetContainer(skinName);
        var skinChangeables = this.GetComponents<ISkinChangeable>();
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