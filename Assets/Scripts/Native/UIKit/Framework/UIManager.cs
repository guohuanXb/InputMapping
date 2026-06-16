using UnityEngine;
using VFramework;

namespace Native.UIKit.Framework
{
    public class UIManager : AbstractSystem,IUIManager
    {

        protected override void OnInit()
        {
            
        }
        
        public T OpenPanel<T>(string prefabName = null, UILevel level = UILevel.Common, IUIData uiData = null)
        {
            var panelName = prefabName ?? typeof(T).Name;
            var resourceSystem = this.GetSystem<IResourceSystem>();
            var packageModel = this.GetModel<IPackageModel>();
            var packageName = packageModel.DefaultPackageName;
            //GameObject prefab = resourceSystem.LoadAssetAsync<GameObject>(panelName, packageName);
            return default;
        }

        public void ClosePanel<T>() where T : UIPanel
        {
            throw new System.NotImplementedException();
        }

        public void ShowPanel<T>() where T : UIPanel
        {
            throw new System.NotImplementedException();
        }

        public void HidePanel<T>() where T : UIPanel
        {
            throw new System.NotImplementedException();
        }

        public T GetPanel<T>() where T : UIPanel
        {
            throw new System.NotImplementedException();
        }

        
    }
}