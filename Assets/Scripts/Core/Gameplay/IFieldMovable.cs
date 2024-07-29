using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core.Gameplay
{
    public interface IFieldMovable
    {
        Task StartMove(Vector3Int endPosition, CancellationToken cancellationToken);
        Vector3Int IntGridPosition { get; }
    }
}