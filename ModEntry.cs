using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
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
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            this.Assets.OnAssetRequested(e);
            this.Fish.OnAssetRequested(e);
        }
    }
}
