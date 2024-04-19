using System;
using UnityEngine;

namespace Save
{
    [Serializable]
    public class SessionBallProgress
    {
        public Vector3Int GridPosition;
        public int Points;
        public int Hat;
    }
}