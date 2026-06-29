using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VFramework;

namespace Native.UIKit.Framework
{
    public interface IUIManager : ISystem
    {
        /// <summary>
        /// 注册场景与面板的映射关系。热更层在 DependencyRegister 中调用，
        /// 告知 UIManager 某个场景进入时需要预加载哪些面板。
        /// </summary>
        void RegisterScenePanels(string sceneName, List<string> panelLocations);
        /// <summary>
        /// 获取已注册的场景面板定位列表，未注册返回 null。
        /// </summary>
        List<string> GetScenePanels(string sceneName);
        /// <summary>
        /// 为指定场景预加载所需的面板预制体。
        /// 约定：requiredPanels 中的每个条目都是 YooAsset 资源定位（即预制体名，与 PanelConfig.Location 一致）。
        /// 这些 UI 面板预制体必然挂载了继承 IPanel 的子类脚本，因此加载出 GameObject 后，
        /// 通过 GetComponent&lt;IPanel&gt;() 取得该脚本的 System.Type，以该 Type 为键把预制体缓存进 _prefabCache，
        /// 供后续 OpenPanel&lt;T&gt; / OpenPanelAsync&lt;T&gt; 用 typeof(T) 直接命中。
        /// 取出预制体后会立刻 ReleaseAssetHandle，使 YooAsset 内部引用计数减 1——
        /// 这样后续调用资源卸载方法时可正常回收底层资源。
        /// 已缓存过的 Type 会跳过，重复调用安全。任一面板加载失败（或预制体未挂 IPanel 脚本）将抛出 AggregateException。
        /// </summary>
        /// <param name="packageName">面板资源所属的资源包名</param>
        /// <param name="requiredPanels">目标场景需要的面板在 YooAsset 中的资源定位列表</param>
        UniTask PreloadPanelForScene(string packageName,List<string> requiredPanels);
        /// <summary>
        /// 初始化UIRoot树
        /// </summary>
        /// <returns></returns>
        UniTask InstantiateLayer();
        /// <summary>
        /// 清除当前场景中Panel预制体缓存，PanelData缓存，UIRoot结构缓存
        /// </summary>
        void ClearUICache();
        T GetPanel<T>() where T : IPanel;
        T OpenPanel<T>(Transform parent = null) where T : IPanel;
        void FocusPanel(IPanel panel);
        void ClosePanel<T>() where T : IPanel;
    }
}