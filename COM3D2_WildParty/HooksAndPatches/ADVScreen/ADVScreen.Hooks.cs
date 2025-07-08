using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace COM3D2.WildParty.Plugin.HooksAndPatches.ADVScreen
{
    internal class Hooks
    {
        internal static string GUID = WildParty.GUID + ".ADVScreen";

        //Prevent the game starts the YotogiStageSelectManager when trying to init yotogi scene?
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WfScreenManager), nameof(WfScreenManager.CallScreen))]
        private static bool WfScreenManagerCallScreenPre(string call_screen_name, WfScreenManager __instance)
        {
            if (StateManager.Instance.ModEventProgress == Constant.EventProgress.YotogiInit && call_screen_name == "StageSelect")
                return false;
            return true;
        }

        //Override the original flow and use our own adv flow 
        [HarmonyPostfix]
        [HarmonyPatch(typeof(WfScreenManager), nameof(WfScreenManager.CallScreen))]
        private static void WfScreenManagerCallScreenPost(string call_screen_name, WfScreenManager __instance)
        {
            Patches.CheckStartADVScene();

        }

        //The function is coded to always returning empty tag list for unknown reason but we want it to return a proper list to make things work, at least for the time when we are trying to use them
        //It is important as without a proper list it will not be able to link the modded adv scene ending back to the normal flow
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ScriptManagerFast.KagTagSupportFast), nameof(ScriptManagerFast.KagTagSupportFast.GetTagList))]
        private static void GetTagListPost(ref Dictionary<string, string> __result)
        {
            Patches.SpoofGetTagList(ref __result);
        }

        //Remove the spoof tag list related state
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ADVKagManager), nameof(ADVKagManager.TagSceneCall))]
        private static void ADVKagManagerTagSceneCallPost(KagTagSupport tag_data, ADVKagManager __instance)
        {
            Patches.SpoofGetTagListEnd();
        }

        //Function for getting a list of maids that assigned to "new yotogi" event for the character select scene
        //Our mod scenario is flag as new yotogi. The system will have to pass through here, set the flag we want
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Schedule.ScheduleAPI), nameof(Schedule.ScheduleAPI.GetNewYotogiMaids))]
        private static void GetNewYotogiMaidsPost()
        {
            StateManager.Instance.RequireCheckModdedSceneFlag = true;
        }

        //Check if the system should enter mod scenario after the character selection
        //This CharacterSelect scene is used in many places (eg from the free mode). We need to have a proper flag set to avoid wrong execution flow
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CharacterSelectMain), nameof(CharacterSelectMain.Finish))]
        private static void FinishPost(CharacterSelectMain __instance)
        {
            Patches.CheckRequireEnterModScenario(__instance);
        }




        [HarmonyPostfix]
        [HarmonyPatch(typeof(ADVKagManager), nameof(ADVKagManager.Update))]
        private static void UpdatePost(ADVKagManager __instance)
        {
            if (__instance.skip_mode)
                Patches.HandleModADVScenarioUserInput();

            Patches.CheckWaitForFullLoadDone(__instance);
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(ADVKagManager), nameof(ADVKagManager.OnClickEvent))]
        private static void OnClickEventPost(ADVKagManager __instance)
        {
            Patches.HandleModADVScenarioUserInput();
        }

        //Force the system to reset all mod related flags and setup if the system has gone back to the main title screen (the player may have terminated the game progress during mod event)
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameMain), nameof(GameMain.LoadScene))]
        private static void LoadScenePre(string f_strSceneName)
        {
            if (f_strSceneName == Constant.CallScreenName.Title)
                Patches.ReturnToMainTitleHandling();
        }

        //For the backlog chopped voice replay
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SoundMgr), nameof(SoundMgr.PlayDummyVoice))]
        private static bool PlayDummyVoice(SoundMgr __instance, string f_strFileName, float f_fFadeTime, bool f_bStreaming, bool f_bLoop, int voice_pitch, AudioSourceMgr.Type soundType)
        {
            return Patches.CheckAudioChoppingPlayback(__instance, f_strFileName, voice_pitch, soundType);
        }

        //After Yotogi play scene the system will default set all characters to invisible. Override it depends on json setting
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BaseKagManager), nameof(BaseKagManager.TagCharaVisibleAllOff))]
        private static bool TagCharaVisibleAllOff(KagTagSupport tag_data)
        {
            return Patches.IsAllowHideCharacters();
        }

        //Record the offset for individual characters in a group in order to make the motion look more natural in ADV scene
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BaseKagManager), nameof(BaseKagManager.TagSetManOffsetPos))]
        private static void TagSetManOffsetPosPre(BaseKagManager __instance, KagTagSupport tag_data)
        {
            Patches.StartIndividualOffsetHandling(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BaseKagManager), nameof(BaseKagManager.TagSetManOffsetPos))]
        private static void TagSetManOffsetPosPost(KagTagSupport tag_data)
        {
            Patches.EndIndividualOffsetHandling();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MotionKagManager), "TagSetMaidOffsetMultiPos2")]
        private static void TagSetMaidOffsetMultiPos2Pre(MotionKagManager __instance, KagTagSupport tag_data)
        {
            Patches.StartMotionKagIndividualOffsetHandling(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MotionKagManager), "TagSetMaidOffsetMultiPos2")]
        private static void TagSetMaidOffsetMultiPos2Post(KagTagSupport tag_data)
        {
            Patches.EndIndividualOffsetHandling();
        }

        //Stop certain SE from playing depend on the scenario setting
        [HarmonyPrefix]
        [HarmonyPatch(typeof(SoundMgr), nameof(SoundMgr.PlaySe))]
        private static bool PlaySe(string f_strFileName)
        {
            return Patches.IsPlaySE(f_strFileName);
        }
    }
}
