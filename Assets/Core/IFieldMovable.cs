using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public interface IFieldMovable
{
    Task StartMove(Vector3Int endPosition, CancellationToken cancellationToken);
}