using System;

namespace Native
{
    [Serializable]
    public struct AssetServerConfig
    {
        public string defaultHostServer;
        public string fallbackHostServe;
        public AssetServerConfig(string defaultHostServer ,string fallbackHostServe)
        {
            this.defaultHostServer = defaultHostServer;
            this.fallbackHostServe = fallbackHostServe;
        }
    }
}