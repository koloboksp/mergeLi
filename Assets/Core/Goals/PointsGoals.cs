using System.Collections.Generic;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class PointsGoals : MonoBehaviour
    {
        [SerializeField] private List<PointsGoal> _goals = new List<PointsGoal>();

        public PointsGoal GetNextGoal(int score)
        {
            foreach (var goal in _goals)
                if (score < goal.Threshold)
                    return goal;

            return _goals[_goals.Count - 1];
        }
    }
}