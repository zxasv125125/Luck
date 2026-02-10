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
using StardewValley.GameData.Objects;;

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
                MethodInfo original = AccessTools.Method(typeof(LocalizedContentManager), nameof(LocalizedContentManager.Load), new[] { typeof(string) });
                
                if (original == null)
                {
                    Monitor.Log("Harmony Error: Could not find LocalizedContentManager.Load", LogLevel.Error);
                    return;
                }

                harmony.Patch(
                    original: original,
                    postfix: new HarmonyMethod(typeof(HarmonyPatchManager), nameof(Postfix_TextureLoad))
                );
                
                Monitor.Log("Harmony successfully patched LocalizedContentManager for virtual textures.", LogLevel.Debug);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to apply Harmony patches: {ex.Message}", LogLevel.Error);
            }
        }
        public static void Postfix_TextureLoad<T>(ref T __result, string assetName)
        {
            if (typeof(T) == typeof(Texture2D) && assetName.Contains("Virtual/Textures/", StringComparison.OrdinalIgnoreCase))
            {
                string fileName = Path.GetFileName(assetName);
                string[] resources = ModAssembly.GetManifestResourceNames();
                string target = resources.FirstOrDefault(r => r.EndsWith($"{fileName}.png", StringComparison.OrdinalIgnoreCase));

                if (target == null) return;

                try
                {
                    using (Stream stream = ModAssembly.GetManifestResourceStream(target))
                    {
                        if (stream != null)
                        {
                            __result = (T)(object)Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
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
