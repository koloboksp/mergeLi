using UnityEngine;

namespace Core.Buffs
{
    public class UIBuffCursor : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;

        public void SetPosition(Vector3 worldPosition)
        {
            _root.position = worldPosition;
            var anchoredPosition3D = _root.anchoredPosition3D;
            anchoredPosition3D.z = 0;
            _root.anchoredPosition3D = anchoredPosition3D;
        }
    }
}