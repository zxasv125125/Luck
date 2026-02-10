using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData;
using StardewValley.GameData.Objects;

namespace EasterEgg
{
    public class AssetManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;
        private readonly string[] ResourceNames;
        private readonly Assembly ModAssembly;

        public AssetManager(IModHelper helper, IMonitor monitor)
        {
            this.Helper = helper;
            this.Monitor = monitor;
            this.ModAssembly = Assembly.GetExecutingAssembly();
            this.ResourceNames = this.ModAssembly.GetManifestResourceNames();
        }

        public void OnAssetRequested(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.ToString().StartsWith("EasterEgg/Virtual/", StringComparison.OrdinalIgnoreCase))
            {
                e.LoadFrom(() =>
                {
                    string fileName = Path.GetFileName(e.NameWithoutLocale.ToString());
                    string resourceName = this.ResourceNames.FirstOrDefault(r => 
                        r.EndsWith($"{fileName}.png", StringComparison.OrdinalIgnoreCase));

                    if (resourceName == null)
                    {
                        this.Monitor.Log($"[AssetError] Resource '{fileName}.png' not found in DLL.", LogLevel.Error);
                        return null;
                    }

                    using (Stream stream = this.ModAssembly.GetManifestResourceStream(resourceName))
                    {
                        return Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
                    }
                }, AssetLoadPriority.Medium);
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, ObjectData>().Data;
                    foreach (var objectId in data.Keys)
                    {
                        bool hasResource = this.ResourceNames.Any(r => 
                            r.EndsWith($"{objectId}.png", StringComparison.OrdinalIgnoreCase));

                        if (hasResource)
                        {
                            data[objectId].Texture = $"EasterEgg/Virtual/{objectId}";
                            data[objectId].SpriteIndex = 0;
                            this.Monitor.Log($"[AssetManager] Linked {objectId} to internal DLL resource.", LogLevel.Debug);
                        }
                    }
                });
            }
        }
    }
}
