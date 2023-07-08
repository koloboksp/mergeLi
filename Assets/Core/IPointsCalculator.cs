using System.Collections.Generic;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public interface IPointsCalculator
    {
        List<List<(Vector3Int intPosition, int points)>> GetPoints(List<List<(Vector3Int intPosition, int points)>> ballsInLines);

    }
}