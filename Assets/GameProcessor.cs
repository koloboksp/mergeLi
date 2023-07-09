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
    public event Action<Step> OnStepCompleted;
    public event Action<Step> OnStepExecute;
    public event Action OnScoreChanged;
    
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
    [SerializeField] private int _generatedBallsCountAfterMove = 4;
    [SerializeField] private int _generatedBallsCountOnStart = 5;
    [SerializeField] private Vector2Int _generatedBallsPointsRange = new Vector2Int(0, 10);

    [SerializeField] private RectTransform _uiScreensRoot;

    [SerializeField] private List<Buff> _buffs;
    [SerializeField] private PurchasesLibrary _purchasesLibrary;

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
    
    IEnumerator InnerProcess()
    {
        _stepMachine.OnStepCompleted += StepMachine_OnStepCompleted;
        
        bool userStep = false;
        bool userStepFinished = false;
        _field.GenerateBalls(_generatedBallsCountOnStart, _generatedBallsPointsRange);
        _field.GenerateNextBallPositions(_generatedBallsCountAfterMove, _generatedBallsPointsRange);
        Field.StepFinishState _stepFinishState = Field.StepFinishState.Move;
        while (_field.IsEmpty)
        {
            userStepFinished = false;

            while (!userStepFinished)
                yield return null;

            
            if (_stepFinishState == Field.StepFinishState.Move)
            {
                
            }
            else
            {
                if (_stepFinishState == Field.StepFinishState.Merge)
                {
                  
                }
                else if (_stepFinishState == Field.StepFinishState.MoveAndRemove)
                {

                }
            }

        }
        
        void StepMachine_OnStepCompleted(Step step)
        {
            if (step.Tag == "Merge")
            {
                _stepFinishState = Field.StepFinishState.Merge;
                userStepFinished = true;
           
                var inverseOperations = step.Operations
                    .Reverse()
                    .Select(operation => operation.GetInverseOperation()).ToArray();
                _stepMachine.AddUndoStep(new Step("UndoMerge", inverseOperations));
            }
            
            if (step.Tag == "Move")
            {
                _stepFinishState = Field.StepFinishState.Move;
                userStepFinished = true;

                var inverseOperations = step.Operations
                    .Reverse()
                    .Select(operation => operation.GetInverseOperation()).ToArray();
                _stepMachine.AddUndoStep(new Step("UndoMove", inverseOperations));
            }
            
            if (step.Tag == "Explode")
            {
                _stepFinishState = Field.StepFinishState.Move;
                userStepFinished = true;

                var inverseOperations = step.Operations
                    .Reverse()
                    .Select(operation => operation.GetInverseOperation()).ToArray();
                _stepMachine.AddUndoStep(new Step("UndoExplode", inverseOperations));
            }
        }
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
                            _stepMachine.AddStep(new Step("Merge",
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
                    _stepMachine.AddStep(new Step("Move", 
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
        OnStepCompleted?.Invoke(step);
    }
    
    public void Undo()
    {
        _stepMachine.Undo();
    }


    public int MinimalBallsInLine => _minimalBallsInLine;
    public List<Buff> Buffs => _buffs;
   

    public void Explode(List<Vector3Int> ballsIndexes)
    {
        _stepMachine.AddStep(
            new Step("Explode", 
                new RemoveOperation(ballsIndexes, _field)));
    }


    public void AddPoints(int points)
    {
        _score += points;
        OnScoreChanged?.Invoke();
    }

    public void RemovePoints(int points)
    {
        _score -= points;
        OnScoreChanged?.Invoke();
    }
}