using System.Collections.Generic;
using UnityEngine;

namespace Core.Market
{
    public class PurchasesLibrary : MonoBehaviour
    {
        [SerializeField] private List<PurchaseItem> _items;

        public List<PurchaseItem> Items => _items;
    }
}