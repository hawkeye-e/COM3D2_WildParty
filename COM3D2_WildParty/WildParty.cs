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
        public const string Version = "1.1.3";

        internal static ManualLogSource Log;

        private void Awake()
        {
            // Plugin startup logic
            Log = base.Logger;

            if (!IsGameVersionCorrect())
            {
                Log.LogInfo($"Incorrect dll version! Abandon all patching.");
                return;
            }

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

                    ModUseData.InitManifest();
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

        private static bool IsGameVersionCorrect()
        {
            string gameVersionText = GameUty.GetGameVersionText();
#if COM3D2_5
#if UNITY_2022_3
            if (gameVersionText.StartsWith("3"))
                return true;
            else
                NDebug.MessageBox("Plugin Error", "Your COM3D2 is V2.5 but the WildParty mod you are using is V2 version. Please download the V2.5 version dll instead.");
#endif
#endif

#if COM3D2
            if (gameVersionText.StartsWith("2"))
                return true;
            else
                NDebug.MessageBox("Plugin Error", "Your COM3D2 is V2 but the WildParty mod you are using is V2.5 version. Please download the V2 version dll instead.");
#endif

            return false;
        }


        
        internal static class Hooks
        {

        }
    }
}