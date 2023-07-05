using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using AStar;
using AStar.Options;
using Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;


public class Field : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IField
{
    public enum StepFinishState
    {
        Move,
        Merge,
        MoveAndRemove
    }
    
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
    
    public event Action<Vector3Int> OnClick;
    
    private Vector2Int _size = new(10, 10);
    private int _minimalBallsCountForCollapse = 5;

    [SerializeField] private FieldView _view;
    [SerializeField] private Scene _scene;
    [SerializeField] private Ball _ballPrefab;
   
    private readonly List<int> _busyIndexes = new List<int>();
    private readonly List<Ball> _balls = new List<Ball>();
    
    private Vector3 _cellSize = new Vector3(256, 256);
    public bool IsEmpty => _balls.Count < _size.x * _size.y;
    public Vector2Int Size => _size;
    
    public Scene Scene => _scene;
    public IFieldView View => _view;
    private void Awake()
    { 
        _cellSize = _view.CellSize();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        var localPosition = _view.Root.InverseTransformPoint(eventData.position);

        var fieldSize = _view.Size;
        var gridPosition = new Vector3Int(
            (int)((localPosition.x / fieldSize.x) * _size.x), 
            (int)((localPosition.y / fieldSize.y) * _size.y));
       
        OnClick?.Invoke(gridPosition);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       
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
            var ballPosition = ball.IntPosition;
            if (ball.IntPosition == from || ball.IntPosition == to) continue;
            
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

    public List<Vector3Int> GenerateBalls(int num)
    {
        return AddBalls(num);
    }

    public Vector3Int CreateBall(Vector3Int position, int points)
    {
        var newBall = Instantiate(_ballPrefab, _view.Root);
        _balls.Add(newBall);
        _busyIndexes.Add(GetIndex(position));

        newBall.SetData(this, position, points);
        var subComponents = newBall.GetComponents<ISubComponent>();
        foreach (var subComponent in subComponents)
            subComponent.SetData();
        

        return position;
    }

    public List<Vector3Int> AddBalls(int num)
    {
        var ballsPositions = new List<Vector3Int>();
        
        for (int i = 0; i < num; i++)
        {
            List<int> freeIndexes = new List<int>();
            for (int x = 0; x < _size.x; x++)
            for (int y = 0; y < _size.y; y++)
                freeIndexes.Add(GetIndex(new Vector3(x, y)));
            foreach (var ball in _balls)
                freeIndexes.Remove(GetIndex(ball.Position));

            var randomElementIndex = Random.Range(0, freeIndexes.Count);
            var freeIndex = freeIndexes[randomElementIndex];
            var position = GetPositionFromIndex(freeIndex);
            
            ballsPositions.Add(CreateBall(position, (int)Mathf.Pow(2, Random.Range(0, 5))));
        }

        return ballsPositions;
    }

    public Vector3 GetPosition(Vector3 position)
    {
        return Vector3.Scale(position, _cellSize);
    }
    
    public Vector3 GetPosition(Vector3Int position)
    {
        return Vector3.Scale(position, _cellSize);
    }
    public Vector3 GetPositionFromWorld(Vector3 position)
    {
        return Vector3.Scale(position, new Vector3(1.0f / _cellSize.x, 1.0f / _cellSize.y, 1.0f / _cellSize.z));
    }
   
    public int GetIndex(Vector3 position)
    {
        return (int)position.y * _size.x + (int)position.x;
    }
    public Vector3Int GetPositionFromIndex(int index)
    {
        return new Vector3Int(index % _size.x, index / _size.y); 
    }

    public Vector3Int GetPointIndex(Vector3 position)
    {
        var pointerLocalPosition = transform.InverseTransformPoint(position);
        var pointerFloatPosition = GetPositionFromWorld(pointerLocalPosition);

        var pointerIndex = GetIndex(pointerFloatPosition);
        return GetPositionFromIndex(pointerIndex);
    }

    public Vector3 GetPointPosition(Vector3Int positionIndex)
    {
        var areaFloatPosition = GetPosition(positionIndex);
        return transform.TransformPoint(areaFloatPosition);
    }
    
    public IEnumerable<T> GetSomething<T>(Vector3Int position) where T : class
    {
        List<T> result = new List<T>();
        foreach (var ball in _balls)
        {
            if(ball.IntPosition != position) continue;
            if(!(ball is T)) continue;
            
            result.Add(ball as T);
        }

        return result;
    }
    
    public void DestroySomething(Ball ball)
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
            if (ball.IntPosition == position)
                return ball;

        return null;
    }

    public void DestroyBalls(List<Ball> ballsToRemove)
    {
        foreach (var ball in ballsToRemove)
        {
            _balls.Remove(ball);
            Destroy(ball.gameObject);
        }
    }
    
    public void AdaptSize(Vector3 leftBottomCorner, Vector3 rightTopCorner, Vector2 rectSize)
    {
       
    }
}