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
            var dllModel = this.GetModel<IDllModel>();
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
            var resourceSystem = this.GetSystem<IResourceSystem>();
            var handle = await resourceSystem.LoadAssetAsync<TextAsset>(dllName, _packageName);
            var text = handle.AssetObject as TextAsset;
            if (text == null || text.bytes == null || text.bytes.Length == 0)
            {
                Debug.LogError($"{dllName} Dll加载异常");
                handle?.Release();
                return null;
            }
            var bytes = text.bytes;
            handle.Release();        // 提取 bytes 后立即释放
            return bytes;
        }
    }
}