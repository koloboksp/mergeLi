using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Goals
{
    public class CastlePartDesc : MonoBehaviour
    {
        [SerializeField] private int _cost;
        [SerializeField] private Sprite _sprite;
        
        public int Cost => _cost;
        public Sprite Image => GetComponent<Image>().sprite;
        
        private void OnValidate()
        {  
            var castlePart = GetComponentInChildren<CastlePart>();
            var castle = transform.parent.GetComponent<Castle>();
            
            castlePart.Owner = castle;
            var anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            castlePart.GridPosition = new Vector2Int(Mathf.RoundToInt(anchoredPosition.x / 100.0f), Mathf.RoundToInt(anchoredPosition.y / 100.0f));
            castlePart.Cost = _cost;
            castlePart.Icon = _sprite;
            UnityEditor.EditorUtility.SetDirty(castlePart);
            
            var allComponents = castlePart.GetComponentsInChildren<MonoBehaviour>();
            foreach (var component in allComponents)
                if (component is ISupportChangesInEditor supportChanges )
                {
                    supportChanges.OnValueUpdatedInEditorMode();
                    UnityEditor.EditorUtility.SetDirty(component);
                }
        }
    }
}