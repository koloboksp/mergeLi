using System;
using System.Collections.Generic;
using System.Linq;
using AStar;
using AStar.Options;
using Core.Utils;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Core.Gameplay
{
    public class Field : MonoBehaviour, IField
    {
        static readonly List<Vector3Int> Directions = new List<Vector3Int>()
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(1, -1, 0),
        };
        
        static readonly List<int> Sides = new List<int>()
        {
            1,
            -1,
        };
    
        public event Action<Vector3Int> OnPointerDown;
    
        private Vector2Int _size = new(9, 9);
        private int _minimalBallsCountForCollapse = 5;

        [SerializeField] private FieldView _view;
        [SerializeField] private Scene _scene;
        [SerializeField] private Ball _ballPrefab;
   
        private readonly List<Ball> _balls = new List<Ball>();
    
        private Vector3 _cellSize;
    
        private readonly List<BallDesc> _nextBallsDescs = new();

        public bool IsFullFilled => _balls.Count >= _size.x * _size.y;
        public bool IsEmpty => _balls.Count == 0;
        public Vector2Int Size => _size;
    
        public IScene Scene => _scene;
        public IFieldView View => _view;

    
        public IReadOnlyList<BallDesc> NextBallsDescs => _nextBallsDescs;
   
        private void Awake()
        { 
            _cellSize = new Vector3(View.RootSize.x / Size.x, View.RootSize.x / Size.y, 0);;
        }

        public void SetData()
        {
            _view.SetData();
        }
    
        public Vector3 ScreenPointToWorld(Vector3 screenPosition)
        {
            if (_view.Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return screenPosition;
        
            return _view.Canvas.worldCamera.ScreenToWorldPoint(screenPosition);
        }

        internal void InnerOnPointerDown(Vector3Int gridPosition)
        {
            OnPointerDown?.Invoke(gridPosition);
        }
    
        List<Vector2Int> CalculatePath(Vector3Int from, Vector3Int to)
        {
            Position dFrom = new Position(from.x, from.y);
            Position dTo = new Position(to.x, to.y);

            var pathfinderOptions = new PathFinderOptions { 
                PunishChangeDirection = true,
                UseDiagonals = false, 
            };

            var tiles = new short[_size.x,_size.y];
            for (var x = 0; x < _size.x; x++)
            for (var y = 0; y < _size.y; y++)
                tiles[x, y] = 1;

            foreach (var ball in _balls)
            {
                var ballPosition = ball.IntGridPosition;
                if (ball.IntGridPosition == from || ball.IntGridPosition == to) continue;
            
                tiles[ballPosition.x, ballPosition.y] = 0;
            }
        
            var worldGrid = new WorldGrid(tiles);
            var pathfinder = new PathFinder(worldGrid, pathfinderOptions);
    
    
            Position[] dPath = pathfinder.FindPath(dFrom, dTo);
        
            var path = new List<Vector2Int>();
            foreach (var dPosition in dPath)
                path.Add(new Vector2Int(dPosition.Row, dPosition.Column));

            return path;
        }

        public List<Vector2Int> GetPath(Vector3Int from, Vector3Int to)
        {
            return CalculatePath(from, to);
        }

        public List<BallDesc> GenerateBalls(int num, BallWeight[] availableValues, string[] availableHats)
        {
            return AddBalls(num, availableValues, availableHats);
        }

        public Vector3Int CreateBall(Vector3Int position, int points, string hat)
        {
            var newBall = PureCreateBall(position, points, hat);
            _balls.Add(newBall);
        
            return position;
        }
    
        public Ball PureCreateBall(Vector3Int position, int points, string hat)
        {
            var newBall = Instantiate(_ballPrefab, _view.Root);
            newBall.SetData(this, position, points, hat);
            var subComponents = newBall.GetComponents<ISubComponent>();
            foreach (var subComponent in subComponents)
                subComponent.SetData();
            newBall.Born();
            
            return newBall;
        }
    
        public void GenerateNextBall(int minimalCount, BallWeight[] availableValues, string[] availableHats)
        {
            var freeIndexes = new List<Vector3Int>();
            for (var x = 0; x < _size.x; x++)
            for (var y = 0; y < _size.y; y++)
                freeIndexes.Add(new Vector3Int(x, y, 0));
            
            var usedHats = new List<string>();
            foreach (var ball in _balls)
            {
                freeIndexes.Remove(ball.IntGridPosition);
                if (!string.IsNullOrEmpty(ball.HatName))
                    usedHats.Add(ball.HatName);
            }

            foreach (var nextBallDesc in _nextBallsDescs)
            {
                freeIndexes.Remove(nextBallDesc.GridPosition);
                if (!string.IsNullOrEmpty(nextBallDesc.HatName))
                    usedHats.Add(nextBallDesc.HatName);
            }

            var hatsToAdd = new List<string>();
            foreach (var activeHat in availableHats)
            {
                if (usedHats.Contains(activeHat))
                {
                
                }
                else
                {
                    hatsToAdd.Add(activeHat);
                }
            }

            var countToGenerate = Mathf.Max(minimalCount - _nextBallsDescs.Count, 0);
            for (var i = 0; i < countToGenerate; i++)
            {
                if (freeIndexes.Count <= 0) break;
            
                var randomElementIndex = Random.Range(0, freeIndexes.Count);
                var freeIndex = freeIndexes[randomElementIndex];
                freeIndexes.RemoveAt(randomElementIndex);

                string hat = null;
                if (hatsToAdd.Count > 0)
                {
                    hat = hatsToAdd[0];
                    hatsToAdd.RemoveAt(0);
                }
                
                var sumWeight = 0;
                for (var ballWeightI = 0; ballWeightI < availableValues.Length; ballWeightI++)
                {
                    var ballWeight = availableValues[ballWeightI];
                    sumWeight += ballWeight.Weight;
                }
                var randomValue = Random.Range(0, sumWeight);

                var currentWeight = 0;
                var points = 0;
                for (var ballWeightI = 0; ballWeightI < availableValues.Length; ballWeightI++)
                {
                    var ballWeight = availableValues[ballWeightI];
                    currentWeight += ballWeight.Weight;
                    if (randomValue < currentWeight)
                    {
                        points = ballWeight.Points;
                        break;
                    }
                }
                
                _nextBallsDescs.Add(new BallDesc(freeIndex, points, hat));
            }
        }

        public IEnumerable<T> GetAll<T>()
        {
            var result = _balls.Where(i => i is T).Cast<T>();
        
            return result;
        }

        public Vector3 CellSize()
        {
            return _cellSize;
        }

        public List<BallDesc> AddBalls(int amount, BallWeight[] availableValues, string[] availableHats)
        {
            foreach (var ball in _balls)
                _nextBallsDescs.RemoveAll(i => i.GridPosition == ball.IntGridPosition);

            if (_nextBallsDescs.Count < amount)
                GenerateNextBall(amount, availableValues, availableHats);
            
            var newBallsData = new List<BallDesc>();

            while (_nextBallsDescs.Count > 0 && newBallsData.Count < amount)
            {
                var ballData = _nextBallsDescs[0];
                newBallsData.Add(new BallDesc(ballData.GridPosition, ballData.Points, ballData.HatName));
                _nextBallsDescs.RemoveAt(0);
            }
          
            foreach (var ballData in newBallsData)
                CreateBall(ballData.GridPosition, ballData.Points, ballData.HatName);
        
            return newBallsData;
        }

        public List<BallDesc> AddBalls(IEnumerable<BallDesc> newBallsData)
        {
            var result = new List<BallDesc>();
            foreach (var ballData in newBallsData)
            {
                var gridPosition = CreateBall(ballData.GridPosition, ballData.Points, ballData.HatName);
                result.Add(new BallDesc(gridPosition, ballData.Points, ballData.HatName));
            }

            return result;
        }

        public void UpdateSiblingIndex(Vector3 gridPosition, Transform target)
        {
            _view.UpdateSiblingIndex(gridPosition, target);
        }

        public Vector3 GetPositionFromGrid(Vector3 gridPosition)
        {
            return Vector3.Scale(gridPosition + Vector3.one * 0.5f, _cellSize);
        }
    
        public Vector3 GetPositionFromGrid(Vector3Int gridPosition)
        {
            return Vector3.Scale(gridPosition + Vector3.one * 0.5f, _cellSize);
        }
    
        public Vector3Int TransformToIntPosition(Vector3 gridPosition)
        {
            return new Vector3Int(Mathf.FloorToInt(gridPosition.x), Mathf.FloorToInt(gridPosition.y), 0);
        }
    
        public Vector3 GetGridPositionFromWorld(Vector3 position)
        {
            return Vector3.Scale(position, new Vector3(1.0f / _cellSize.x, 1.0f / _cellSize.y, 1.0f / _cellSize.z));
        }

        public Vector3Int GetPointGridIntPosition(Vector3 worldPosition)
        {
            var pointerLocalPosition = transform.InverseTransformPoint(worldPosition);
            var pointerGridPosition = GetGridPositionFromWorld(pointerLocalPosition);

            return TransformToIntPosition(pointerGridPosition);
        }

        public Vector3 GetWorldPosition(Vector3Int gridPosition)
        {
            var areaFloatPosition = GetPositionFromGrid(gridPosition);
            return transform.TransformPoint(areaFloatPosition);
        }

        public Vector3 GetWorldPosition(Vector3 gridPosition)
        {
            var areaFloatPosition = GetPositionFromGrid(gridPosition);
            return transform.TransformPoint(areaFloatPosition);
        }

        public Vector3 GetWorldCellSize()
        {
            return View.Root.TransformVector(_cellSize);
        }

        public IEnumerable<T> GetSomething<T>(Vector3Int intPosition) where T : class
        {
            var result = new List<T>();
            foreach (var ball in _balls)
            {
                if (ball.IntGridPosition != intPosition) 
                    continue;
                if (!(ball is T)) 
                    continue;
            
                result.Add(ball as T);
            }

            return result;
        }
    
        public void DestroyBall(Ball ball)
        {
            _balls.Remove(ball);
            Destroy(ball.gameObject);
        }
    
        public List<List<Ball>> CheckCollapse(Vector3Int checkingPosition)
        {
            return Check(checkingPosition, _minimalBallsCountForCollapse);
        }

        List<List<Ball>> Check(Vector3Int position, int requiredCount)
        {
            var ballsOnLines = new List<List<Ball>>();
        
            var originBall = GetBall(position);
            for (int directionI = 0; directionI < Directions.Count; directionI++)
            {
                var ballsOnLine = new List<Ball>();
                ballsOnLine.Add(originBall);
            
                for (var sideI = 0; sideI < Sides.Count; sideI++)
                {
                    for (var distance = 1; ; distance++)
                    {
                        var checkingPosition = position + Directions[directionI] *  Sides[sideI] * distance;
                        if (checkingPosition.x < 0 || checkingPosition.y < 0 || checkingPosition.x >= _size.x || checkingPosition.y >= _size.y)
                            break;

                        var foundBall = GetBall(checkingPosition);
                        if (foundBall == null) 
                            break;
                        if (originBall.Points != foundBall.Points) 
                            break;
                    
                        ballsOnLine.Add(foundBall);
                    }
                }
            
                if(ballsOnLine.Count >= requiredCount)
                    ballsOnLines.Add(ballsOnLine);
            }
        
            return ballsOnLines;
        }
   
        private Ball GetBall(Vector3Int position)
        {
            foreach (var ball in _balls)
                if (ball.IntGridPosition == position)
                    return ball;

            return null;
        }

        public void DestroyBalls(List<Ball> ballsToRemove, bool force)
        {
            foreach (var ball in ballsToRemove)
            {
                _balls.Remove(ball);
                ball.Remove(force);
                Destroy(ball.gameObject);
            }
        }
    
        public void AdaptSize(Vector3 leftBottomCorner, Vector3 rightTopCorner, Vector2 rectSize)
        {
       
        }

        public int CalculateEmptySpacesCount()
        {
            return _size.x * _size.y - _balls.Count;
        }

        public void Clear()
        {
            var ballsToRemove = new List<Ball>(_balls);
            DestroyBalls(ballsToRemove, true);
            
            _nextBallsDescs.Clear();
        }

        public void Init()
        {
        
        }

        public void SetNextBalls(IEnumerable<BallDesc> nextBallsInfos)
        {
            _nextBallsDescs.Clear();
            _nextBallsDescs.AddRange(nextBallsInfos);
        }

        public bool HasMergeSteps(int[] availableValues)
        {
            var ballsGrid = new Ball[_size.x,_size.y];
            foreach (var ball in _balls)
            {
                var ballPosition = ball.IntGridPosition;
                ballsGrid[ballPosition.x, ballPosition.y] = ball;
            }

            var checkingBallOffsets = new Vector2Int[]
            {
                Vector2Int.left,
                Vector2Int.right,
                Vector2Int.down,
                Vector2Int.up,
            };
            var maxAvailablePoints = availableValues[availableValues.Length - 1];
            for (var x = 0; x < ballsGrid.GetLength(0); x++)
            {
                for (var y = 0; y < ballsGrid.GetLength(1); y++)
                {
                    var ballPosition = new Vector2Int(x, y);
                    var ball = ballsGrid[ballPosition.x, ballPosition.y];
                    if (ball.Points == maxAvailablePoints)
                        continue;
                    for (var offsetI = 0; offsetI < checkingBallOffsets.Length; offsetI++)
                    {
                        var checkingBallPosition = ballPosition + checkingBallOffsets[offsetI];
                        if ((checkingBallPosition.x >= 0 && checkingBallPosition.x < ballsGrid.GetLength(0))
                            && (checkingBallPosition.y >= 0 && checkingBallPosition.y < ballsGrid.GetLength(1)))
                        {
                            var checkingBall = ballsGrid[checkingBallPosition.x, checkingBallPosition.y];
                            if (ball.Points == checkingBall.Points)
                                return true;
                        }
                    }
                }
            }
        
            return false;

        }
    }
}