using System.Collections.Generic;
using Core.Steps;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core
{
    public class FieldView : MonoBehaviour, IFieldView, IPointerDownHandler, IPointerUpHandler
    {
        private readonly List<CanvasGroup> _noAllocFoundCanvasGroups = new();

        [SerializeField] private Field _model;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _root;

        private RectTransform[] _rowDepthSeparators;
    
        private bool _stepExecuted = false;
        public Canvas Canvas => _canvas;
        public Transform Root => _root;
        public Vector2 RootSize => _root.rect.size;
    
        public void LockInput(bool lockState)
        {
            _canvasGroup.interactable = !lockState;
        }

        public void SetData()
        {
            _model.Scene.GameProcessor.OnBeforeStepStarted += GameProcessor_OnBeforeStepStarted;
            _model.Scene.GameProcessor.OnStepCompleted += GameProcessor_OnStepCompleted;
        
            _rowDepthSeparators = new RectTransform[_model.Size.y];
        
            for (var rowIndex = 0; rowIndex < _model.Size.y; rowIndex++)
            {
                var depthSeparator = new GameObject($"rowSeparator_{_model.Size.y - rowIndex - 1}", typeof(RectTransform));
                depthSeparator.transform.SetParent(_root);
                depthSeparator.transform.localScale = Vector3.one;
                _rowDepthSeparators[rowIndex] = depthSeparator.transform as RectTransform;
            }
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

        public void UpdateSiblingIndex(Vector3 gridPosition, Transform target)
        {
            var rowDepthIndex = Mathf.FloorToInt(_model.Size.y - gridPosition.y - 1);
            var depthSeparatorSiblingIndex = _rowDepthSeparators[rowDepthIndex].GetSiblingIndex();
            target.SetSiblingIndex(depthSeparatorSiblingIndex + 1);
        }
    }
}