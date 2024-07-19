using System;
using UnityEngine;

namespace Core.Gameplay
{
    [Serializable]
    public class BallDesc
    {
        public Vector3Int GridPosition;
        public int Points;
        public string HatName;
    
        public BallDesc()
        {
        }

        public BallDesc(Vector3Int gridPosition, int points, string hatName)
        {
            GridPosition = gridPosition;
            Points = points;
            HatName = hatName;
        }
    
    }
}