using System;
using System.IO;
using System.Linq;
using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace EasterEgg
{
    public class AssetManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;
        private readonly string[] ResourceNames;

        public AssetManager(IModHelper helper, IMonitor monitor)
        {
            this.Helper = helper;
            this.Monitor = monitor;
            this.ResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
        }

        public void OnAssetRequested(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.ToString().StartsWith("Assets/Fish/", StringComparison.OrdinalIgnoreCase))
            {
                e.LoadFrom(() => 
                {
                    string assetName = Path.GetFileName(e.NameWithoutLocale.ToString());
                    string targetResource = this.ResourceNames.FirstOrDefault(r => 
                        r.Contains(assetName, StringComparison.OrdinalIgnoreCase) && r.EndsWith(".png"));

                    if (targetResource == null)
                    {
                        this.Monitor.Log($"[AssetError] Could not find any resource matching '{assetName}' in DLL.", LogLevel.Error);
                        return null;
                    }

                    return Assembly.GetExecutingAssembly().GetManifestResourceStream(targetResource);
                }, AssetLoadPriority.Medium);
            }
        }
    }
}
