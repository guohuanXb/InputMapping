using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VFramework;
using YooAsset;

namespace Native
{
    public class GameLauncher : MonoBehaviour,IController
    {
        private const string InitialSceneName = "Main";
        public IArchitecture GetArchitecture() => GameManager.Interface;
        private ResourceDownloaderOperation _downloader;
        private void Start()
        {
            Launcher().Forget();
        }

        async UniTaskVoid Launcher()
        {
            try
            {
                var packageName = this.GetModel<IPackageModel>().DefaultPackageName;
                _downloader = await this.SendCommand(new InitResCommand(packageName,UpdateProgress,SetProgressInfo));
                RegisterDownloader();
                var result = await this.SendCommand(new StartDownloadCommand(_downloader));
                await this.SendCommand(new LoadDllDataCommand(packageName));
                this.SendCommand(new StartGameCommand(packageName,InitialSceneName));
                
            }
            catch (Exception e)
            {
                Debug.Log($"{e}");
            }
        }

        public Slider loadingProgress;
        public TMP_Text loadingInfoText;
        public TMP_Text loadingProgressText;
        
        
        void UpdateProgress(float progress)
        {
            loadingProgress.value = progress;
            loadingProgressText.text = $"{progress * 100}";
        }

        void SetProgressInfo(string info)
        {
            loadingInfoText.text = info;
        }
        
        
        void RegisterDownloader()
        {
            _downloader.DownloadFileBeginCallback += data =>
            {
                SetProgressInfo($"Start downloading file:{data.FileName},Size:{data.FileSize}");
            };
            _downloader.DownloadUpdateCallback += data =>
            {
                UpdateProgress(data.Progress);
            };
        }
        void UnregisterDownloader()
        {
            _downloader.DownloadFileBeginCallback = null;
            _downloader.DownloadUpdateCallback = null;
        }
    }
}