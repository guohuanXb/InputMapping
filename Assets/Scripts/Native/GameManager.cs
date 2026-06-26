using Native.UIKit.Framework;
using Native.Utility_Class;
using VFramework;
using YooAsset;

namespace Native
{
    public class GameManager :Architecture<GameManager>
    {
        protected override void Init()
        {
            RegisterUtility<IAnimate>(new PrimeTweenAnimation());
            RegisterUtility<ISerializable>(new Serialization());
            RegisterModel<IPackageModel>(new PackageModel());
            RegisterModel<IDllModel>(new DllModel());
            RegisterModel<IServerConfigModel>(new ServerConfigModel());
            RegisterSystem<IResourceSystem>(new YooAssetResourceSystem());
            RegisterSystem<IHotUpdateSystem>(new HybridClrSystem());
            RegisterSystem<IUIManager>(new UIManager());
        }
    }
}