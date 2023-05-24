using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Steps;
using Core.Steps.CustomOperations;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class GameProcessor : MonoBehaviour
{
    [SerializeField] private Field _field;
    [SerializeField] private StepMachine _stepMachine;
        
    [SerializeField] private DestroyBallEffect _destroyBallEffectPrefab;
    [SerializeField] private NoPathEffect _noPathEffectPrefab;

    [FormerlySerializedAs("undoBtn")] [SerializeField] private Button _undoBtn;
    
    private Ball _selectedBall;
    private Ball _otherSelectedBall;
    
    void Awake()
    {
        _undoBtn.onClick.AddListener(Undo);
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
        _field.GenerateBalls(5);
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
                
               // var generateOperationData = step.GetData<GenerateOperationData>();
               // var mergeOperationData = step.GetData<MergeOperationData>();
               // var moveOperationData = step.GetData<MoveOperationData>();
               // _stepMachine.AddUndoStep(new Step("UndoMerge",
               //     new RemoveGeneratedItems(generateOperationData.NewPositions, _field),
               //     new UnmergeOperation(mergeOperationData.Position, mergeOperationData.Points, mergeOperationData.MergeablesCount, _field),
               //     new MoveOperation(mergeOperationData.Position, moveOperationData.StartPosition, _field)));
                
                var operations = step.Operations.Reverse();
                var inverseOperations = operations.Select(operation => operation.GetInverseOperation()).ToArray();
                _stepMachine.AddUndoStep(new Step("UndoMerge", inverseOperations));
            }
            if (step.Tag == "Move")
            {
                _stepFinishState = Field.StepFinishState.Move;
                userStepFinished = true;

                var operations = step.Operations.Reverse();
                var inverseOperations = operations.Select(operation => operation.GetInverseOperation()).ToArray();
                _stepMachine.AddUndoStep(new Step("UndoMove", inverseOperations));
                
                //var col = step.GetData<CollapseOperationData>();
                //var generateOperationData = step.GetData<GenerateOperationData>();
                //var moveOperationData = step.GetData<MoveOperationData>();
                //_stepMachine.AddUndoStep(new Step("UndoMove",
                //    new UncollapseOperation(col, _field),
                //    generateOperationData != null ? new RemoveGeneratedItems(generateOperationData.NewPositions, _field) : null,
                //    new MoveOperation(moveOperationData.EndPosition, moveOperationData.StartPosition, _field)));
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
                                new CollapseOperation(pointerGridPosition, _destroyBallEffectPrefab, _field),
                                new GenerateOperation(1, _field),
                                new CollapseOperation(_destroyBallEffectPrefab, _field)));
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
                        new CollapseOperation(pointerGridPosition, _destroyBallEffectPrefab, _field),
                        new CollapseCheckOperation(
                            null,
                            new List<Operation>(){
                                new GenerateOperation(4, _field),
                                new CollapseOperation(_destroyBallEffectPrefab, _field)})));
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
    
    private void OnStepExecute(Step step)
    {
        _undoBtn.interactable = false;
    }
    
    private void OnStepCompleted(Step step)
    {
        _undoBtn.interactable = true;
    }
}