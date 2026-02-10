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
using StardewValley.GameData.Objects

namespace EasterEgg
{
    public class AssetManager
    {
        private readonly IMonitor Monitor;
        private readonly string[] ResourceNames;
        private const string VirtualPath = "EasterEgg/Virtual/Textures/";

        public AssetManager(IModHelper helper, IMonitor monitor)
        {
            this.Monitor = monitor;
            this.ResourceNames = Assembly.GetExecutingAssembly().GetManifestResourceNames();
        }

        public void OnAssetRequested(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.ToString().StartsWith(VirtualPath, StringComparison.OrdinalIgnoreCase))
            {
                e.LoadFrom(() =>
                {
                    string id = e.NameWithoutLocale.ToString().Substring(VirtualPath.Length);
                    return ResourceRedirector.ForceLoadTexture(id, this.Monitor);
                }, AssetLoadPriority.High);
            }
        }
    }
}
