using System.Threading;
using System.Threading.Tasks;

namespace Core.Steps.CustomOperations
{
    public class UndoOperation : Operation
    {
        private readonly StepMachine _stepMachine;
        
        public UndoOperation(StepMachine stepMachine)
        {
            _stepMachine = stepMachine;
        }
        
        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            _stepMachine.Undo();
            return null;
        }

        public override Operation GetInverseOperation()
        {
            return null;
        }
    }
}