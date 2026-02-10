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
using StardewValley.GameData.Objects;;

namespace EasterEgg
{
    public class PhysicalFileDeployer
    {
        private readonly IMonitor Monitor;
        private readonly Assembly ModAssembly;
        private readonly string TargetBaseDir = Path.Combine(Constants.GamePath, "GameFiles", "Content", "EasterEgg", "Virtual", "Textures");

        public PhysicalFileDeployer(IMonitor monitor)
        {
            this.Monitor = monitor;
            this.ModAssembly = Assembly.GetExecutingAssembly();
            this.DeployResources();
        }

        private void DeployResources()
        {
            try
            {
                if (!Directory.Exists(this.TargetBaseDir))
                {
                    Directory.CreateDirectory(this.TargetBaseDir);
                }
                string[] resourceNames = this.ModAssembly.GetManifestResourceNames();
                var xnbResources = resourceNames.Where(r => r.EndsWith(".xnb", StringComparison.OrdinalIgnoreCase));

                foreach (string resourcePath in xnbResources)
                {
                    string fileName = resourcePath.Split('.').Reverse().Take(2).Reverse().Aggregate((a, b) => a + "." + b);
                    string fullOutputPath = Path.Combine(this.TargetBaseDir, fileName);
                    if (!File.Exists(fullOutputPath))
                    {
                        using (Stream stream = this.ModAssembly.GetManifestResourceStream(resourcePath))
                        using (FileStream fileStream = File.Create(fullOutputPath))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
