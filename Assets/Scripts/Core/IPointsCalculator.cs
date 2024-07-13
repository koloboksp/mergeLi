using System.Collections.Generic;

namespace Core
{
    public interface IPointsCalculator
    {
        List<List<(BallDesc ball, PointsDesc points)>> GetPoints(List<List<BallDesc>> ballsInLines);

        void UpdateHatsExtraPoints(IEnumerable<(string hatName, int points)> hatsExtraPoints);
    }
}