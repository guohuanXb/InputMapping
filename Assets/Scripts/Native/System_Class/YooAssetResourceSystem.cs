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
            return $"{_defaultHostServer}/{fileName}";
        }
        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
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

    public interface IResourceSystem<TPackage,TDownload> :ISystem
    {
        TPackage GetResourcePackage(string packageName);
        UniTask Initialize(string packageName,AssetServerConfig config = default);
        UniTask<string> RequestPackageVersion(string packageName);
        UniTask<bool> UpdatePackageManifest(string packageName, string packageVersion);
        TDownload GetDownloader(string packageName, int downloadingMaxNum = 10, int failedTryAgain = 3);
        UniTask<bool> Download(TDownload downloader);
        UniTask DestroyPackage(string packageName);
        UniTask<TOut> LoadAssetAsync<TOut>(string location,string packageName) where TOut : Object;

        UniTask LoadSceneAsync(string packageName,SceneConfig config);
    }
    public class YooAssetResourceSystem :AbstractSystem,IResourceSystem<ResourcePackage,DownloaderOperation>
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
            return YooAssets.TryGetPackage(packageName);
        }
        

        public async UniTask Initialize(string packageName, AssetServerConfig config = default)
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
            if (initOperation.Status == EOperationStatus.Succeed)
            {
                Debug.Log("资源包初始化成功！");
            }
            else
            {
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");
            }
        }

        public async UniTask<string> RequestPackageVersion(string packageName)
        {
            var package = GetResourcePackage(packageName);
            var operation = package.RequestPackageVersionAsync();
            await operation.ToUniTask();

            if (operation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                string packageVersion = operation.PackageVersion;
                Debug.Log($"请求包裹成功 , 版本 : {packageVersion}");
                return packageVersion;
            }
            else
            {
                //更新失败
                Debug.LogError($"请求包裹失败！ 失败信息 : {operation.Error}");
                return null;
            }
        }

        public async UniTask<bool> UpdatePackageManifest(string packageName, string packageVersion)
        {
            var package = GetResourcePackage(packageName);
            var operation = package.UpdatePackageManifestAsync(packageVersion);
            await operation.ToUniTask();

            if (operation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                Debug.Log($"更新包裹清单成功 , 版本 : {packageVersion}");
                return true;
            }
            else
            {
                //更新失败
                Debug.LogError($"更新包裹清单失败！ 失败信息 : {operation.Error}");
                return false;
            }
        }

        public DownloaderOperation GetDownloader(string packageName, int downloadingMaxNum = 10, int failedTryAgain = 3)
        {
            var package = GetResourcePackage(packageName);
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
            return downloader;
        }

        public async UniTask<bool> Download(DownloaderOperation downloader)
        {
            if (downloader.TotalDownloadCount == 0)
            {
                Debug.Log("没有需要更新的资源！");
                return true;
            }
            Debug.Log($"需要下载{downloader.TotalDownloadCount}个文件 , 共{downloader.TotalDownloadBytes/1024f/1024f:F2} MB");
            downloader.BeginDownload();
            await downloader.ToUniTask();
            
            if (downloader.Status == EOperationStatus.Succeed)
            {
                Debug.Log("下载文件包裹成功！");
                return true;
            }
            else
            {
                Debug.LogError($"下载文件包裹失败: {downloader.Error}");
                return false;
            }
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
        

        public async UniTask<TOut> LoadAssetAsync<TOut>(string location,string packageName) where TOut : Object
        {
            var package = GetResourcePackage(packageName);
            AssetHandle handle = package.LoadAssetAsync<TOut>(location);
            await handle.ToUniTask();
            if (handle.Status == EOperationStatus.Succeed)
            {
                Debug.Log($"加载资源{location}成功!");
                return handle.AssetObject as TOut;
            }
            Debug.LogError($"加载资源{location}失败!");
            return null;
        }

        public async UniTask LoadSceneAsync(string packageName,SceneConfig config)
        {
            var package = GetResourcePackage(packageName);
            SceneHandle handle= package.LoadSceneAsync(config.Location,config.SceneMode, config.PhysicsMode, config.SuspendLoad, config.Priority);
            await handle.ToUniTask();
            if (handle.Status == EOperationStatus.Succeed)
            {
                Debug.Log($"加载场景{config.Location}成功!");
            }
            Debug.LogError($"加载场景{config.Location}失败!");
        }
        
    }
}