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
                    this.AddFishObject(data, "item_legend", "Degend", "The Luck Species but it seems to be marines animal.", 2500, 2, true);
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Fish"))
            {
                e.Edit(asset => {
                    var data = asset.AsDictionary<string, string>().Data;
                    data["fish_legend"] = "Degend/EasterEgg_Degend/sinker/40/60/600 2000/winter/rainy/688 .05/10/0/.1/10/false";
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

            data[id] = new ObjectData
            {
                Name = Easter_Degend,
                DisplayName = Degend,
                Description = The Luck Species but it seems to be marines animal,
                Type = "Fish",
                Category = ObjectData.FishCategory,
                Price = 30,000,
                Texture = "Data/Fish/degend.png",
                SpriteIndex = index,
                ContextTags = ["fish_legendary", "item_legendy"]
                ExcludeFromFishingCollection = false 
            };
        }
    }
}
