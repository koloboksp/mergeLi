using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Steps.CustomOperations
{
    public class PointsCalculator : IPointsCalculator
    {
        private readonly GameRulesSettings _rulesSettings;
        public PointsCalculator(GameRulesSettings rulesSettingsSettings)
        {
            _rulesSettings = rulesSettingsSettings;
        }
        
        public List<List<BallDesc>> GetPoints(List<List<BallDesc>> ballsInLines)
        {
            var resultBallsInLinesPoints = new List<List<BallDesc>>();
            for (var lineI = 0; lineI < ballsInLines.Count; lineI++)
                resultBallsInLinesPoints.Add(GetLinePoints(ballsInLines[lineI], lineI, _rulesSettings.MinimalBallsInLine));

            return resultBallsInLinesPoints;
        }
        
        private List<BallDesc> GetLinePoints(List<BallDesc> ballsInLine, int lineIndex, int minimalBallsInLine)
        {
            var resultBallsInLinePoints = new List<BallDesc>();
            for (var index = 0; index < ballsInLine.Count; index++)
            {
                var ballInLine = ballsInLine[index];
                var overPointsCoef = 1;
                if (index >= minimalBallsInLine)
                    overPointsCoef = (index - minimalBallsInLine + 1) + 1;
                
                resultBallsInLinePoints.Add(
                    new BallDesc(ballInLine.GridPosition, ballInLine.Points * overPointsCoef * (lineIndex + 1), 0));
            }

            return resultBallsInLinePoints;
        }
    }
}