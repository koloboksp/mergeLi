using System.Collections.Generic;
using UnityEngine;

public class HatsLibrary : MonoBehaviour
{
    [SerializeField] private List<SkinContainer> _skinContainers;

    public IEnumerable<SkinContainer> Containers => _skinContainers;
    
    public SkinContainer GetContainer(string skinName)
    {
        return _skinContainers.Find(i => i.Name == skinName);
    }
}