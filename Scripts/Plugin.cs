// ---------------------------------------
// An example of a starting point for a Script that is added to the mod


using System;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Bindito.Core;
using TimberApi.ConsoleSystem;
using TimberApi.DependencyContainerSystem;
using TimberApi.ModSystem;

using System.Collections.Generic;
using System.Linq;
using System.Text;

using HarmonyLib;
using UnityEngine;


namespace Battery.WoodWorks
{
    public class Plugin : IModEntrypoint 
    {
        public const string PluginGuid = "battery.WoodWorks";
        public const string PluginName = "WoodWorks";
        public const string PluginVersion = "0.0.1";
        public static IConsoleWriter Log;

        public void Entry(IMod mod, IConsoleWriter consoleWriter)
        {
            Log = consoleWriter;
            Plugin.Log.LogInfo($"Loaded {PluginName} version {PluginVersion}");
            new Harmony(PluginGuid).PatchAll();
        }
    }

    [HarmonyPatch(typeof(Debug), "LogWarning", typeof(object))]
    public class LogWarningPatch
    {
        static bool Prefix(object message, bool __runOriginal)
        {
            if (__runOriginal)
            {
                string mess = message as string;
            }
            return __runOriginal;
        }
    }
    
    static class ExtensionMethods
    {
        /// <summary>
        /// Rounds Vector3 to a number of decimal places.
        /// </summary>
        /// <param name="vector3"></param>
        /// <param name="decimalPlaces"></param>
        /// <returns></returns>
        public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
        {
            float multiplier = 1;
            for (int i = 0; i < decimalPlaces; i++)
            {
                multiplier *= 10f;
            }
            return new Vector3(
                Mathf.Round(vector3.x * multiplier) / multiplier,
                Mathf.Round(vector3.y * multiplier) / multiplier,
                Mathf.Round(vector3.z * multiplier) / multiplier
            );
        }
    }
}