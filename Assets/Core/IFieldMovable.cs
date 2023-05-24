using System;
using UnityEngine;

public interface IFieldMovable
{
    void StartMove(Vector3Int endPosition, Action<IFieldMovable, bool> onMovingComplete);
}