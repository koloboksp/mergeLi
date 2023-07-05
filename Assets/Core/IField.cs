using System.Collections.Generic;
using Core;
using UnityEngine;

public interface IField
{
    IEnumerable<T> GetSomething<T>(Vector3Int position) where T : class;
    List<List<Ball>> CheckCollapse(Vector3Int checkingPosition);
    void DestroyBalls(List<Ball> ballsToRemove);
    List<Vector3Int> GenerateBalls(int count);
    Vector3Int CreateBall(Vector3Int position, int points);
    Vector3 GetPosition(Vector3Int position);
    public IFieldView View { get; }
}

public interface IFieldView
{
    Transform Root { get; }
}