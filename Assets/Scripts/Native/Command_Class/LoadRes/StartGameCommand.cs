using System;
using Cysharp.Threading.Tasks;
using VFramework;
using YooAsset;

namespace Native
{
    public class StartGameCommand :AbstractCommand<UniTask>
    {
        private string _packageName;
        private string _sceneName;
        public StartGameCommand(string packageName, string sceneName)
        {
            _packageName = packageName;
            _sceneName = sceneName;
        }

        protected override async UniTask OnExecute()
        {
            InvokeDependencyRegister();
            await this.SendCommand(new SwitchMainSceneCommand(_packageName, _sceneName));
        }
        
        void InvokeDependencyRegister()
        {
            var hotUpdateAss = this.GetSystem<IHotUpdateSystem>().HotUpdateAssembly;
            if (hotUpdateAss == null)
            {
                throw new InvalidOperationException("HotUpdateAssembly is null，Can't Invoke DependencyRegister");
            }
            var type = hotUpdateAss.GetType("Example.HotUpdateEntry");
            if (type == null)
            {
                throw new InvalidOperationException("Can't Find Example.HotUpdateEntry");
            }
            var method = type.GetMethod("DependencyRegister");
            if (method == null)
            {
                throw new InvalidOperationException("Can't Find Function Example.HotUpdateEntry.DependencyRegister");
            }
            method.Invoke(null, null);
        }
        
    }
}