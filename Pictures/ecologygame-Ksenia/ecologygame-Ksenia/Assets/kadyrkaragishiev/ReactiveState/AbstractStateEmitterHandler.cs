
namespace kadyrkaragishiev.ReactiveState
{
    public abstract class AbstractStateEmitterHandler<TEmitted, THandled> : AbstractStateEmitter<TEmitted>, IStateHandler<THandled>
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

        protected abstract void OnStateChanged(IStateEmitter<THandled> caller, THandled state);

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
