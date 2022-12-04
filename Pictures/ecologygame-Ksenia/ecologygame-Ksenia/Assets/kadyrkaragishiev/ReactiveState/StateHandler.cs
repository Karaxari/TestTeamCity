namespace kadyrkaragishiev.ReactiveState
{
    public class StateHandler<T> : AbstractStateHandler<T>
    {
        protected override void OnStateChanged(IStateEmitter<T> caller, T state) => throw new System.NotImplementedException();
    }
}
