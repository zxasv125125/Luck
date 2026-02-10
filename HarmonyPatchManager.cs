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
    public class HarmonyPatchManager
    {
        private static IMonitor Monitor;
        private static Assembly ModAssembly;

        public HarmonyPatchManager(string uniqueId, IMonitor monitor)
        {
            Monitor = monitor;
            ModAssembly = Assembly.GetExecutingAssembly();
            var harmony = new Harmony(uniqueId);

            try
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(Game1), "get_xnb_content"), 
                    postfix: new HarmonyMethod(typeof(HarmonyPatchManager), nameof(Postfix_TextureLoad))
                );
                
                Monitor.Log("Harmony patch applied to override internal asset loading.", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to apply Harmony patches: {ex.Message}", LogLevel.Error);
            }
        }

        public static void Postfix_TextureLoad(ref Texture2D __result, string assetName)
        {
            if (assetName.Contains("Virtual/Textures/", StringComparison.OrdinalIgnoreCase))
            {
                string fileName = Path.GetFileName(assetName);
                string resourceName = $"EasterEgg.Assets.Fish.{fileName}.png"; 

                try
                {
                    using (Stream stream = ModAssembly.GetManifestResourceStream(resourceName))
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

