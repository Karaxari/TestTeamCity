using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace kadyrkaragishiev.ReactiveState.StateMachine
{
    public class StateMachineBehaviour<TState> : StateEmitterBehaviour<TState>, IStateMachine<TState>
    {
        public List<IStateMachineTransition<TState>> Transitions { get => _transitions.OfType<IStateMachineTransition<TState>>().ToList(); set => _transitions = value.OfType<StateMachineTransition<TState>>().ToList(); }

        [SerializeField]
        private List<StateMachineTransition<TState>> _transitions;

        [SerializeField]
        private TState _state;

        protected override bool DoRequestState(TState state)
        {
            if (State.Equals(state)) return false;
            if (!Transitions.Any(x => State.Equals(x.From) && state.Equals(x.To))) return false;

            _state = state;
            return true;
        }

        protected override TState GetState() => _state;
    }
}
