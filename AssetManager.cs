using System.IO;
using System.Reflection;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace EasterEgg
{
    public class AssetManager
    {
        private IModHelper Helper;
        private IMonitor Monitor;
        private string RootPath = "EasterEgg.Assets.Fish"; 

        public AssetManager(IModHelper helper, IMonitor monitor)
        {
            this.Helper = helper;
            this.Monitor = monitor;
        }

        public void OnAssetRequested(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Assets/Fish/degend"))
            {
                e.LoadFrom(() => this.GetResourceStream("degend.png"), AssetLoadPriority.Medium);
            }
        }
        private Stream GetResourceStream(string fileName)
        {
            string path = $"{this.RootPath}.{fileName}";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            
            if (stream == null)
            {
                this.Monitor.Log($"[AssetError] Find {path} in DLL not Found!", LogLevel.Error);
                return null;
            }
            return stream;
        }
    }
}
