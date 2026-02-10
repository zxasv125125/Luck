using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Fish;

namespace EasterEgg
{
    public class FishManager
    {
        private IModHelper Helper;
        private IMonitor Monitor;

        public FishManager(IModHelper helper, IMonitor monitor)
        {
            this.Helper = helper;
            this.Monitor = monitor;
        }

        public void OnAssetRequested(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit(asset => {
                    var data = asset.AsDictionary<string, ObjectData>().Data;
                    this.AddFishObject(data, "fish_common", "name", "A very common small fish.", 50, 0, false);
                    this.AddFishObject(data, "fish_rare", "name", "A beautiful and rare golden fish.", 500, 1, false, isRare: true);
                    this.AddFishObject(data, "fish_legend", "name", "The ruler of the deep ocean.", 2500, 2, true);
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Fish"))
            {
                e.Edit(asset => {
                    var data = asset.AsDictionary<string, string>().Data;
                    data["fish_common"] = "name/40/smooth/5/15/600 2600/spring summer/both/-1/0/.4/.1/0/true";
                    data["fish_lare"] = "name/75/mixed/15/30/600 1900/fall/sunny/685 .35/3/.2/.3/0/true";
                    data["fish_legend"] = "name/110/sinker/40/60/600 2000/winter/rainy/688 .05/10/0/.1/10/false";
                });
            }
        }

        private void AddFishObject(IDictionary<string, ObjectData> data, string id, string name, string description, int price, int index, bool isLegendary, bool isRare = false)
        {
            var contextTags = new List<string>();
            if (isLegendary)
            {
                contextTags.Add("fish_legendary");
                contextTags.Add("item_legendary");
            }
            if (isRare)
            {
                contextTags.Add("item_rare");
                contextTags.Add("fish_rare"); //
            }

            data[id] = new ObjectData
            {
                Name = id,
                DisplayName = name,
                Description = description,
                Type = "Fish",
                Category = ObjectData.FishCategory,
                Price = price,
                Texture = "Maps/FishSprites",
                SpriteIndex = index,
                ContextTags = contextTags,
                ExcludeFromFishingCollection = false 
            };
        }
    }
}
