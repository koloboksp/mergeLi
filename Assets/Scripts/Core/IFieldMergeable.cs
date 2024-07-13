using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public interface IFieldMergeable
    {
        Task<bool> MergeAsync(IEnumerable<IFieldMergeable> others, CancellationToken cancellationToken);
    }
}