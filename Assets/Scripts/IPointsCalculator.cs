using System.Collections.Generic;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public interface IPointsCalculator
    {
        List<List<(BallDesc ball, PointsDesc points)>> GetPoints(List<List<BallDesc>> ballsInLines);

    }
}