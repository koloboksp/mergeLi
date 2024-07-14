using System;
using UnityEngine;

namespace Core.Gameplay
{
    [Serializable]
    public class BallWeight
    {
        [SerializeField] private int _points;
        [SerializeField] private int _weight;

        public int Points
        {
            get => _points;
            set => _points = value;
        }

        public int Weight
        {
            get => _weight;
            set => _weight = value;
        }
    }
}