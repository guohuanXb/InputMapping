using VFramework;

namespace Native
{
    public interface IServerConfigModel :IModel
    {
        AssetServerConfig ServerPath { get; }
    }

    public class ServerConfigModel :AbstractModel,IServerConfigModel
    {
        protected override void OnInit()
        {
            ServerPath = new()
            {
                defaultHostServer = "http://172.17.127.72/PC/v1.0",
                fallbackHostServe = "http://172.17.127.72/PC/v1.0"
            };
        }

        public AssetServerConfig ServerPath { get; private set; }
    }
}