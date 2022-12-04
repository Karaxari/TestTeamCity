using UnityEngine.UI;

namespace kadyrkaragishiev.ReactiveState
{
    public abstract class StateEmitterUIDebugger<T> : StateHandlerBehaviour<T>
    {
        public Text Text;

        private void OnEnable() => Subscribe();

        private void OnDisable() => Unsubscribe();

        protected override void OnStateChanged(IStateEmitter<T> caller, T state) => Text.text = state.ToString();
    }
}
