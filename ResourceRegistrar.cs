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
    public class ResourceRegistrar
    {
        private readonly IMonitor Monitor;
        private readonly Assembly ModAssembly;
        private readonly string[] ResourceNames;
        private const string TargetPath = "Virtual/Textures/";

        public ResourceRegistrar(IModHelper helper, IMonitor monitor)
        {
            this.Monitor = monitor;
            this.ModAssembly = Assembly.GetExecutingAssembly();
            this.ResourceNames = this.ModAssembly.GetManifestResourceNames();
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            
            this.Monitor.Log("Resource Registrar initialized: Monitoring 'Virtual/Textures/' in DLL.", LogLevel.Debug);
        }

        private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.ToString().StartsWith(TargetPath, StringComparison.OrdinalIgnoreCase))
            {
                e.LoadFrom(() =>
                {
                    string fileName = Path.GetFileName(e.NameWithoutLocale.ToString());
                    string? fullResourceName = this.ResourceNames.FirstOrDefault(r => 
                        r.EndsWith($"{fileName}.png", StringComparison.OrdinalIgnoreCase));

                    if (fullResourceName == null)
                    {
                        this.Monitor.Log($"[DLL Search] Could not find '{fileName}.png' embedded in DLL.", LogLevel.Trace);
                        return null;
                    }

                    try
                    {
                        using (Stream? stream = this.ModAssembly.GetManifestResourceStream(fullResourceName))
                        {
                            if (stream == null) return null;
                            return Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Monitor.Log($"[DLL Error] Failed to stream {fullResourceName}: {ex.Message}", LogLevel.Error);
                        return null;
                    }
                }, AssetLoadPriority.High);
            }
        }
    }
}

