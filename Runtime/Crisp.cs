using System;
using System.Threading;
using System.Threading.Tasks;
using Framework.Loop;
using UnityEngine;

namespace dev.goreng.crisp
{
    public static class Crisp
    {
        private static TweenRunner _runner;

        public static void Init(IRunLoop runLoop)
        {
            if (_runner == null)
            {
                _runner = new TweenRunner();
            }
            _runner.SetRunLoop(runLoop);
        }

        public static Tween Create(float duration, Action<float> onUpdate)
        {
            if (_runner == null)
            {
                Debug.LogWarning("Crisp not initialized with IRunLoop. Call Crisp.Init() first.");
                // Fallback or throw? For now, create a runner but it won't update until Init is called.
                _runner = new TweenRunner();
            }

            Tween t = new Tween
            {
                Duration = duration,
                OnUpdate = onUpdate,
                EaseType = EaseType.Linear,
                Loops = 1,
                LoopType = LoopType.Restart
            };

            int id = _runner.Add(t);
            // We need to get the struct back with the ID assigned
            return _runner.GetTween(id);
        }

        public static Tween Delay(float duration, Action onComplete = null)
        {
            // Create a tween with no update callback for maximum performance
            // The runner will skip OnUpdate invoke if null
            var t = Create(duration, null);
            if (onComplete != null) SetOnComplete(t.ID, onComplete);
            return t;
        }

        public static Tween Value(float startValue, float endValue, float duration, Action<float> onUpdate)
        {
            return Create(duration, t => 
            {
                float val = Mathf.LerpUnclamped(startValue, endValue, t);
                onUpdate(val);
            });
        }

        public static void Kill(int id)
        {
            _runner?.Kill(id);
        }

        internal static void ReportError(Exception e)
        {
            OnError?.Invoke(e);
        }

        public static event Action<Exception> OnError;

        public static void SetEase(int id, EaseType easeType)
        {
            if (_runner == null) return;
            try { _runner.GetTween(id).EaseType = easeType; } 
            catch (Exception e) { OnError?.Invoke(e); }
        }

        public static void SetCustomEase(int id, Func<float, float> customEase)
        {
            if (_runner == null) return;
            try 
            { 
                ref var t = ref _runner.GetTween(id);
                t.EaseType = EaseType.Custom;
                t.CustomEase = customEase;
            } 
            catch (Exception e) { OnError?.Invoke(e); }
        }

        public static void SetLoops(int id, int loops, LoopType loopType)
        {
            if (_runner == null) return;
            try 
            { 
                ref var t = ref _runner.GetTween(id);
                t.Loops = loops;
                t.LoopType = loopType;
            } 
            catch (Exception e) { OnError?.Invoke(e); }
        }

        public static void SetOnComplete(int id, Action onComplete)
        {
            if (_runner == null) return;
            try { _runner.GetTween(id).OnComplete = onComplete; } 
            catch (Exception e) { OnError?.Invoke(e); }
        }
        
        public static void SetOnStepComplete(int id, Action onStepComplete)
        {
            if (_runner == null) return;
            try { _runner.GetTween(id).OnStepComplete = onStepComplete; } 
            catch (Exception e) { OnError?.Invoke(e); }
        }

        public static void SetOnStart(int id, Action onStart)
        {
            if (_runner == null) return;
            try { _runner.GetTween(id).OnStart = onStart; } 
            catch (Exception e) { OnError?.Invoke(e); }
        }

        public static void SetDelay(int id, float delay)
        {
            if (_runner == null) return;
            try { _runner.GetTween(id).Delay = delay; } 
            catch (Exception e) { OnError?.Invoke(e); }
        }

        public static void ChainOnComplete(int id, Action callback)
        {
            try { _runner?.ChainOnComplete(id, callback); }
            catch (Exception e) { OnError?.Invoke(e); }
        }

        // Awaitable support
        public static async Awaitable ToAwaitable(this Tween tween, CancellationToken cancellationToken = default)
        {
            bool completed = false;
            
            if (_runner == null) return;

            // Use helper method to avoid ref in async
            ChainOnComplete(tween.ID, () => completed = true);

            while (!completed)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Kill(tween.ID);
                    return; // Gracefully exit instead of throwing
                }
                
                try
                {
                    await Awaitable.NextFrameAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    Kill(tween.ID);
                    return;
                }
            }
        }

        public static async Task ToTask(this Tween tween, CancellationToken cancellationToken = default)
        {
            await tween.ToAwaitable(cancellationToken);
        }

        public static Awaitable ToAwaitable(this Tween tween, Framework.Events.DisposeBag disposeBag)
        {
            return tween.ToAwaitable(disposeBag.cancellationTokenSource.Token);
        }

        public static Task ToTask(this Tween tween, Framework.Events.DisposeBag disposeBag)
        {
            return tween.ToTask(disposeBag.cancellationTokenSource.Token);
        }
    }
}
