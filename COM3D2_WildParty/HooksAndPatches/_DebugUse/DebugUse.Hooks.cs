using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;

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

        /* B: Clothing; X: Coordinates; Z: dialogue files; C: Camera; V: Apply hardcoded motion 
        */
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ADVKagManager), nameof(ADVKagManager.Update))]
        private static void UpdatePost(ADVKagManager __instance)
        {
            Patches.ProcessAudioFileChopping();
            Patches.PrintCameraInfo();
            Patches.PrintAllCapturedDialoguesAndTexts();
            Patches.PrintCharacterSetup();
            Patches.ApplyAnimationInStudio();
        }

        //for logging down the motion file and label
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ScriptManager), nameof(ScriptManager.LoadMotionScript))]
        private static void LoadMotionScriptPre(int sloat, bool is_next, string file_name, string label_name, string maid_guid, string man_guid, bool face_fix, bool valid_pos, bool disable_diff_pos)
        {
            //WildParty.Log.LogInfo("LoadMotionScriptPre: file_name: " + file_name + ", label: " + label_name);
            Patches.CaptureMotionFileNames(file_name, label_name);
            if (DebugHelper.DebugState.Instance.ScriptInfoCapture.ContainsKey(file_name))
            {
                if (!DebugHelper.DebugState.Instance.ScriptInfoCapture[file_name].Contains(label_name))
                    DebugHelper.DebugState.Instance.ScriptInfoCapture[file_name].Add(label_name);
            }
            else
            {
                List<string> list = new List<string>();
                list.Add(label_name);
                DebugHelper.DebugState.Instance.ScriptInfoCapture.Add(file_name, list);
            }
            
        }

        //Log the current clip name for each group playing
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TBody), nameof(TBody.LoadAnime), new Type[] { typeof(string), typeof(AFileSystemBase), typeof(string), typeof(bool), typeof(bool) })]
        private static void LoadAnime(TBody __instance, string tag, AFileSystemBase fileSystem, string filename, bool additive, bool loop)
        {
            //WildParty.Log.LogInfo("TBody.LoadAnime: " + __instance?.maid?.status?.fullNameJpStyle + ", filename: " + filename + ", tag: " + tag);
        }

        //log down the corresponding face anime for the motion
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Maid), nameof(Maid.FaceAnime))]
        private static void FaceAnimePre(Maid __instance, string tag, float t, int chkcode)
        {
            //WildParty.Log.LogInfo("FaceAnimePre: " + __instance.status.fullNameJpStyle + ", tag: " + tag);
            Patches.CaptureMotionFaceInfo(tag);
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(AudioSourceMgr), nameof(AudioSourceMgr.LoadPlay))]
        private static void LoadPlayPre(AudioSourceMgr __instance, string f_strFileName, float f_fFadeTime, bool f_bStreaming, bool f_bLoop)
        {
            //WildParty.Log.LogInfo("LoadPlayPre f_strFileName: " + f_strFileName + ", f_bLoop: " + f_bLoop);
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
        private static void YotogiManagerUpdatePost(YotogiManager __instance)
        {
            Patches.YotogiPlayGroupArrangement();
            Patches.CaptureMotionSoundInfo();            
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BgMgr), nameof(BgMgr.ChangeBg))]
        private static void ChangeBg(string f_strPrefubName)
        {
            WildParty.Log.LogInfo("BgMgr.ChangeBg f_strPrefubName: " + f_strPrefubName);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(SoundMgr), nameof(SoundMgr.PlaySe))]
        private static void PlaySe(string f_strFileName, bool f_bLoop)
        {
            //WildParty.Log.LogInfo("SoundMgr.PlaySe f_strFileName: " + f_strFileName + ", f_bLoop:" + f_bLoop);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BgMgr), nameof(BgMgr.AddPrefabToBg))]
        private static void AddPrefabToBg(string f_strSrc, string f_strName, string f_strDest, Vector3 f_vPos, Vector3 f_vRot)
        {
            //WildParty.Log.LogInfo("AddPrefabToBg f_strSrc: " + f_strSrc
            //    + ", f_strName: " + f_strName
            //    + ", f_strDest: " + f_strDest
            //    );

        }

        //For getting prefab name in studio mode
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PhotoBGObjectData), nameof(PhotoBGObjectData.Instantiate))]
        private static void PhotoBGObjectDataInstantiate(PhotoBGObjectData __instance, string name)
        {
            //WildParty.Log.LogInfo("PhotoBGObjectData.Instantiate name: " + name
            //    + ", prefab: " + __instance.create_prefab_name
            //    + ", assetbundle: " + __instance.create_asset_bundle_name
            //    );
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MessageWindowMgr), nameof(MessageWindowMgr.CreateSelectButtons))]
        private static void CreateSelectButtons(List<KeyValuePair<string, KeyValuePair<string, bool>>> dicSelectButton, Action<string, string> onClickCallBack)
        {
            foreach (var kvp in dicSelectButton)
            {
                if (!DebugHelper.DebugState.Instance.SelectOptions.Contains(kvp.Key))
                    DebugHelper.DebugState.Instance.SelectOptions.Add(kvp.Key);
                WildParty.Log.LogInfo(kvp.Key);
            }
        }
    }
}
