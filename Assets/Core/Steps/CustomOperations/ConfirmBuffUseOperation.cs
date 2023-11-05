using System.Threading;
using System.Threading.Tasks;

namespace Core.Steps.CustomOperations
{
    public class ConfirmBuffUseOperation : Operation
    {
        private readonly Buff _buff;
        
        public ConfirmBuffUseOperation(Buff buff)
        {
            _buff = buff;
        }
        
        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            return null;
        }

        public override Operation GetInverseOperation()
        {
            var undo = new UndoConfirmBuffUseOperation(_buff);
            return undo;
        }
    }
}