using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VFramework;

namespace Native.Utility_Class
{
    public interface IAnimate :IUtility
    {
        UniTask Alpha(CanvasGroup canvasGroup, float endValue,float duration, CancellationToken ct);
    }
}