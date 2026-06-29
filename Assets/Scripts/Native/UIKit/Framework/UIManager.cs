using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VFramework;
using YooAsset;

namespace Native.UIKit.Framework
{
    public class UIManager :AbstractSystem,IUIManager
    {
        private const string RootLocation = "UIRoot";
        private IResourceSystem _resourceSystem;
        /// <summary>
        /// 预加载的面板预制体缓存，以 Panel 的 System.Type 为键。
        /// Type 由预制体上挂载的 IPanel 子类脚本 GetComponent&lt;IPanel&gt;().GetType() 取得，
        /// 这样 OpenPanel&lt;T&gt; 可直接用 typeof(T) 命中，无需 location 反查 Type。
        /// 只持有 GameObject 预制体本身的引用，不持有 AssetHandle——
        /// 加载完成后会立刻 Release 句柄，使 YooAsset 内部引用计数减 1，
        /// 这样后续调用资源卸载方法时可正常回收底层资源。
        /// </summary>
        private Dictionary<Type, GameObject> _prefabCache;
        /// <summary>
        /// 所有面板元数据
        /// </summary>
        private Dictionary<Type,PanelData> _panelDic;
        private Dictionary<UILayer,Transform> _uiLayerDic;
        /// <summary>
        /// 场景→面板定位列表映射，由热更层通过 RegisterScenePanels 填充。
        /// </summary>
        private Dictionary<string, List<string>> _scenePanelMap;
        protected override void OnInit()
        {
            _resourceSystem = this.GetSystem<IResourceSystem>();
            _panelDic = new();
            _prefabCache = new Dictionary<Type, GameObject>();
            _uiLayerDic = new();
            _scenePanelMap = new Dictionary<string, List<string>>();
        }
        
        
        public async UniTask InstantiateLayer()
        {
            _uiLayerDic?.Clear();
            var packageModel = this.GetModel<IPackageModel>();
            var rootHandle = await _resourceSystem.LoadAssetAsync<GameObject>(RootLocation,packageModel.DefaultPackageName);
            var rootPrefab = rootHandle.AssetObject as GameObject;
            var root =  GameObject.Instantiate(rootPrefab);
            rootHandle.Release();
            foreach (Transform child in root.transform)
            {
                var layer = Enum.Parse<UILayer>(child.name,ignoreCase:true);
                _uiLayerDic.Add(layer,child);
            }
        }

        

        
        public void RegisterScenePanels(string sceneName, List<string> panelLocations)
        {
            if (string.IsNullOrEmpty(sceneName) || panelLocations == null || panelLocations.Count == 0)
                return;

            if (!_scenePanelMap.TryGetValue(sceneName, out var list))
            {
                list = new List<string>();
                _scenePanelMap[sceneName] = list;
            }

            foreach (var location in panelLocations)
            {
                if (!string.IsNullOrEmpty(location) && !list.Contains(location))
                    list.Add(location);
            }
        }

        public List<string> GetScenePanels(string sceneName)
        {
            if (string.IsNullOrEmpty(sceneName) || !_scenePanelMap.TryGetValue(sceneName, out var list))
                return null;
            return list;
        }

