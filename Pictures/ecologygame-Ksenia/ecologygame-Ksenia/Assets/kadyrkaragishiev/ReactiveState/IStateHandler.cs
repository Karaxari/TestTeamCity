namespace kadyrkaragishiev.ReactiveState
{
    public interface IStateHandler { }

    public interface IStateHandler<T> : IStateHandler
    {
        public IStateEmitter<T> StateEmitter { get; set; }
    }
}
