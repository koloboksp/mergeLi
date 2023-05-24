using System.Collections.Generic;

namespace Core.Steps.CustomOperations
{
    public class CollapseCheckOperation : Operation
    {
        private readonly List<Operation> _onSuccess = new List<Operation>();
        private readonly List<Operation> _onFail = new List<Operation>();
        public CollapseCheckOperation(List<Operation> onSuccess, List<Operation> onFail)
        {
            if(onSuccess != null)
                _onSuccess.AddRange(onSuccess);
            if(onFail != null)
                _onFail.AddRange(onFail);
        }

        protected override void InnerExecute()
        {
            base.InnerExecute();

            var collapseOperationData = Owner.GetData<CollapseOperationData>();
            if (collapseOperationData.CollapseLines.Count > 0)
                Owner.AddOperations(_onSuccess);
            else
                Owner.AddOperations(_onFail);

            Complete(null);
        }

        public override Operation GetInverseOperation()
        {
            return null;
        }
    }
}