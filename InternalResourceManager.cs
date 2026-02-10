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
    public class InternalResourceManager
    {
        private readonly IMonitor Monitor;
        private readonly Assembly ModAssembly;
        private readonly string[] ResourceNames;
        private const string TargetPath = "Virtual/Textures/";

        public InternalResourceManager(IModHelper helper, IMonitor monitor)
        {
            this.Monitor = monitor;
            this.ModAssembly = Assembly.GetExecutingAssembly();
            this.ResourceNames = this.ModAssembly.GetManifestResourceNames();

            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            
            this.Monitor.Log($"Internal Resource Manager active. Monitoring: {TargetPath}", LogLevel.Debug);
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.ToString().StartsWith(TargetPath, StringComparison.OrdinalIgnoreCase))
            {
                e.LoadFrom(() =>
                           string fileName = Path.GetFileName(e.NameWithoutLocale.ToString());
                    string? resourcePath = this.ResourceNames.FirstOrDefault(r => 
                        r.EndsWith($"{fileName}.png", StringComparison.OrdinalIgnoreCase));

                    if (resourcePath == null)
                    {
                        this.Monitor.Log($"[DLL Search] Resource '{fileName}.png' not found in DLL Manifest.", LogLevel.Trace);
                        return null;
                    }

                    try
                    {
                        using (Stream? stream = this.ModAssembly.GetManifestResourceStream(resourcePath))
                        {
                            if (stream == null) return null;
                            return Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Monitor.Log($"[DLL Error] Failed to load {resourcePath}: {ex.Message}", LogLevel.Error);
                        return null;
                    }
                }, AssetLoadPriority.High);
            }
        }
    }
}
