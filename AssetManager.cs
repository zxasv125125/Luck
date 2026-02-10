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
        private readonly string AssemblyName;

        public AssetManager(IModHelper helper, IMonitor monitor)
        {
            this.Helper = helper;
            this.Monitor = monitor;
            this.AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        }

        public void OnAssetRequested(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Assets/Fish/degend"))
            {
                e.LoadFrom(() => 
                {
                    string resourceName = $"{this.AssemblyName}.Assets.Fish.degend.png";
                    
                    var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

                    if (stream == null)
                    {
                        this.Monitor.Log($"[AssetError] Not Found: {resourceName}", LogLevel.Error);
                        foreach (var name in Assembly.GetExecutingAssembly().GetManifestResourceNames())
                        {
                            this.Monitor.Log($"Filee inside DLL: {name}", LogLevel.Trace);
                        }
                        return null;
                    }

                    return stream;
                }, AssetLoadPriority.Medium);
            }
        }
    }
}
