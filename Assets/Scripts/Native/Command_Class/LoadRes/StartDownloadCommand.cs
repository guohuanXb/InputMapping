using System;
using Cysharp.Threading.Tasks;
using Native.Event_Class;
using UnityEngine;
using VFramework;
using YooAsset;

namespace Native
{
    public class StartDownloadCommand : AbstractCommand<UniTask>
    {
        private ResourceDownloaderOperation _downloader;

        public StartDownloadCommand(ResourceDownloaderOperation downloader)
        {
            _downloader = downloader;
        }

        protected override async UniTask OnExecute()
        {
            Register();
            try
            {
                if (_downloader.TotalDownloadCount == 0)
                {
                    return;
                }
                Debug.Log($"需要下载{_downloader.TotalDownloadCount}个文件 , 共{_downloader.TotalDownloadBytes / 1024f / 1024f:F2} MB");
                _downloader.BeginDownload();
                await _downloader.ToUniTask();

                if (_downloader.Status == EOperationStatus.Failed)
                {
                    throw new InvalidOperationException(
                        $"Resource download failed, please check your network and try again.Info{_downloader.Error}");
                }
            }
            finally
            {
                // 无论成功、异常、还是走 0 文件的 return，都保证反订阅
                Unregister();
            }
        }

        private void Register()
        {
            _downloader.DownloadUpdateCallback   += OnDownloadUpdate;
            _downloader.DownloadFileBeginCallback += OnDownloadFileBegin;
        }

        private void Unregister()
        {
            _downloader.DownloadUpdateCallback    -= OnDownloadUpdate;
            _downloader.DownloadFileBeginCallback -= OnDownloadFileBegin;
        }

        private void OnDownloadUpdate(DownloadUpdateData data)
        {
            this.SendEvent(new UpdateProgressPercEvent
            {
                ProgressPercentage = data.Progress
            });
        }

        private void OnDownloadFileBegin(DownloadFileData data)
        {
            this.SendEvent(new UpdateProgressInfoEvent
            {
                Info = $"Starting download file {data.FileName},Size:{data.FileSize}"
            });
        }
    }
}