using UnityEngine;

namespace Core.Tutorials
{
    public class UITutorialElement : MonoBehaviour
    {
        [SerializeField] private string _tag;
        [SerializeField] private RectTransform _root;

        public string Tag => _tag;
        public RectTransform Root => _root;
    }
}