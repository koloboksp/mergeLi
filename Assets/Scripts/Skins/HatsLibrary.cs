using System.Collections.Generic;
using Core;
using Core.Gameplay;
using UnityEngine;

namespace Skins
{
    public class HatsLibrary : MonoBehaviour
    {
        [SerializeField] private List<Hat> _hats;
        [SerializeField] private List<HatGroup> _hatGroups;

        public IReadOnlyList<Hat> Hats => _hats;
        public IReadOnlyList<HatGroup> HatGroups => _hatGroups;

        public Hat GetHat(string hatName)
        {
            return _hats.Find(i => i.Id == hatName);
        }

        public HatGroup GetHatGroup(int index)
        {
            return _hatGroups[index];
        }
        
        public Hat GetDefaultHat()
        {
            return _hats[0];
        }
    }
}