using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.GameData;
using StardewValley.Menus;
using StardewValley.Tools;

namespace EasterEgg
{
    public class ModEntry : Mod
    {
        private AssetRepository? assetRepository;
        private FishModule? fishModule;
        private ContentInjector? contentInjector;
        private EventLinker? eventLinker;
        private FishingEngineExtension? fishingEngine;

        public override void Entry(IModHelper helper)
        {
            this.assetRepository = new AssetRepository();
            this.fishingEngine = new FishingEngineExtension(helper);
            this.contentInjector = new ContentInjector(helper, this.assetRepository);
            this.fishModule = new FishModule(helper, this.assetRepository, this.fishingEngine);
            this.eventLinker = new EventLinker(helper, this.fishModule, this.contentInjector);

            this.Initialize();
        }

        private void Initialize()
        {
            this.eventLinker?.Subscribe();
        }
    }

    internal class AssetRepository
    {
        private readonly string RootPath = "Virtual/Textures";
        private readonly Dictionary<string, string> internalPaths = new();

        public AssetRepository()
        {
            this.internalPaths.Add($"{this.RootPath}/degend".ToLower(), "EasterEgg.Virtual.Textures.degend.png");
            this.internalPaths.Add($"{this.RootPath}/degend_gold".ToLower(), "EasterEgg.Virtual.Textures.degend_gold.png");
            this.internalPaths.Add($"{this.RootPath}/eggshell".ToLower(), "EasterEgg.Virtual.Textures.eggshell.png");
        }

        public bool TryGetResource(string assetName, out string? resourcePath)
        {
            return this.internalPaths.TryGetValue(assetName.ToLower(), out resourcePath);
        }

        public string GetVirtualPath(string filename)
        {
            return $"{this.RootPath}/{filename}";
        }
    }

    internal class ContentInjector
    {
        private readonly IModHelper helper;
        private readonly AssetRepository repository;

        public ContentInjector(IModHelper helper, AssetRepository repository)
        {
            this.helper = helper;
            this.repository = repository;
        }

        public void OnAssetRequested(AssetRequestedEventArgs e)
        {
            if (this.repository.TryGetResource(e.NameWithoutLocale.Name, out string? resourcePath) && resourcePath != null)
            {
                e.LoadFrom(() =>
                {
                    using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
                    if (stream == null) return null!;
                    return Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
                }, AssetLoadPriority.Medium);
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Objects"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, ObjectData>().Data;
                    this.ApplyObjectPatches(data);
                });
            }

            if (e.NameWithoutLocale.IsEquivalentTo("Data/Fish"))
            {
                e.Edit(asset =>
                {
                    var data = asset.AsDictionary<string, string>().Data;
                    this.ApplyFishPatches(data);
                });
            }
        }

        private void ApplyObjectPatches(IDictionary<string, ObjectData> data)
        {
            data["EasterEgg_Degend"] = new ObjectData
            {
                Name = "EasterEgg_Degend",
                DisplayName = "Degend",
                Description = "A mythical marine creature thought to be an urban legend.",
                Type = "Fish",
                Category = -4,
                Price = 30000,
                Texture = this.repository.GetVirtualPath("degend"),
                SpriteIndex = 0,
                ContextTags = new List<string> { "category_fish", "fish_legendary", "item_legendary" }
            };

            data["EasterEgg_Degend_Gold"] = new ObjectData
            {
                Name = "EasterEgg_Degend_Gold",
                DisplayName = "Golden Degend",
                Description = "A shimmering variant of the already rare Degend.",
                Type = "Fish",
                Category = -4,
                Price = 100000,
                Texture = this.repository.GetVirtualPath("degend_gold"),
                SpriteIndex = 0,
                ContextTags = new List<string> { "category_fish", "fish_legendary", "item_legendary" }
            };

            data["EasterEgg_EggShell"] = new ObjectData
            {
                Name = "EasterEgg_EggShell",
                DisplayName = "Fragmented Shell",
                Description = "A pixelated shell fragment.",
                Type = "Basic",
                Category = -28,
                Price = 500,
                Texture = this.repository.GetVirtualPath("eggshell"),
                SpriteIndex = 0
            };
        }

        private void ApplyFishPatches(IDictionary<string, string> data)
        {
            data["EasterEgg_Degend"] = "Degend/110/sinker/40/60/600 2000/winter spring/rainy/688 .05/10/0/.1/10/false";
            data["EasterEgg_Degend_Gold"] = "Golden Degend/140/mixed/50/80/600 2000/winter/rainy/688 .01/12/0/.05/15/false";
        }
    }

    internal class FishModule
    {
        private readonly IModHelper helper;
        private readonly AssetRepository repository;
        private readonly FishingEngineExtension engine;
        private readonly Random random = new Random();

        public FishModule(IModHelper helper, AssetRepository repository, FishingEngineExtension engine)
        {
            this.helper = helper;
            this.repository = repository;
            this.engine = engine;
        }

        public void OnUpdateTicked(UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.player == null) return;

            if (Game1.player.CurrentTool is FishingRod rod && rod.isFishing)
            {
                if (Game1.player.currentLocation?.Name == "Beach" && Game1.isRaining)
                {
                    if (rod.pullingFish && !rod.hit)
                    {
                        var field = typeof(FishingRod).GetField("whichFish", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (field != null && (string?)field.GetValue(rod) == "EasterEgg_Degend")
                        {
                            if (this.random.NextDouble() < 0.15)
                            {
                                field.SetValue(rod, "EasterEgg_Degend_Gold");
                            }
                        }
                    }
                }
            }
        }
    }

    internal class EventLinker
    {
        private readonly IModHelper helper;
        private readonly FishModule fishModule;
        private readonly ContentInjector contentInjector;

        public EventLinker(IModHelper helper, FishModule fishModule, ContentInjector contentInjector)
        {
            this.helper = helper;
            this.fishModule = fishModule;
            this.contentInjector = contentInjector;
        }

        public void Subscribe()
        {
            this.helper.Events.Content.AssetRequested += (s, e) => this.contentInjector.OnAssetRequested(e);
            this.helper.Events.GameLoop.UpdateTicked += (s, e) => this.fishModule.OnUpdateTicked(e);
            this.helper.Events.Display.MenuChanged += this.OnMenuChanged;
        }

        private void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu is DialogueBox)
            {
                if (Game1.currentSpeaker?.Name == "Willy")
                {
                    var dialogue = Game1.currentSpeaker.CurrentDialogue.Peek();
                    if (dialogue != null)
                    {
                        string? text = null;
                        if (Game1.player.fishCaught.ContainsKey("(O)EasterEgg_Degend_Gold"))
                            text = "The Golden Degend... I thought it was just a fisherman's fever dream.";
                        else if (Game1.player.fishCaught.ContainsKey("(O)EasterEgg_Degend"))
                            text = "You caught a Degend? Remarkable. The tides are shifting.";

                        if (text != null)
                        {
                            var field = typeof(Dialogue).GetField("text", BindingFlags.Instance | BindingFlags.NonPublic) ?? typeof(Dialogue).GetField("_text", BindingFlags.Instance | BindingFlags.NonPublic);
                            if (field != null) field.SetValue(dialogue, text);
                        }
                    }
                }
            }
        }
    }

    internal class FishingEngineExtension
    {
        private readonly IModHelper helper;
        public FishingEngineExtension(IModHelper helper) => this.helper = helper;
    }
}
