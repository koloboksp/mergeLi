using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Steps;
using Core.Steps.CustomOperations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public interface IRules
{
    int MinimalBallsInLine { get; }
}

public interface IPointsChangeListener
{
    void AddPoints(int points);
    void RemovePoints(int points);
}

public class GameProcessor : MonoBehaviour, IRules, IPointsChangeListener
{
    public const string MoveStepTag = "Move";
    public const string MergeStepTag = "Merge";
    public const string UndoMoveStepTag = "UndoMove";
    public const string UndoMergeStepTag = "UndoMerge";
    public const string ExplodeStepTag = "Explode";
    public const string UndoExplodeStepTag = "UndoExplode";
    public const string UndoStepTag = "Undo";

    public event Action<Step> OnStepCompleted;
    public event Action<Step> OnStepExecute;
    public event Action<int> OnScoreChanged;
    
    [SerializeField] private Scene _scene;
    [SerializeField] private Field _field;
    [SerializeField] private StepMachine _stepMachine;
    [SerializeField] private PlayerInfo _playerInfo;
    [SerializeField] private DefaultMarket _market;
    [SerializeField] private GoalsLibrary _goalsLibrary;
    
    [SerializeField] private DestroyBallEffect _destroyBallEffectPrefab;
    [SerializeField] private NoPathEffect _noPathEffectPrefab;
    [SerializeField] private CollapsePointsEffect _collapsePointsEffectPrefab;
   
    [SerializeField] private int _minimalBallsInLine = 5;
    [SerializeField] private int _generatedBallsCountAfterMerge = 2;
    [SerializeField] private int _generatedBallsCountAfterMove = 3;
    [SerializeField] private int _generatedBallsCountOnStart = 5;
    [SerializeField] private Vector2Int _generatedBallsPointsRange = new Vector2Int(0, 10);

    [SerializeField] private RectTransform _uiScreensRoot;

    [SerializeField] private List<Buff> _buffs;
    [SerializeField] private PurchasesLibrary _purchasesLibrary;
    [SerializeField] private CastleSelector _castleSelector;
    
    private Ball _selectedBall;
    private Ball _otherSelectedBall;
    private PointsCalculator _pointsCalculator;
    private int _score;
    
    public Scene Scene => _scene;
    public PlayerInfo PlayerInfo => _playerInfo;
    public int Score => _score;
    public IMarket Market => _market;
    public GoalsLibrary GoalsLibrary => _goalsLibrary;
    public PurchasesLibrary PurchasesLibrary => _purchasesLibrary;

    void Awake()
    {
        _pointsCalculator = new PointsCalculator(this);
    }
    
    private void Start()
    {
        _field.OnClick += Field_OnClick;
        _stepMachine.OnStepExecute += StepMachine_OnStepExecute;
        _stepMachine.OnStepCompleted += StepMachine_OnStepCompleted;

        ApplicationController.Instance.UIPanelController.SetScreensRoot(_uiScreensRoot);
        ApplicationController.Instance.UIPanelController.PushScreen(typeof(UIGameScreen), new UIGameScreenData(){GameProcessor = this,});
        StartCoroutine(InnerProcess());
    }
    bool _userStepFinished = false;
    
    IEnumerator InnerProcess()
    {
        bool userStep = false;

        _playerInfo.Load();
        
        var lastSelectedCastle = _playerInfo.GetLastSelectedCastle();
        if (string.IsNullOrEmpty(lastSelectedCastle))
            _playerInfo.SelectCastle(_castleSelector.Library.Castles[0].Name);

        var foundCastle = _castleSelector.Library.Castles.FirstOrDefault(i => i.Name == lastSelectedCastle);
        if (foundCastle == null)
        {
            foreach (var castle in _castleSelector.Library.Castles)
            {
                var castleProgress = _playerInfo.GetCastleProgress(castle.Name);
                if(castleProgress == null || !castleProgress.IsCompleted)
                    _playerInfo.SelectCastle(castle.Name);
            }
        }
        
        _castleSelector.Init();
        _castleSelector.OnCastleCompleted += CastleSelector_OnCastleCompleted;
        
        _field.GenerateBalls(_generatedBallsCountOnStart, _generatedBallsPointsRange);
        _field.GenerateNextBallPositions(_generatedBallsCountAfterMove, _generatedBallsPointsRange);

        
        while (_field.IsEmpty)
        {
            _userStepFinished = false;

            while (!_userStepFinished)
                yield return null;
        }
    }

    private void CastleSelector_OnCastleCompleted()
    {
        SelectNextCastle();
    }


