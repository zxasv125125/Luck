using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace EasterEgg
{
    public class AssetManager
    {
        private IModHelper Helper;
        private IMonitor Monitor;
        private string RootPath = "MyCustomMod.Assets"; // ชื่อ Namespace.โฟลเดอร์

        public AssetManager(IModHelper helper, IMonitor monitor)
        {
            this.Helper = helper;
            this.Monitor = monitor;
        }

        public void OnAssetRequested(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Maps/MyFishSprites"))
                e.LoadFrom(() => this.LoadFromDll("fish.png"), AssetLoadPriority.Medium);
            if (e.NameWithoutLocale.IsEquivalentTo("Animals/MyCustomAnimals"))
                e.LoadFrom(() => this.LoadFromDll("animals.png"), AssetLoadPriority.Medium);
        }

        private Texture2D LoadFromDll(string fileName)
        {
            string path = $"{this.RootPath}.{fileName}";
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            if (stream == null)
            {
                this.Monitor.Log($"ค้นหาไฟล์ {path} ใน DLL ไม่เจอ!", LogLevel.Error);
                return null;
            }
            return Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
        }
    }
}

