using System.Collections.Generic;
using Core;
using UnityEngine;

public interface IBall : IFieldMovable, IFieldSelectable, IFieldMergeable
{
    public Vector3Int IntGridPosition { get; }
    public int Points { get; }
}

public interface IField
{
    IEnumerable<T> GetSomething<T>(Vector3Int position) where T : class;
    List<List<Ball>> CheckCollapse(Vector3Int checkingPosition);
    void DestroyBalls(List<Ball> ballsToRemove);
    List<(Vector3Int intPosition, int points)> GenerateBalls(int count, Vector2Int valueRange);
    Vector3Int CreateBall(Vector3Int position, int points);
    Vector3 GetPositionFromGrid(Vector3Int gridPosition);
    public IFieldView View { get; }
    void GenerateNextBallPositions(int count, Vector2Int valueRange);
    IEnumerable<T> GetAll<T>();
    Vector3 CellSize();
}

public interface IFieldView
{
    Transform Root { get; }
    Vector2 RootSize { get; }
}