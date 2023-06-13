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
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class Scene : MonoBehaviour
{
    private SkinsLibrary _skinsLibrary;
    
    public void SetSkin(string skinName)
    {
        var skinContainer = _skinsLibrary.GetContainer(skinName);
        var skinChangeables = this.GetComponents<ISkinChangeable>();
        foreach (var skinChangeable in skinChangeables)
        {
            skinChangeable.ChangeSkin(skinContainer);
        }
    }
}

public class Field : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IField, IFieldView
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
    
    private int _size = 10;
    private int _minimalBallsCount = 5;

    public RectTransform _fieldRoot;

    private List<int> _busyIndexes = new List<int>();
    private List<Ball> _balls = new List<Ball>();
    
 
    private Vector3 _cellSize = new Vector3(107, 107);
    public bool IsEmpty => _balls.Count < _size * _size;
    public int size => _size;

    public Ball _ballPrefab;
    // Start is called before the first frame update

    private void Awake()
    {
        var cellSize = _fieldRoot.rect.size.x / _size;
        _cellSize = new Vector3(cellSize, cellSize);
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        var inverseTransformPoint = _fieldRoot.InverseTransformPoint(eventData.position);

        var rect = _fieldRoot.rect;
        Vector3Int pointerGridPosition = new Vector3Int((int)((inverseTransformPoint.x /rect.width) * _size), (int)((inverseTransformPoint.y /rect.height) * _size));
       
        OnClick?.Invoke(pointerGridPosition);
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

        var tiles = new short[_size,_size];
        for (int x = 0; x < _size; x++)
            for (int y = 0; y < _size; y++)
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
        var newBall = Instantiate(_ballPrefab, _fieldRoot);
        _balls.Add(newBall);
        _busyIndexes.Add(GetIndex(position));
        
        newBall.SetData(this, position, points);
        
        return position;
    }

    public List<Vector3Int> AddBalls(int num)
    {
        var ballsPositions = new List<Vector3Int>();
        
        for (int i = 0; i < num; i++)
        {
            List<int> freeIndexes = new List<int>();
            for (int x = 0; x < _size; x++)
                for (int y = 0; y < _size; y++)
                    freeIndexes.Add(GetIndex(new Vector3(x, y)));
            foreach (var ball in _balls)
                freeIndexes.Remove(GetIndex(ball.Position));
            
            var randomElementIndex = Random.Range(0, freeIndexes.Count);
            var freeIndex = freeIndexes[randomElementIndex];
           // _busyIndexes.Add(freeIndex);
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

   
    public int GetIndex(Vector3 position)
    {
        return (int)position.y * _size + (int)position.x;
    }
    public Vector3Int GetPositionFromIndex(int index)
    {
        return new Vector3Int(index % size, index / size); 
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
        return Check(checkingPosition, _minimalBallsCount);
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
                                               || checkingPosition.x >= _size || checkingPosition.x >= _size)
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

    public IFieldView FieldView => this;
    public Transform FieldRoot => _fieldRoot;
}