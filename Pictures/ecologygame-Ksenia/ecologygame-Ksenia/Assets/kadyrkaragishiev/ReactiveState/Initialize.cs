using UnityEngine;
using Object = UnityEngine.Object;

namespace kadyrkaragishiev.ReactiveState
{
    public class Initialize<THandled> : RunnerBehaviour, IStateHandler<THandled>
    {
        public IStateEmitter<THandled> StateEmitter
        {
            get => _stateEmitterAsInterface ?? _stateEmitter as IStateEmitter<THandled>;
            set
            {
                _stateEmitter = value as Object;
                _stateEmitterAsInterface = value;
            }
        }

        [SerializeField]
        private Object _stateEmitter;

        private IStateEmitter<THandled> _stateEmitterAsInterface;

        public THandled With;

        public bool SyncOrAsync;

        public override void Run() => _ = SyncOrAsync ? StateEmitter.RequestStateAsync(With) : StateEmitter.RequestState(With);
    }
}
