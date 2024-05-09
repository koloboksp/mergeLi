using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Save
{
    [Serializable]
    public class SessionBallProgress
    {
        public Vector3Int GridPosition;
        public int Points;
        public string HatHame;
    }
}