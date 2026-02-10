using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace MyCustomMod
{
    public class ModEntry : Mod
    {
        // สร้าง Instance ของ Manager ต่างๆ
        public AssetManager Assets;
        public FishManager Fish;
        public AnimalManager Animals;

        public override void Entry(IModHelper helper)
        {
            // เริ่มต้นระบบ Manager
            this.Assets = new AssetManager(helper, this.Monitor);
            this.Fish = new FishManager(helper, this.Monitor);
            this.Animals = new AnimalManager(helper, this.Monitor);

            // ลงทะเบียน Event หลักที่นี่ที่เดียว
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            // ส่งต่อ Event ไปให้ Manager แต่ละตัวจัดการส่วนของตัวเอง
            this.Assets.OnAssetRequested(e);
            this.Fish.OnAssetRequested(e);
            this.Animals.OnAssetRequested(e);
        }
    }
}
