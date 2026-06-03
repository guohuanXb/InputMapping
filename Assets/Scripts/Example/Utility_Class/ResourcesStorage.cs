using UnityEngine;
using UnityEngine.InputSystem;
using VFramework;

namespace Example.Utility_Class
{
    public interface IResourcesStore :IUtility
    {
        T LoadInputActionAsset<T>(string path) where T : ScriptableObject;
    }

    public class ResourcesStorage : IResourcesStore
    {
        public T LoadInputActionAsset<T>(string path)where T : ScriptableObject
        {
            var asset = Resources.Load<T>(path);
            if(asset!= null)
                return asset;
            return null;
        }
    }
}