using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Steps.CustomOperations
{
    public class PurchasesLibrarySceneReference : MonoBehaviour
    {
        [SerializeField] private PurchasesLibrary _reference;

        public PurchasesLibrary Reference => _reference;
    }
}