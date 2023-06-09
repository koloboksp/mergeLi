﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class Ball : MonoBehaviour, IFieldMovable, IFieldSelectable, IFieldMergeable
    {
        public event Action OnSelectedChanged;
        public event Action OnTransparencyChanged;
        public event Action<int> OnPointsChanged;

        [SerializeField] private Vector3 _gridPosition;
        [SerializeField] private Field _field;
        [SerializeField] private BallView _view;
    
        private int _points = 1;
        private bool _selected;
        private float _moveSpeed = 15.0f;
        private float _transparency = 0.0f;
        
        public int Points => _points;
        public bool Selected => _selected;
        
        public Vector3 GridPosition => _gridPosition;
        public Vector3Int IntGridPosition => _field.TransformToIntPosition(_gridPosition);
        
        public Field Field => _field;
        public float Transparency
        {
            get => _transparency;
            set
            {
                _transparency = value;
                OnTransparencyChanged?.Invoke();
            }
        }

        public IEnumerator InnerMove(Vector3Int to, Action<bool> onComplete)
        {
            var path = _field.GetPath(new Vector3Int((int)_gridPosition.x, (int)_gridPosition.y), to);
            var pathFound = path.Count > 0;
            if (pathFound)
            {
                bool moving = true;
        
                float timer = 0;
                for (int i = 0; i < path.Count - 1; i++)
                {
                    var startP = new Vector3(path[i].x, path[i].y);
                    var endP = new Vector3(path[i + 1].x, path[i + 1].y);
                    var pathVec = endP - startP;
                    var pathLength = pathVec.magnitude;
                    var moveTime = pathLength / _moveSpeed;
            
                    while (timer <= moveTime)
                    {
                        timer += Time.deltaTime;
                        var newPosition = (startP + Vector3.Lerp(Vector3.zero, pathVec, timer / moveTime));
                        UpdateGridPosition(newPosition);
                
                        yield return null;
                    }
                    timer -= moveTime;
                }
        
                UpdateGridPosition(new Vector3(to.x, to.y));
            }
        
            onComplete?.Invoke(pathFound);
        }
    
        public IEnumerator InnerMerge(IEnumerable<IFieldMergeable> others, Action onComplete)
        {
            var newPoints = _points;
            foreach (var other in others)
                newPoints += other.Points;
            UpdatePoints(newPoints);
            
            foreach (var other in others)
                _field.DestroySomething(other as Ball);

            yield return null;
            
            onComplete?.Invoke();
        }

        public void SetData(Field field, Vector3 startPosition, int points)
        {
            _field = field;
            UpdatePoints(points);
            
            UpdateGridPosition(startPosition);
        }

        void UpdateGridPosition(Vector3 gridPosition)
        {
            _gridPosition = gridPosition;
            transform.localPosition = _field.GetPositionFromGrid(_gridPosition);
        }

        public void StartMove(Vector3Int endPosition, Action<IFieldMovable, bool> onMovingComplete)
        {
            StartCoroutine(InnerMove(endPosition, OnComplete));

            void OnComplete(bool pathFound)
            {
                onMovingComplete?.Invoke(this, pathFound);
            }
        }
    
        public void StartMerge(IEnumerable<IFieldMergeable> others, Action<IFieldMergeable> onMergeComplete)
        {
            StartCoroutine(InnerMerge(others, OnComplete));

            void OnComplete()
            {
                onMergeComplete?.Invoke(this);
            }
        }
        
        public void Select(bool state)
        {
            _selected = state;
            OnSelectedChanged?.Invoke();
        }

        private void UpdatePoints(int newPoints)
        {
            var oldPoints = _points;
            _points = newPoints;
            OnPointsChanged?.Invoke(oldPoints);
        }
    }
    
}