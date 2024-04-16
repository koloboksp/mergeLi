using System;
using System.Collections.Generic;
using Core.Steps;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FieldView : MonoBehaviour, IFieldView, IPointerDownHandler, IPointerUpHandler
{
    private readonly List<CanvasGroup> _noAllocFoundCanvasGroups = new();

    [SerializeField] private Field _model;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _root;

    private bool _stepExecuted = false;
    public Canvas Canvas => _canvas;
    public Transform Root => _root;
    public Vector2 RootSize => _root.rect.size;
    
    public void LockInput(bool lockState)
    {
        _canvasGroup.interactable = !lockState;
    }

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
        if (!UIBuff.ParentGroupAllowsInteraction(_root, _noAllocFoundCanvasGroups))
            return;

        if (_stepExecuted)
            return;
        
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