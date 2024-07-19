using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Core.Gameplay
{
    [Serializable]
    public class BallsMask
    {
        public List<BallDesc> Balls = new List<BallDesc>();
    }
}