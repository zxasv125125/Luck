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
    public static class InternalImageLoader
    {
        private static readonly Assembly ModAssembly = Assembly.GetExecutingAssembly();

        public static Texture2D? GetTextureFromDLL(string fileName, IMonitor monitor)
        {
            string[] resourceNames = ModAssembly.GetManifestResourceNames();
            string? target = resourceNames.FirstOrDefault(r => 
                r.EndsWith($"{fileName}.png", StringComparison.OrdinalIgnoreCase) ||
                r.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

            if (target == null)
            {
                monitor.Log($"[Critical] DLL Resource '{fileName}' NOT FOUND. Resources available: {string.Join(", ", resourceNames)}", LogLevel.Error);
                return null;
            }

            try
            {
                using (Stream? stream = ModAssembly.GetManifestResourceStream(target))
                {
                    if (stream == null) return null;
                    return Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
                }
            }
            catch (Exception ex)
            {
                monitor.Log($"[Critical] Failed to load {target}: {ex.Message}", LogLevel.Error);
                return null;
            }
        }
    }
}

