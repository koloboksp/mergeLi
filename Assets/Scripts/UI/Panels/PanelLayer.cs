using UnityEngine;

namespace Core
{
    public class PanelLayer : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;

        public RectTransform Root => _root;
    }
}