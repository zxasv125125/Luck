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
                // เปลี่ยนมาใช้ LoadFromDll ที่คืนค่าเป็น Stream
                e.LoadFrom(() => this.LoadFromDll("degend.png"), AssetLoadPriority.Medium);
            }
        }
        private Stream LoadFromDll(string fileName)
        {
            string path = $"{this.RootPath}.{fileName}";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            
            if (stream == null)
            {
                this.Monitor.Log($"Can't find {path} in DLL!", LogLevel.Error);
                return null;
            }
            return stream; 
        }
    }
}
