using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.GameData.FarmAnimals;

namespace EasterEgg
{
    public class AnimalManager
    {
        private IModHelper Helper;
        private IMonitor Monitor;

        public AnimalManager(IModHelper helper, IMonitor monitor)
        {
            this.Helper = helper;
            this.Monitor = monitor;
        }

        public void OnAssetRequested(AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo("Data/FarmAnimals"))
            {
                e.Edit(asset => {
                    var data = asset.AsDictionary<string, FarmAnimalData>().Data;
                    this.CreateAnimal(data, "Custom_SuperCow", "Barn", 15000, "White Cow");
                    this.CreateAnimal(data, "Custom_GoldenSheep", "Barn", 25000, "Sheep");
                    this.CreateAnimal(data, "Custom_MagicChicken", "Coop", 8000, "White Chicken");
                    this.CreateAnimal(data, "Custom_CrystalDuck", "Coop", 12000, "Duck");
                });
            }
          if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops"))
          {
            e.Edit(asset => {
              var data = asset.AsDictionary<string, StardewValley.GameData.Shops.ShopData>().Data;
              if (data.TryGetValue("AnimalShop", out var shop))
              {
                shop.Items.Add(new StardewValley.GameData.Shops.ShopItemData
                               {
                                 Id = "Custom_SuperCow_Sale",
                                   ItemId = "Custom_SuperCow", // ต้องตรงกับ ID ใน Data/FarmAnimals
                                 TradeItemId = "(O)74", // (ตัวเลือก) ถ้าอยากให้แลกด้วย Prismatic Shard แทนเงิน
                                 Price = 15000
                                 });
              }
            });
          }
        }
        private void CreateAnimal(IDictionary<string, FarmAnimalData> data, string id, string houseType, int price, string shadowType)
        {
            data[id] = new FarmAnimalData
            {
                DisplayName = id,
                HouseBank = new List<string> { houseType }, // หัวใจสำคัญ: ระบุ "Barn" หรือ "Coop"
                PurchasePrice = price,
                Texture = $"Animals/{id}", // ชื่อ Asset รูปภาพที่เราจะ Load ใน AssetManager
                DaysToMature = 3,
                ExperienceGained = 15,
                MaxHappiness = 255,
                FullnessDrain = 7,
                HappinessDrain = 7,
                ProduceItemIds = new List<FarmAnimalProduceData> {
                    new FarmAnimalProduceData { ItemId = "(O)176", MinimumHappiness = 0 } 
                },
                Shadow = shadowType 
            };
        }
    }
}

