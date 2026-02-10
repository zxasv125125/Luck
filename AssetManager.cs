using System;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;

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
                e.LoadFrom(() => this.LoadFromDll("degend.png"), AssetLoadPriority.Medium);
            }
        }

        private Texture2D LoadFromDll(string fileName)
        {
            string path = $"{this.RootPath}.{fileName}";
            
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            
            if (stream == null)
            {
                this.Monitor.Log($"Can't find {path} in DLL!", LogLevel.Error);
                return null;
            }
            return Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
        }
    }
}
