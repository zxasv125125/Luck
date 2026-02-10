using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Fish;
using StardewValley.GameData.Objects;

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
                    
                    // ใช้ ID เฉพาะของมอดเราเอง (EasterEgg_Degend)
                    this.AddFishObject(data, "EasterEgg_Degend", "Degend", "The Luck Species but it seems to be marines animal.", 30000, 0, true);
                });
            }
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Fish"))
            {
                e.Edit(asset => {
                    var data = asset.AsDictionary<string, string>().Data;
                    data["EasterEgg_Degend"] = "Degend/110/sinker/40/60/600 2000/winter/rainy/688 .05/10/0/.1/10/false";
                });
            }
        }

        private void AddFishObject(IDictionary<string, ObjectData> data, string id, string name, string description, int price, int index, bool isLegendary)
        {
            var contextTags = new List<string>();
            if (isLegendary)
            {
                contextTags.Add("fish_legendary");
                contextTags.Add("item_legendary");
            }

            data[id] = new ObjectData
            {
                Name = id, // EasterEgg_Degend
                DisplayName = name, // Degend
                Description = description,
                Type = "Fish",
                Category = ObjectData.FishCategory,
                Price = price,
                Texture = "Assets/Fish/degend",
                SpriteIndex = index,
                ContextTags = contextTags,
                ExcludeFromFishingCollection = false 
            };
        }
    }
}
