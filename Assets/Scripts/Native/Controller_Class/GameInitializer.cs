using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Native.Event_Class;
using Native.Utility_Class;
using UnityEngine;
using VFramework;

namespace Native
{
    public class GameInitializer :MonoBehaviour,IController
    {
        public IArchitecture GetArchitecture()=>GameManager.Interface;
        public CanvasGroup logo;
        public LoadingPanel loadingPanel;
        public ErrorPanel errorPanel;
        private const string MainSceneName = "Main";
        private void Start()
        {
            Launch().Forget();
            this.RegisterEvent<RetryEvent>(e =>
            {
                Launch().Forget();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private static string FlattenException(Exception ex)
        {
            if (ex == null) return string.Empty;
            var result = ex.ToString();
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                result += $"\n  ---> (inner) {ex.Message}";
            }
            return result;
        }

        async UniTaskVoid Launch()
        {
            var packageName = this.GetModel<IPackageModel>().DefaultPackageName;
            var tool = this.GetUtility<IAnimate>();
            CancellationTokenSource cts = new CancellationTokenSource();
            List<UniTask> tasks = new();
            // 1. 初始化资源包
            try
            {
                tasks.Add(this.SendCommand(new InitResCommand(packageName)));
                tasks.Add(tool.Alpha(logo,1,2,cts.Token));
                await UniTask.WhenAll(tasks);
                loadingPanel.gameObject.SetActive(true);
            }
            catch (Exception e)
            {
                Debug.LogError($"{packageName} InitPackage Exception: {FlattenException(e)}");
                cts?.Cancel();
                cts?.Dispose();
                return;
            }

            try
            {
                // 2. 请求资源版本
                var version = await this.SendCommand(new ReqPackageVerCommand(packageName));
                // 3. 更新资源清单
                await this.SendCommand(new UpdateResManiCommand(packageName,version));
                // 4. 下载资源
                var downloader = this.GetSystem<IResourceSystem>().GetDownloader(packageName);
                await this.SendCommand(new StartDownloadCommand(downloader));
                // 5. 加载 DLL
                await this.SendCommand(new LoadDllDataCommand(packageName));
                // 6. 启动游戏
                await this.SendCommand(new StartGameCommand(packageName, MainSceneName));
            }
            catch (Exception e)
            {
                if (errorPanel != null)
                    errorPanel.gameObject.SetActive(true);
                this.SendCommand(new SendExceptionCommand(e.Message));
                Debug.LogError($"{FlattenException(e)}");
            }
        }
    }
}
