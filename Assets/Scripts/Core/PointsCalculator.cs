using System.Collections.Generic;

namespace Core
{
    public class PointsCalculator : IPointsCalculator
    {
        private readonly GameRulesSettings _rulesSettings;
        private readonly List<(string hatName, int points)> _hatExtraPoints = new();
        
        public PointsCalculator(GameRulesSettings rulesSettingsSettings)
        {
            _rulesSettings = rulesSettingsSettings;
        }

        public void UpdateHatsExtraPoints(IEnumerable<(string hatName, int points)> hatsExtraPoints)
        {
            _hatExtraPoints.Clear();
            _hatExtraPoints.AddRange(hatsExtraPoints);
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

                var foundHatI = _hatExtraPoints.FindIndex(i => i.hatName == ballInLine.HatName);
                var extraHatPoints = foundHatI >= 0 ? _hatExtraPoints[foundHatI].points : 0;
                resultBallsInLinePoints.Add((ballInLine, new PointsDesc(ballInLine.Points, extraPoints, extraHatPoints)));
            }   

            return resultBallsInLinePoints;
        }
    }
}