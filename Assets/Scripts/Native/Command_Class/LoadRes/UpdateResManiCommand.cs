using System;
using Cysharp.Threading.Tasks;
using Native.Event_Class;
using VFramework;
using YooAsset;

namespace Native
{
    public class UpdateResManiCommand :AbstractCommand<UniTask>
    {
        private string _packageName;
        private string _version;
        public UpdateResManiCommand(string packageName,string version)
        {
            _packageName = packageName;
            _version = version;
        }

        protected override async UniTask OnExecute()
        {
            var resourceSystem = this.GetSystem<IResourceSystem>();
            var operation = await resourceSystem.UpdatePackageManifest(_packageName, _version);
            if (operation.Status == EOperationStatus.Failed)
            {
                throw new InvalidOperationException($"Failed to update the resource list, please check your network connection,Info{operation.Error}");
            }
            this.SendEvent(new UpdateProgressInfoEvent()
            {
                Info = "Resource manifest Update completed",
            });
            this.SendEvent(new UpdateProgressPercEvent()
            {
               
                ProgressPercentage = 1f
            });
        }
    }
}