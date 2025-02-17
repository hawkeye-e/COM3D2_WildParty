using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace COM3D2.WildParty.Plugin.HooksAndPatches.DailyScreen
{
    internal class Hooks
    {
        internal static string GUID = WildParty.GUID + ".DailyScreen";

        //This function will run everytime the daily screen shown, so need proper flag handling to avoid duplicate setup
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DailyMgr), nameof(DailyMgr.Init))]
        private static void DailyMgrInitPost(DailyMgr __instance)
        {
            Patches.PrepareExtraCommandWindow();
            Patches.InjectScheduleOptions();
        }

        //Load the modded icon in the schedule window
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ImportCM), nameof(ImportCM.TryCreateTexture))]
        private static void TryCreateTexturePost(string f_strFileName, ref Texture2D __result)
        {
            Texture2D icon = Patches.LoadModScenarioIcon(f_strFileName);
            if (icon != null)
                __result = icon;
        }

        [HarmonyFinalizer]
        [HarmonyPatch(typeof(ImportCM), nameof(ImportCM.CreateTexture), new Type[] { typeof(string) })]
        private static Exception CatchCreateTextureErrors(string f_strFileName, Exception __exception, ref Texture2D __result)
        {
            if (__exception != null)
            {
                Texture2D icon = Patches.LoadModScenarioIcon(f_strFileName);
                if (icon != null)
                {
                    __result = icon;
                    return null;
                }
                else
                {
                    //nothing to do with modding, let the exception thrown
                    return __exception;
                }
            }
            return null;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(FileSystemWindows), nameof(FileSystemWindows.IsExistentFile))]
        private static void FileSystemWindows_IsExistentFilePost(string file_name, ref bool __result)
        {
            Patches.SpoofFileExistence(file_name, ref __result);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(FileSystemArchive), nameof(FileSystemArchive.IsExistentFile))]
        private static void FileSystemArchive_IsExistentFilePost(string file_name, ref bool __result)
        {
            Patches.SpoofFileExistence(file_name, ref __result);
        }

#if COM3D2_5
#if UNITY_2022_3
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ExportCMUtility.FileSystemKCESExport), nameof(ExportCMUtility.FileSystemKCESExport.IsExistentFile))]
        private static void ExportCMUtility_FileSystem_KCESExportIsExistentFilePost(string internalName, ref bool __result)
        {
            Patches.SpoofFileExistence(internalName, ref __result);
        }
#endif
#endif

        //Patch the result to entertain some special flag checking defined by the mod (call initiated from ScheduleAPI.EnableNightWork)
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MaidStatus.Status), nameof(MaidStatus.Status.GetFlag))]
        private static void GetFlag(string flagName, ref int __result, MaidStatus.Status __instance)
        {
            Patches.GetModDefinedFlagResult(flagName, ref __result, __instance);
        }


        //Make sure the schedule fulfill all the requirement before the panel is closed
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ScheduleMgr), nameof(ScheduleMgr.CheckSelectedDataBeforeClosePanel))]
        private static bool CheckSelectedDataBeforeClosePanelPre()
        {
            return Patches.CheckScheduleTaskValid();
        }

        //To show a warning icon in the task schedule if the maid count doesnt meet the requirement
        [HarmonyPostfix]
        [HarmonyPatch(typeof(OnHoverTaskIcon), "Update")]
        private static void OnHoverTaskIconUpdate(OnHoverTaskIcon __instance)
        {
            Patches.ApplyCrossMarkIfInvalid(__instance);
        }
    }
}