    void Field_OnClick(Vector3Int pointerGridPosition)
    {
        var balls = _field.GetSomething<Ball>(pointerGridPosition).ToList();
        Ball ball = null;
        if(balls.Count != 0)
            ball = balls[0];
            
        _otherSelectedBall = null;
    
        if (_selectedBall != null)
        {
            if (ball != null)
            {
                if (_selectedBall == ball)
                {
                    _stepMachine.AddStep(new Step("Deselect", new SelectOperation(pointerGridPosition, false, _field)
                        .SubscribeCompleted(OnDeselectBall)));
                }
                else
                {
                    if (_selectedBall.Points == ball.Points)
                    {
                        _otherSelectedBall = ball;
                        var path = _field.GetPath(_selectedBall.IntGridPosition, pointerGridPosition);
                        if (path.Count > 0)
                        {
                            _stepMachine.AddStep(new Step(GameProcessor.MergeStepTag,
                                new MoveOperation(_selectedBall.IntGridPosition, pointerGridPosition, _field),
                                new MergeOperation(pointerGridPosition, _field),
                                new SelectOperation(pointerGridPosition, false, _field)
                                    .SubscribeCompleted(OnDeselectBall),
                                new CollapseOperation(pointerGridPosition, _collapsePointsEffectPrefab, _destroyBallEffectPrefab, _field, _pointsCalculator, this),
                                new CheckIfGenerationIsNecessary(
                                    null,
                                    new List<Operation>(){
                                        new GenerateOperation(_generatedBallsCountAfterMerge, _generatedBallsCountAfterMove, _generatedBallsPointsRange, _field),
                                        new CollapseOperation(_collapsePointsEffectPrefab, _destroyBallEffectPrefab, _field, _pointsCalculator, this)})));
                        }
                        else
                        {
                            _stepMachine.AddStep(new Step("NoPath",
                                new PathNotFoundOperation(pointerGridPosition, _noPathEffectPrefab, _field)));
                        }
                    }
                }
            }
            else
            {
                var path = _field.GetPath(_selectedBall.IntGridPosition, pointerGridPosition);
                if (path.Count > 0)
                {
                    _stepMachine.AddStep(new Step(GameProcessor.MoveStepTag, 
                        new MoveOperation(_selectedBall.IntGridPosition, pointerGridPosition, _field),
                        new SelectOperation(pointerGridPosition, false, _field)
                            .SubscribeCompleted(OnDeselectBall),
                        new CollapseOperation(pointerGridPosition, _collapsePointsEffectPrefab, _destroyBallEffectPrefab, _field, _pointsCalculator, this),
                        new CheckIfGenerationIsNecessary(
                            null,
                            new List<Operation>(){
                                new GenerateOperation(_generatedBallsCountAfterMove, _generatedBallsCountAfterMove, _generatedBallsPointsRange, _field),
                                new CollapseOperation( _collapsePointsEffectPrefab, _destroyBallEffectPrefab, _field, _pointsCalculator, this)
                            })));
                }
                else
                {
                    _stepMachine.AddStep(new Step("NoPath",
                        new PathNotFoundOperation(pointerGridPosition, _noPathEffectPrefab, _field)));
                }
            }
        }
        else
        {
            if (ball != null)
            {
                _stepMachine.AddStep(new Step("Select", new SelectOperation(pointerGridPosition, true, _field)
                    .SubscribeCompleted(OnSelectBall)));
            }
        }
    }
    
    private void OnSelectBall(Operation sender, object result)
    {
        _selectedBall = (Ball)result;
    }
    
    private void OnDeselectBall(Operation sender, object result)
    {
        _selectedBall = null;
    }
    
    private void StepMachine_OnStepExecute(Step step)
    {
        OnStepExecute?.Invoke(step);
    }
    
    private void StepMachine_OnStepCompleted(Step step)
    {
        if (step.Tag == GameProcessor.MergeStepTag)
        {
            _userStepFinished = true;
           
            var inverseOperations = step.Operations
                .Reverse()
                .Select(operation => operation.GetInverseOperation()).ToArray();
            _stepMachine.AddUndoStep(new Step(GameProcessor.UndoMergeStepTag, inverseOperations));
        }
            
        if (step.Tag == GameProcessor.MoveStepTag)
        {
            _userStepFinished = true;

            var inverseOperations = step.Operations
                .Reverse()
                .Select(operation => operation.GetInverseOperation()).ToArray();
            _stepMachine.AddUndoStep(new Step(GameProcessor.UndoMoveStepTag, inverseOperations));
        }
            
        if (step.Tag == GameProcessor.ExplodeStepTag)
        {
            _userStepFinished = true;

            var inverseOperations = step.Operations
                .Reverse()
                .Select(operation => operation.GetInverseOperation()).ToArray();
            _stepMachine.AddUndoStep(new Step(GameProcessor.UndoExplodeStepTag, inverseOperations));
        }
        OnStepCompleted?.Invoke(step);
    }
    
    
    public bool HasUndoSteps()
    {
        return _stepMachine.HasUndoSteps();
    }

    public int MinimalBallsInLine => _minimalBallsInLine;
    public List<Buff> Buffs => _buffs;
   
    


    public void AddPoints(int points)
    {
        _score += points;
        OnScoreChanged?.Invoke(points);
    }

    public void RemovePoints(int points)
    {
        _score -= points;
        OnScoreChanged?.Invoke(points);
    }
    
    public void UseUndoBuff(int cost)
    { 
        _stepMachine.AddStep(
            new Step(GameProcessor.UndoStepTag,
                new SpendOperation(cost, _playerInfo, false),
                new UndoOperation(_stepMachine)));
    }

    public void UseExplodeBuff(int cost, List<Vector3Int> ballsIndexes)
    {
        _stepMachine.AddStep(
            new Step(GameProcessor.ExplodeStepTag, 
                new SpendOperation(cost, _playerInfo, true),
                new RemoveOperation(ballsIndexes, _field)));
    }

    public void UseShowNextBallsBuff(int cost)
    {
        
    }

    public void SelectNextCastle()
    {
        foreach (var castle in _castleSelector.Library.Castles)
        {
            var castleProgress = _playerInfo.GetCastleProgress(castle.Name);
            
            if (castleProgress == null || !castleProgress.IsCompleted)
            {
                _playerInfo.SelectCastle(castle.Name);
                break;
            }
        }
        
    }
}