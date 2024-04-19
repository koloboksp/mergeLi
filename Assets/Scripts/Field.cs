using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using AStar;
using AStar.Options;
using Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;


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
    
    private readonly List<BallDesc> _nextBallsData = new();

    public bool IsEmpty => _balls.Count < _size.x * _size.y;
    public Vector2Int Size => _size;
    
    public IScene Scene => _scene;
    public IFieldView View => _view;

    
    public IReadOnlyList<BallDesc> NextBallsData => _nextBallsData;
   
    private void Awake()
    { 
        _cellSize = new Vector3(View.RootSize.x / Size.x, View.RootSize.x / Size.y, 0);;
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

    public List<BallDesc> GenerateBalls(int num, int[] availableValues, int[] availableHats)
    {
        return AddBalls(num, availableValues, availableHats);
    }

    public Vector3Int CreateBall(Vector3Int position, int points, int hat)
    {
        var newBall = PureCreateBall(position, points, hat);
        _balls.Add(newBall);
        
        return position;
    }
    
    public Ball PureCreateBall(Vector3Int position, int points, int hat)
    {
        var newBall = Instantiate(_ballPrefab, _view.Root);
        newBall.SetData(this, position, points, hat);
        var subComponents = newBall.GetComponents<ISubComponent>();
        foreach (var subComponent in subComponents)
            subComponent.SetData();
        
        return newBall;
    }
    
    public void GenerateNextBallPositions(int count, int[] availableValues, int[] availableHats)
    {
        var freeIndexes = new List<Vector3Int>();
        var usedHats = new List<int>();
        for (var x = 0; x < _size.x; x++)
        for (var y = 0; y < _size.y; y++)
            freeIndexes.Add(new Vector3Int(x, y, 0));
        foreach (var ball in _balls)
        {
            freeIndexes.Remove(ball.IntGridPosition);
            if (ball.Hat != 0)
                usedHats.Add(ball.Hat);
        }

        foreach (var nextBall in _nextBallsData)
            freeIndexes.Remove(nextBall.GridPosition);

        var hatsToAdd = new List<int>();
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
        
        for (var i = 0; i < count; i++)
        {
            if (freeIndexes.Count <= 0) break;
            
            var randomElementIndex = Random.Range(0, freeIndexes.Count);
            var freeIndex = freeIndexes[randomElementIndex];
            freeIndexes.RemoveAt(randomElementIndex);

            var hat = 0;
            if (hatsToAdd.Count > 0)
            {
                hat = hatsToAdd[0];
                hatsToAdd.RemoveAt(0);
            }
            
            _nextBallsData.Add(new BallDesc(freeIndex, availableValues[Random.Range(0, availableValues.Length - 1)], hat));
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

    public List<BallDesc> AddBalls(int amount, int[] availableValues, int[] availableHats)
    {
        foreach (var ball in _balls)
            _nextBallsData.RemoveAll(i => i.GridPosition == ball.IntGridPosition);

        if (_nextBallsData.Count < amount)
            GenerateNextBallPositions(amount - _nextBallsData.Count, availableValues, availableHats);
        else
        {
            if (_nextBallsData.Count > amount)
            {
                var numToRemove = _nextBallsData.Count - amount;
                for (var i = 0; i < numToRemove; i++)
                    _nextBallsData.RemoveAt(_nextBallsData.Count - 1);
            }
        }
        
        var newBallsData = new List<BallDesc>();
        foreach (var ballData in _nextBallsData)
            newBallsData.Add(new BallDesc(ballData.GridPosition, ballData.Points, ballData.Hat));
        
        _nextBallsData.Clear();

        foreach (var ballData in newBallsData)
            CreateBall(ballData.GridPosition, ballData.Points, ballData.Hat);
        
        return newBallsData;
    }

    public List<BallDesc> AddBalls(IEnumerable<BallDesc> newBallsData)
    {
        var result = new List<BallDesc>();
        foreach (var ballData in newBallsData)
        {
            var gridPosition = CreateBall(ballData.GridPosition, ballData.Points, ballData.Hat);
            result.Add(new BallDesc(gridPosition, ballData.Points, ballData.Hat));
        }

        return result;
    }

    public Vector3 GetPositionFromGrid(Vector3 gridPosition)
    {
        return Vector3.Scale(gridPosition + Vector3.one * 0.5f, _cellSize);
    }
    
    public Vector3 GetPositionFromGrid(Vector3Int gridPosition)
    {
        return Vector3.Scale(gridPosition + Vector3.one * 0.5f, _cellSize);
    }
    
    public Vector3Int TransformToIntPosition(Vector3 position)
    {
        return  new Vector3Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y), 0);
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
            if(ball.IntGridPosition != intPosition) continue;
            if(!(ball is T)) continue;
            
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
        List<List<Ball>> ballsOnLines = new List<List<Ball>>();
        
        var originBall = GetBall(position);
        for (int directionI = 0; directionI < Directions.Count; directionI++)
        {
            List<Ball> ballsOnLine = new List<Ball>();
            ballsOnLine.Add(originBall);
            
            for (int sideI = 0; sideI < Sides.Count; sideI++)
            {
                for (int distance = 1; ; distance++)
                {
                    Vector3Int checkingPosition = position + Directions[directionI] *  Sides[sideI] * distance;
                    if (checkingPosition.x < 0 || checkingPosition.y < 0
                                               || checkingPosition.x >= _size.x || checkingPosition.y >= _size.y)
                        break;
                    
                    var foundBall = GetBall(checkingPosition);
                    if(foundBall == null) break;
                    if(originBall.Points != foundBall.Points) break;
                    
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
    }

    public void Init()
    {
        
    }

    public void SetNextBalls(IEnumerable<BallDesc> nextBallsInfos)
    {
        _nextBallsData.Clear();
        _nextBallsData.AddRange(nextBallsInfos);
    }
}