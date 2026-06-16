using VFramework;
using YooAsset;

namespace Native
{
    public class StartGameCommand :AbstractCommand
    {
        private string _packageName;
        private string _sceneName;
        public StartGameCommand(string packageName, string sceneName)
        {
            _packageName = packageName;
            _sceneName = sceneName;
        }

        protected override void OnExecute()
        {
            InvokeDependencyRegister();
            LoadScene();
        }
        
        void InvokeDependencyRegister()
        {
            var hotUpdateAss = this.GetSystem<IHotUpdateSystem>().HotUpdateAssembly;
            var type = hotUpdateAss.GetType("Example.HotUpdateEntry");
            type?.GetMethod("DependencyRegister")?.Invoke(null, null);
        }

        void LoadScene()
        {
            var resourceSystem = this.GetSystem<IResourceSystem>();
            resourceSystem.LoadSceneAsync(_packageName,new SceneConfig(_sceneName));
        }
    }
}