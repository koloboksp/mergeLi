using System.Collections.Generic;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public interface IPointsCalculator
    {
        List<List<(Vector3Int position, int points)>> GetPoints(List<List<(Vector3Int position, int points)>> ballsInLines);

    }
}