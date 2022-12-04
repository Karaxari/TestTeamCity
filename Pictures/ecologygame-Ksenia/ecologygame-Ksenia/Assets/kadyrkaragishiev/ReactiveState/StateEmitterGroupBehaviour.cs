using System;
using System.Collections.Generic;
using System.Linq;

namespace kadyrkaragishiev.ReactiveState
{
    public class StateEmitterGroupBehaviour<T> : StateEmitterBehaviour<T>
    {
        public List<StateEmitterBehaviour<T>> GroupMembers;

        protected override bool DoRequestState(T state) => GroupMembers.All(x => x.RequestState(state));

        protected override bool DoRequestStateAsync(T state, Action onStateChanged)
        {
            if (GroupMembers.Count == 0)
            {
                onStateChanged?.Invoke();
                return false;
            }

            var asyncCompleted = new List<bool>(new bool [GroupMembers.Count]);
            
            var b = true;

            for (var i = 0; i < GroupMembers.Count; i++)
            {
                var idx = i; // Lambdas are not closures
                b = b && GroupMembers[idx]
                    .RequestStateAsync(
                        state, (a, b) =>
                        {
                            asyncCompleted[idx] = true;
                            if (asyncCompleted.All(x => x)) onStateChanged?.Invoke();
                        }
                    );
            }

            return b;
        }

        protected override T GetState() => GroupMembers.Select(x => x.State).FirstOrDefault();
    }
}
