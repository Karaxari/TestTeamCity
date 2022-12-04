using UnityEngine;
using Object = UnityEngine.Object;

namespace kadyrkaragishiev.ReactiveState
{
    public abstract class StateHandlerBehaviour<THandled> : MonoBehaviour, IStateHandler<THandled>
    {
        public IStateEmitter<THandled> StateEmitter
        {
            get => _stateEmitterAsInterface ?? _stateEmitter as IStateEmitter<THandled>;
            set
            {
                Unsubscribe();
                _stateEmitter = value as Object;
                _stateEmitterAsInterface = value;
                Subscribe();
            }
        }

        [SerializeField]
        private Object _stateEmitter;

        private IStateEmitter<THandled> _stateEmitterAsInterface;
        protected abstract void OnStateChanged(IStateEmitter<THandled> caller, THandled state);

        protected virtual void OnValidate()
        {
            if (_stateEmitter is IStateEmitter<THandled> || _stateEmitter == null) return;

            Debug.LogError(
                $"StateEmitter must be of type {typeof(IStateEmitter<THandled>)}. Got StateEmitter of type {_stateEmitter.GetType()}"
            );
            _stateEmitter = null;
        }

        protected void Subscribe()
        {
            if (StateEmitter != null) StateEmitter.StateChanged += OnStateChanged;
        }

        protected void Unsubscribe()
        {
            if (StateEmitter != null) StateEmitter.StateChanged -= OnStateChanged;
        }
    }
}
