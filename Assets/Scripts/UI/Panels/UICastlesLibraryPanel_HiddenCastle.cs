using UnityEngine;

namespace Core
{
    public class UICastlesLibraryPanel_HiddenCastle : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        
        public RectTransform Root => _root;
    }
}