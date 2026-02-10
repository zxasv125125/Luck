using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData;
using StardewValley.GameData.Objects;

namespace EasterEgg
{
    public class DllImageProvider
    {
        private readonly IMonitor Monitor;
        private readonly Assembly ModAssembly;
        private readonly string[] ManifestResources;
        private const string VirtualPathPrefix = "Virtual/Textures/";

        public DllImageProvider(IModHelper helper, IMonitor monitor)
        {
            this.Monitor = monitor;
            this.ModAssembly = Assembly.GetExecutingAssembly();
            this.ManifestResources = this.ModAssembly.GetManifestResourceNames();
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.ToString().StartsWith(VirtualPathPrefix, StringComparison.OrdinalIgnoreCase))
            {
                e.LoadFrom(() =>
                {
                    string assetKey = Path.GetFileName(e.NameWithoutLocale.ToString());
                    string resourceName = this.ManifestResources.FirstOrDefault(r => 
                        r.EndsWith($"{assetKey}.png", StringComparison.OrdinalIgnoreCase) ||
                        r.Contains($".{assetKey}."));

                    if (resourceName == null)
                    {
                    }

                    try
                    {
                        using (Stream stream = this.ModAssembly.GetManifestResourceStream(resourceName))
                        {
                            if (stream == null) return null;
                            return Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }, AssetLoadPriority.High);
            }
        }
    }
}

