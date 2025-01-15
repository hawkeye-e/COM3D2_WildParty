using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace COM3D2.WildParty.Plugin.HooksAndPatches.DebugUse
{
    internal class Hooks
    {
        internal static string GUID = WildParty.GUID + ".DebugUse";

        //To capture all the voice file names and texts and put it in memory
        [HarmonyPrefix]
        [HarmonyPatch(typeof(MessageWindowMgr), nameof(MessageWindowMgr.SetText))]
        private static void MessageWindowMgrSetTextPre(string name, string text, string voice_file, int voice_pitch, AudioSourceMgr.Type sound_type)
        {
            Patches.CaptureSpokenDialoguesInfo(name, text, voice_file);
        }

        //To capture the voice file names and texts for private touch event
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PrivateMaidTouchManager), "PlayVoice")]
        private static void OnTouchPoint(PrivateMaidTouchManager.VoiceData voiceData)
        {
            Patches.CapturePrivateTouchEventDialogues(voiceData);
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(ADVKagManager), nameof(ADVKagManager.Update))]
        private static void UpdatePost(ADVKagManager __instance)
        {
            Patches.ProcessAudioFileChopping();
            Patches.PrintCameraInfo();
            Patches.PrintAllCapturedDialoguesAndTexts();
            Patches.PrintCharacterSetup();

        }

        //for logging down the motion file and label
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ScriptManager), nameof(ScriptManager.LoadMotionScript))]
        private static void LoadMotionScriptPre(int sloat, bool is_next, string file_name, string label_name, string maid_guid, string man_guid, bool face_fix, bool valid_pos, bool disable_diff_pos, bool body_mix_ok)
        {
            Patches.CaptureMotionFileNames(file_name, label_name);
        }

        //log down the corresponding face anime for the motion
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Maid), nameof(Maid.FaceAnime))]
        private static void FaceAnimePre(string tag, float t, int chkcode)
        {
            Patches.CaptureMotionFaceInfo(tag);
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(AudioSourceMgr), nameof(AudioSourceMgr.LoadPlay))]
        private static void LoadPlayPre(AudioSourceMgr __instance, string f_strFileName, float f_fFadeTime, bool f_bStreaming, bool f_bLoop)
        {
            Patches.CaptureMotionVoiceFile(__instance, f_strFileName, f_bLoop);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(YotogiPlayManager), nameof(YotogiPlayManager.NextSkill))]
        private static void NextSkillPost(YotogiPlayManager __instance)
        {
            Patches.CaptureSkillName(__instance);
        }

        /*
         * Numpad 1: Reset mind to -100
         *        2: Clear MotionSoundDictionary
         *        3: Print all Face info
         *        4: Print Group position and rotation in json format
         *        0: Print all MotionSoundDictionary data
         */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(YotogiManager), nameof(YotogiManager.Update))]
        private static void YotogiManagerUpdatePost(ADVKagManager __instance)
        {
            Patches.YotogiPlayGroupArrangement();
            Patches.CaptureMotionSoundInfo();            
        }
    }
}
