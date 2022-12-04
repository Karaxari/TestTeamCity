using System.Collections.Generic;

namespace kadyrkaragishiev.ReactiveState.StateMachine
{
    public interface IStateMachine<TState> : IStateEmitter<TState>
    {
        public List<IStateMachineTransition<TState>> Transitions { get; set; }
    }
}
