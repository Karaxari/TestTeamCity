using System;
using System.Collections;
using UnityEngine;

namespace kadyrkaragishiev.ReactiveState
{
    [Flags]
    public enum UnityCallbacks
    {
        None = 0,
        Awake = 1,
        OnEnable = 2,
        Start = 4,
        FrameAfterStart = 8,
        Update = 16,
        OnDisable = 32,
        Everything = 63
    }
    
    public abstract class RunnerBehaviour : MonoBehaviour
    {
        public UnityCallbacks On;

        public abstract void Run();

        protected virtual void Awake()
        {
            if (On.HasFlag(UnityCallbacks.Awake)) Run();
        }

        protected virtual void OnEnable()
        {
            if (On.HasFlag(UnityCallbacks.OnEnable)) Run();
        }

        protected virtual IEnumerator Start()
        {
            if (On.HasFlag(UnityCallbacks.Start)) Run();

            yield return null;

            if (On.HasFlag(UnityCallbacks.FrameAfterStart)) Run();
        }

        protected virtual void Update()
        {
            if (On.HasFlag(UnityCallbacks.Update)) Run();
        }

        protected virtual void OnDisable()
        {
            if (On.HasFlag(UnityCallbacks.OnDisable)) Run();
        }
    }
}
