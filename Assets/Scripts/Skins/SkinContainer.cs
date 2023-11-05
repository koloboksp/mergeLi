using System.Collections.Generic;
using Core;
using UnityEngine;

public class SkinContainer : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private List<Skin> _skins;
    
    public string Name => _name;
    
    public Skin GetSkin(string name)
    {
        return _skins.Find(i => i.Name == name);
    }
}