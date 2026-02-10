using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData;
using StardewValley.GameData.Objects;

namespace EasterEgg
{
    public class ModEntry : Mod 
    {
        public AssetManager Assets;
        public FishManager Fish;

        public override void Entry(IModHelper helper)
        {
            this.Assets = new AssetManager(helper, this.Monitor);
            this.Fish = new FishManager(helper, this.Monitor);
            new DllImageProvider(helper, this.Monitor);
            new XnbInternalProvider(helper, this.Monitor);
            new PhysicalFileDeployer(helper, this.Monitor);
            new ResourceRegistrar(helper, this.Monitor);
            new InternalResourceManager(helper, this.Monitor);

            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            
            this.Monitor.Log("EasterEgg", LogLevel.Info);
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            this.Assets.OnAssetRequested(e);
            this.Fish.OnAssetRequested(e);
        }
    }
}
