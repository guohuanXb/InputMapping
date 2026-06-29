using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using VFramework;
using YooAsset;
using Object = UnityEngine.Object;
using SceneHandle = YooAsset.SceneHandle;

namespace Native
{
    internal class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }
        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            var url = $"{_defaultHostServer}/{fileName}";
            Debug.Log($"[YooAsset] 远程请求 URL: {url}");
            return url;
        }
        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            var url = $"{_fallbackHostServer}/{fileName}";
            Debug.Log($"[YooAsset] 回退请求 URL: {url}");
            return url;
        }
    }

    public class SceneConfig
    {
        public string Location;
        public LoadSceneMode SceneMode;
        public LocalPhysicsMode PhysicsMode;
        public bool SuspendLoad;
        public uint Priority;
        public SceneConfig(string location)
        {
            Location = location;
            SceneMode = LoadSceneMode.Single;
            PhysicsMode = LocalPhysicsMode.None;
            SuspendLoad = false;
            Priority = 0U;
        }
    }

    public interface IResourceSystem :ISystem
    {
        ResourcePackage GetResourcePackage(string packageName);
        UniTask<InitializationOperation> Initialize(string packageName,AssetServerConfig config = default);
        UniTask<RequestPackageVersionOperation> RequestPackageVersion(string packageName);
        UniTask<UpdatePackageManifestOperation> UpdatePackageManifest(string packageName, string packageVersion);
        ResourceDownloaderOperation GetDownloader(string packageName, int downloadingMaxNum = 10, int failedTryAgain = 3);
        UniTask<AssetHandle> LoadAssetAsync<TOut>(string location,string packageName) where TOut : Object;
        UniTask<SceneHandle> LoadSceneAsync(string packageName,SceneConfig config);
        UniTask DestroyPackage(string packageName);
        /// <summary>
        /// 卸载所有引用计数为零的资源包。
        /// </summary>
        /// <returns></returns>
        UniTask UnloadUnUsedAssetsAsync(string packageName);
        /// <summary>
        /// 尝试卸载指定的资源对象
        /// 注意：如果该资源还在被使用，该方法会无效。
        /// </summary>
        /// <returns></returns>
        void TryUnloadUnusedAsset(string packageName,string location);
        /// <summary>
        /// 强制卸载所有资源包，该方法请在合适的时机调用。
        /// 注意：Package在销毁的时候也会自动调用该方法。
        /// </summary>
        /// <returns></returns>
        UniTask ForceUnloadAllAssets(string packageName);
        
    }
    public class YooAssetResourceSystem :AbstractSystem,IResourceSystem
    {
        private EPlayMode _mode;
        protected override void OnInit()
        {
            YooAssets.Initialize();
#if UNITY_EDITOR
            _mode = EPlayMode.EditorSimulateMode;
#else
            _mode = EPlayMode.HostPlayMode;
#endif
        }
        public ResourcePackage GetResourcePackage(string packageName)
        {
            var package = YooAssets.TryGetPackage(packageName) ?? YooAssets.CreatePackage(packageName);
            return package;
        }
        

        public async UniTask<InitializationOperation> Initialize(string packageName, AssetServerConfig config = default)
        {
            var package = GetResourcePackage(packageName);
            InitializeParameters createParameters;
            switch (_mode)
            {
                case EPlayMode.EditorSimulateMode:
                {
                    var buildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
                    var packageRoot = buildResult.PackageRootDirectory;
                    var fileSystemParams =
                        FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
                    createParameters = new EditorSimulateModeParameters
                    {
                        EditorFileSystemParameters = fileSystemParams
                    };
                    
                    break;
                }
                case EPlayMode.OfflinePlayMode:
                {
                    var fileSystemParams =
                        FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                    createParameters = new OfflinePlayModeParameters
                    {
                        BuildinFileSystemParameters = fileSystemParams
                    };
                    break;
                }
                case EPlayMode.HostPlayMode:
                {
                    string defaultHostServer = config.defaultHostServer;
                    string fallbackHostServer = config.defaultHostServer;
                    IRemoteServices remoteServices =
                        new RemoteServices(defaultHostServer, fallbackHostServer);
                    var cacheFileSystemParams =
                        FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
                    var buildInFileSystemParams =
                        FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                    createParameters = new HostPlayModeParameters
                    {
                        BuildinFileSystemParameters = buildInFileSystemParams,
                        CacheFileSystemParameters = cacheFileSystemParams
                    };
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(_mode), _mode, "不支持的运行模式");
            }
            var initOperation = package.InitializeAsync(createParameters);
            await initOperation.ToUniTask();
            return initOperation;
        }

        public async UniTask<RequestPackageVersionOperation> RequestPackageVersion(string packageName)
        {
            var package = GetResourcePackage(packageName);
            var operation = package.RequestPackageVersionAsync();
            await operation.ToUniTask();
            return operation;
        }

        public async UniTask<UpdatePackageManifestOperation> UpdatePackageManifest(string packageName, string packageVersion)
        {
            var package = GetResourcePackage(packageName);
            var operation = package.UpdatePackageManifestAsync(packageVersion);
            await operation.ToUniTask();
            return operation;
        }

        public ResourceDownloaderOperation GetDownloader(string packageName, int downloadingMaxNum = 10, int failedTryAgain = 3)
        {
            var package = GetResourcePackage(packageName);
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
            return downloader;
        }
        
        public async UniTask DestroyPackage(string packageName)
        {
            var package = GetResourcePackage(packageName);
            DestroyOperation operation = package.DestroyAsync();
            await operation.ToUniTask();
            if (operation.Status == EOperationStatus.Succeed)
            {
                Debug.Log("DestroyPackage Success");
                //然后移除资源包
                if (YooAssets.RemovePackage(package))
                {
                    Debug.Log("RemovePackage Success");
                }
                else
                {
                    Debug.LogError("DestroyPackage Fail");
                }
            }
            else
            {
                Debug.LogError("RemovePackage Fail");
            }
        }

        public async UniTask UnloadUnUsedAssetsAsync(string packageName)
        {
            var package = GetResourcePackage(packageName);
            var operation =  package.UnloadUnusedAssetsAsync();
            await operation.ToUniTask();
        }

        public void TryUnloadUnusedAsset(string packageName,string location)
        {
            var package = GetResourcePackage(packageName);
            package.TryUnloadUnusedAsset(location);
        }

        public async UniTask ForceUnloadAllAssets(string packageName)
        {
            var package = GetResourcePackage(packageName);
            var operation = package.UnloadAllAssetsAsync();
            await operation.ToUniTask();
        }

        


        public async UniTask<AssetHandle> LoadAssetAsync<TOut>(string location,string packageName) where TOut : Object
        {
            var package = GetResourcePackage(packageName);
            AssetHandle handle = package.LoadAssetAsync<TOut>(location);
            await handle.ToUniTask();
            return handle;
        }
        
        public async UniTask<SceneHandle> LoadSceneAsync(string packageName,SceneConfig config)
        {
            var package = GetResourcePackage(packageName);
            SceneHandle handle= package.LoadSceneAsync(config.Location,config.SceneMode, config.PhysicsMode, config.SuspendLoad, config.Priority);
            await handle.ToUniTask();
            return handle;
        }
        
    }
}