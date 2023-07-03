using System.Collections.Generic;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class PointsGoals : MonoBehaviour
    {
        [SerializeField] private List<PointsGoal> Goals = new List<PointsGoal>();

        public PointsGoal GetNextGoal(int score)
        {
            foreach (var goal in Goals)
                if (score < goal.Threshold)
                    return goal;
            
            return null;
        }
    }
}