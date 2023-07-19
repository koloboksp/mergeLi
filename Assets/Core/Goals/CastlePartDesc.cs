using UnityEngine;
using UnityEngine.UI;

namespace Core.Goals
{
    public class CastlePartDesc : MonoBehaviour
    {
        [SerializeField] private Vector2Int _gridPosition;
        [SerializeField] private int _cost;
       
        public Vector2Int GridPosition => _gridPosition;
        public int Cost => _cost;
        public Sprite Image => GetComponent<Image>().sprite;

        public void EditorPartsDisable()
        {
            GetComponent<Image>().enabled = false;
        }
    }
}