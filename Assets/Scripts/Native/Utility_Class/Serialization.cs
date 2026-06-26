using Newtonsoft.Json;
using VFramework;

namespace Native.Utility_Class
{
    public interface ISerializable :IUtility
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string json);
    }

    public class Serialization :ISerializable
    {
        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}