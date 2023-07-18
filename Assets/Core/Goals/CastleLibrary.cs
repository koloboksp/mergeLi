using System.Collections.Generic;
using Core.Goals;
using UnityEngine;

public class CastleLibrary : MonoBehaviour
{
    [SerializeField] private List<Castle> _castles = new List<Castle>();

    public IReadOnlyList<Castle> Castles => _castles;
    
    public Castle GetCastle(string castleName)
    {
        return _castles.Find(i => i.Name == castleName);
    }
}