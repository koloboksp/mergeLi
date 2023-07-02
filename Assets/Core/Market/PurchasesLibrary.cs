using System.Collections.Generic;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class PurchasesLibrary : MonoBehaviour
    {
        [SerializeField] private List<PurchaseItem> _items;

        public List<PurchaseItem> Items => _items;
    }
}