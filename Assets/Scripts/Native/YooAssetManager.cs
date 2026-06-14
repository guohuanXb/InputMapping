using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace Native
{
    public class YooAssetManager
    {
        private string _packageName;
        private EPlayMode _playMode;
        public YooAssetManager(string packageName,EPlayMode playMode)
        {
            _packageName = packageName;
            _playMode = playMode;
        }
        /// <summary>
        /// 初始化YooAsset资源管理系统，设置默认的资源包并对资源包进行初始化。
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async UniTask<ResourcePackage> InitPackage()
        {
            // 初始化资源系统
            YooAssets.Initialize();
            // 获取指定的资源包，如果没有找到不会报错
            var package = YooAssets.TryGetPackage(_packageName) ?? YooAssets.CreatePackage(_packageName);
            // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
            YooAssets.SetDefaultPackage(package);
            InitializationOperation initOperation = null;
            switch (_playMode)
            {
                case EPlayMode.EditorSimulateMode:
                {
                    var buildResult = EditorSimulateModeHelper.SimulateBuild(_packageName);
                    var packageRoot = buildResult.PackageRootDirectory;
                    var fileSystemParams =
                        FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
                    var createParameters = new EditorSimulateModeParameters
                    {
                        EditorFileSystemParameters = fileSystemParams
                    };
                    initOperation = package.InitializeAsync(createParameters);
                    break;
                }
                case EPlayMode.OfflinePlayMode:
                {
                    var fileSystemParams =
                        FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                    var createParameters = new OfflinePlayModeParameters
                    {
                        BuildinFileSystemParameters = fileSystemParams
                    };
                    initOperation = package.InitializeAsync(createParameters);
                    break;
                }
                case EPlayMode.HostPlayMode:
                {
                    // TODO: 正式上线时替换为真实CDN地址
                    string defaultHostServer = "127.0.0.1";
                    string fallbackHostServer = "127.0.0.1";
                    IRemoteServices remoteServices =
                        new RemoteServices(defaultHostServer, fallbackHostServer);
                    var cacheFileSystemParams =
                        FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
                    var buildInFileSystemParams =
                        FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                    var createParameters = new HostPlayModeParameters
                    {
                        BuildinFileSystemParameters = buildInFileSystemParams,
                        CacheFileSystemParameters = cacheFileSystemParams
                    };
                    initOperation = package.InitializeAsync(createParameters);
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(_playMode), _playMode, "不支持的运行模式");                
            }
            await initOperation.ToUniTask();

            if (initOperation.Status == EOperationStatus.Succeed)
            {
                Debug.Log("资源包初始化成功！");
                return package;
            }
            else
            {
                Debug.LogError($"资源包初始化失败：{initOperation.Error}");
                return null;
            }
        }
        
        /// <summary>
        /// 请求获取最新的资源包版本号
        /// </summary>
        /// <param name="package">目标包裹</param>
        /// <returns>最新版本号，失败时返回 null</returns>
        public async UniTask<string> RequestPackageVersion(ResourcePackage package)
        {
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
        
        
        /// <summary>
        ///  更新资源包清单（Manifest）
        /// </summary>
        /// <param name="package">目标包裹</param>
        /// <param name="packageVersion">目标版本号</param>
        public async UniTask<bool> UpdatePackageManifest(ResourcePackage package,string packageVersion)
        {
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
        
        /// <summary>
        /// 获取资源下载器
        /// </summary>
        /// <param name="package">目标包裹</param>
        /// <returns>补丁下载器</returns>
        public ResourceDownloaderOperation GetDownloader(ResourcePackage package)
        {
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
            return downloader;
        }
        
        /// <summary>
        /// 下载资源
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        public async UniTask<bool> Download(ResourcePackage package)
        {
            var downloader = GetDownloader(package);
            if (downloader.TotalDownloadCount == 0)
            {
                Debug.Log("没有需要更新的资源！");
                return true;
            }
            Debug.Log($"需要下载{downloader.TotalDownloadCount}个文件 , 共{downloader.TotalDownloadBytes/1024f/1024f:F2} MB");

            // #region 注册回调方法
            // downloader.DownloadFinishCallback = data =>
            // {
            //     onDownloadFinishCallback?.Invoke(data.Succeed);
            // }; //当下载器结束（无论成功或失败）
            // downloader.DownloadErrorCallback = data =>
            // {
            //     onDownloadErrorCallback?.Invoke(data.FileName,data.ErrorInfo);
            // }; //当下载器发生错误
            // downloader.DownloadUpdateCallback = data =>
            // {
            //     onDownloadUpdateCallback?.Invoke(data.CurrentDownloadBytes,data.TotalDownloadBytes,data.Progress);
            // }; //当下载进度发生变化
            // downloader.DownloadFileBeginCallback = data =>
            // {
            //     onDownloadFileBeginCallback?.Invoke(data.FileName,data.FileSize);
            // }; //当开始下载某个文件
            //#endregion
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
        
        
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="location"></param>
        /// <param name="package"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async UniTask<T> LoadAssetAsync<T>(string location,ResourcePackage package) where T : UnityEngine.Object
        {
            AssetHandle handle = package.LoadAssetAsync<T>(location);
            await handle.ToUniTask();
            if (handle.Status == EOperationStatus.Succeed)
            {
                Debug.Log($"加载资源{location}成功!");
                return handle.AssetObject as T;
            }
            Debug.LogError($"加载资源{location}失败!");
            return null;
        }
        
        public async UniTask DestroyPackage()
        {
            // 先销毁资源包
            var package = YooAssets.GetPackage(_packageName);
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

       
    }

    
    
    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
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
}