using System;
using UnityEngine;

namespace dev.goreng.crisp
{
    public static class TweenExtensions
    {
        #region Fluent API
        public static Tween SetEase(this Tween t, EaseType ease)
        {
            Crisp.SetEase(t.ID, ease);
            t.EaseType = ease;
            return t;
        }

        public static Tween SetEase(this Tween t, Func<float, float> customEase)
        {
            Crisp.SetCustomEase(t.ID, customEase);
            t.EaseType = EaseType.Custom;
            t.CustomEase = customEase;
            return t;
        }

        public static Tween SetLoops(this Tween t, int loops, LoopType loopType = LoopType.Restart)
        {
            Crisp.SetLoops(t.ID, loops, loopType);
            t.Loops = loops;
            t.LoopType = loopType;
            return t;
        }

        public static Tween SetOnComplete(this Tween t, Action action)
        {
            Crisp.SetOnComplete(t.ID, action);
            t.OnComplete = action;
            return t;
        }
        
        public static Tween OnStepComplete(this Tween t, Action action)
        {
            Crisp.SetOnStepComplete(t.ID, action);
            t.OnStepComplete = action;
            return t;
        }

        public static Tween OnStart(this Tween t, Action action)
        {
            Crisp.SetOnStart(t.ID, action);
            t.OnStart = action;
            return t;
        }

        public static Tween SetDelay(this Tween t, float delay)
        {
            Crisp.SetDelay(t.ID, delay);
            t.Delay = delay;
            return t;
        }
        #endregion

        #region Transform Shortcuts
        #region Transform Shortcuts
        public static Tween Move(this Transform target, Vector3 endValue, float duration, bool relative = false)
        {
            Vector3 startValue = target.position;
            Vector3 finalEndValue = relative ? startValue + endValue : endValue;
            return Crisp.Create(duration, t => target.position = Vector3.LerpUnclamped(startValue, finalEndValue, t));
        }

        public static Tween MoveX(this Transform target, float endValue, float duration, bool relative = false)
        {
            float startValue = target.position.x;
            float finalEndValue = relative ? startValue + endValue : endValue;
            return Crisp.Create(duration, t => 
            {
                var pos = target.position;
                pos.x = Mathf.LerpUnclamped(startValue, finalEndValue, t);
                target.position = pos;
            });
        }
        
        public static Tween MoveY(this Transform target, float endValue, float duration, bool relative = false)
        {
            float startValue = target.position.y;
            float finalEndValue = relative ? startValue + endValue : endValue;
            return Crisp.Create(duration, t => 
            {
                var pos = target.position;
                pos.y = Mathf.LerpUnclamped(startValue, finalEndValue, t);
                target.position = pos;
            });
        }
        
        public static Tween MoveZ(this Transform target, float endValue, float duration, bool relative = false)
        {
            float startValue = target.position.z;
            float finalEndValue = relative ? startValue + endValue : endValue;
            return Crisp.Create(duration, t => 
            {
                var pos = target.position;
                pos.z = Mathf.LerpUnclamped(startValue, finalEndValue, t);
                target.position = pos;
            });
        }

        public static Tween LocalMove(this Transform target, Vector3 endValue, float duration, bool relative = false)
        {
            Vector3 startValue = target.localPosition;
            Vector3 finalEndValue = relative ? startValue + endValue : endValue;
            return Crisp.Create(duration, t => target.localPosition = Vector3.LerpUnclamped(startValue, finalEndValue, t));
        }

        public static Tween LocalMoveX(this Transform target, float endValue, float duration, bool relative = false)
        {
            float startValue = target.localPosition.x;
            float finalEndValue = relative ? startValue + endValue : endValue;
            return Crisp.Create(duration, t => 
            {
                var pos = target.localPosition;
                pos.x = Mathf.LerpUnclamped(startValue, finalEndValue, t);
                target.localPosition = pos;
            });
        }

        public static Tween LocalMoveY(this Transform target, float endValue, float duration, bool relative = false)
        {
            float startValue = target.localPosition.y;
            float finalEndValue = relative ? startValue + endValue : endValue;
            return Crisp.Create(duration, t => 
            {
                var pos = target.localPosition;
                pos.y = Mathf.LerpUnclamped(startValue, finalEndValue, t);
                target.localPosition = pos;
            });
        }

        public static Tween LocalMoveZ(this Transform target, float endValue, float duration, bool relative = false)
        {
            float startValue = target.localPosition.z;
            float finalEndValue = relative ? startValue + endValue : endValue;
            return Crisp.Create(duration, t => 
            {
                var pos = target.localPosition;
                pos.z = Mathf.LerpUnclamped(startValue, finalEndValue, t);
                target.localPosition = pos;
            });
        }

        public static Tween Rotate(this Transform target, Vector3 endValue, float duration, bool relative = false)
        {
            Quaternion startValue = target.rotation;
            Quaternion endRotation = relative ? startValue * Quaternion.Euler(endValue) : Quaternion.Euler(endValue);
            return Crisp.Create(duration, t => target.rotation = Quaternion.SlerpUnclamped(startValue, endRotation, t));
        }

        public static Tween RotateX(this Transform target, float endValue, float duration, bool relative = false)
        {
            float startValue = target.eulerAngles.x;
            float finalEndValue = relative ? startValue + endValue : endValue;
            return Crisp.Create(duration, t => 
            {
                var euler = target.eulerAngles;
                euler.x = Mathf.LerpAngle(startValue, finalEndValue, t);
                target.eulerAngles = euler;
            });
        }

