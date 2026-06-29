using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

namespace Native.Utility_Class
{
    public class PrimeTweenAnimation :IAnimate
    {
        public async UniTask Alpha(CanvasGroup canvasGroup, float endValue,float duration, CancellationToken ct)
        {
            var tween = Tween.Alpha(canvasGroup,endValue,duration);
            try
            {
                await tween.ToYieldInstruction().ToUniTask(cancellationToken:ct);
            }
            catch (OperationCanceledException)
            {
                tween.Stop();
            }
        }
    }
}