using UnityEngine;

public class GameRulesSettings : MonoBehaviour
{
    [SerializeField] private int _minimalBallsInLine = 5;
    [SerializeField] private int _generatedBallsCountAfterMerge = 2;
    [SerializeField] private int _generatedBallsCountAfterMove = 3;
    [SerializeField] private int _generatedBallsCountOnStart = 5;
    [SerializeField] private int _maxBallPoints = 512;
    [SerializeField] private int[] _generatedBallsPointsRange;
    
    public int GeneratedBallsCountAfterMerge => _generatedBallsCountAfterMerge;
    public int GeneratedBallsCountAfterMove => _generatedBallsCountAfterMove ;
    public int GeneratedBallsCountOnStart => _generatedBallsCountOnStart;
    public int[] GeneratedBallsPointsRange => _generatedBallsPointsRange;
    public int MaxBallPoints => _maxBallPoints;
    public int MinimalBallsInLine => _minimalBallsInLine;
}