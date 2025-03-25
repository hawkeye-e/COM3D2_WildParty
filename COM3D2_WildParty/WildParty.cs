using UnityEngine;
using BepInEx;
//using BepInEx.Unity.Mono;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;



namespace COM3D2.WildParty.Plugin
{

    [BepInPlugin(GUID, PluginName, Version)]
    public class WildParty : BaseUnityPlugin
    {
        public const string PluginName = "WildParty";
        public const string GUID = "COM3D2.WildParty.Plugin";
        public const string Version = "0.3.0";

        internal static ManualLogSource Log;

        private void Awake()
        {
            // Plugin startup logic
            Log = base.Logger;
            Log.LogInfo($"Plugin {GUID} is loaded!");

            Plugin.Config.Init(this);

            if (Plugin.Config.Enabled)
            {
                try
                {
                    StateManager.Instance = new StateManager();
                    Harmony.CreateAndPatchAll(typeof(Hooks), GUID);

                    Harmony.CreateAndPatchAll(typeof(HooksAndPatches.ADVScreen.Hooks), HooksAndPatches.ADVScreen.Hooks.GUID);
                    Harmony.CreateAndPatchAll(typeof(HooksAndPatches.CharacterManager.Hooks), HooksAndPatches.CharacterManager.Hooks.GUID);
                    Harmony.CreateAndPatchAll(typeof(HooksAndPatches.DailyScreen.Hooks), HooksAndPatches.DailyScreen.Hooks.GUID);
                    Harmony.CreateAndPatchAll(typeof(HooksAndPatches.Init.Hooks), HooksAndPatches.Init.Hooks.GUID);
                    Harmony.CreateAndPatchAll(typeof(HooksAndPatches.YotogiScreen.Hooks), HooksAndPatches.YotogiScreen.Hooks.GUID);

                    ModUseData.Init();

                    if (Plugin.Config.DeveloperMode)
                    {
                        Harmony.CreateAndPatchAll(typeof(HooksAndPatches.DebugUse.Hooks), HooksAndPatches.DebugUse.Hooks.GUID);
                        DebugHelper.DebugState.Instance = new DebugHelper.DebugState();
                    }

                }
                catch (Exception ex)
                {
                    Log.LogInfo(ex.StackTrace);
                }
            }

        }


        
        internal static class Hooks
        {

        }
    }
}