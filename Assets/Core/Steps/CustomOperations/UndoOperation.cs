namespace Core.Steps.CustomOperations
{
    public class UndoOperation : Operation
    {
        private readonly StepMachine _stepMachine;
        
        public UndoOperation(StepMachine stepMachine)
        {
            _stepMachine = stepMachine;
        }
        
        protected override void InnerExecute()
        {
            _stepMachine.Undo();
            Complete(null);
        }

        public override Operation GetInverseOperation()
        {
            return null;
        }
    }
}