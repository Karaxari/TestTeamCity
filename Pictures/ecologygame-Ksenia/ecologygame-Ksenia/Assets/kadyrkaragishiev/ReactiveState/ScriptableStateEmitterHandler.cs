using UnityEngine;

namespace kadyrkaragishiev.ReactiveState
{
    public abstract class ScriptableStateEmitterHandler<TEmitted, THandled> : ScriptableStateEmitter<TEmitted>, IStateHandler<THandled>
    {
        public IStateEmitter<THandled> StateEmitter
        {
            get => (IStateEmitter<THandled>) _stateEmitter;
            set
            {
                Unsubscribe();
                _stateEmitter = (Object) value;
                Subscribe();
            }
        }

        [SerializeField]
        private Object _stateEmitter;

        protected abstract void OnStateChanged(IStateEmitter<THandled> caller, THandled state);

        protected virtual void OnEnable() => Subscribe();

        protected virtual void OnDisable() => Unsubscribe();

        protected virtual void OnValidate()
        {
            if (_stateEmitter is IStateEmitter<THandled>) return;

            Debug.LogError(
                $"StateEmitter must be of type {typeof(IStateEmitter<THandled>)}. Got StateEmitter of type {_stateEmitter.GetType()}"
            );
            _stateEmitter = null;
        }

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
