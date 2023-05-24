using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Steps
{
    public class Step
    {
        public event Action<Step> OnComplete;

        private readonly string _tag;
        private readonly List<Operation> _operations = new List<Operation>();
        private int _executeOperationIndex = 0;
        private bool _completed = false;
        private bool _launched = false;
        private readonly Dictionary<Type, object> _operationsData = new Dictionary<Type, object>();

        public string Tag => _tag;
        public bool Completed => _completed;
        public bool Launched => _launched;

        public IEnumerable<Operation> Operations => _operations;
        
        public Step(string tag, params Operation[] operations)
        {
            _tag = tag;
            AddOperations(operations);
        }

        public void AddOperations(IEnumerable<Operation> operations)
        {
            if (operations != null)
                foreach (var operation in operations)
                    if(operation != null)
                        _operations.Add(operation);
            
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
            if (_executeOperationIndex < _operations.Count)
            {
                var operation = _operations[_executeOperationIndex];
                if (!operation.Launched)
                {
                    operation.OnComplete += Operation_OnCompleted;
                    operation.Execute();
                }
            }
        }
    
        void Operation_OnCompleted(Operation sender, object data)
        {
            sender.OnComplete -= Operation_OnCompleted;
            _executeOperationIndex += 1;
            
            if (_executeOperationIndex == _operations.Count)
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
            if(_operationsData.TryGetValue(typeof(T), out var value))
                return value as T;
            return null;
        }
    }
}