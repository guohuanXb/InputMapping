using VFramework;
using YooAsset;

namespace Native
{
    public class GameManager :Architecture<GameManager>
    {
        protected override void Init()
        {
            RegisterModel<IPackageModel>(new PackageModel());
            RegisterModel<IDllModel>(new DllModel());
            RegisterModel<IServerConfigModel>(new ServerConfigModel());
            RegisterSystem<IResourceSystem<ResourcePackage,ResourceDownloaderOperation>>(new YooAssetResourceSystem());
            RegisterSystem<IHotUpdateSystem>(new HybridClrSystem());
        }
    }
}