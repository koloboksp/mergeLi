namespace Core.Steps.CustomOperations
{
    public class ProcessBuffCooldownOperation : Operation
    {
        private readonly Buff _buff;
        private int _stepValue = 1;
        
        public ProcessBuffCooldownOperation(Buff buff)
        {
            _buff = buff;
        }
        
        protected override void InnerExecute()
        {
            _buff.ConsumeCooldown(_stepValue);
            
            Complete(null);
        }

        public override Operation GetInverseOperation()
        {
            var undo = new ProcessBuffCooldownOperation(_buff)
            {
                _stepValue = -1
            };
            return undo;
        }
    }
}