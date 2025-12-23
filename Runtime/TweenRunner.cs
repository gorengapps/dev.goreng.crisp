using System;
using System.Collections.Generic;
using Framework.Loop;
using UnityEngine;

namespace dev.goreng.crisp
{
    internal class TweenRunner
    {
        private Tween[] _tweens;
        private int _count;
        private int _capacity;
        private Stack<int> _freeIndices;
        private IRunLoop _runLoop;
        private int _idCounter;

        public TweenRunner(int initialCapacity = 64)
        {
            _capacity = initialCapacity;
            _tweens = new Tween[_capacity];
            _freeIndices = new Stack<int>(_capacity);
            _count = 0;
            _idCounter = 1;

            // Initialize free indices
            for (int i = _capacity - 1; i >= 0; i--)
            {
                _freeIndices.Push(i);
            }
        }

        private IDisposable _updateSubscription;

        public void SetRunLoop(IRunLoop runLoop)
        {
            _updateSubscription?.Dispose();
            _runLoop = runLoop;
            if (_runLoop != null)
            {
                _updateSubscription = _runLoop.onUpdate.Subscribe(OnUpdate);
            }
        }

        public int Add(Tween tween)
        {
            if (_freeIndices.Count == 0)
            {
                Expand();
            }

            int index = _freeIndices.Pop();
            tween.ID = _idCounter++;
            tween.IsActive = true;
            _tweens[index] = tween;
            _count++;
            return tween.ID;
        }

        public void Kill(int id)
        {
            // Linear search for now, can optimize with a dictionary map if needed
            // But for zero alloc, array scan is often better for small-medium counts
            for (int i = 0; i < _capacity; i++)
            {
                if (_tweens[i].IsActive && _tweens[i].ID == id)
                {
                    _tweens[i].IsActive = false;
                    _freeIndices.Push(i);
                    _count--;
                    return;
                }
            }
        }

        public ref Tween GetTween(int id)
        {
             for (int i = 0; i < _capacity; i++)
            {
                if (_tweens[i].IsActive && _tweens[i].ID == id)
                {
                    return ref _tweens[i];
                }
            }
            throw new ArgumentException($"Tween with ID {id} not found");
        }

        public bool IsActive(int id)
        {
             for (int i = 0; i < _capacity; i++)
            {
                if (_tweens[i].IsActive && _tweens[i].ID == id)
                {
                    return true;
                }
            }
            return false;
        }

        public void ChainOnComplete(int id, Action callback)
        {
             for (int i = 0; i < _capacity; i++)
            {
                if (_tweens[i].IsActive && _tweens[i].ID == id)
                {
                    Action original = _tweens[i].OnComplete;
                    _tweens[i].OnComplete = () => {
                        original?.Invoke();
                        callback?.Invoke();
                    };
                    return;
                }
            }
        }

        private void Expand()
        {
            int newCapacity = _capacity * 2;
            Tween[] newTweens = new Tween[newCapacity];
            Array.Copy(_tweens, newTweens, _capacity);
            _tweens = newTweens;

            for (int i = newCapacity - 1; i >= _capacity; i--)
            {
                _freeIndices.Push(i);
            }

            _capacity = newCapacity;
        }

        private void OnUpdate(object sender, float deltaTime)
        {
            if (_count == 0) return;

            for (int i = 0; i < _capacity; i++)
            {
                // Direct array access, no copying
                if (!_tweens[i].IsActive) continue;
                if (_tweens[i].IsPaused) continue;

                ref Tween t = ref _tweens[i];

                if (t.Delay > 0)
                {
                    t.Delay -= deltaTime;
                    if (t.Delay > 0) continue;
                    
                    // Delay finished, consume the overshoot
                    deltaTime = -t.Delay; 
                    t.Delay = 0;
                }

                if (!t._started)
                {
                    t._started = true;
                    try { t.OnStart?.Invoke(); }
                    catch (Exception e) { Crisp.ReportError(e); }
                }

                if (t._playingBackwards)
                {
                    t.Elapsed -= deltaTime;
                }
                else
                {
                    t.Elapsed += deltaTime;
                }

                bool completedIteration = false;
                bool finished = false;

                if (!t._playingBackwards && t.Elapsed >= t.Duration)
                {
                    t.Elapsed = t.Duration;
                    completedIteration = true;
                }
                else if (t._playingBackwards && t.Elapsed <= 0f)
                {
                    t.Elapsed = 0f;
                    completedIteration = true;
                }

                float normalizedTime = t.Duration > 0 ? t.Elapsed / t.Duration : 1f;
                float easedTime = normalizedTime;
                
                try 
                {
                    easedTime = t.EaseType == EaseType.Custom && t.CustomEase != null 
                        ? t.CustomEase(normalizedTime) 
                        : Ease.Evaluate(normalizedTime, t.EaseType);
                }
                catch (Exception e) { Crisp.ReportError(e); }

                try { t.OnUpdate?.Invoke(easedTime); }
                catch (Exception e) { Crisp.ReportError(e); }

                if (completedIteration)
                {
                    try { t.OnStepComplete?.Invoke(); }
                    catch (Exception e) { Crisp.ReportError(e); }
                    
                    t._completedLoops++;

                    if (t.Loops != -1 && t._completedLoops >= t.Loops)
                    {
                        finished = true;
                    }
                    else
                    {
                        // Handle looping
                        switch (t.LoopType)
                        {
                            case LoopType.Restart:
                                t.Elapsed = 0f;
                                break;
                            case LoopType.Yoyo:
                                t._playingBackwards = !t._playingBackwards;
                                break;
                            case LoopType.Incremental:
                                t.Elapsed = 0f;
                                // Incremental logic would need to modify start/end values, 
                                // which requires more generic handling or callbacks.
                                // For now, treat as Restart.
                                break;
                        }
                    }
                }

                if (finished)
                {
                    try { t.OnComplete?.Invoke(); }
                    catch (Exception e) { Crisp.ReportError(e); }
                    
                    t.IsActive = false;
                    _freeIndices.Push(i);
                    _count--;
                }
            }
        }
    }
}
