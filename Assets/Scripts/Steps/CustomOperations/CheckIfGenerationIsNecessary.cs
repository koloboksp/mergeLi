using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Steps.CustomOperations
{
    public class CheckIfGenerationIsNecessary : Operation
    {
        private readonly List<Operation> _onSuccess = new List<Operation>();
        private readonly List<Operation> _onFail = new List<Operation>();
        public CheckIfGenerationIsNecessary(List<Operation> onSuccess, List<Operation> onFail)
        {
            if(onSuccess != null)
                _onSuccess.AddRange(onSuccess);
            if(onFail != null)
                _onFail.AddRange(onFail);
        }

        protected override async Task<object> InnerExecuteAsync(CancellationToken cancellationToken)
        {
            base.InnerExecuteAsync(cancellationToken);

            var collapseOperationData = Owner.GetData<CollapseOperationData>();
            if (collapseOperationData.CollapseLines.Count > 0)
                Owner.AddOperations(_onSuccess);
            else
                Owner.AddOperations(_onFail);

            return null;
        }

        public override Operation GetInverseOperation()
        {
            return null;
        }
    }
}