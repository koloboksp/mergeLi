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
        
        public List<List<(BallDesc ball, PointsDesc points)>> GetPoints(List<List<BallDesc>> ballsInLines)
        {
            var resultBallsInLinesPoints = new List<List<(BallDesc ball, PointsDesc points)>>();
            for (var lineI = 0; lineI < ballsInLines.Count; lineI++)
                resultBallsInLinesPoints.Add(GetLinePoints(ballsInLines[lineI], lineI, _rulesSettings.MinimalBallsInLine));

            return resultBallsInLinesPoints;
        }
        
        private List<(BallDesc ball, PointsDesc points)> GetLinePoints(List<BallDesc> ballsInLine, int lineIndex, int minimalBallsInLine)
        {
            var resultBallsInLinePoints = new List<(BallDesc ball, PointsDesc points)>();
            for (var index = 0; index < ballsInLine.Count; index++)
            {
                var ballInLine = ballsInLine[index];
                
                var extraPoints = 0;
                if (index >= minimalBallsInLine)
                    extraPoints += ballInLine.Points * (index + 1 - minimalBallsInLine);
                if (lineIndex > 0)
                    extraPoints += ballInLine.Points * lineIndex;
                
                var extraHatPoints = ballInLine.Hat;
                resultBallsInLinePoints.Add(
                    (ballInLine, new PointsDesc(ballInLine.Points, extraPoints, extraHatPoints)));
            }   

            return resultBallsInLinePoints;
        }
    }
}