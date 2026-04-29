using Framework;
using UnityEngine;

namespace Example
{
    public interface IStorage :IUtility
    {
        string LoadString(string name);
        void SaveString(string name ,string value);
    }

    public class PlayerPrefsStore : IStorage
    {
        public string LoadString(string name)
        {
            return PlayerPrefs.GetString(name,"");
        }

        public void SaveString(string name, string value)
        {
            PlayerPrefs.SetString(name,value);
        }
    }
}