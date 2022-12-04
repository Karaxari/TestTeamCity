namespace kadyrkaragishiev.ReactiveState
{
    public interface IStateEmitter { }

    public interface IStateEmitter<T> : IStateEmitter

    {
        public T State { get; set; }

        public event StateChangedEventHandler<T> StateChanged;

        public bool RequestStateAsync(T state);
        public bool RequestStateAsync(T state, StateChangedEventHandler<T> callback);

        public bool RequestState(T state);
    }

    public delegate void StateChangedEventHandler<T>(IStateEmitter<T> caller, T state);
}
