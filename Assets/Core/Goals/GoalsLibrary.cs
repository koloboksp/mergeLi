using UnityEngine;

namespace Core.Steps.CustomOperations
{
 
    public class GoalsLibrary : MonoBehaviour
    {
        [SerializeField] private PointsGoals _pointsGoals;
        
        public PointsGoals PointsGoals => _pointsGoals;
    }
}