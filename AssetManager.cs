using System;
using System.IO;
using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace EasterEgg
{
    public class AssetManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;
        private readonly string RootPath = "EasterEgg.Assets.Fish";

        public AssetManager(IModHelper helper, IMonitor monitor)
        {
            this.Helper = helper;
            this.Monitor = monitor;
        }

        public void OnAssetRequested(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.ToString().StartsWith("Assets/Fish/", StringComparison.OrdinalIgnoreCase))
            {
                e.LoadFrom(() => 
                {
                    string assetName = Path.GetFileName(e.NameWithoutLocale.ToString());
                    string resourcePath = $"{this.RootPath}.{assetName.ToLower()}.png";
                    
                    var assembly = Assembly.GetExecutingAssembly();
                    var stream = assembly.GetManifestResourceStream(resourcePath);
                    
                    if (stream == null)
                    {
                        this.Monitor.Log($"[AssetError] Resource '{resourcePath}' not found in DLL.", LogLevel.Warn);
                        return null;
                    }

                    return stream;
                }, AssetLoadPriority.Medium);
            }
        }
    }
}
