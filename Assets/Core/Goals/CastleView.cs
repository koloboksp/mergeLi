using UnityEngine;

namespace Core.Goals
{
    public class CastleView : MonoBehaviour
    {
        [SerializeField] private Castle _model;
        [SerializeField] private RectTransform _root;

        public RectTransform Root => _root;
    }
}