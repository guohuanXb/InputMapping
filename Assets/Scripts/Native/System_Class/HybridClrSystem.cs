using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using HybridCLR;
using UnityEngine;
using VFramework;

namespace Native
{
    public interface IHotUpdateSystem :ISystem
    {
        Assembly HotUpdateAssembly { get; }
        void LoadMetadataForAOTAssembly(Dictionary<string, byte[]> dllMetaData);
        void LoadHotUpdateAssembly(string hotUpdateDllName,byte[] dllBytes);
    }

    public class HybridClrSystem :AbstractSystem,IHotUpdateSystem
    {
        protected override void OnInit()
        {
            
        }

        public Assembly HotUpdateAssembly { get; private set; }

        public void LoadMetadataForAOTAssembly(Dictionary<string, byte[]> dllMetaData)
        {
            Debug.Log("开始为AOT程序集加载元数据");
            foreach (var dll in dllMetaData)
            {
                var err = RuntimeApi.LoadMetadataForAOTAssembly(dll.Value, HomologousImageMode.SuperSet);
                Debug.Log($"成功加载元数据:{dll.Key}. ret:{err}");
            }
        }
        

        public void LoadHotUpdateAssembly(string hotUpdateDllName,byte[] dllBytes)
        {
#if UNITY_EDITOR
            var assemblyName = Path.GetFileNameWithoutExtension(hotUpdateDllName);
            HotUpdateAssembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == assemblyName);
            Debug.Log($"成功加载元数据: {hotUpdateDllName}");
            
#else
            if (dllBytes == null && dllBytes.Length == 0)
            {
                throw new Exception("{hotUpdateDllName} 加载失败！");
            }
            try
            {
                HotUpdateAssembly = Assembly.Load(dllBytes);
                Debug.Log($"成功加载元数据: {hotUpdateDllName}");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
#endif
        }
    }
}