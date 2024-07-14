using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Gameplay;

namespace Core.Steps
{
    public class Step
    {
        public event Action<Step> OnComplete;

        private readonly StepTag _tag;
        private readonly List<Operation> _operations = new List<Operation>();
        private int _executeOperationIndex = 0;
        private bool _completed = false;
        private bool _launched = false;
        private readonly Dictionary<Type, object> _operationsData = new Dictionary<Type, object>();

        public StepTag Tag => _tag;
        public bool Completed => _completed;
        public bool Launched => _launched;

        public IEnumerable<Operation> Operations => _operations;
        
        public Step(StepTag tag, params Operation[] operations)
        {
            _tag = tag;
            AddOperations(operations);
        }

        public void AddOperations(IEnumerable<Operation> operations)
        {
            if (operations == null) 
                return;

            foreach (var operation in operations)
            {
                if (operation == null) 
                    continue;
                
                _operations.Add(operation);
                operation.Owner = this;
            }
        }
        
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _launched = true;

            for (var oI = 0; oI < _operations.Count; oI++)
            {
                var operation = _operations[oI];
                await operation.ExecuteAsync(cancellationToken);
            }

            _launched = false;
            _completed = true;
            
            OnComplete?.Invoke(this);
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

        public T FindOperation<T>() where T : Operation
        {
            return _operations.Find(i => i.GetType() == typeof(T)) as T;
        }
    }

    public enum StepExecutionType
    {
        Redo,
        Undo
    }
}