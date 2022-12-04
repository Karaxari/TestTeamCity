using System.Collections.Generic;
using System.Linq;

namespace kadyrkaragishiev.ReactiveState.StateMachine
{
    public class StateMachine<TState> : AbstractStateEmitter<TState>, IStateMachine<TState>
    {
        public List<IStateMachineTransition<TState>> Transitions { get; set; }

        protected TState state;

        protected override bool DoRequestState(TState state)
        {
            if (!Transitions.Any(x => this.state.Equals(x.From) && state.Equals(x.To))) return false;
            if (this.state.Equals(state)) return true;

            this.state = state;

            return true;
        }

        protected override TState GetState() => state;
    }
}
