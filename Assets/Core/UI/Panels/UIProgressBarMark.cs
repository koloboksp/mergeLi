using UnityEngine;

namespace Core
{
    public class UIProgressBarMark : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;

        public RectTransform Root => _root;
    }
}