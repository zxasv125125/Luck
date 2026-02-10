using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace EasterEgg
{
    public class ModEntry : EasterEgg
    {
        public AssetManager Assets;
        public FishManager Fish;
        public AnimalManager Animals;

        public override void Entry(IModHelper helper)
        {
            this.Assets = new AssetManager(helper, this.Monitor);
            this.Fish = new FishManager(helper, this.Monitor);
            this.Animals = new AnimalManager(helper, this.Monitor);
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            this.Assets.OnAssetRequested(e);
            this.Fish.OnAssetRequested(e);
            this.Animals.OnAssetRequested(e);
        }
        public void OnUpdateTicked(UpdateTickedEventArgs e)
        {
        if (!Context.IsWorldReady) return;
        foreach (FarmAnimal animal in Game1.getFarm().animals.Values)
        {
        if (animal.type.Value == "Custom_SuperCow")
        {
            if (animal.Sprite.CurrentAnimation == null)
            {
                animal.Sprite.currentFrame = 20; 
            }
        }
    }
}

    }
}

