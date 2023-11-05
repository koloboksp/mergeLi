using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Steps
{
    public abstract class Operation
    {
        public event Action<Operation, object> OnComplete;

        internal Step Owner { get; set; }
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var result = await InnerExecuteAsync(cancellationToken);
            OnComplete?.Invoke(this, result);
        }

        protected virtual async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            return null;
        }
        
        public Operation SubscribeCompleted(Action<Operation, object> action)
        {
            OnComplete += action;
            return this;
        }

        public virtual Operation GetInverseOperation() => null;
    }
}