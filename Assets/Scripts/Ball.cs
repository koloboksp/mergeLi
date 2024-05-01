using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public class Ball : MonoBehaviour, IBall
    {
        public event Action OnSelectedChanged;
        public event Action OnMovingStateChanged;
        public event Action OnTransparencyChanged;
        public event Action<int, bool> OnPointsChanged;
        public event Action<int, bool> OnHatChanged;
        public event Action OnPathNotFound;

        public static event Action<Ball, bool> OnMovingStateChangedGlobal;

        [SerializeField] private Vector3 _gridPosition;
        [SerializeField] private IField _field;
        [SerializeField] private BallView _view;
    
        private int _points = 1;
        private int _hat = 0;
        private bool _selected;
        private bool _moving;
        private float _moveSpeed = 15.0f;
        private float _transparency = 0.0f;
        
        public BallView View => _view;
        public int Points => _points;
        public int Hat => _hat;

        public bool Selected => _selected;
        public bool Moving => _moving;

        public Vector3 GridPosition => _gridPosition;
        public Vector3Int IntGridPosition => _field.TransformToIntPosition(_gridPosition);
        
        public IField Field => _field;
        public float Transparency
        {
            get => _transparency;
            set
            {
                _transparency = value;
                OnTransparencyChanged?.Invoke();
            }
        }

     

        public async Task InnerMove(Vector3Int to, CancellationToken cancellationToken)
        {
            var path = _field.GetPath(new Vector3Int((int)_gridPosition.x, (int)_gridPosition.y), to);
            var pathFound = path.Count > 0;
            if (pathFound)
            {
                _moving = true;
                OnMovingStateChanged?.Invoke();
                OnMovingStateChangedGlobal?.Invoke(this, _moving);

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
                        cancellationToken.ThrowIfCancellationRequested();
                        
                        timer += Time.deltaTime;
                        var newPosition = (startP + Vector3.Lerp(Vector3.zero, pathVec, timer / moveTime));
                        UpdateGridPosition(newPosition);

                        await Task.Yield();
                    }
                    timer -= moveTime;
                }
        
                UpdateGridPosition(new Vector3(to.x, to.y));
                _moving = false;
                OnMovingStateChanged?.Invoke();
                OnMovingStateChangedGlobal?.Invoke(this, _moving);
            }
        }

        private async Task InnerMerge(IEnumerable<IFieldMergeable> others, CancellationToken cancellationToken)
        {
            var newPoints = _points;
            var newHat = _hat;
            foreach (var other in others)
            {
                if (other is IBall otherBall)
                {
                    newPoints += otherBall.Points;
                    newHat = Mathf.Max(newHat, otherBall.Hat);
                }
            }

            UpdatePoints(newPoints, false);
            UpdateHat(newHat, false);
            
            foreach (var other in others)
                _field.DestroyBall(other as Ball);
        }

        public bool CanGrade(int level)
        {
            var newPoints = _points;
            var currentLevel = 0;
            while (currentLevel < Math.Abs(level))
            {
                if(level > 0)
                    newPoints *= 2;
                else
                {
                    if(newPoints > 1)
                        newPoints /= 2;
                    else if (newPoints == 1)
                        newPoints = 0;
                    else
                        return false;
                }
                currentLevel++;
            }

            return true;
        }
        
        public async Task InnerGrade(int level, CancellationToken cancellationToken)
        {
            var newPoints = _points;
            var currentLevel = 0;
            while (currentLevel < Math.Abs(level))
            {
                if (level > 0)
                {
                    if (newPoints == 0)
                        newPoints = 1;
                    else
                        newPoints *= 2;
                }
                else
                    newPoints /= 2;
                
                currentLevel++;
            }
         
            UpdatePoints(newPoints, false);
        }

        public void SetData(IField field, Vector3 startPosition, int points, int hat)
        {
            _field = field;
            UpdatePoints(points, true);
            UpdateHat(hat, true);
            
            UpdateGridPosition(startPosition);
        }

        private void UpdateGridPosition(Vector3 gridPosition)
        {
            var oldGridPosition = _gridPosition;
            _gridPosition = gridPosition;
            transform.localPosition = _field.GetPositionFromGrid(_gridPosition);
           
            var oldGridPositionInt = _field.TransformToIntPosition(oldGridPosition);
            var gridPositionInt = _field.TransformToIntPosition(_gridPosition);
            if (oldGridPositionInt != gridPositionInt)
                _field.UpdateSiblingIndex(_gridPosition, transform);
        }

        public async Task StartMove(Vector3Int endPosition, CancellationToken cancellationToken)
        {
            await InnerMove(endPosition, cancellationToken);
        }
    
        public async Task<bool> MergeAsync(IEnumerable<IFieldMergeable> others, CancellationToken cancellationToken)
        {
            await InnerMerge(others, cancellationToken);

            return true;
        }
        
        public async Task<Ball> StartGrade(int level, CancellationToken cancellationToken)
        {
            await InnerGrade(level, cancellationToken);
            return this;
        }
        
        public int GetColorIndex()
        {
            var colorIndex = Mathf.RoundToInt(Mathf.Log(_points, 2));
            if (colorIndex < 0)
                colorIndex = 0;
            else
                colorIndex += 1;
            
            return colorIndex;
        }
        
        public static int GetColorIndex(int points, int count)
        {
            var colorIndex = Mathf.RoundToInt(Mathf.Log(points, 2));
            colorIndex %= count;
            return colorIndex;
        }
        
        public void Select(bool state)
        {
            _selected = state;
            if (_selected)
            {
                transform.SetAsLastSibling();
            }
            else
            {
                _field.UpdateSiblingIndex(_gridPosition, transform);
            }
            OnSelectedChanged?.Invoke();
        }

        private void UpdatePoints(int newPoints, bool force)
        {
            var oldPoints = _points;
            _points = newPoints;
            OnPointsChanged?.Invoke(oldPoints, force);
        }

        private void UpdateHat(int newHat, bool force)
        {
            var oldHat = _hat;
            _hat = newHat;
            OnHatChanged?.Invoke(oldHat, force);
        }

        public void PathNotFound()
        {
            OnPathNotFound?.Invoke();
        }

        public void Remove(bool force)
        {
            _view.Remove(force);
        }
    }
    
}