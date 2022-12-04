using System;
using UnityEngine;

namespace kadyrkaragishiev.ReactiveState.StateMachine
{
    [Serializable]
    public class StateMachineTransition<TState> : IStateMachineTransition<TState>
    {
        public TState From { get => _from; set => _from = value; }
        public TState To { get => _to; set => _to = value; }

        [SerializeField]
        private TState _from;

        [SerializeField]
        private TState _to;

    }
}
