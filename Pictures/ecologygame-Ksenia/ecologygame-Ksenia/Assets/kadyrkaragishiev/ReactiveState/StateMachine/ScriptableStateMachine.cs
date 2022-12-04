using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace kadyrkaragishiev.ReactiveState.StateMachine
{
    public class ScriptableStateMachine<TState> : ScriptableStateEmitter<TState>, IStateMachine<TState>
    {
        public List<IStateMachineTransition<TState>> Transitions
        {
            get => _transitions.OfType<IStateMachineTransition<TState>>().ToList();
            set => _transitions = value.OfType<StateMachineTransition<TState>>().ToList();
        }

        [SerializeField] private List<StateMachineTransition<TState>> _transitions;

        [SerializeField] public TState DefaultState;

        public List<TState> PossibleStates => _transitions.SelectMany(x => new[] {x.From, x.To}).Distinct().ToList();

        protected TState state;

        private void Awake() => state = DefaultState;

        protected override bool DoRequestState(TState state)
        {
            if (this.state == null) this.state = DefaultState;
            
            if (this.state.Equals(state)) return false;
            if (!Transitions.Any(x => this.state.Equals(x.From) && state.Equals(x.To))) return false;

            this.state = state;
            return true;
        }

        protected override TState GetState() => state;

        protected virtual void OnValidate() => state = DefaultState;
    }
}
