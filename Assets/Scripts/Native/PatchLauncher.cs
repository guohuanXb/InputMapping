using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace Native
{
    public class PatchLauncher :MonoBehaviour
    {
        private EPlayMode _runningMode;

        private void Start()
        {
#if UNITY_EDITOR
            _runningMode = EPlayMode.EditorSimulateMode;
#else
            _runningMode = EPlayMode.PlayerMode;
#endif
        }

        async UniTask Launcher()
        {
            // 1. 初始化包裹
           
            // 2. 请求最新版本号
            
            // 3. 更新资源清单
            
            // 4. 下载的资源
           


            // 5. 加载程序集文件
            

            // 6. 加载AOT程序集元数据
           
            // 7. 加载热更新程序集
            
            // 8. 启动游戏
            
        }
    }
}