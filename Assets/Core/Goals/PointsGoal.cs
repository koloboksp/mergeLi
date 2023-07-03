using System;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    [Serializable]
    public class PointsGoal
    {
        [SerializeField] private int _threshold;
        [SerializeField] private string _achievementId;

        public int Threshold => _threshold;
    }
}