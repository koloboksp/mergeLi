namespace Core.Steps.CustomOperations
{
    public class UndoConfirmBuffUseOperation : Operation
    {
        private readonly Buff _buff; 
        public UndoConfirmBuffUseOperation(Buff buff)
        {
            _buff = buff;
        }
        
        protected override void InnerExecute()
        {
            _buff.ConsumeCooldown(-1);
            
            Complete(null);
        }
    }
}