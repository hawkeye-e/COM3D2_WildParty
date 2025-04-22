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

                        YotogiHandling.CheckYotogiMiscSetup(StateManager.Instance.PartyGroupList[i]);
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
                foreach (PartyGroup group in StateManager.Instance.PartyGroupList)
                    YotogiHandling.ResetYotogiMiscSetup(group);

                //Change the maid for maid[0], keep man[0] and shuffle all the remaining
                Maid selectedMaid = StateManager.Instance.SelectedMaidsList.Where(x => x.status.guid == maid_guid).First();
                StateManager.Instance.SpoofActivateMaidObjectFlag = true;
                
                //For the lesbian case, maid[1] is changed
                if (ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].IsLesbianSetup)
                {
                    if (selectedMaid.status.guid == StateManager.Instance.PartyGroupList[0].Maid1.status.guid)
                    {
                        //Special case: the selected maid is the main maid, randomly pick a maid and fill in the empty spot left by the main maid
                        List<Maid> tempRandom = new List<Maid>(StateManager.Instance.SelectedMaidsList);
                        tempRandom.Remove(selectedMaid);
                        int index = RNG.Random.Next(tempRandom.Count);
                        Maid replacement = tempRandom[index];
                        GameMain.Instance.CharacterMgr.SetActiveMaid(replacement, 0);
                    }

                    GameMain.Instance.CharacterMgr.SetActiveMaid(selectedMaid, 1);
                }
                else
                    GameMain.Instance.CharacterMgr.SetActiveMaid(selectedMaid, 0);
                
                StateManager.Instance.SpoofActivateMaidObjectFlag = false;
                
                CharacterHandling.StopCurrentAnimation();
                
                if (string.IsNullOrEmpty(formationID))
                {
                    if (ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].IsLesbianSetup)
                        CharacterHandling.AssignPartyGroupingRandomCaseLesbian(true);
                    else
                        CharacterHandling.AssignPartyGroupingRandom(true);
                }
                else
                    CharacterHandling.AssignPartyGroupingBySetupInfo(formationID, true);
                
                YotogiHandling.SetupYotogiSceneInitialSkill(Util.GetCurrentDefaultSexPosID());
                CharacterHandling.SetGroupZeroActive();
                
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
                StopAllQueueMovement();

                foreach (var group in StateManager.Instance.PartyGroupList)
                    group.DetachAllIK();

                CharacterHandling.AssignPartyGrouping(formationID, true);

                YotogiHandling.SetupYotogiSceneInitialSkill(Util.GetCurrentDefaultSexPosID());
                CharacterHandling.SetGroupZeroActive();

                YotogiHandling.UpdateParameterView(StateManager.Instance.PartyGroupList[0].Maid1);

                PlayableSkill.SkillItem skill = Util.GetGroupSkillIDBySexPosID(StateManager.Instance.PartyGroupList[0], Util.GetCurrentDefaultSexPosID());
                YotogiHandling.ChangeMainGroupSkill(skill.YotogiSkillID);

                YotogiHandling.SetGroupToScene();

                StateManager.Instance.ExtraCommandWindow.SetVisible(false);

                GameMain.Instance.MainCamera.FadeIn(ConfigurableValue.CameraFadeTime);
            });
            
            

        }


        internal static void HaremKing_SwapMainGroupMaid(int originalMaidIndex, int workingMaidIndex, bool isMovingRight)
        {
            Maid originalMaid = StateManager.Instance.YotogiWorkingMaidList[originalMaidIndex];
            Maid selectedMaid = StateManager.Instance.YotogiWorkingMaidList[workingMaidIndex];
            
            PartyGroup originalMaidGroup = Util.GetPartyGroupByGUID(originalMaid.status.guid);
            
            foreach (PartyGroup group in StateManager.Instance.PartyGroupList)
                group.BlockMotionScriptChange = false;

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
            
            int mainGroupSexPosID = Util.GetCurrentDefaultSexPosID();
            int bgSexPosID = ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].BackgroundSexPosID;
            
            PlayableSkill.SkillItem newMainGroupSkill = Util.GetGroupSkillIDBySexPosID(StateManager.Instance.PartyGroupList[0], mainGroupSexPosID);
            
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

                    CharacterHandling.LoadMotionScript(0, false, setting.Motion.ScriptFile, setting.Motion.ScriptLabel, maidGUID,
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

        private static void SwapPartyGroup(PartyGroup groupZero, PartyGroup targetGroup, bool IsSwapCoordinates)
        {
            //swap the yotogiworkingmaid list
            int workingIndex0 = -1;
            int workingIndexTarget = -1;
            for (int i = 0; i < StateManager.Instance.YotogiWorkingMaidList.Count; i++)
            {
                if (StateManager.Instance.YotogiWorkingMaidList[i] == groupZero.Maid1)
                    workingIndex0 = i;
                else if (StateManager.Instance.YotogiWorkingMaidList[i] == targetGroup.Maid1)
                    workingIndexTarget = i;
            }
            
            if (workingIndex0 >= 0 && workingIndexTarget >= 0)
            {
                StateManager.Instance.YotogiWorkingMaidList.Remove(groupZero.Maid1);
                StateManager.Instance.YotogiWorkingMaidList.Remove(targetGroup.Maid1);

                StateManager.Instance.YotogiWorkingMaidList.Insert(workingIndex0, targetGroup.Maid1);
                StateManager.Instance.YotogiWorkingMaidList.Insert(workingIndexTarget, groupZero.Maid1);
            }
            
            //swap the group
            int targetGroupIndex = -1;
            for (int i = 0; i < StateManager.Instance.PartyGroupList.Count; i++)
                if (StateManager.Instance.PartyGroupList[i] == targetGroup)
                {
                    targetGroupIndex = i;
                    break;
                }
            
            if (targetGroupIndex < 0)
                return;

            groupZero.IsAutomatedGroup = true;
            targetGroup.IsAutomatedGroup = false;

            StateManager.Instance.PartyGroupList.Remove(targetGroup);
            StateManager.Instance.PartyGroupList.Remove(groupZero);

            StateManager.Instance.PartyGroupList.Insert(0, targetGroup);
            StateManager.Instance.PartyGroupList.Insert(targetGroupIndex, groupZero);

            
            if (IsSwapCoordinates)
            {
                foreach (var mapCoorindateInfo in ModUseData.MapCoordinateList.Values)
                {
                    MapCoorindates.CoordinateListInfo currentFormationCoorindateInfoList = mapCoorindateInfo.CoordinateList.Where(x => x.MaxGroup >= StateManager.Instance.PartyGroupList.Count).OrderBy(x => x.MaxGroup).First();
                    MapCoorindates.CoordinatesInfo positionZeroInfo = currentFormationCoorindateInfoList.GroupCoordinates.Where(x => x.ArrayPosition == 0).First();
                    MapCoorindates.CoordinatesInfo targetPositionInfo = currentFormationCoorindateInfoList.GroupCoordinates.Where(x => x.ArrayPosition == targetGroupIndex).First();
                    
                    //swap the pos rot info in modusedata
                    targetPositionInfo.ArrayPosition = 0;
                    positionZeroInfo.ArrayPosition = targetGroupIndex;

                    //swap the extra man info in modusedata
                    List<MapCoorindates.ExtraManCoordinatesInfo> positionZeroExtraManInfoList = currentFormationCoorindateInfoList.ExtraManInfo.Where(x => x.GroupIndex == 0).ToList();
                    List<MapCoorindates.ExtraManCoordinatesInfo> targetPositionExtraManInfoList = currentFormationCoorindateInfoList.ExtraManInfo.Where(x => x.GroupIndex == targetGroupIndex).ToList();
                    
                    foreach (var info in positionZeroExtraManInfoList)
                        info.GroupIndex = targetGroupIndex;
                    foreach (var info in targetPositionExtraManInfoList)
                        info.GroupIndex = 0;
                }
            }
            

        }

        private static void StopAllQueueMovement()
        {
            HardCodeMotion.ManWalkController.StopAllMovements();
            StateManager.Instance.TimeEndTriggerList.Clear();

            foreach(var group in StateManager.Instance.PartyGroupList)
            {

                group.MovingExtraManIndexList.Clear();
                group.MovingGroupMemberList.Clear();
                
                //May have converted the bone of the man, recover all the man characters to play safe
                foreach (var kvp in group.ExtraManList)
                    Helper.BoneNameConverter.RecoverConvertedManStructure(kvp.Value);
                Helper.BoneNameConverter.RecoverConvertedManStructure(group.Man1);


                //Move queue forward to make sure position 0 is used
                if (group.ExtraManList.Count > 0)
                {
                    while (group.ExtraManList[0] == null)
                    {
                        for (int i = 1; i < group.ExtraManList.Keys.Max(x => x); i++)
                        {
                            group.ExtraManList[i - 1] = group.ExtraManList[i];
                            group.ExtraManList[i] = null;
                        }
                    }
                }
            }
        }

        //The formation etc is kept but the main group is changed to group of the selected maid
        internal static void ChangeTargetGroup_Callback(string maid_guid, bool isManSwap, bool IsSwapCoordinates)
        {
            GameMain.Instance.MainCamera.FadeOut(ConfigurableValue.CameraFadeTime, f_dg: delegate
            {
                StopAllQueueMovement();

                //swap the group
                Maid selectedMaid = StateManager.Instance.SelectedMaidsList.Where(x => x.status.guid == maid_guid).First();
                PartyGroup targetGroup = Util.GetPartyGroupByCharacter(selectedMaid);
                PartyGroup originalGroup = StateManager.Instance.PartyGroupList[0];
                
                targetGroup.StopNextReviewTime();
                CharacterHandling.StopCurrentAnimation(targetGroup);
                CharacterHandling.StopCurrentAnimation(originalGroup);
                
                if (isManSwap)
                    CharacterHandling.AssignPartyGrouping_SwapMember(originalGroup.Man1, targetGroup.Man1);
                
                SwapPartyGroup(originalGroup, targetGroup, IsSwapCoordinates);
                
                //the sexposid of the target group may not be a valid playable skill, update it if necessary
                if (!ModUseData.ValidSkillList[targetGroup.Maid1.status.personal.id][targetGroup.GroupType].Any(x => x.SexPosID == targetGroup.SexPosID))
                {
                    var newSkill = YotogiHandling.GetSkill(targetGroup.Maid1.status.personal.id, targetGroup.GroupType);
                    targetGroup.SexPosID = newSkill.SexPosID;
                }
                

                //make change to the character array in the KISS system
                CharacterHandling.SetGroupZeroActive();
                //Prepare the change of yotogi skill for the scene
                
                int initialSexPosID = targetGroup.SexPosID;
                YotogiHandling.SetupYotogiSceneInitialSkill(initialSexPosID);
                
                BackgroundGroupMotionManager.InitNextReviewTimer();
                YotogiHandling.UpdateParameterView(StateManager.Instance.PartyGroupList[0].Maid1);
                
                //need to update the main group
                //var initialSkill = YotogiHandling.GetRandomSkill(StateManager.Instance.PartyGroupList[0].Maid1.status.personal.id, StateManager.Instance.PartyGroupList[0].GroupType);
                var initialSkill = YotogiHandling.GetSkill(StateManager.Instance.PartyGroupList[0].Maid1.status.personal.id, StateManager.Instance.PartyGroupList[0].GroupType, initialSexPosID);
                CharacterHandling.CleanseCharacterMgrArray();
                YotogiHandling.ChangeMainGroupSkill(initialSkill.YotogiSkillID);
                
                YotogiHandling.SetGroupToScene();
                
                Util.ResetAllGroupPosition();
                
                StateManager.Instance.ExtraCommandWindow.SetVisible(false);
            });
            GameMain.Instance.MainCamera.FadeIn(ConfigurableValue.CameraFadeTime);
        }
    }
}
