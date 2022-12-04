namespace kadyrkaragishiev.ReactiveState
{
    public abstract class AbstractStateHandler<T> : IStateHandler<T>
    {
        public IStateEmitter<T> StateEmitter
        {
            get => _stateEmitter;
            set
            {
                Unsubscribe();
                _stateEmitter = value;
                Subscribe();
            }
        }

        private readonly StateChangedEventHandler<T> _handler;

        private IStateEmitter<T> _stateEmitter;

        protected abstract void OnStateChanged(IStateEmitter<T> caller, T state);

        private void Subscribe()
        {
            if (StateEmitter != null) StateEmitter.StateChanged += OnStateChanged;
        }

        private void Unsubscribe()
        {
            if (StateEmitter != null) StateEmitter.StateChanged -= OnStateChanged;
        }
    }
}
