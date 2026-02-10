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
    public class XnbInternalProvider
    {
        private readonly Assembly ModAssembly;
        private readonly string[] ResourceNames;
        private const string TargetPath = "Virtual/Textures/";

        public XnbInternalProvider(IModHelper helper)
        {
            this.ModAssembly = Assembly.GetExecutingAssembly();
            this.ResourceNames = this.ModAssembly.GetManifestResourceNames();
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.ToString().StartsWith(TargetPath, StringComparison.OrdinalIgnoreCase))
            {
                e.LoadFrom(() =>
                {
                    string fileName = Path.GetFileNameWithoutExtension(e.NameWithoutLocale.ToString());
                    
                    string target = this.ResourceNames.FirstOrDefault(r => r.EndsWith($"{fileName}.xnb", StringComparison.OrdinalIgnoreCase))
                                 ?? this.ResourceNames.FirstOrDefault(r => r.EndsWith($"{fileName}.png", StringComparison.OrdinalIgnoreCase));

                    if (target == null) return null;

                    using (Stream stream = this.ModAssembly.GetManifestResourceStream(target))
                    {
                        if (stream == null) return null;

                        if (target.EndsWith(".xnb", StringComparison.OrdinalIgnoreCase))
                        {
                            return (object)stream;
                        }
                        return Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
                    }
                }, AssetLoadPriority.High);
            }
        }
    }
}

