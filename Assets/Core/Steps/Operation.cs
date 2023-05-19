using System;

namespace Core.Steps
{
    public class Operation
    {
        public event Action<Operation, object> OnComplete;

        internal Step Owner { get; set; }
    
        private bool _completed = false;
        private bool _launched = false;

        public bool Completed => _completed;
        public bool Launched => _launched;

        public void Execute()
        {
            if(_launched) return;

            _launched = true;
        
            InnerExecute();
        }

        protected virtual void InnerExecute()
        {
        }

        protected void Complete(object result)
        {
            _launched = false;
            _completed = true;
        
            OnComplete?.Invoke(this, result);
        }

        public Operation SubscribeCompleted(Action<Operation, object> action)
        {
            OnComplete += action;
            return this;
        }
    }
}