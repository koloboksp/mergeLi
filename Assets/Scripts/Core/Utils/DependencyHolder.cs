namespace Core.Utils
{
    public struct DependencyHolder<T> where T : class
    {
        private bool _initialized;
        private T _value;
        
        public T Value
        {
            get
            {
                if (!_initialized)
                {
                    _value = DependenciesController.Instance.Get<T>();
                    _initialized = true;
                }

                return _value;
            }
        }
    }
}