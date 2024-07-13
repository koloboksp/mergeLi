using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public interface IFieldMovable
    {
        Task StartMove(Vector3Int endPosition, CancellationToken cancellationToken);
    }
}