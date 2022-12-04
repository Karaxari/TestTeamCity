using UnityEngine;

namespace kadyrkaragishiev.ReactiveState
{
    public abstract class ScriptableStateHandler<T> : ScriptableObject, IStateHandler<T>
    {
        public IStateEmitter<T> StateEmitter
        {
            get => _stateEmitter as IStateEmitter<T>;
            set
            {
                Unsubscribe();
                _stateEmitter = (Object) value;
                Subscribe();
            }
        }

        [SerializeField]
        private Object _stateEmitter;

        protected abstract void OnStateChanged(IStateEmitter<T> caller, T state);

        protected virtual void OnEnable() => Subscribe();

        protected virtual void OnDisable() => Unsubscribe();

        protected virtual void OnValidate()
        {
            if (_stateEmitter is IStateEmitter<T>) return;

            Debug.LogError(
                $"StateEmitter must be of type IStateEmitter<{typeof(T)}>. Got StateEmitter of type {_stateEmitter.GetType()}"
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
