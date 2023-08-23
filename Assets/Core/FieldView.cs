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
    public Vector2 RectSize => _root.rect.size;

    private void Awake()
    {
        _model.Scene.GameProcessor.OnStepExecute += GameProcessor_OnStepExecute;
        _model.Scene.GameProcessor.OnStepCompleted += GameProcessor_OnStepCompleted;
    }

    private void GameProcessor_OnStepExecute(Step step)
    {
        _stepExecuted = true;
    }

    private void GameProcessor_OnStepCompleted(Step step)
    {
        _stepExecuted = false;
    }

    public Vector3 CellSize()
    {
        return new Vector3(_root.rect.size.x / _model.Size.x, _root.rect.size.x / _model.Size.y, 0);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(_stepExecuted) return;
        
        var localPosition = Root.InverseTransformPoint(_model.ScreenPointToWorld(eventData.position));
      
        var fieldSize = RectSize;
        var gridPosition = new Vector3Int(
            (int)((localPosition.x / fieldSize.x) * _model.Size.x), 
            (int)((localPosition.y / fieldSize.y) * _model.Size.y));
       
        _model.InnerOnPointerDown(gridPosition);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       
    }
}