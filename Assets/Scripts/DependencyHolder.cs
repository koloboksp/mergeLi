namespace Core
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
                    _value = ApplicationController.Instance.DependenciesController.Get<T>();
                    _initialized = true;
                }

                return _value;
            }
        }
    }
}