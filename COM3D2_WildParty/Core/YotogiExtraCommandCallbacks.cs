using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.WildParty.Plugin.Core
{
    internal class YotogiExtraCommandCallbacks
    {
        internal static void ChangeMainGroupSkill_Callback(string skillID)
        {
            GameMain.Instance.MainCamera.FadeOut(ConfigurableValue.CameraFadeTime, f_dg: delegate
            {
                YotogiHandling.ChangeMainGroupSkill(skillID);

                StateManager.Instance.ExtraCommandWindow.SetVisible(false);
            });
            GameMain.Instance.MainCamera.FadeIn(ConfigurableValue.CameraFadeTime);
        }

        internal static void MassChangeGroupSkill_Callback(string skillID)
        {
            GameMain.Instance.MainCamera.FadeOut(ConfigurableValue.CameraFadeTime, f_dg: delegate
            {
                //this will only apply to group that have the same man-woman combination as the main group
                var groupZero = StateManager.Instance.PartyGroupList[0];

                var skillItem = ModUseData.ValidSkillList[groupZero.Maid1.status.personal.id][groupZero.GroupType].Where(x => x.YotogiSkillID == skillID).First();
                //change all other groups
                for (int i = 1; i < StateManager.Instance.PartyGroupList.Count; i++)
                {
                    if (StateManager.Instance.PartyGroupList[i].GroupType == groupZero.GroupType)
                    {
                        YotogiHandling.ChangeBackgroundGroupSexPosition(StateManager.Instance.PartyGroupList[i], skillItem.SexPosID, true);
                    }
                }
                YotogiHandling.ChangeMainGroupSkill(skillID);

                StateManager.Instance.ExtraCommandWindow.SetVisible(false);
            });
            GameMain.Instance.MainCamera.FadeIn(ConfigurableValue.CameraFadeTime);
        }

        internal static void ChangeFormation_Callback(string formationID)
        {
            PartyGroup.CurrentFormation = formationID;
            GameMain.Instance.MainCamera.FadeOut(ConfigurableValue.CameraFadeTime, f_dg: delegate
            {
                YotogiHandling.SetGroupFormation(formationID);

                StateManager.Instance.ExtraCommandWindow.SetVisible(false);
            });
            GameMain.Instance.MainCamera.FadeIn(ConfigurableValue.CameraFadeTime);
        }

        /*
         * formationID: Set up the party grouping accordingly if a formationID is supplied, otherwise random
         */ 
        internal static void ChangeTargetMaid_Callback(string maid_guid, string formationID = "", int initialSexPosID = -1)
        {
            GameMain.Instance.MainCamera.FadeOut(ConfigurableValue.CameraFadeTime, f_dg: delegate
            {
                //Change the maid for maid[0], keep man[0] and shuffle all the remaining
                Maid selectedMaid = StateManager.Instance.SelectedMaidsList.Where(x => x.status.guid == maid_guid).First();
                StateManager.Instance.SpoofActivateMaidObjectFlag = true;
                GameMain.Instance.CharacterMgr.SetActiveMaid(selectedMaid, 0);
                StateManager.Instance.SpoofActivateMaidObjectFlag = false;

                CharacterHandling.StopCurrentAnimation();

                if(string.IsNullOrEmpty(formationID))
                    CharacterHandling.AssignPartyGroupingRandom(true);
                else
                    CharacterHandling.AssignPartyGroupingBySetupInfo(formationID, true);
                BackgroundGroupMotionManager.InitNextReviewTimer();
                YotogiHandling.UpdateParameterView(StateManager.Instance.PartyGroupList[0].Maid1);

                //need to update the main group
                //var initialSkill = YotogiHandling.GetRandomSkill(StateManager.Instance.PartyGroupList[0].Maid1.status.personal.id, StateManager.Instance.PartyGroupList[0].GroupType);
                var initialSkill = YotogiHandling.GetSkill(StateManager.Instance.PartyGroupList[0].Maid1.status.personal.id, StateManager.Instance.PartyGroupList[0].GroupType, initialSexPosID);
                CharacterHandling.CleanseCharacterMgrArray();
                YotogiHandling.ChangeMainGroupSkill(initialSkill.YotogiSkillID);

                YotogiHandling.SetGroupToScene();



                Util.ResetAllGroupPosition();

                CameraHandling.SetCameraLookAt(StateManager.Instance.PartyGroupList[0].Maid1);

                StateManager.Instance.ExtraCommandWindow.SetVisible(false);

            });
            GameMain.Instance.MainCamera.FadeIn(ConfigurableValue.CameraFadeTime);
        }

        internal static void ChangeFormationWithNewGroup_Callback(string formationID)
        {
            PartyGroup.CurrentFormation = formationID;
            GameMain.Instance.MainCamera.FadeOut(ConfigurableValue.CameraFadeTime, f_dg: delegate
            {
                foreach (var group in StateManager.Instance.PartyGroupList)
                    group.DetachAllIK();

                CharacterHandling.AssignPartyGrouping(formationID, true);

                YotogiHandling.UpdateParameterView(StateManager.Instance.PartyGroupList[0].Maid1);

                //Core.YotogiHandling.YotogiSkillCall(StateManager.Instance.YotogiManager, ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].DefaultSexPosID);
                PlayableSkill.SkillItem skill = Util.GetMainGroupSkillIDBySexPosID(ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].DefaultSexPosID);
                YotogiHandling.ChangeMainGroupSkill(skill.YotogiSkillID);

                YotogiHandling.SetGroupToScene();
                //YotogiHandling.SetGroupFormation(formationID);

                StateManager.Instance.ExtraCommandWindow.SetVisible(false);

                GameMain.Instance.MainCamera.FadeIn(ConfigurableValue.CameraFadeTime);
            });
            
            

        }


        internal static void HaremKing_SwapMainGroupMaid(int originalMaidIndex, int workingMaidIndex, bool isMovingRight)
        {
            Maid originalMaid = StateManager.Instance.YotogiWorkingMaidList[originalMaidIndex];
            Maid selectedMaid = StateManager.Instance.YotogiWorkingMaidList[workingMaidIndex];

            PartyGroup originalMaidGroup = Util.GetPartyGroupByGUID(originalMaid.status.guid);

            PlayPostMovementMotion(isMovingRight, StateManager.Instance.PartyGroupList[0], originalMaidGroup);

            StateManager.Instance.SpoofActivateMaidObjectFlag = true;
            GameMain.Instance.CharacterMgr.SetActiveMaid(selectedMaid, 0);
            StateManager.Instance.SpoofActivateMaidObjectFlag = false;
            
            CharacterHandling.AssignPartyGrouping_SwapMember(originalMaid, selectedMaid);

            //Update the group variable after swap
            originalMaidGroup = Util.GetPartyGroupByGUID(originalMaid.status.guid);

            //Update the coordinates of the groups
            Vector3 positionOffset = Vector3.zero;
            Quaternion originalRotation = selectedMaid.transform.rotation;
            Quaternion forceRotation = selectedMaid.transform.rotation;
            MapCoorindates coorindateInfo = ModUseData.MapCoordinateList[PartyGroup.CurrentFormation];//.CoordinateList.Where(x => x.MaxGroup >= StateManager.Instance.PartyGroupList.Count).OrderBy(x => x.MaxGroup).First();
            
            if (coorindateInfo.SpecialSetting != null)
                if (coorindateInfo.SpecialSetting.MainGroupMotionOffset != null)
                {
                    positionOffset = coorindateInfo.SpecialSetting.MainGroupMotionOffset.Pos;
                    forceRotation = coorindateInfo.SpecialSetting.MainGroupMotionOffset.Rot;
                }

            MapCoorindates.CoordinateListInfo currentFormationCoorindateInfo = coorindateInfo.CoordinateList.Where(x => x.MaxGroup >= StateManager.Instance.PartyGroupList.Count).OrderBy(x => x.MaxGroup).First();
            MapCoorindates.CoordinatesInfo originalMaidGroupCoordinateInfo = currentFormationCoorindateInfo.GroupCoordinates.Where(x => x.ArrayPosition == originalMaidIndex).First();
            MapCoorindates.CoordinatesInfo selectedMaidGroupCoordinateInfo = currentFormationCoorindateInfo.GroupCoordinates.Where(x => x.ArrayPosition == workingMaidIndex).First();

            StateManager.Instance.PartyGroupList[0].SetGroupPosition(selectedMaidGroupCoordinateInfo.Pos + positionOffset, forceRotation);

            originalMaidGroup.SetGroupPosition(originalMaidGroupCoordinateInfo.Pos, originalMaidGroupCoordinateInfo.Rot);
            
            int mainGroupSexPosID = ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].DefaultSexPosID;
            int bgSexPosID = ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].BackgroundSexPosID;

            PlayableSkill.SkillItem newMainGroupSkill = Util.GetMainGroupSkillIDBySexPosID(mainGroupSexPosID);

            //original maid motion
            YotogiHandling.ChangeBackgroundGroupSexPosition(originalMaidGroup, bgSexPosID, true);
            
            YotogiHandling.UpdateParameterView(StateManager.Instance.PartyGroupList[0].Maid1);

            //need to update the main group
            CharacterHandling.CleanseCharacterMgrArray();
            YotogiHandling.ChangeMainGroupSkill(newMainGroupSkill.YotogiSkillID, false, true);
        }

        internal static void ApplySpecialSettingMovementMotion(Maid maid, MapCoorindates.ManualMovementSettingInfo.MotionAndOffset setting)
        {
            if (maid == null || setting == null)
                return;

            if (setting.Offset != null)
            {
                maid.transform.position += setting.Offset.Pos;
                maid.transform.rotation = setting.Offset.Rot;
            }

            if(setting.TweenOffset != null)
            {
                Quaternion targetRotation = setting.TweenOffset.Rot;
                Util.SmoothMoveMaidPosition(maid, maid.transform.position + setting.TweenOffset.Pos, targetRotation);
            }

            if (setting.Motion != null)
            {
                if (!string.IsNullOrEmpty(setting.Motion.ScriptFile))
                {
                    PartyGroup group = Util.GetPartyGroupByCharacter(maid);
                    string maidGUID = group.Maid1.status.guid;
                    string manGUID = "";
                    if (group.Man1 != null)
                        manGUID = group.Man1.status.guid;

                    GameMain.Instance.ScriptMgr.LoadMotionScript(0, false, setting.Motion.ScriptFile, setting.Motion.ScriptLabel, maidGUID,
                        manGUID, false, false, false, false);
                }

                if (!string.IsNullOrEmpty(setting.Motion.MotionFile) && !string.IsNullOrEmpty(setting.Motion.MotionTag))
                {
                    CharacterHandling.PlayAnimation(maid, setting.Motion.MotionFile, setting.Motion.MotionTag, setting.Motion.IsLoopMotion, setting.Motion.IsBlend);
                }
            }

        }

        internal static void PlayPreMovementMotion(bool isMovingRight, int indexOffset, PartyGroup mainMaidGroup, PartyGroup targetMaidGroup)
        {
            MapCoorindates coorindatesInfo = ModUseData.MapCoordinateList[PartyGroup.CurrentFormation];

            if (coorindatesInfo.SpecialSetting != null)
            {
                int currentMaidIndex = Util.GetIndexPositionInWorkingYotogiArrayForMaid(mainMaidGroup.Maid1);

                Vector3 positionOffset = Vector3.zero;
                Quaternion forceRotation = StateManager.Instance.YotogiWorkingMaidList[currentMaidIndex + indexOffset].transform.rotation;
                MapCoorindates coorindateInfo = ModUseData.MapCoordinateList[PartyGroup.CurrentFormation];//.CoordinateList.Where(x => x.MaxGroup >= StateManager.Instance.PartyGroupList.Count).OrderBy(x => x.MaxGroup).First();
                if (coorindateInfo.SpecialSetting != null)
                    if (coorindateInfo.SpecialSetting.MainGroupMotionOffset != null)
                    {
                        positionOffset = coorindateInfo.SpecialSetting.MainGroupMotionOffset.Pos;
                        forceRotation = coorindateInfo.SpecialSetting.MainGroupMotionOffset.Rot;
                    }

                MapCoorindates.CoordinateListInfo currentFormationCoorindateInfo = coorindateInfo.CoordinateList.Where(x => x.MaxGroup >= StateManager.Instance.PartyGroupList.Count).OrderBy(x => x.MaxGroup).First();
                MapCoorindates.CoordinatesInfo targetMaidGroupCoordinateInfo = currentFormationCoorindateInfo.GroupCoordinates.Where(x => x.ArrayPosition == currentMaidIndex + indexOffset).First();

                targetMaidGroup.SetGroupPosition(targetMaidGroupCoordinateInfo.Pos + positionOffset, forceRotation);
                
                MapCoorindates.ManualMovementSettingInfo movementSetting;

                if (isMovingRight)
                    movementSetting = coorindatesInfo.SpecialSetting.MoveRightSetting;
                else
                    movementSetting = coorindatesInfo.SpecialSetting.MoveLeftSetting;

                if (movementSetting != null)
                {
                    if (movementSetting.PreMoveMotion != null)
                    {
                        CharacterHandling.StopCurrentAnimation(mainMaidGroup);

                        ApplySpecialSettingMovementMotion(mainMaidGroup.Maid1, movementSetting.PreMoveMotion.MainGroupMaid);
                        ApplySpecialSettingMovementMotion(mainMaidGroup.Man1, movementSetting.PreMoveMotion.MainGroupMan);

                        ApplySpecialSettingMovementMotion(targetMaidGroup.Maid1, movementSetting.PreMoveMotion.TargetGroupMaid);
                        ApplySpecialSettingMovementMotion(targetMaidGroup.Man1, movementSetting.PreMoveMotion.TargetGroupMan);
                    }
                }

            }
        }

        internal static void PlayPostMovementMotion(bool isMovingRight, PartyGroup mainMaidGroup, PartyGroup originalMaidGroup)
        {
            MapCoorindates coorindatesInfo = ModUseData.MapCoordinateList[PartyGroup.CurrentFormation];

            if (coorindatesInfo.SpecialSetting != null)
            {
                MapCoorindates.ManualMovementSettingInfo movementSetting;

                if (isMovingRight)
                    movementSetting = coorindatesInfo.SpecialSetting.MoveRightSetting;
                else
                    movementSetting = coorindatesInfo.SpecialSetting.MoveLeftSetting;

                if (movementSetting != null)
                {
                    if (movementSetting.PostMoveMotion != null)
                    {
                        CharacterHandling.StopCurrentAnimation(mainMaidGroup);

                        ApplySpecialSettingMovementMotion(mainMaidGroup.Maid1, movementSetting.PostMoveMotion.MainGroupMaid);
                        ApplySpecialSettingMovementMotion(mainMaidGroup.Man1, movementSetting.PostMoveMotion.MainGroupMan);
                        
                        ApplySpecialSettingMovementMotion(originalMaidGroup.Maid1, movementSetting.PostMoveMotion.OriginalGroupMaid);
                        ApplySpecialSettingMovementMotion(originalMaidGroup.Man1, movementSetting.PostMoveMotion.OriginalGroupMan);
                    }
                }

            }
        }
    }
}
