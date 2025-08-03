using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace COM3D2.WildParty.Plugin.HooksAndPatches.YotogiScreen
{
    internal class Hooks
    {
        internal static string GUID = WildParty.GUID + ".YotogiScreen";

        //Replace the internal array with our full list so that the undressing button applies to all maids
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UndressingManager), nameof(UndressingManager.OnClickButton))]
        private static void UndressingManagerOnClickButtonPre(UndressingManager __instance, Maid[] __state)
        {
            __state = Patches.SpoofUndressingMaidArray(__instance);
        }

        //Recover the maid array
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UndressingManager), nameof(UndressingManager.OnClickButton))]
        private static void UndressingManagerOnClickButtonPost(UndressingManager __instance, Maid[] __state)
        {
            Patches.RecoverUndressingMaidArray(__instance, __state);
        }

        //Apply the same logic to all selected maids
        [HarmonyPostfix]
        [HarmonyPatch(typeof(YotogiPlayManager), nameof(YotogiPlayManager.OnClickReleaseSeieki))]
        private static void OnClickReleaseSeiekiPost(YotogiPlayManager __instance)
        {
            Patches.RemoveAllSemen();
        }

        //Do not display the skill icon in the yotogi result screen if it is set to no exp gain
        [HarmonyPrefix]
        [HarmonyPatch(typeof(YotogiResultManager), nameof(YotogiResultManager.AddSkillUnit))]
        private static bool AddSkillUnitPre(Yotogi.SkillDataPair skill_data_pair)
        {
            return Patches.CanEarnExperience(StateManager.Instance.UndergoingModEventID);
        }




        [HarmonyPrefix]
        [HarmonyPatch(typeof(YotogiPlayManager), nameof(YotogiPlayManager.OnClickNext))]
        private static void YotogiPlayManagerOnClickNextPre()
        {
            Patches.DestroyExtraCommandWindow();
            Patches.HandleYotogiPlayEnd();
        }


        //The class is inherited by UIButton. Use it to detect if the player hover on the injected yotogi command buttons.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIButtonColor), "OnHover")]
        private static void UIButtonColorOnHoverPost(UIButtonColor __instance, bool isOver)
        {
            Patches.DisplayExecConditionPanel(__instance, isOver);
        }


        /// Try to inject our own set of base commands in the yotogi scene if necessary
        [HarmonyPostfix]
        [HarmonyPatch(typeof(YotogiPlayManager), nameof(YotogiPlayManager.UpdateCommand))]
        private static void UpdateCommandPost()
        {
            Patches.InitExtraCommandButtons();
        }

        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(YotogiCommandFactory), nameof(YotogiCommandFactory.AddCommand))]
        private static bool YotogiCommandFactoryAddCommandPre(Yotogis.Skill.Data.Command.Data command_data, string __state)
        {
            //Backup and update the TJSScript Setting
            __state = Patches.OverrideCommandTJSScriptSetting(command_data);

            //Check if this command should be hidden based on the override setting of the mod
            return Patches.IsEnableCommand(command_data);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(YotogiCommandFactory), nameof(YotogiCommandFactory.AddCommand))]
        private static void YotogiCommandFactoryAddCommandPost(YotogiCommandFactory __instance, Yotogis.Skill.Data.Command.Data command_data, string __state)
        {
            //Restore the TJSScript
            if (!string.IsNullOrEmpty(__state))
                Traverse.Create(command_data.basic).Field("request_tjsscript").SetValue(__state);

            //For easy access the yotogi command factory object, we have to remember the instance here
            StateManager.Instance.YotogiCommandFactory = __instance;
        }


        //Init the yotogi scene according to the scenario progress
        [HarmonyPostfix]
        [HarmonyPatch(typeof(YotogiManager), nameof(YotogiManager.Start))]
        private static void YotogiManagerStartPost(YotogiManager __instance)
        {
            Patches.InitForYotogiScene(__instance);
        }

        //Determine if need to apply the unlimited mind effect
        [HarmonyPostfix]
        [HarmonyPatch(typeof(YotogiPlayManager), nameof(YotogiPlayManager.ApplyExecCommandStatus))]
        private static void ApplyExecCommandStatus(Maid maid, Yotogis.Skill.Data.Command.Data cm)
        {
            Patches.ApplyUnlimitedMind(maid);
        }

        //Take appropiate action depends on the state of the next button
        [HarmonyPrefix]
        [HarmonyPatch(typeof(WfScreenChildren), nameof(WfScreenChildren.Finish))]
        private static void YotogiNullManagerFinishPost(WfScreenChildren __instance)
        {
            Patches.ProcessYotogiPlayEnd(__instance);
            if (StateManager.Instance.IsFinalYotogi)
            {
                Patches.ProcessYotogiResultEnd(__instance);
            }
            else
            {
                Patches.LoadADVSceneAfterYotogi(__instance);
            }
        }

        //Try to look at the first maid whenever fade in occur during yotogi play
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CameraMain), nameof(CameraMain.FadeIn))]
        private static void CameraMainFadeInPost(float f_fTime, bool f_bUIEnable, CameraMain.dgOnCompleteFade f_dg, bool f_bSkipable, bool f_bColorIsSameOut, Color f_color)
        {
            Patches.SetCameraFocusOnDefaultMaid();
        }

        //This function may alter the camera set up after we have set the camera we want, patch to override it again
        [HarmonyPostfix]
        [HarmonyPatch(typeof(CameraMain), nameof(CameraMain.SetFromScriptOnTarget))]
        private static void SetFromScriptOnTargetPost()
        {
            Patches.SetCameraFocusOnDefaultMaid();
        }

        //Allow player to switch different yotogi skills more than 7 times by always resetting the skill array
        [HarmonyPostfix]
        [HarmonyPatch(typeof(YotogiManager), nameof(YotogiManager.AddPlaySkill))]
        private static void AddPlaySkillPost(YotogiManager __instance, Yotogis.Skill.Data skillData)
        {
            Patches.ResetYotogiPlaySkillArray(__instance, skillData);
        }

        //The system will try to force it to change the bg when displaying the yotogi result. No idea why but prevent it from doing so anyway
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BaseKagManager), nameof(BaseKagManager.TagSetBg))]
        private static bool TagSetBgPre(KagTagSupport tag_data)
        {
            return Patches.IsAllowChangeBackground();
        }

        //Connect the game to jump to the correct part based on the situation
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BaseKagManager), nameof(BaseKagManager.JumpLabel))]
        private static bool JumpLabelPre(string label_name, BaseKagManager __instance)
        {
            //Jump back to adv part if the stage is selected
            bool isContinue = Patches.CheckJumpLabelStageSelected(label_name);
            if (!isContinue)
                return isContinue;

            //Jump back to normal flow if it is showing the final result
            isContinue = Patches.CheckJumpLabelSessionResultEnd(__instance, label_name);

            return isContinue;
        }

        //Only Display the stages that allowed for selection based on json setting
        [HarmonyPrefix]
        [HarmonyPatch(typeof(YotogiStageUnit), nameof(YotogiStageUnit.SetStageData))]
        private static void SetStageDataPost(YotogiStageSelectManager.StageExpansionPack stagePack, ref bool enabled, bool isDaytime)
        {
            Patches.FilterStageSelection(stagePack, ref enabled);
        }

        //The original system use its own array position to locate the maid or man game object and assign motion or related stuff. We need to spoof to redirect it to assign to a different game object for those background groups.
        [HarmonyPrefix]
        [HarmonyPatch(typeof(ScriptManager), nameof(ScriptManager.LoadMotionScript))]
        private static bool LoadMotionScriptPre(int sloat, bool is_next, string file_name, string label_name, string maid_guid, string man_guid, bool face_fix, bool valid_pos, bool disable_diff_pos)
        {
            Patches.UpdateMotionScriptDataForGroup(maid_guid, file_name, label_name);
            if (Patches.CheckBlockLoadMotionScript(maid_guid))
                return false;
            Patches.StartSpoofingLoadMotionScript(label_name, maid_guid, man_guid);

            Patches.ResetGroupIK();

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ScriptManager), nameof(ScriptManager.LoadMotionScript))]
        private static void LoadMotionScriptPost(ScriptManager __instance, int sloat, bool is_next, string file_name, string label_name, string maid_guid, string man_guid, bool face_fix, bool valid_pos, bool disable_diff_pos)
        {
            Patches.ApplyIKRectify(__instance, file_name, label_name);

            Patches.SetupDelayOrgasmMotion(label_name);
            Patches.RandomizeMaidConvertedManFaceAnime();

            Patches.EndSpoofingLoadMotionScript();
            //When this function is called the system will always try to set the main group to its default position of the map which is not what we want. Apply the position setting again.
            if (StateManager.Instance.UndergoingModEventID > 0 && StateManager.Instance.ModEventProgress == Constant.EventProgress.YotogiPlay)
                if (!Patches.CheckBlockLoadMotionScript(maid_guid))
                    Util.ResetAllGroupPosition();
        }

        //Patch to hide the extra command window when the main command window is sliding out of screen
        [HarmonyPostfix]
        [HarmonyPatch(typeof(iTween), nameof(iTween.MoveTo), new Type[] { typeof(GameObject), typeof(System.Collections.Hashtable) })]
        private static void iTweenMoveToPost(GameObject target, System.Collections.Hashtable args)
        {
            //Locate the component of the main command window. We dont want to use the reflection everytime this function is called so put it in memory.
            Patches.LocateCommandMaskGroupTransform();

            Patches.CheckHideExtraCommandWindow(target, args);
        }

        //Update the name displayed in the skill icon to make it more fit to the situation
        [HarmonyPrefix]
        [HarmonyPatch(typeof(YotogiSkillNameHoverPlate), nameof(YotogiSkillNameHoverPlate.SetName))]
        private static void YotogiSkillNameHoverPlateSetNamePre(ref string skillName, string skillNameTerm, bool isExpLock)
        {
            Patches.SetDesiredSkillName(ref skillName);
        }

        //Remember the game object of the command category. Would use it later to update the command name to make it more fit to the situation
        [HarmonyPostfix]
        [HarmonyPatch(typeof(wf.Utility), nameof(wf.Utility.SetLocalizeTerm), new Type[] { typeof(Component), typeof(string), typeof(bool) })]
        private static void UtilitySetLocalizeTermPost(Component obj, ref string term, bool forceApply)
        {
            if (obj.name == Constant.DefinedGameObjectNames.YotogiCommandCategoryTitle)   
                StateManager.Instance.CommandLabel = (UILabel)obj;
        }

        //Update the yotogi skill name displayed above all the commands to make it more fit to the situation
        [HarmonyPostfix]
        [HarmonyPatch(typeof(YotogiCommandFactory), nameof(YotogiCommandFactory.ReserveSkillCommand))]
        private static void ReserveSkillCommandPost(Yotogis.Skill.Data.Command skill_command)
        {
            Patches.SetDesiredSkillNameForCommandCategory();
        }

        //Some of the yotogi skills included may not have appropiate dialogues (eg harem skill in orgy party, the maids will always talk about the master but our situation are for customers),
        //replace it so that it doesnt feel too weird
        [HarmonyPrefix]
        [HarmonyPatch(typeof(AudioSourceMgr), nameof(AudioSourceMgr.LoadPlay))]
        private static void LoadPlayPre(AudioSourceMgr __instance, ref string f_strFileName, float f_fFadeTime, bool f_bStreaming, ref bool f_bLoop)
        {
            Patches.ReplaceNotSuitableVoice(__instance, ref f_strFileName, ref f_bLoop);
            Patches.CheckVoiceloopTrigger(__instance, f_bLoop);
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(Maid), nameof(Maid.SetPos))]
        private static void MaidSetPosPost(Maid __instance, Vector3 f_vecLocalPos)
        {
            //The system will calculate a offset vector for the motion of a character in a blackbox,
            //we have to capture it to apply it to the characters in the ADV scene in order to make the motion looks more natural
            Patches.SetOffsetVectorForGroup(__instance, f_vecLocalPos);

            //This is trying to force the group zero to stay in our defined position instead of jumping to the default one defined by KISS when clicking command button
            //We have to avoid using SetPos and SetRot in YotogiPlay due to we have modified the behaviour here
            Patches.ForceGroupPosition();
        }

        //This is trying to force the group zero to stay in our defined position instead of jumping to the default one defined by KISS when clicking command button
        //We have to avoid using SetPos and SetRot in YotogiPlay due to we have modified the behaviour here
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Maid), nameof(Maid.SetRot))]
        private static void MaidSetRotPost(Maid __instance, Vector3 f_vecLocalRot)
        {
            Patches.ForceGroupPosition();
        }

        //This original code by KISS will cause some groups floating. Patch it to fix it to zero vector to aviod the problem. We only use maid.transform.position to process the coordinate to reduce complication.
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterMgr), nameof(CharacterMgr.CharaAllOfsetPos))]
        private static void CharaAllOfsetPosPre(ref Vector3 f_vecLocalPos)
        {
            Patches.FixGroupOffsetVector(ref f_vecLocalPos);
        }

        //We should have our own definition of rotation for each group and do not need the default one
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CharacterMgr), nameof(CharacterMgr.CharaAllOfsetRot))]
        private static void CharaAllOfsetRotPre(ref Vector3 f_vecLocalRot)
        {
            if (StateManager.Instance.UndergoingModEventID > 0 && StateManager.Instance.ModEventProgress == Constant.EventProgress.YotogiPlay)
                f_vecLocalRot = Vector3.zero;
        }

        //Log the current clip name for each group playing
        [HarmonyPrefix]
        [HarmonyPatch(typeof(TBody), nameof(TBody.LoadAnime), new Type[] { typeof(string), typeof(AFileSystemBase), typeof(string), typeof(bool), typeof(bool) })]
        private static void LoadAnime1Pre(TBody __instance, string tag, AFileSystemBase fileSystem, string filename, bool additive, bool loop)
        {
            Patches.StoreAnimationClipNameForGroup(__instance, tag);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TBody), nameof(TBody.LoadAnime), new Type[] { typeof(string), typeof(AFileSystemBase), typeof(string), typeof(bool), typeof(bool) })]
        private static void LoadAnime1Post(TBody __instance, string tag, AFileSystemBase fileSystem, string filename, bool additive, bool loop)
        {
            Patches.ApplyForceSetting(__instance.maid);
        }

        

        //This is the function called whenever the player click the orgasm command. Record the men info and update the count.
        //In order not to make things overcomplicated, when this function is called we simply count all the man in the whole group orgasm once even the motion may not be the case.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(YotogiPlayManager), "PlayNormalClimax")]
        private static void PlayNormalClimaxPost(Yotogis.Skill.Data.Command.Data command_data, bool lockRRUpdate)
        {
            Patches.AddOrgasmCountForGroup();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(YotogiPlayManager), nameof(YotogiPlayManager.Awake))]
        private static void YotogiPlayManagerAwakePost(YotogiPlayManager __instance)
        {
            //Hide the position changer as it is not suit the need of the mod event
            Patches.HideDefaultPositionChangeButton(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(YotogiManager), nameof(YotogiManager.Update))]
        private static void YotogiManagerUpdatePost(ADVKagManager __instance)
        {
            //Check Review
            Core.BackgroundGroupMotionManager.CheckReviewForEachGroup(__instance, StateManager.Instance.PartyGroupList);
            Core.YotogiHandling.CheckTimeEndTrigger();
        }

        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(YotogiPlayManager), nameof(YotogiPlayManager.NextSkill))]
        private static void NextSkillPost(YotogiPlayManager __instance)
        {
            //In the NextSkill method, the maid in index 1 could be set to not visible if the main group changes from FFM to MF etc. Need to set it back
            Patches.ResetMaidVisibility();
        }

        //Prevent the Yotogi scene change its background if the spoof flag is on
        [HarmonyPrefix]
        [HarmonyPatch(typeof(YotogiStageSelectManager.StageExpansionPack), nameof(YotogiStageSelectManager.StageExpansionPack.ChangeBG))]
        private static bool ChangeBG(Maid maid)
        {
            return !StateManager.Instance.SpoofChangeBackgroundFlag;
        }

        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(YotogiPlayManager), "OnClickCommand")]
        private static void OnClickCommandPre(Yotogis.Skill.Data.Command.Data command_data)
        {
            //Record down the type of command clicked
            Patches.RecordCommandTypeClicked(command_data);
            //Update progress info
            Patches.UpdateYotogiProgressInfoCommandClick(command_data);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(YotogiPlayManager), "OnClickCommand")]
        private static void OnClickCommandPost(Yotogis.Skill.Data.Command.Data command_data)
        {
            //Apply linked group motion if any when player click a command button
            Patches.ApplyLinkedGroupMotionUponCommandClicked(command_data);
            //Force Man Change if it is required in the scenario
            Patches.ForceChangeManUponCommandClicked(command_data);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Maid), "Update")]
        private static void MaidUpdatePre(Maid __instance)
        {
            Patches.SpoofSexFlagForMaidUpdate(__instance);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Maid), "Update")]
        private static void MaidUpdatePost(Maid __instance)
        {
            Patches.EndSpoofSexFlagForMaidUpdate(__instance);

            Patches.CheckMaidAnimationTrigger(__instance);
            Patches.CheckAnimationChangeTrigger(__instance);
            Core.YotogiHandling.CheckManWalkTrigger(__instance);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(YotogiPlayManager), "OnClickViweReset")]
        private static bool YotogiPlayManagerOnClickViewResetPost()
        {
            //Try to fix the camera to zoom to the main maid instead of some weird locations.
            return Patches.HandleCameraReset();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BgMgr), nameof(BgMgr.DelPrefabFromBgAll))]
        private static void DelPrefabFromBgAllPre(BgMgr __instance, out List<KeyValuePair<string, GameObject>> __state)
        {
            __state = new List<KeyValuePair<string, GameObject>>();
            Patches.BackupModAddedExtraObjects(__instance, __state);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BgMgr), nameof(BgMgr.DelPrefabFromBgAll))]
        private static void DelPrefabFromBgAllPost(BgMgr __instance, List<KeyValuePair<string, GameObject>> __state)
        {
            Patches.RestoreModAddedExtraObjects(__instance, __state);

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(YotogiManager), nameof(YotogiManager.ResetWorld))]
        private static void ResetWorldPre()
        {
            Patches.PrepareIgnoreResetPropList();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(YotogiManager), nameof(YotogiManager.ResetWorld))]
        private static void ResetWorldPost()
        {
            //In the yotogi scene, the clothes will be reset at the beginning. Apply the clothing setting if it is defined in the json file.
            Patches.ApplyClothesSetting();
            Patches.CleanUpIgnoreResetPropList();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Maid), nameof(Maid.ResetProp), new Type[] { typeof(string), typeof(bool) })]
        private static bool ResetProp(Maid __instance, string mpn, bool force_reset)
        {
            return Patches.IsResetProp(__instance);

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BaseKagManager), nameof(BaseKagManager.TagTexMulAdd))]
        private static bool TagTexMulAdd(BaseKagManager __instance, KagTagSupport tag_data)
        {
            return Patches.IsAddTexture(__instance, tag_data);
        }

#if COM3D2
        //The V2 version does not set the maid and man in the MotionKagManager for the main group during LoadMotionScript. We have to set it so that we can identify the main group when the system trying to get the maid / man.
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BaseKagManager), nameof(BaseKagManager.LoadScriptFile))]
        private static void LoadScriptFile(BaseKagManager __instance, string file_name, string label_name)
        {
            Patches.SetMainGroupMaidAndManInfoInMotionKagManager(__instance);
        }
#endif

        [HarmonyPostfix]
        [HarmonyPatch(typeof(YotogiCommandFactory), "GetCommandName")]
        private static void YotogiCommandFactoryGetCommandNamePost(Yotogis.Skill.Data.Command.Data.Basic commandDataBasic, ref string __result)
        {
            __result = Patches.GetOverrideCommandName(commandDataBasic, __result);
        }

    }
}
