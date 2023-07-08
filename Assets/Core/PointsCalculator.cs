using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class PointsCalculator : IPointsCalculator
    {
        private readonly IRules _rules;
        public PointsCalculator(IRules rules)
        {
            _rules = rules;
        }
        
        public List<List<(Vector3Int intPosition, int points)>> GetPoints(List<List<(Vector3Int intPosition, int points)>> ballsInLines)
        {
            var resultBallsInLinesPoints = new List<List<(Vector3Int intPosition, int points)>>();
            for (var lineI = 0; lineI < ballsInLines.Count; lineI++)
                resultBallsInLinesPoints.Add(GetLinePoints(ballsInLines[lineI], lineI, _rules.MinimalBallsInLine));

            return resultBallsInLinesPoints;
        }
        
        List<(Vector3Int intPosition, int points)> GetLinePoints(List<(Vector3Int intPosition, int points)> ballsInLine, int lineIndex, int minimalBallsInLine)
        {
            var resultBallsInLinePoints = new List<(Vector3Int intPosition, int points)>();
            for (var index = 0; index < ballsInLine.Count; index++)
            {
                var ballInLine = ballsInLine[index];
                var overPointsCoef = 1;
                if (index >= minimalBallsInLine)
                    overPointsCoef = (index - minimalBallsInLine + 1) + 1;
                
                resultBallsInLinePoints.Add((position: ballInLine.intPosition, ballInLine.points * overPointsCoef * (lineIndex + 1)));
            }

            return resultBallsInLinePoints;
        }
    }
}