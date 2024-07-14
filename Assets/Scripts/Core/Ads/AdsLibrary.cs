using System.Collections.Generic;
using UnityEngine;

namespace Core.Ads
{
    public class AdsLibrary : MonoBehaviour
    {
        [SerializeField] private List<AdsItem> _items;
        public IReadOnlyList<AdsItem> Items => _items;
    }
}