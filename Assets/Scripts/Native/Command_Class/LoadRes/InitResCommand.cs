using System;
using Cysharp.Threading.Tasks;
using VFramework;
using YooAsset;

namespace Native
{
    public class InitResCommand : AbstractCommand<UniTask>
    {
        private string _packageName;
        public InitResCommand(string packageName)
        {
            _packageName = packageName;
        }
        protected override async UniTask OnExecute()
        {
            var resourceSystem = this.GetSystem<IResourceSystem>();
            var config = this.GetModel<IServerConfigModel>().ServerPath;
            var operation = await resourceSystem.Initialize(_packageName, config);
            if(operation.Status == EOperationStatus.Failed)
                throw new InvalidOperationException("Failed to initialize resource pack");
        }
    }
}