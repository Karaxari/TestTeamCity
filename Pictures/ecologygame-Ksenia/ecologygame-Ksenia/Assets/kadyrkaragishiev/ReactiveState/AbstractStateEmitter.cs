using System;
using System.Linq;

namespace kadyrkaragishiev.ReactiveState
{
    public abstract class AbstractStateEmitter<T> : IStateEmitter<T>
    {
        public virtual T State
        {
            get => GetState();
            set => RequestState(value);
        }

        public event StateChangedEventHandler<T> StateChanged
        {
            add
            {
                if(stateChanged == null || !stateChanged.GetInvocationList().Contains(value)) stateChanged += value;
                value?.Invoke(this, GetState());
            }
            remove => stateChanged -= value;
        }

        protected event StateChangedEventHandler<T> stateChanged;


        public bool RequestStateAsync(T state) => RequestStateAsync(state, null);

        public bool RequestStateAsync(T state, StateChangedEventHandler<T> callback) => DoRequestStateAsync(state, () =>
        {
            callback?.Invoke(this, GetState());
            InvokeStateChanged();
        });

        public bool RequestState(T state)
        {
            var b = DoRequestState(state);
            InvokeStateChanged();
            return b;
        }

        protected virtual bool DoRequestStateAsync(T state, Action onStateChanged)
        {
            var b = DoRequestState(state);
            onStateChanged?.Invoke();
            return b;
        }

        protected void InvokeStateChanged() => stateChanged?.Invoke(this, GetState());

        protected abstract bool DoRequestState(T state);

        protected abstract T GetState();
    }
}
