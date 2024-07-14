using System.Linq;
using UnityEngine;

namespace Core.Gameplay
{
    public class GameRulesSettings : MonoBehaviour
    {
        [SerializeField] private int _minimalBallsInLine = 5;
        [SerializeField] private int _generatedBallsCountAfterMerge = 2;
        [SerializeField] private int _generatedBallsCountAfterMove = 3;
        [SerializeField] private int _generatedBallsCountOnStart = 5;
        [SerializeField] private int _maxBallPoints = 512;
        [SerializeField] private BallWeight[] _generatedBallWeightsRange;
        [SerializeField] private int _maxActiveHats = 5;

        public int GeneratedBallsCountAfterMerge => _generatedBallsCountAfterMerge;
        public int GeneratedBallsCountAfterMove => _generatedBallsCountAfterMove ;
        public int GeneratedBallsCountOnStart => _generatedBallsCountOnStart;
        public BallWeight[] GeneratedBallWeightsRange => _generatedBallWeightsRange;

        public int[] GeneratedBallPointsRange
        {
            get
            {
                return _generatedBallWeightsRange
                    .Select(i => i.Points)
                    .ToArray();
            }
        }
        
        public int MaxBallPoints => _maxBallPoints;
        public int MinimalBallsInLine => _minimalBallsInLine;
        public int MaxActiveHats => _maxActiveHats;
    }
}