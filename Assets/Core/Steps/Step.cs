using System;
using System.Collections.Generic;

namespace Core.Steps
{
    public class Step
    {
        public event Action<Step> OnComplete;

        private string _tag;
        private List<Operation> _operations = new List<Operation>();
        private bool _completed = false;
        private bool _launched = false;
        private Dictionary<Type, object> _operationsData = new Dictionary<Type, object>();

        public string Tag => _tag;
        public bool Completed => _completed;
        public bool Launched => _launched;
        public Step(string tag, params Operation[] operations)
        {
            _tag = tag;
            if(operations != null)
                _operations.AddRange(operations);
        
            _operations.ForEach(i => i.Owner = this);
        }
    
        public void Execute()
        {
            if(_launched) return;

            _launched = true;

            RunNext();
        }

        private void RunNext()
        {
            if (_operations.Count > 0)
            {
                var operation = _operations[0];
                if (!operation.Launched)
                {
                    operation.OnComplete += Operation_OnCompleted;
                    operation.Execute();
                }
            }
        }
    
        void Operation_OnCompleted(Operation sender, object data)
        {
            var foundI = _operations.IndexOf(sender);
            if (foundI >= 0)
            {
                _operations[foundI].OnComplete -= Operation_OnCompleted;
                _operations.RemoveAt(foundI);
            }

            if (_operations.Count == 0)
            {
                _launched = false;
                _completed = true;
                OnComplete?.Invoke(this);
            }
            else
            {
                RunNext();
            }
        }

        public void SetData(object value)
        {
            _operationsData[value.GetType()] = value;
        }
    
        public T GetData<T>() where T : class
        {
            return _operationsData[typeof(T)] as T;
        }
    }
}