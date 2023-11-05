using System.Collections.Generic;
using UnityEngine;

namespace Core.Buffs
{
    public class RectExplodeBuff : ExplodeBuff
    {
        [SerializeField] private int _halfSize = 1;

        protected override List<Vector3Int> GetAffectingArea(Vector3Int pointerGridPosition)
        {
            var size = GetSize(_halfSize);
            var affectingArea = new List<Vector3Int>();
            
            for (var x = 0; x < size ; x++)
            for (var y = 0; y < size; y++)
            {
                var localGridPosition = new Vector3Int(x - _halfSize, y - _halfSize, 0);
                if (IsAreaValid(pointerGridPosition + localGridPosition, _gameProcessor.Scene.Field.Size))
                    affectingArea.Add(localGridPosition);
            }

            return affectingArea;
        }
                
        private static int GetSize(int halfSize)
        {
            return halfSize * 2 + 1;
        }
    }
}