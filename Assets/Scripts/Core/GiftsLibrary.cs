using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class GiftsLibrary : MonoBehaviour
    {
        [SerializeField] private List<GiftItem> _items;
        public IReadOnlyList<GiftItem> Items => _items;
    }
}