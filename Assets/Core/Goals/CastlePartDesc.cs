using UnityEngine;

namespace Core.Goals
{
    public class CastlePartDesc : MonoBehaviour
    {
        [SerializeField] private Vector2Int _gridPosition;
        [SerializeField] private int _cost;
        [SerializeField] private Sprite _image;

        public Vector2Int GridPosition => _gridPosition;
        public int Cost => _cost;
        public Sprite Image => _image;
    }
}