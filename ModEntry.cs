using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace EasterEgg
{
    public class ModEntry : EasterEgg
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