        public static Tween RotateY(this Transform target, float endValue, float duration, bool relative = false)
        {
            float startValue = target.eulerAngles.y;
            float finalEndValue = relative ? startValue + endValue : endValue;
            return Crisp.Create(duration, t => 
            {
                var euler = target.eulerAngles;
                euler.y = Mathf.LerpAngle(startValue, finalEndValue, t);
                target.eulerAngles = euler;
            });
        }

        public static Tween RotateZ(this Transform target, float endValue, float duration, bool relative = false)
        {
            float startValue = target.eulerAngles.z;
            float finalEndValue = relative ? startValue + endValue : endValue;
            return Crisp.Create(duration, t => 
            {
                var euler = target.eulerAngles;
                euler.z = Mathf.LerpAngle(startValue, finalEndValue, t);
                target.eulerAngles = euler;
            });
        }

        public static Tween LocalRotate(this Transform target, Vector3 endValue, float duration, bool relative = false)
        {
            Quaternion startValue = target.localRotation;
            Quaternion endRotation = relative ? startValue * Quaternion.Euler(endValue) : Quaternion.Euler(endValue);
            return Crisp.Create(duration, t => target.localRotation = Quaternion.SlerpUnclamped(startValue, endRotation, t));
        }

        public static Tween LocalRotateX(this Transform target, float endValue, float duration, bool relative = false)
        {
            float startValue = target.localEulerAngles.x;
            float finalEndValue = relative ? startValue + endValue : endValue;
            return Crisp.Create(duration, t => 
            {
                var euler = target.localEulerAngles;
                euler.x = Mathf.LerpAngle(startValue, finalEndValue, t);
                target.localEulerAngles = euler;
            });
        }

        public static Tween LocalRotateY(this Transform target, float endValue, float duration, bool relative = false)
        {
            float startValue = target.localEulerAngles.y;
            float finalEndValue = relative ? startValue + endValue : endValue;
            return Crisp.Create(duration, t => 
            {
                var euler = target.localEulerAngles;
                euler.y = Mathf.LerpAngle(startValue, finalEndValue, t);
                target.localEulerAngles = euler;
            });
        }

        public static Tween LocalRotateZ(this Transform target, float endValue, float duration, bool relative = false)
        {
            float startValue = target.localEulerAngles.z;
            float finalEndValue = relative ? startValue + endValue : endValue;
            return Crisp.Create(duration, t => 
            {
                var euler = target.localEulerAngles;
                euler.z = Mathf.LerpAngle(startValue, finalEndValue, t);
                target.localEulerAngles = euler;
            });
        }

        public static Tween Scale(this Transform target, Vector3 endValue, float duration, bool relative = false)
        {
            Vector3 startValue = target.localScale;
            Vector3 finalEndValue = relative ? startValue + endValue : endValue;
            return Crisp.Create(duration, t => target.localScale = Vector3.LerpUnclamped(startValue, finalEndValue, t));
        }
        
        public static Tween Scale(this Transform target, float endValue, float duration, bool relative = false)
        {
            Vector3 startValue = target.localScale;
            Vector3 endVector = new Vector3(endValue, endValue, endValue);
            Vector3 finalEndValue = relative ? startValue + endVector : endVector;
            return Crisp.Create(duration, t => target.localScale = Vector3.LerpUnclamped(startValue, finalEndValue, t));
        }

        public static Tween ScaleX(this Transform target, float endValue, float duration, bool relative = false)
        {
            float startValue = target.localScale.x;
            float finalEndValue = relative ? startValue + endValue : endValue;
            return Crisp.Create(duration, t => 
            {
                var scale = target.localScale;
                scale.x = Mathf.LerpUnclamped(startValue, finalEndValue, t);
                target.localScale = scale;
            });
        }

        public static Tween ScaleY(this Transform target, float endValue, float duration, bool relative = false)
        {
            float startValue = target.localScale.y;
            float finalEndValue = relative ? startValue + endValue : endValue;
            return Crisp.Create(duration, t => 
            {
                var scale = target.localScale;
                scale.y = Mathf.LerpUnclamped(startValue, finalEndValue, t);
                target.localScale = scale;
            });
        }

        public static Tween ScaleZ(this Transform target, float endValue, float duration, bool relative = false)
        {
            float startValue = target.localScale.z;
            float finalEndValue = relative ? startValue + endValue : endValue;
            return Crisp.Create(duration, t => 
            {
                var scale = target.localScale;
                scale.z = Mathf.LerpUnclamped(startValue, finalEndValue, t);
                target.localScale = scale;
            });
        }
        #endregion
        #endregion

        #region CanvasGroup Shortcuts
        public static Tween Fade(this CanvasGroup target, float endValue, float duration)
        {
            float startValue = target.alpha;
            return Crisp.Create(duration, t => target.alpha = Mathf.LerpUnclamped(startValue, endValue, t));
        }
        #endregion

        #region Image Shortcuts
        public static Tween FillAmount(this UnityEngine.UI.Image target, float endValue, float duration)
        {
            float startValue = target.fillAmount;
            return Crisp.Create(duration, t => target.fillAmount = Mathf.LerpUnclamped(startValue, endValue, t));
        }
        #endregion
    }
}