        public async UniTask PreloadPanelForScene(string packageName, List<string> requiredPanels)
        {
            if (requiredPanels == null || requiredPanels.Count == 0)
            {
                return;
            }

            // 仅过滤掉空定位、并对本批次去重；是否已缓存交由加载后按 Type 判定
            var pending = new List<string>(requiredPanels.Count);
            var seen = new HashSet<string>();
            foreach (var location in requiredPanels)
            {
                if (string.IsNullOrEmpty(location)) continue;
                if (!seen.Add(location)) continue;            // 本批次去重，避免并发加载同一预制体两次
                pending.Add(location);
            }

            if (pending.Count == 0)
            {
                return;
            }

            // 并发加载：取出预制体后立即 Release 句柄，只保留 GameObject 引用，
            // 再通过 GetComponent<IPanel>() 拿到面板脚本 Type 作缓存键；按 Type 去重已缓存
            var errors = new List<Exception>(pending.Count);
            var loadedThisRound = new List<Type>(pending.Count);
            async UniTask LoadOnePanel(string location)
            {
                AssetHandle handle = null;
                try
                {
                    handle = await _resourceSystem.LoadAssetAsync<GameObject>(location, packageName);
                    if (handle.Status == EOperationStatus.Failed)
                    {
                        errors.Add(new InvalidOperationException(
                            $"Preload panel failed, location: {location}, package: {packageName}, info: {handle.LastError}"));
                        return;
                    }

                    var prefab = handle.AssetObject as GameObject;
                    if (prefab == null)
                    {
                        errors.Add(new InvalidOperationException(
                            $"Preload panel asset is not a GameObject, location: {location}"));
                        return;
                    }

                    // 关键映射：预制体上挂载的 IPanel 子类脚本 Type 作为缓存键
                    var panel = prefab.GetComponent<IPanel>();
                    if (panel == null)
                    {
                        errors.Add(new InvalidOperationException(
                            $"Preload panel has no IPanel component, location: {location}"));
                        return;
                    }

                    var panelType = panel.GetType();
                    // 已缓存则跳过，避免覆盖（同 Type 由不同 location 指向时以先入者为准）
                    if (!_prefabCache.TryAdd(panelType, prefab))
                    {
                        return;
                    }
                    loadedThisRound.Add(panelType);
                }
                catch (Exception ex)
                {
                    errors.Add(ex);
                }
                finally
                {
                    // 无论成功失败都立即释放句柄：成功时只用预制体，失败时回收已分配的引用
                    handle?.Release();
                }
            }

            await UniTask.WhenAll(pending.Select(LoadOnePanel));

            if (errors.Count > 0)
            {
                // 任一加载失败：清理本轮已缓存的不完整结果，保持缓存状态干净，再向上聚合抛出
                foreach (var panelType in loadedThisRound)
                {
                    _prefabCache.Remove(panelType);
                }
                throw new AggregateException("Preload panels for scene failed, see inner exceptions.", errors);
            }
        }

        public void ClearUICache()
        {
            _prefabCache.Clear();
            _panelDic.Clear();
            _uiLayerDic.Clear();
            _scenePanelMap.Clear();
        }


        public T GetPanel<T>()where T : IPanel
        {
            if(_panelDic.TryGetValue(typeof(T),out var panelData) &&
               panelData.IsOpened &&
               panelData.Panel !=null)
                return (T)panelData.Panel;
            return default;
        }

        public T OpenPanel<T>(Transform parent = null)where T : IPanel
        {
            var panelType = typeof(T);
            if (_panelDic.TryGetValue(panelType, out var panelData)
                && panelData.Panel != null)
            {
                if (!panelData.IsOpened)
                {
                    panelData.Panel.OnOpen();
                    panelData.IsOpened = true;
                }
                return (T)panelData.Panel;
            }

            if (!_prefabCache.TryGetValue(panelType, out var prefab))
            {
                return default; // 未预加载，调用方需自行判空
            }

            // 先取预制体上的 IPanel 以确定归属层：Layer 在预制体上即可读取，无需实例化
            var protoPanel = prefab.GetComponent<IPanel>();
            GameObject instance;
            if (parent == null)
            {
                if (_uiLayerDic == null || !_uiLayerDic.TryGetValue(protoPanel.Layer, out var layer))
                {
                    throw new InvalidOperationException(
                        $"OpenPanel<{typeof(T).Name}> 找不到 UILayer '{protoPanel.Layer}' 对应的层节点，请确认已调用 InstantiateLayer 且 UIRoot 预制体包含该层节点");
                }
                instance = GameObject.Instantiate(prefab, layer.transform, false);
            }
            else
            {
                instance = GameObject.Instantiate(prefab, parent, false);
            }
            var panel = instance.GetComponent<IPanel>();

            panel.OnInit();
            panel.OnOpen();
            _panelDic[panelType] = new PanelData { Panel = panel, IsOpened = true };
            return (T)panel;
        }
        

        public void FocusPanel(IPanel panel)
        {
            panel.OnFocus();
        }


        public void ClosePanel<T>()where T : IPanel
        {
            var type = typeof(T);
            if (!_panelDic.TryGetValue(type, out var panelData) || !panelData.IsOpened || panelData.Panel == null)
                return;
            panelData.Panel.OnClose();
            panelData.IsOpened = false;
        }
    }
}