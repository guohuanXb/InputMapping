using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VFramework;
using YooAsset;

namespace Native
{
    public class LoadDllDataCommand :AbstractCommand<UniTask>
    {
        private string _packageName;

        public LoadDllDataCommand(string packageName)
        {
            _packageName = packageName;
        }

        protected override async UniTask OnExecute()
        {
            var hotUpdateSystem = this.GetSystem<IHotUpdateSystem>();
            var dllModel = this.GetModel<DllModel>();
            Dictionary<string, byte[]> aotDllDic = new();
            //6. 加载程序集文件
            foreach (var dllName in dllModel.AotAllName)
            {
                var bytes = await LoadDllFile(dllName);
                aotDllDic.Add(dllName, bytes);
            }
            var hotUpdateDllBytes = await LoadDllFile(dllModel.HotUpdateDllName);
            //7. 补充AOT元数据
            hotUpdateSystem.LoadMetadataForAOTAssembly(aotDllDic);
            //8. 加载热更新程序集
            hotUpdateSystem.LoadHotUpdateAssembly(dllModel.HotUpdateDllName,hotUpdateDllBytes);
        }
        
        async UniTask<byte[]> LoadDllFile(string dllName)
        {
            var resourceSystem = this.GetSystem<IResourceSystem<ResourcePackage, ResourceDownloaderOperation>>();
            var text = await resourceSystem.LoadAssetAsync<TextAsset>(dllName, _packageName);
            if (text == null || text.bytes == null || text.bytes.Length == 0)
            {
                Debug.LogError($"{dllName} Dll加载异常");
                return null;
            }
            return text.bytes;
        }
    }
}