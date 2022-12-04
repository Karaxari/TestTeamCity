namespace kadyrkaragishiev.ReactiveState.StateMachine
{
    public interface IStateMachineTransition<TState>
    {
        public TState From { get; set; }
        public TState To { get; set; }
    }
}
