using System;
using Core.Steps;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FieldView : MonoBehaviour, IFieldView, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Field _model;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private RectTransform _root;

    private bool _stepExecuted = false;
    public Canvas Canvas => _canvas;
    public Transform Root => _root;
    public Vector2 RootSize => _root.rect.size;
   
    private void Awake()
    {
        _model.Scene.GameProcessor.OnBeforeStepStarted += GameProcessor_OnBeforeStepStarted;
        _model.Scene.GameProcessor.OnStepCompleted += GameProcessor_OnStepCompleted;
    }

    private void GameProcessor_OnBeforeStepStarted(Step step, StepExecutionType executionType)
    {
        _stepExecuted = true;
    }

    private void GameProcessor_OnStepCompleted(Step step, StepExecutionType executionType)
    {
        _stepExecuted = false;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if(_stepExecuted) return;
        
        var localPosition = Root.InverseTransformPoint(_model.ScreenPointToWorld(eventData.position));
      
        var fieldSize = RootSize;
        var gridPosition = new Vector3Int(
            (int)((localPosition.x / fieldSize.x) * _model.Size.x), 
            (int)((localPosition.y / fieldSize.y) * _model.Size.y));
       
        _model.InnerOnPointerDown(gridPosition);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       
    }
}