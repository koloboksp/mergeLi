using System.Threading;
using System.Threading.Tasks;

namespace Core.Steps.CustomOperations
{
    public class UndoConfirmBuffUseOperation : Operation
    {
        private readonly Buff _buff; 
        public UndoConfirmBuffUseOperation(Buff buff)
        {
            _buff = buff;
        }
        
        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            _buff.ConsumeCooldown(-1);

            return null;
        }
    }
}