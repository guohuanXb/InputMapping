using Cysharp.Threading.Tasks;
using VFramework;
using YooAsset;

namespace Native
{
    public class InitResCommand : AbstractCommand<UniTask<ResourceDownloaderOperation>>
    {
        private string _packageName;
        public InitResCommand(string packageName)
        {
            _packageName = packageName;
        }

        protected override async UniTask<ResourceDownloaderOperation> OnExecute()
        {
            var resourceSystem = this.GetSystem<IResourceSystem<ResourcePackage, ResourceDownloaderOperation>>();
            var config = this.GetModel<IServerConfigModel>().ServerPath;
            
            //1. 初始化资源包
            await resourceSystem.Initialize(_packageName, config);
            //2. 请求最新版本号
            var version = await resourceSystem.RequestPackageVersion(_packageName);
            if(version == null)
                return null;
            //3. 更新资源清单
            var result = await resourceSystem.UpdatePackageManifest(_packageName, version);
            if(!result)
                return null;
            //4. 创建下载器
            var downloader = resourceSystem.GetDownloader(_packageName);
            return downloader;
        }
    }
}