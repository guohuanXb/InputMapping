using System;
using Cysharp.Threading.Tasks;
using Native.Event_Class;
using VFramework;
using YooAsset;

namespace Native
{
    public class ReqPackageVerCommand :AbstractCommand<UniTask<string>>
    {
        private string _packageName;

        public ReqPackageVerCommand(string packageName)
        {
            _packageName = packageName;
        }

        protected override async UniTask<string> OnExecute()
        {
            var resourceSystem = this.GetSystem<IResourceSystem>();
            this.SendEvent(new UpdateProgressInfoEvent()
            {
                Info = "the resource package Initialize completed",
            });
            this.SendEvent(new UpdateProgressPercEvent()
            {
                
                ProgressPercentage = 0.3f
            });
            var operation = await resourceSystem.RequestPackageVersion(_packageName);
            if (operation.Status == EOperationStatus.Failed)
            {
                throw new InvalidOperationException($"Failed to get the resource version, please check your network connection,Info{operation.Error}");
            }

            this.SendEvent(new UpdateProgressInfoEvent()
            {
                Info = "Resource package version request completed",
            });
            this.SendEvent(new UpdateProgressPercEvent()
            {
                
                ProgressPercentage = 0.65f
            });
            return operation.PackageVersion;
        }
    }
}