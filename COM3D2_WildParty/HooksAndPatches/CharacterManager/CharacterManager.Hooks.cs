using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace COM3D2.WildParty.Plugin.HooksAndPatches.CharacterManager
{
    internal class Hooks
    {
        internal static string GUID = WildParty.GUID + ".CharacterManager";

        //Use the GUID stored in state instead of the man no to return the correct man object to make animation etc works
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterMgr), nameof(CharacterMgr.GetMan))]
        private static void GetManPost(int nManNo, ref Maid __result)
        {
            Patches.GetSpoofMan(nManNo, ref __result);
        }

        //Use the GUID stored in state instead of the maid no to return the correct maid object to make animation etc works
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterMgr), nameof(CharacterMgr.GetMaid), new Type[] { typeof(int) })]
        private static void GetMaidIntPost(int nMaidNo, ref Maid __result)
        {
            Patches.GetSpoofMaid(nMaidNo, ref __result);            
        }

        //Make the function to search through the StateManager array
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterMgr), nameof(CharacterMgr.GetMaid), new Type[] { typeof(string) })]
        private static void GetMaidStringPost(string guid, ref Maid __result)
        {
            Patches.GetSpoofMaidByString(guid, ref __result);
        }

        //Play or do not play the audio depend on the spoof flag
        [HarmonyPrefix]
        [HarmonyPatch(typeof(AudioSource), nameof(AudioSource.Play), new Type[] { })]
        private static bool Play(AudioSource __instance)
        {
            return !StateManager.Instance.SpoofAudioLoadPlay;
        }


        //We dont want this function to execute if the spoof flag is set
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterMgr), nameof(CharacterMgr.Deactivate))]
        private static bool Deactivate(int f_nActiveSlotNo, bool f_bMan)
        {
            return Patches.CheckSpoofActivateMaidObject();
        }

        //We dont want this function to execute if the spoof flag is set
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Maid), nameof(Maid.DutPropAll))]
        private static bool DutPropAll()
        {
            return Patches.CheckSpoofActivateMaidObject();
        }

        //We dont want this function to execute if the spoof flag is set
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Maid), nameof(Maid.AllProcPropSeqStart))]
        private static bool AllProcPropSeqStart()
        {
            return Patches.CheckSpoofActivateMaidObject();
        }

        //The system will always call the man/maid array index to retrieve the character. That part of logic is from an external dll so we cant modify that.
        //By observation it seems it will always get the maid first and then man, so we record the result and use it to return the correct man character later.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BaseKagManager), nameof(BaseKagManager.GetMaidAndMan), new Type[] { typeof(KagTagSupport), typeof(bool) })]
        private static void GetMaidAndManPost(BaseKagManager __instance, KagTagSupport tag_data, bool maid_priority, ref Maid __result)
        {
            Patches.SpoofGetManCharacters(__instance, tag_data, ref __result);
        }

        //Some of the Yotogi Skill will try to load the new body. Since we havent setup the 3.0 body properly in this mod, the man character generated lacks the pairman and will cause crashing here.
        //Since 3.0 body is not supported, force it to stop this process if the game is running a mod event.
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterMgr), nameof(CharacterMgr.SwapNewManBody))]
        private static void SwapNewManBody(int activeSlot, ref bool toNewBody)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
                toNewBody = false;
        }

    }
}
