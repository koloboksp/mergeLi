using System;
using System.Collections.Generic;

namespace Core.Utils
{
    public class DependenciesController
    {
        private static readonly DependenciesController _instance = new DependenciesController();

        public static DependenciesController Instance => _instance;
        
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