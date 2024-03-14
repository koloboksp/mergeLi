using System.Collections.Generic;
using UnityEngine;

public class HatsLibrary : MonoBehaviour
{
    [SerializeField] private List<Hat> _hats;

    public IEnumerable<Hat> Hats => _hats;
    
    public Hat GetHat(string hatName)
    {
        return _hats.Find(i => i.Name == hatName);
    }

    public Hat GetDefaultHat()
    {
        return _hats[0];
    }
}