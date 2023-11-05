using System.Collections.Generic;
using UnityEngine;

namespace Core.Buffs
{
    public enum ExplodeDirection
    {
        Horizontal,
        Vertical,
    }
    
    public class LineExplodeBuff : ExplodeBuff
    {
        [SerializeField] private ExplodeDirection _direction;

        protected override List<Vector3Int> GetAffectingArea(Vector3Int pointerGridPosition)
        {
            var affectingArea = new List<Vector3Int>();

            var fieldSize = _gameProcessor.Scene.Field.Size;
            if (IsAreaValid(pointerGridPosition, fieldSize))
            {
                if (_direction == ExplodeDirection.Vertical)
                {
                    for (int i = 0; i < fieldSize.y; i++)
                        affectingArea.Add(new Vector3Int(0, -pointerGridPosition.y + i));
                }
                else
                {
                    for (int i = 0; i < fieldSize.x; i++)
                        affectingArea.Add(new Vector3Int(-pointerGridPosition.x + i, 0));
                }
            }
           
            return affectingArea;
        }
    }
}