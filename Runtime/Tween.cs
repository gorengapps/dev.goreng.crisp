using System;
using UnityEngine;

namespace dev.goreng.crisp
{
    public struct Tween
    {
        public int ID;
        public bool IsActive;
        public float Duration;
        public float Delay;
        public float Elapsed;
        public bool IsPaused;
        public EaseType EaseType;
        public Func<float, float> CustomEase;
        public int Loops; // -1 for infinite
        public LoopType LoopType;
        public Action OnStart;
        public Action<float> OnUpdate; // Normalized time 0-1
        public Action OnComplete;
        public Action OnStepComplete; // Called at end of each loop

        // Internal state
        internal bool _started;
        internal int _completedLoops;
        internal bool _playingBackwards;

        public void Kill()
        {
            Crisp.Kill(ID);
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Play()
        {
            IsPaused = false;
        }
    }

    public enum LoopType
    {
        Restart,
        Yoyo,
        Incremental
    }
}
