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
using AssetManager;

namespace EasterEgg
{
    public static class ResourceRedirector
    {
        private static readonly Assembly ModAssembly = Assembly.GetExecutingAssembly();
        private static readonly string[] ResourceNames = ModAssembly.GetManifestResourceNames();

        public static Texture2D ForceLoadTexture(string assetName, IMonitor monitor)
        {
            try
            {
                string resourceName = ResourceNames.FirstOrDefault(r => 
                    r.Contains(assetName, StringComparison.OrdinalIgnoreCase) && 
                    r.EndsWith(".png", StringComparison.OrdinalIgnoreCase));

                if (resourceName == null)
                {
                    monitor.Log($"[Critical] Resource matching '{assetName}' not found in DLL.", LogLevel.Error);
                    return null;
                }

                using (Stream stream = ModAssembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null) return null;
                    return Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
                }
            }
            catch (Exception ex)
            {
                monitor.Log($"[Critical] DLL Stream Error: {ex.Message}", LogLevel.Error);
                return null;
            }
        }
    }
}
