using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Native.Event_Class;
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
            float average = 0.8f/dllModel.AotAllName.Count;
            float progress = 0f;
            this.SendEvent(new UpdateProgressInfoEvent()
            {
                Info = "Loading Dll Data",
            });
            this.SendEvent(new UpdateProgressPercEvent()
            {
                ProgressPercentage = progress
            });
            //6. 加载程序集文件
            foreach (var dllName in dllModel.AotAllName)
            {
                var bytes = await LoadDllFile(dllName);
                progress += average;
                if (bytes == null)
                {
                    throw new InvalidOperationException($"AOT MetaData DLL Loading failed：{dllName}");
                }
                aotDllDic.Add(dllName, bytes);
                this.SendEvent(new UpdateProgressInfoEvent()
                {
                    Info = $"Loaded Dll {dllName}",
                });
                this.SendEvent(new UpdateProgressPercEvent()
                {
                    ProgressPercentage = progress
                });
            }
            var hotUpdateDllBytes = await LoadDllFile(dllModel.HotUpdateDllName);
            if (hotUpdateDllBytes == null)
            {
                throw new InvalidOperationException($"HotUpdate DLL Loading failed：{dllModel.HotUpdateDllName}");
            }
            this.SendEvent(new  UpdateProgressInfoEvent()
            {
                Info = $"Loaded Dll {dllModel.HotUpdateDllName}",
            });
            this.SendEvent(new UpdateProgressPercEvent()
            {
                
                ProgressPercentage = 1f
            });
            //7. 补充AOT元数据
            hotUpdateSystem.LoadMetadataForAOTAssembly(aotDllDic);
            //8. 加载热更新程序集
            hotUpdateSystem.LoadHotUpdateAssembly(dllModel.HotUpdateDllName, hotUpdateDllBytes);
        }
        
        async UniTask<byte[]> LoadDllFile(string dllName)
        {
            var resourceSystem = this.GetSystem<IResourceSystem>();
            var handle = await resourceSystem.LoadAssetAsync<TextAsset>(dllName, _packageName);
            if (handle.Status == EOperationStatus.Failed)
            {
                throw new InvalidOperationException($"Failed to load {dllName},Info:{handle.LastError}");
            }
            var text = handle.AssetObject as TextAsset;
            if (text == null || text.bytes == null || text.bytes.Length == 0)
            {
                handle?.Release();
                throw new InvalidOperationException($"Failed to load {dllName},Info: Dll file may be empty");
            }
            var bytes = text.bytes;
            handle.Release();        // 提取 bytes 后立即释放
            return bytes;
        }
    }
}