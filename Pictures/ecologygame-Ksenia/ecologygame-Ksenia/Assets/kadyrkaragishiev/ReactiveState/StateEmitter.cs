namespace kadyrkaragishiev.ReactiveState
{
    public class StateEmitter<T> : AbstractStateEmitter<T>
    {
        protected T state;

        protected override bool DoRequestState(T state)
        {
            this.state = state;
            return true;
        }

        protected override T GetState() => state;
    }
}
