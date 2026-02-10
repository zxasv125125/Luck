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

        public AssetManager(IModHelper helper, IMonitor monitor)
        {
            this.Helper = helper;
            this.Monitor = monitor;
        }

        public void OnAssetRequested(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Assets/Fish/degend"))
            {
                e.LoadFrom(() => 
                {
                    string resourcePath = "EasterEgg.Assets.Fish.degend.png";
                    
                    var assembly = Assembly.GetExecutingAssembly();
                    var stream = assembly.GetManifestResourceStream(resourcePath);

                    if (stream == null)
                    {
                        this.Monitor.Log($"[Critical Error] not found '{resourcePath}' in DLL!", LogLevel.Error);
                        this.Monitor.Log("😡:", LogLevel.Warn);
                        foreach (string resName in assembly.GetManifestResourceNames())
                        {
                            this.Monitor.Log($" -> {resName}", LogLevel.Warn);
                        }
                        return null;
                    }

                    this.Monitor.Log($"[Success] loaded {resourcePath} successfully!", LogLevel.Debug);
                    return stream;
                }, AssetLoadPriority.Medium);
            }
        }
    }
}
