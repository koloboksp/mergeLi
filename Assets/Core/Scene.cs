using System;
using UnityEngine;

public class Scene : MonoBehaviour
{
    [SerializeField]
    private SkinsLibrary _skinsLibrary;

    private SkinContainer _activeSkin;
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