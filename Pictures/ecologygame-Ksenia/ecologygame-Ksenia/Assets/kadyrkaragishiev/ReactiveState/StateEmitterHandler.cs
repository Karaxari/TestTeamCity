namespace kadyrkaragishiev.ReactiveState
{
    public class StateEmitterHandler<TEmitted, THandled> : StateEmitter<TEmitted>, IStateHandler<THandled>
    {
        public IStateEmitter<THandled> StateEmitter
        {
            get => _stateEmitter;
            set
            {
                Unsubscribe();
                _stateEmitter = value;
                Subscribe();
            }
        }

        private IStateEmitter<THandled> _stateEmitter;

        protected virtual void OnStateChanged(IStateEmitter<THandled> caller, THandled state) { }

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
