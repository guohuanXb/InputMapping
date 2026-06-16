using Cysharp.Threading.Tasks;
using VFramework;
using YooAsset;

namespace Native
{
    public class StartDownloadCommand:AbstractCommand<UniTask<bool>>
    {
        private ResourceDownloaderOperation _downloader;
        public StartDownloadCommand(ResourceDownloaderOperation downloader)
        {
            _downloader = downloader;
        }

        protected override async UniTask<bool> OnExecute()
        {
            var resourceSystem = this.GetSystem<IResourceSystem>();
            // 5. 开始下载
            var result = await resourceSystem.Download(_downloader);
            return result;
        }
    }
}