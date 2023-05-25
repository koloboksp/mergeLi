using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Steps;
using Core.Steps.CustomOperations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public interface IRules
{
    int MinimalBallsInLine { get; }
}

public class GameProcessor : MonoBehaviour, IRules
{
    [SerializeField] private Field _field;
    [SerializeField] private StepMachine _stepMachine;
        
    [SerializeField] private DestroyBallEffect _destroyBallEffectPrefab;
    [SerializeField] private NoPathEffect _noPathEffectPrefab;
    [SerializeField] private CollapsePointsEffect _collapsePointsEffectPrefab;
   
    [SerializeField] private int _minimalBallsInLine = 5;
    [SerializeField] private int _generatedBallsCountAfterMerge = 1;
    [SerializeField] private int _generatedBallsCountAfterMove = 4;
    [SerializeField] private int _generatedBallsCountOnStart = 5;

    [FormerlySerializedAs("undoBtn")] [SerializeField] private Button _undoBtn;
    
    private Ball _selectedBall;
    private Ball _otherSelectedBall;
    private PointsCalculator _pointsCalculator;

    void Awake()
    {
        _undoBtn.onClick.AddListener(Undo);
        _pointsCalculator = new PointsCalculator(this);
    }

    private void Undo()
    {
        _stepMachine.Undo();
    }

    private void Start()
    {
        _field.OnClick += Field_OnClick;
        _stepMachine.OnStepExecute += OnStepExecute;
        _stepMachine.OnStepCompleted += OnStepCompleted;
        StartCoroutine(InnerProcess());
    }

    


    IEnumerator InnerProcess()
    {
        _stepMachine.OnStepCompleted += StepMachine_OnStepCompleted;
        
        bool userStep = false;
        bool userStepFinished = false;
        _field.GenerateBalls(_generatedBallsCountOnStart);
        Field.StepFinishState _stepFinishState = Field.StepFinishState.Move;
        while (_field.IsEmpty)
        {
            userStep = false;
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

        void Field_OnUserStep()
        {
            userStep = true;
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
                        var path = _field.GetPath(_selectedBall.IntPosition, pointerGridPosition);
                        if (path.Count > 0)
                        {
                            _stepMachine.AddStep(new Step("Merge",
                                new MoveOperation(_selectedBall.IntPosition, pointerGridPosition, _field),
                                new MergeOperation(pointerGridPosition, _field),
                                new SelectOperation(pointerGridPosition, false, _field)
                                    .SubscribeCompleted(OnDeselectBall),
                                new CollapseOperation(pointerGridPosition, _collapsePointsEffectPrefab, _destroyBallEffectPrefab, _field, _pointsCalculator)
                                    .SubscribeCompleted(Collapse_OnComplete),
                                new CheckIfGenerationIsNecessary(
                                    null,
                                    new List<Operation>(){
                                        new GenerateOperation(_generatedBallsCountAfterMerge, _field),
                                        new CollapseOperation(_collapsePointsEffectPrefab, _destroyBallEffectPrefab, _field, _pointsCalculator)
                                            .SubscribeCompleted(Collapse_OnComplete)})));
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
                var path = _field.GetPath(_selectedBall.IntPosition, pointerGridPosition);
                if (path.Count > 0)
                {
                    _stepMachine.AddStep(new Step("Move", 
                        new MoveOperation(_selectedBall.IntPosition, pointerGridPosition, _field),
                        new SelectOperation(pointerGridPosition, false, _field)
                            .SubscribeCompleted(OnDeselectBall),
                        new CollapseOperation(pointerGridPosition, _collapsePointsEffectPrefab, _destroyBallEffectPrefab, _field, _pointsCalculator)
                            .SubscribeCompleted(Collapse_OnComplete),
                        new CheckIfGenerationIsNecessary(
                            null,
                            new List<Operation>(){
                                new GenerateOperation(_generatedBallsCountAfterMove, _field),
                                new CollapseOperation( _collapsePointsEffectPrefab, _destroyBallEffectPrefab, _field, _pointsCalculator)
                                    .SubscribeCompleted(Collapse_OnComplete)})));
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

    private void Collapse_OnComplete(Operation arg1, object arg2)
    {
        
    }

    private void OnSelectBall(Operation sender, object result)
    {
        _selectedBall = (Ball)result;
    }
    
    private void OnDeselectBall(Operation sender, object result)
    {
        _selectedBall = null;
    }
    
    private void OnStepExecute(Step step)
    {
        _undoBtn.interactable = false;
    }
    
    private void OnStepCompleted(Step step)
    {
        _undoBtn.interactable = true;
    }

    public int MinimalBallsInLine => _minimalBallsInLine;
}