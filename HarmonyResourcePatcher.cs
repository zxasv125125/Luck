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
    public class HarmonyResourcePatcher
    {
        private static IMonitor? Monitor;
        private static Assembly? ModAssembly;

        public HarmonyResourcePatcher(string id, IMonitor monitor)
        {
            Monitor = monitor;
            ModAssembly = Assembly.GetExecutingAssembly();
            var harmony = new Harmony(id);

            try
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(Game1), "get_xnb_content"),
                    postfix: new HarmonyMethod(typeof(HarmonyResourcePatcher), nameof(Postfix_TextureLoad))
                );
                
                Monitor.Log("Harmony patches applied successfully for internal resources.", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Harmony patching failed: {ex.Message}", LogLevel.Error);
            }
        }
        public static void Postfix_TextureLoad(ref Texture2D __result, string assetName)
        {
            if (assetName.StartsWith("Virtual/Textures/", StringComparison.OrdinalIgnoreCase))
            {
                string fileName = Path.GetFileName(assetName);
                string resourceName = $"EasterEgg.Assets.Fish.{fileName}.png";

                try
                {
                    using (Stream? stream = ModAssembly?.GetManifestResourceStream(resourceName))
                    {
                        if (stream != null)
                        {
                            __result = Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }
}

