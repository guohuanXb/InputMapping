using System;
using Cysharp.Threading.Tasks;
using VFramework;
using YooAsset;

namespace Native
{
    public class InitResCommand : AbstractCommand<UniTask<ResourceDownloaderOperation>>
    {
        private string _packageName;
        private Action<float> _progressCallback;
        private Action<string> _infoCallback;
        public InitResCommand(string packageName, Action<float> progressCallback, Action<string> infoCallback)
        {
            _packageName = packageName;
            _progressCallback = progressCallback;
            _infoCallback = infoCallback;
        }
        //TODO：错误处理

        protected override async UniTask<ResourceDownloaderOperation> OnExecute()
        {
            var resourceSystem = this.GetSystem<IResourceSystem>();
            var config = this.GetModel<IServerConfigModel>().ServerPath;
            _progressCallback.Invoke(0);
            _infoCallback.Invoke("Initializing resource pack");
            //1. 初始化资源包
            await resourceSystem.Initialize(_packageName, config);
            _progressCallback.Invoke(0.25f);
            _infoCallback.Invoke("Requesting the latest version number");
            //2. 请求最新版本号
            var version = await resourceSystem.RequestPackageVersion(_packageName);
            if(version == null)
                return null;
            _progressCallback.Invoke(0.5f);
            _infoCallback.Invoke("Update resource list");
            //3. 更新资源清单
            var result = await resourceSystem.UpdatePackageManifest(_packageName, version);
            if(!result)
                return null;
            _progressCallback.Invoke(1f);
            _infoCallback.Invoke("Create Downloader");
            //4. 创建下载器
            var downloader = resourceSystem.GetDownloader(_packageName);
            return downloader;
        }
    }
}