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
        private static IMonitor Monitor;
        private static Assembly ModAssembly;

        public HarmonyResourcePatcher(string id, IMonitor monitor)
        {
            Monitor = monitor;
            ModAssembly = Assembly.GetExecutingAssembly();
            var harmony = new Harmony(id);

            try
            {
                MethodInfo original = AccessTools.Method(typeof(LocalizedContentManager), nameof(LocalizedContentManager.Load), new[] { typeof(string) }, new[] { typeof(Texture2D) });
                
                if (original == null) {
                    Monitor.Log("Harmony Error: Could not find LocalizedContentManager.Load method.", LogLevel.Error);
                    return;
                }

                harmony.Patch(
                    original: original,
                    postfix: new HarmonyMethod(typeof(HarmonyResourcePatcher), nameof(Postfix_Load))
                );
            }
            catch (Exception ex)
            {
                Monitor.Log($"Harmony patching failed: {ex.Message}", LogLevel.Error);
            }
        }

        public static void Postfix_Load<T>(ref T __result, string assetName)
        {
            if (typeof(T) == typeof(Texture2D) && assetName.Contains("Virtual/Textures/", StringComparison.OrdinalIgnoreCase))
            {
                string fileName = Path.GetFileName(assetName);
                string resourceName = $"EasterEgg.Assets.Fish.{fileName}.png";

                try
                {
                    using (Stream stream = ModAssembly.GetManifestResourceStream(resourceName))
                    {
                        if (stream != null)
                        {
                            __result = (T)(object)Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
                        }
                    }
                }
                catch { }
            }
        }
    }
}
