using System;
using System.Linq;
using UnityEngine;

namespace kadyrkaragishiev.ReactiveState
{
    public abstract class StateEmitterBehaviour<T> : MonoBehaviour, IStateEmitter<T>
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

        public bool RequestStateAsync(T state, StateChangedEventHandler<T> callback)
        {
            Debug.Log($"Will {GetType()}.{nameof(RequestState)} on {name}");

            var b = DoRequestStateAsync(state, () =>
            {
                callback?.Invoke(this, GetState());
                InvokeStateChanged();
                Debug.Log($"State of {GetType()}.{nameof(RequestState)}({state}) on {name} was changed");
            });

            var willOrNot = b ? "will" : "won't";

            Debug.Log($"Did {GetType()}.{nameof(RequestState)}({state}) on {name}. State {willOrNot} be changed");

            return b;
        }

        public bool RequestState(T state)
        {
            Debug.Log($"Will {GetType()}.{nameof(RequestState)}({state}) on {name}");
            var b = DoRequestState(state);

            var wasOrNot = b ? "was" : "wasn't";
            Debug.Log($"Did {GetType()}.{nameof(RequestState)}({state}) on {name}. State {wasOrNot} changed");

            if (b) InvokeStateChanged();

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
