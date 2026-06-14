using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VFramework;
using YooAsset;

namespace Native
{
    public class GameLauncher : MonoBehaviour,IController
    {
        private const string InitialSceneName = "Main";
        public IArchitecture GetArchitecture() => GameManager.Interface;
        private void Start()
        {
            Launcher().Forget();
        }

        async UniTaskVoid Launcher()
        {
            try
            {
                var packageName = this.GetModel<IPackageModel>().DefaultPackageName;
                var downloader = await this.SendCommand(new InitResCommand(packageName));
                var result = await this.SendCommand(new StartDownloadCommand(downloader));
                await this.SendCommand(new LoadDllDataCommand(packageName));
                this.SendCommand(new StartGameCommand(packageName,InitialSceneName));
            }
            catch (Exception e)
            {
                Debug.Log($"{e}");
            }
        }
    }
}