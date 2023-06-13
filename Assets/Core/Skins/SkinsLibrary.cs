using System.Collections.Generic;
using UnityEngine;

public class SkinsLibrary : MonoBehaviour
{
    [SerializeField] private List<SkinContainer> _skinContainers;

    public SkinContainer GetContainer(string skinName)
    {
        return _skinContainers.Find(i => i.Name == skinName);
    }
}