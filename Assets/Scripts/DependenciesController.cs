using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace Core
{
    public class DependenciesController
    {
        private readonly Dictionary<Type, object> _dependencies = new();
        public void Set<T>(T value) where T : class
        {
            _dependencies[typeof(T)] = value;
        }

        public T Get<T>() where T : class
        {
            return _dependencies[typeof(T)] as T;
        }
    }
}