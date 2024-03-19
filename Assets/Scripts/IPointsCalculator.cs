using System.Collections.Generic;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public interface IPointsCalculator
    {
        List<List<BallDesc>> GetPoints(List<List<BallDesc>> ballsInLines);

    }
}