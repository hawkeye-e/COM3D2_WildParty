using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using UnityEngine;
using HarmonyLib;
using COM3D2.WildParty.Plugin.Trigger;

namespace COM3D2.WildParty.Plugin.Core
{
    internal class CommandChainedActionManager
    {
        private static Dictionary<string, ProcessingDetail> ProcessingDict = new Dictionary<string, ProcessingDetail>();

        private class ProcessingDetail
        {
            internal string ChainedActionCode;
            internal string CurrentStepID;
            internal Maid ProcessTargetMaid;
            internal Dictionary<string, object> Parameters;
            internal EventDelegate OnFinishExecute = null;
        }

        internal static void ProcessChainedMotion(string chainedActionCode, Dictionary<string, object> parameters, EventDelegate onFinishExecute = null)
        {
            if (!ModUseData.CommandChainedActionList.ContainsKey(chainedActionCode))
                return;

            ProcessingDetail processingDetail = new ProcessingDetail();
            processingDetail.ChainedActionCode = chainedActionCode;
            processingDetail.CurrentStepID = ModUseData.CommandChainedActionList[chainedActionCode].FirstStep;
            processingDetail.Parameters = parameters;
            processingDetail.OnFinishExecute = onFinishExecute;

            string guid = Guid.NewGuid().ToString();
            ProcessingDict.Add(guid, processingDetail);

            ProcessChainedMotionStep(guid);
        }

        private static void ProcessChainedMotionStep(string guid)
        {
            if (!ProcessingDict.ContainsKey(guid))
                return;

            ProcessingDetail chainedActionInfo = ProcessingDict[guid];

            CommandChainedAction.StepDetail stepInfo = ModUseData.CommandChainedActionList[chainedActionInfo.ChainedActionCode].Steps.Where(x => x.StepID == chainedActionInfo.CurrentStepID).FirstOrDefault();
            if (stepInfo == null)
            {
                ChaninedActionEnd(guid, chainedActionInfo.OnFinishExecute);
                return;
            }

            switch (stepInfo.ActionType)
            {
                case CommandChainedAction.StepActionType.ResetMotionToWaiting:
                    ProcessChainedMotion_ResetMotionToWaiting(guid, stepInfo);
                    break;
                case CommandChainedAction.StepActionType.LoadMotion:
                    ProcessChainedMotion_LoadMotion(guid, stepInfo);
                    break;
                case CommandChainedAction.StepActionType.ResetIK:
                    ProcessChainedMotion_ResetIK(guid, stepInfo);
                    break;
                case CommandChainedAction.StepActionType.GetNextExtraMan:
                    ProcessChainedMotion_GetNextExtraMan(guid, stepInfo);
                    break;
                case CommandChainedAction.StepActionType.PositionOffset:
                    ProcessChainedMotion_PositionOffset(guid, stepInfo);
                    break;
                case CommandChainedAction.StepActionType.SetPosition:
                    ProcessChainedMotion_SetPosition(guid, stepInfo);
                    break;
                case CommandChainedAction.StepActionType.Trigger:
                    ProcessChainedMotion_Trigger(guid, stepInfo);
                    break;
                case CommandChainedAction.StepActionType.SetMaidToExtraMan:
                    ProcessChainedMotion_SetMaidToExtraMan(guid, stepInfo);
                    break;
                case CommandChainedAction.StepActionType.ManWalk:
                    ProcessChainedMotion_ManWalk(guid, stepInfo);
                    break;
                case CommandChainedAction.StepActionType.SetGroupMember:
                    ProcessChainedMotion_SetGroupMember(guid, stepInfo);
                    break;
                case CommandChainedAction.StepActionType.RemoveExtraMan:
                    ProcessChainedMotion_RemoveExtraMan(guid, stepInfo);
                    break;
                case CommandChainedAction.StepActionType.UpdateSexPos:
                    ProcessChainedMotion_UpdateSexPos(guid, stepInfo);
                    break;
                case CommandChainedAction.StepActionType.LoadMotionLabelGroup:
                    ProcessChainedMotion_LoadMotionLabelGroup(guid, stepInfo);
                    break;
                case CommandChainedAction.StepActionType.SwapGroupMember:
                    ProcessChainedMotion_SwapGroupMember(guid, stepInfo);
                    break;
            }
        }

        private static void ProcessChainedMotion_ResetMotionToWaiting(string guid, CommandChainedAction.StepDetail stepInfo)
        {
            var target = GetTarget(guid, stepInfo.Target);
            if (target is PartyGroup)
            {
                PartyGroup group = (PartyGroup)target;
                
                MotionSpecialLabel spLabel = YotogiHandling.ChangeBackgroundGroupMotionWithSpecificLabel(group, SexState.StateType.Waiting);
                CharacterHandling.SetCharacterVoiceEntry(group.Maid1, PersonalityVoice.VoiceEntryType.Waiting, group.ExcitementLevel, spLabel.VoiceType1, group.IsEstrus, false);
                CharacterHandling.SetCharacterVoiceEntry(group.Maid2, PersonalityVoice.VoiceEntryType.Waiting, group.ExcitementLevel, spLabel.VoiceType2, group.IsEstrus, false);
            }

            AdvanceToNextActionStep(guid, stepInfo);
        }

        private static void ProcessChainedMotion_LoadMotion(string guid, CommandChainedAction.StepDetail stepInfo)
        {
            var target = GetTarget(guid, stepInfo.Target);
            Maid targetMaid = null;
            if (target is Maid)
            {
                targetMaid = (Maid)target;
                
                MotionInfo motionInfo = new MotionInfo(stepInfo.MotionInfo);
                if (!string.IsNullOrEmpty(stepInfo.MotionInfo.ParamMotion)) 
                {
                    string fileName = ProcessingDict[guid].Parameters[stepInfo.MotionInfo.ParamMotion].ToString();
                    
                    if (Helper.CustomAnimLoader.IsAnimFileNameCustom(fileName))
                    {
                        motionInfo.CustomMotionFile = fileName;
                        motionInfo.MotionTag = fileName.ToLower();
                    }
                    else
                    {
                        motionInfo.MotionFile = fileName;
                        motionInfo.MotionTag = fileName.ToLower();
                    }
                }
                                    
                CharacterHandling.ApplyMotionInfoToCharacter(targetMaid, motionInfo);
            }

            AdvanceToNextActionStep(guid, stepInfo, targetMaid);
        }

        private static void ProcessChainedMotion_ResetIK(string guid, CommandChainedAction.StepDetail stepInfo)
        {
            var target = GetTarget(guid, stepInfo.Target);
            Maid targetMaid = null;
            if (target is Maid)
            {
                targetMaid = (Maid)target;
#if COM3D2_5
#if UNITY_2022_3
                targetMaid.body0.fullBodyIK.AllIKDetach();
#endif
#endif

#if COM3D2
                targetMaid.AllIKDetach();
#endif
            }else if(target is PartyGroup)
            {
                ((PartyGroup)target).DetachAllIK();
            }

            AdvanceToNextActionStep(guid, stepInfo, targetMaid);
        }

        private static void ProcessChainedMotion_GetNextExtraMan(string guid, CommandChainedAction.StepDetail stepInfo)
        {
            if (stepInfo.ExtraManInfo.Type == CommandChainedAction.ExtraManType.Group)
            {
                var target = GetTarget(guid, stepInfo.Target);
                if (target is PartyGroup)
                {
                    PartyGroup group = (PartyGroup)target;
                    if (stepInfo.ExtraManInfo.GetNextMethod == CommandChainedAction.ExtraManInfo.NextMethodType.Increment)
                    {
                        bool isFound = false;
                        while (!isFound)
                        {
                            group.CurrentExtraManIndex++;
                            if (!group.ExtraManList.ContainsKey(group.CurrentExtraManIndex))
                                group.CurrentExtraManIndex = 0;

                            if (group.ExtraManList[group.CurrentExtraManIndex] != null)
                                isFound = true;
                        }
                    }
                    else if(stepInfo.ExtraManInfo.GetNextMethod == CommandChainedAction.ExtraManInfo.NextMethodType.Random)
                    {
                        List<int> keyList = group.ExtraManList.Keys.ToList();
                        int rndIndex = RNG.Random.Next(keyList.Count);
                        group.CurrentExtraManIndex = keyList[rndIndex];
                    }

                    
                    Maid targetMaid = group.ExtraManList[group.CurrentExtraManIndex];
                    ProcessingDict[guid].ProcessTargetMaid = targetMaid;
                }
            }else if (stepInfo.ExtraManInfo.Type == CommandChainedAction.ExtraManType.Global)
            {
                //Not currently in use
            }

            AdvanceToNextActionStep(guid, stepInfo);
        }

        private static void ProcessChainedMotion_PositionOffset(string guid, CommandChainedAction.StepDetail stepInfo)
        {
            var target = GetTarget(guid, stepInfo.Target);
            if (target is Maid)
            {
                Maid targetMaid = (Maid)target;
                Vector3 positionOffset = Vector3.zero;
                Vector3 rotationOffset = Vector3.zero;
                
                if (!string.IsNullOrEmpty(stepInfo.OffsetInfo.Param))
                {
                    string rawData = ProcessingDict[guid].Parameters[stepInfo.OffsetInfo.Param].ToString();
                    Util.ParseRawOffsetString(rawData, out positionOffset, out rotationOffset);
                }else if (stepInfo.OffsetInfo.Offset != null)
                {
                    positionOffset = stepInfo.OffsetInfo.Offset.Pos;
                    rotationOffset = stepInfo.OffsetInfo.Offset.Rot;
                }
                
                Quaternion newRotation = targetMaid.transform.rotation * Quaternion.Euler(Vector3.up * rotationOffset.y);
                
                if (stepInfo.OffsetInfo.IsSmoothAnimation)
                {

                    Util.SmoothMoveMaidPosition(targetMaid, targetMaid.transform.position + positionOffset, newRotation);
                }
                else {
                    targetMaid.transform.position += positionOffset;
                    targetMaid.transform.rotation = newRotation;
                }
            }

            AdvanceToNextActionStep(guid, stepInfo);
        }

        private static void ProcessChainedMotion_SetPosition(string guid, CommandChainedAction.StepDetail stepInfo)
        {
            var target = GetTarget(guid, stepInfo.Target);
            if (target is Maid)
            {
                Maid targetMaid = (Maid)target;
                if (stepInfo.PositionInfo != null)
                {
                    Vector3 newPosition = targetMaid.transform.position;
                    Quaternion newRotation = targetMaid.transform.rotation;

                    if (!string.IsNullOrEmpty(stepInfo.PositionInfo.DefinedPointParamName))
                    {
                        string pointName = ProcessingDict[guid].Parameters[stepInfo.PositionInfo.DefinedPointParamName].ToString();
                        MapCoorindates.SpecialDefinedPoint point = ModUseData.MapCoordinateList[PartyGroup.CurrentFormation].SpecialDefinedPoints.Where(x => x.Name == pointName).First();

                        newPosition = point.Location.Pos;
                        newRotation = point.Location.Rot;
                    }
                    if (!string.IsNullOrEmpty(stepInfo.PositionInfo.DefinedPointName))
                    {
                        MapCoorindates.SpecialDefinedPoint point = ModUseData.MapCoordinateList[PartyGroup.CurrentFormation].SpecialDefinedPoints.Where(x => x.Name == stepInfo.PositionInfo.DefinedPointName).First();
                        
                        newPosition = point.Location.Pos;
                        newRotation = point.Location.Rot;

                    }
                    else if (stepInfo.PositionInfo.Target != null)
                    {
                        var destinationTarget = GetTarget(guid, stepInfo.PositionInfo.Target);

                        if (destinationTarget is Maid)
                        {
                            newPosition = ((Maid)destinationTarget).transform.position;
                            newRotation = ((Maid)destinationTarget).transform.rotation;
                        }else if(destinationTarget is PartyGroup)
                        {
                            PartyGroup group = (PartyGroup)destinationTarget;
                            newPosition = group.Maid1.transform.position;
                            newRotation = group.Maid1.transform.rotation;
                        }
                    }

                    //Apply the change
                    if (stepInfo.PositionInfo.IsSmoothAnimation)
                        Util.SmoothMoveMaidPosition(targetMaid, newPosition, newRotation);
                    else
                    {
                        Util.StopSmoothMove(targetMaid);
                        targetMaid.transform.position = newPosition;
                        targetMaid.transform.rotation = newRotation;
                    }
                }
                    
            }

            AdvanceToNextActionStep(guid, stepInfo);
        }

        private static void ProcessChainedMotion_Trigger(string guid, CommandChainedAction.StepDetail stepInfo)
        {
            
            if(stepInfo.TriggerSetting.TriggerType == CommandChainedAction.NextStepTriggerType.TimeEnd)
            {
                float timeToWait = 0;
                switch (stepInfo.TriggerSetting.TimeToWaitType)
                {
                    case CommandChainedAction.TriggerSettingInfo.TimeEndTriggerWaitType.DefaultAnimationBlendTime:
                        timeToWait = ConfigurableValue.AnimationBlendTime;
                        break;
                    case CommandChainedAction.TriggerSettingInfo.TimeEndTriggerWaitType.DefaultTweenMoveTime:
                        timeToWait = Constant.DefaultTweenMoveTime;
                        break;
                    case CommandChainedAction.TriggerSettingInfo.TimeEndTriggerWaitType.Custom:
                        timeToWait = stepInfo.TriggerSetting.CustomTimeToWait;
                        break;
                }

                TimeEndTrigger trigger = new TimeEndTrigger();
                trigger.DueTime = DateTime.Now.AddMilliseconds(timeToWait * 1000);
                trigger.ToBeExecuted = new EventDelegate(() => AdvanceToNextActionStep(guid, stepInfo));
                StateManager.Instance.TimeEndTriggerList.Add(trigger);
            }

        }

        private static void ProcessChainedMotion_SetMaidToExtraMan(string guid, CommandChainedAction.StepDetail stepInfo)
        {
            var target = GetTarget(guid, stepInfo.Target);
            if (target is Maid)
            {
                Maid targetMaid = (Maid)target;
                if (stepInfo.SetExtraManInfo != null)
                {
                    Dictionary<int, Maid> extraManList = null;
                    List<MapCoorindates.CoordinatesInfo> extraManSetupInfo = null;


                    if (!string.IsNullOrEmpty(stepInfo.SetExtraManInfo.GroupParamName))
                    {
                        PartyGroup group = (PartyGroup)ProcessingDict[guid].Parameters[stepInfo.SetExtraManInfo.GroupParamName];

                        extraManList = group.ExtraManList;
                        extraManSetupInfo = group.ExtraManSetupInfo;
                    }
                    else
                    {
                        if (stepInfo.SetExtraManInfo.Type == CommandChainedAction.ExtraManType.Group)
                        {
                            extraManList = StateManager.Instance.PartyGroupList[stepInfo.SetExtraManInfo.TargetGroupPosition].ExtraManList;
                            extraManSetupInfo = StateManager.Instance.PartyGroupList[stepInfo.SetExtraManInfo.TargetGroupPosition].ExtraManSetupInfo;
                        }
                        else
                        {
                            extraManList = PartyGroup.SharedExtraManList;
                            extraManSetupInfo = PartyGroup.SharedExtraManSetupInfo;
                        }
                    }

                    //find the empty spots
                    var emptyPositionList = extraManList.Where(x => x.Value == null).ToList();
                    
                    int indexToAdd = -1;
                    if (stepInfo.SetExtraManInfo.LocateEmptyMethod == CommandChainedAction.SetExtraManInfo.WayToLocateEmpty.FirstEmpty)
                        indexToAdd = emptyPositionList.OrderBy(x => x.Key).First().Key;
                    else if (stepInfo.SetExtraManInfo.LocateEmptyMethod == CommandChainedAction.SetExtraManInfo.WayToLocateEmpty.LastEmpty)
                        indexToAdd = emptyPositionList.OrderByDescending(x => x.Key).First().Key;
                    else if (stepInfo.SetExtraManInfo.LocateEmptyMethod == CommandChainedAction.SetExtraManInfo.WayToLocateEmpty.Random)
                    {
                        int rndIndex = RNG.Random.Next(emptyPositionList.Count);
                        indexToAdd = emptyPositionList[rndIndex].Key;
                    }
                    
                    extraManList[indexToAdd] = targetMaid;

                    //apply the position change and animation change
                    if (stepInfo.SetExtraManInfo.IsSmoothPositionChange)
                        Util.SmoothMoveMaidPosition(targetMaid, extraManSetupInfo[indexToAdd].Pos, extraManSetupInfo[indexToAdd].Rot);
                    else
                    {
                        Util.StopSmoothMove(targetMaid);
                        targetMaid.transform.position = extraManSetupInfo[indexToAdd].Pos;
                        targetMaid.transform.rotation = extraManSetupInfo[indexToAdd].Rot;
                    }
                    
                    CharacterHandling.ApplyMotionInfoToCharacter(targetMaid, extraManSetupInfo[indexToAdd].Motion);
                }

            }

            AdvanceToNextActionStep(guid, stepInfo);
        }

        private static void ProcessChainedMotion_RemoveExtraMan(string guid, CommandChainedAction.StepDetail stepInfo)
        {
            var target = GetTarget(guid, stepInfo.Target);
            if (target is Maid)
            {
                Maid targetMaid = (Maid)target;

                Dictionary<int, Maid> extraManList = null;
                if (!string.IsNullOrEmpty(stepInfo.RemoveExtraManInfo.GroupParamName))
                {
                    PartyGroup group = (PartyGroup)ProcessingDict[guid].Parameters[stepInfo.RemoveExtraManInfo.GroupParamName];
                    
                    extraManList = group.ExtraManList;
                }
                else
                {
                    if (stepInfo.RemoveExtraManInfo.Type == CommandChainedAction.ExtraManType.Group)
                        extraManList = StateManager.Instance.PartyGroupList[stepInfo.RemoveExtraManInfo.TargetGroupPosition].ExtraManList;
                    else if (stepInfo.RemoveExtraManInfo.Type == CommandChainedAction.ExtraManType.Global)
                        extraManList = PartyGroup.SharedExtraManList;
                }
                
                List<int> toBeRemoved = extraManList.Where(x => x.Value == targetMaid).Select(x => x.Key).ToList();
                foreach (int key in toBeRemoved)
                    extraManList[key] = null;
            }
            AdvanceToNextActionStep(guid, stepInfo);
        }

        private static void ProcessChainedMotion_ManWalk(string guid, CommandChainedAction.StepDetail stepInfo)
        {
            var target = GetTarget(guid, stepInfo.Target);
            if (target is Maid)
            {
                Maid manToMove = (Maid)target;

                var destinationTarget = GetTarget(guid, stepInfo.ManWalkSetupInfo.Destination);

                Vector3 maidMotionOffset = stepInfo.ManWalkSetupInfo.MaidMotionOffset;

                HardCodeMotion.ManWalkController.StandingMotionType standingMotionType = new HardCodeMotion.ManWalkController.StandingMotionType();
                standingMotionType.AnimationFileName = stepInfo.ManWalkSetupInfo.StandingAnimationFile;
                standingMotionType.StandingMotionOffset = stepInfo.ManWalkSetupInfo.StandingMotionOffset;
                standingMotionType.WalkingMotionOffset = stepInfo.ManWalkSetupInfo.WalkingMotionOffset;
                standingMotionType.RotationOffset = stepInfo.ManWalkSetupInfo.RotationOffset;

                if (!string.IsNullOrEmpty(stepInfo.ManWalkSetupInfo.SetupParam))
                {
                    string setUpName = ProcessingDict[guid].Parameters[stepInfo.ManWalkSetupInfo.SetupParam].ToString();
                    HardCodeMotionSetup motionSetupInfo = ModUseData.HardCodeMotionSetupList[setUpName];

                    maidMotionOffset = motionSetupInfo.ManWalkSetting.MaidMotionOffset;
                    standingMotionType.AnimationFileName = motionSetupInfo.ManWalkSetting.StandingAnimationFile;
                    standingMotionType.StandingMotionOffset = motionSetupInfo.ManWalkSetting.StandingMotionOffset;
                    standingMotionType.WalkingMotionOffset = motionSetupInfo.ManWalkSetting.WalkingMotionOffset;
                    standingMotionType.RotationOffset = motionSetupInfo.ManWalkSetting.RotationOffset;
                }

                Vector3 destinationVector = Vector3.zero;
                if (destinationTarget is Maid)
                    destinationVector = ((Maid)destinationTarget).transform.position;
                else if (destinationTarget is PartyGroup)
                    destinationVector = ((PartyGroup)destinationTarget).GroupPosition;

                float angle = manToMove.transform.rotation.eulerAngles.y;
                Vector3 maidMotionOffsetRespectToRotation = Quaternion.AngleAxis(angle, Vector3.up) * maidMotionOffset;
                float distance = Vector3.Distance(manToMove.transform.position, destinationVector + maidMotionOffsetRespectToRotation);

                
                HardCodeMotion.ManWalkController.MoveForward(manToMove, distance, standingMotionType, new EventDelegate(() => AdvanceToNextActionStep(guid, stepInfo)));
            }
        }

        private static void ProcessChainedMotion_SetGroupMember(string guid, CommandChainedAction.StepDetail stepInfo)
        {
            
            PartyGroup group;

            if (!string.IsNullOrEmpty(stepInfo.SetGroupMemberInfo.TargetParam))
            {
                group = (PartyGroup)ProcessingDict[guid].Parameters[stepInfo.SetGroupMemberInfo.TargetParam];
            }
            else
            {
                int groupIndex = stepInfo.SetGroupMemberInfo.TargetGroupPosition;
                group = StateManager.Instance.PartyGroupList[groupIndex];
            }

            ProcessSetGroupMemberRequest(guid, group, true, 0, stepInfo.SetGroupMemberInfo.Man1);
            ProcessSetGroupMemberRequest(guid, group, true, 1, stepInfo.SetGroupMemberInfo.Man2);
            ProcessSetGroupMemberRequest(guid, group, true, 2, stepInfo.SetGroupMemberInfo.Man3);
            ProcessSetGroupMemberRequest(guid, group, false, 0, stepInfo.SetGroupMemberInfo.Maid1);
            ProcessSetGroupMemberRequest(guid, group, false, 1, stepInfo.SetGroupMemberInfo.Maid2);
    
            if (stepInfo.SetGroupMemberInfo.NewSexPos != null)
            {
                group.SexPosID = Convert.ToInt32(GetValueData(guid, stepInfo.SetGroupMemberInfo.NewSexPos));


                if (group == StateManager.Instance.PartyGroupList[0])
                {
                    //need update some info if it is main group
                    CharacterHandling.SetGroupZeroActive();

                    YotogiHandling.UpdateParameterView(group.Maid1);

                    //need to update the main group
                    var initialSkill = YotogiHandling.GetSkill(group.Maid1.status.personal.id, group.GroupType, group.SexPosID);
                    CharacterHandling.CleanseCharacterMgrArray();
                    YotogiHandling.ChangeMainGroupSkill(initialSkill.YotogiSkillID); 

                    if (stepInfo.SetGroupMemberInfo.NewSexPos != null)
                    {
                        Traverse.Create(StateManager.Instance.YotogiManager.play_mgr).Field(Constant.DefinedClassFieldNames.YotogiPlayManagerPlayerState).SetValue(stepInfo.SetGroupMemberInfo.PlayerState);
                    }
                }
            }
            else if (stepInfo.SetGroupMemberInfo.IsFinalizedGroupUpdateStep)
            {
                if (group == StateManager.Instance.PartyGroupList[0])
                {
                    //need update some info if it is main group
                    CharacterHandling.SetGroupZeroActive();

                    YotogiHandling.UpdateParameterView(group.Maid1);

                    CharacterHandling.CleanseCharacterMgrArray();
                }

            }


            AdvanceToNextActionStep(guid, stepInfo);
        }

        private static void ProcessSetGroupMemberRequest(string guid, PartyGroup group, bool isMan, int positionIndex, CommandChainedAction.TargetInfo targetInfo)
        {
            if (targetInfo == null)
                return;

            var target = GetTarget(guid, targetInfo);

            Maid maidToSet = null;
            if (target is Maid)
                maidToSet = (Maid)target;

            
            if (isMan)
                group.SetManAtIndex(positionIndex, maidToSet);
            else
                group.SetMaidAtIndex(positionIndex, maidToSet);
        }

        private static void ProcessChainedMotion_UpdateSexPos(string guid, CommandChainedAction.StepDetail stepInfo)
        {
            var target = GetTarget(guid, stepInfo.Target);
            if (target is PartyGroup)
            {
                PartyGroup group = (PartyGroup)target;
                
                int newSexPos = Convert.ToInt32(GetValueData(guid, stepInfo.UpdateSexPosInfo.Value));
                if (group == StateManager.Instance.PartyGroupList[0]) {
                    var skillItem = Util.GetGroupSkillIDBySexPosID(group, newSexPos);

                    YotogiHandling.ChangeMainGroupSkill(skillItem.YotogiSkillID);

                    bool isSmoothAnimation = stepInfo.UpdateSexPosInfo.IsSmoothAnimation;
                    if (!string.IsNullOrEmpty(stepInfo.UpdateSexPosInfo.IsSmoothAnimationParamName))
                        if (ProcessingDict[guid].Parameters.ContainsKey(stepInfo.UpdateSexPosInfo.IsSmoothAnimationParamName))
                            isSmoothAnimation = Convert.ToBoolean(ProcessingDict[guid].Parameters[stepInfo.UpdateSexPosInfo.IsSmoothAnimationParamName]);

                    //if (!stepInfo.UpdateSexPosInfo.IsSmoothAnimation)
                    if(!isSmoothAnimation)
                        group.ReloadAnimation(false);

                    Traverse.Create(StateManager.Instance.YotogiManager.play_mgr).Field(Constant.DefinedClassFieldNames.YotogiPlayManagerPlayerState).SetValue(stepInfo.UpdateSexPosInfo.PlayerState);
                }
                else
                {
                    //implement this if applied to background group too
                }
            }

            AdvanceToNextActionStep(guid, stepInfo);
        }

        private static void ProcessChainedMotion_LoadMotionLabelGroup(string guid, CommandChainedAction.StepDetail stepInfo)
        {
            var target = GetTarget(guid, stepInfo.Target);
            if (target is PartyGroup)
            {
                PartyGroup group = (PartyGroup)target;

                bool isSmoothAnimation = stepInfo.LoadMotionLabelGroupInfo.IsSmoothAnimation;
                if (!string.IsNullOrEmpty(stepInfo.LoadMotionLabelGroupInfo.IsSmoothAnimationParamName))
                    if (ProcessingDict[guid].Parameters.ContainsKey(stepInfo.LoadMotionLabelGroupInfo.IsSmoothAnimationParamName))
                        isSmoothAnimation = Convert.ToBoolean(ProcessingDict[guid].Parameters[stepInfo.LoadMotionLabelGroupInfo.IsSmoothAnimationParamName]);


                int labelGroupID = Convert.ToInt32(GetValueData(guid, stepInfo.LoadMotionLabelGroupInfo.Value));
                group.CurrentLabelGroupID = labelGroupID;
                YotogiHandling.ChangeBackgroundGroupSexPosition(group, group.SexPosID, false, isSmoothAnimation: isSmoothAnimation);
            }

            AdvanceToNextActionStep(guid, stepInfo);
        }

        private static void ProcessChainedMotion_SwapGroupMember(string guid, CommandChainedAction.StepDetail stepInfo)
        {
            var target = GetTarget(guid, stepInfo.Target);
            if (target is PartyGroup)
            {
                PartyGroup group = (PartyGroup)target;

                var groupMember1 = GetTarget(guid, stepInfo.SwapGroupMemberInfo.Target1);
                var groupMember2 = GetTarget(guid, stepInfo.SwapGroupMemberInfo.Target2);

                Maid maid1 = (Maid)groupMember1;
                Maid maid2 = (Maid)groupMember2;

                CharacterHandling.AssignPartyGrouping_SwapMember(maid1, maid2);
            }

            AdvanceToNextActionStep(guid, stepInfo);
        }



        private static object GetTarget(string guid, CommandChainedAction.TargetInfo targetInfo)
        {
            if (targetInfo.Type == CommandChainedAction.TargetType.Param)
            {
                if(targetInfo.ParamGroupMember != CommandChainedAction.TargetInfo.SubParamType.None)
                {
                    PartyGroup group = (PartyGroup)ProcessingDict[guid].Parameters[targetInfo.ParamName];
                    if (targetInfo.ParamGroupMember == CommandChainedAction.TargetInfo.SubParamType.Man1)
                        return group.Man1;
                    else if (targetInfo.ParamGroupMember == CommandChainedAction.TargetInfo.SubParamType.Man2)
                        return group.Man2;
                    else if (targetInfo.ParamGroupMember == CommandChainedAction.TargetInfo.SubParamType.Man3)
                        return group.Man3;
                    else if (targetInfo.ParamGroupMember == CommandChainedAction.TargetInfo.SubParamType.Maid1)
                        return group.Maid1;
                    else if (targetInfo.ParamGroupMember == CommandChainedAction.TargetInfo.SubParamType.Maid2)
                        return group.Maid2;
                    else
                        return null;
                }
                else
                    return ProcessingDict[guid].Parameters[targetInfo.ParamName];
            }
            else if (targetInfo.Type == CommandChainedAction.TargetType.Group)
                return StateManager.Instance.PartyGroupList[targetInfo.TargetGroupPosition];
            else if (targetInfo.Type == CommandChainedAction.TargetType.GroupMaid)
                return StateManager.Instance.PartyGroupList[targetInfo.TargetGroupPosition].GetMaidAtIndex(targetInfo.TargetMaidPosition);
            else if (targetInfo.Type == CommandChainedAction.TargetType.GroupMan)
                return StateManager.Instance.PartyGroupList[targetInfo.TargetGroupPosition].GetManAtIndex(targetInfo.TargetMaidPosition);
            else if (targetInfo.Type == CommandChainedAction.TargetType.Maid)
                return StateManager.Instance.YotogiWorkingMaidList[targetInfo.TargetMaidPosition];
            else if (targetInfo.Type == CommandChainedAction.TargetType.Man)
                return StateManager.Instance.YotogiWorkingManList[targetInfo.TargetMaidPosition];
            else if (targetInfo.Type == CommandChainedAction.TargetType.TargetGroup)
                return null;    //TODO: set target
            else if (targetInfo.Type == CommandChainedAction.TargetType.TargetMaid)
                return ProcessingDict[guid].ProcessTargetMaid;
            
            return null;
        }

        private static object GetValueData(string guid, CommandChainedAction.ValueData value)
        {

            if (!string.IsNullOrEmpty(value.ParamValue))
            {
                return ProcessingDict[guid].Parameters[value.ParamValue];
            }
            else
                return value.Value;
        }

        private static void AdvanceToNextActionStep(string guid, CommandChainedAction.StepDetail stepInfo, Maid target = null)
        {
            ProcessingDict[guid].CurrentStepID = stepInfo.NextStep;
            if (stepInfo.NextStepTriggerType == CommandChainedAction.NextStepTriggerType.None)
            {
                ProcessChainedMotionStep(guid);
            }
            else if (stepInfo.NextStepTriggerType == CommandChainedAction.NextStepTriggerType.AnimationEnd)
            {
                AnimationEndTrigger trigger = new AnimationEndTrigger(target, new EventDelegate(() => ProcessChainedMotionStep(guid)));
                StateManager.Instance.AnimationChangeTrigger = trigger;
            }
        }

        private static void ChaninedActionEnd(string guid, EventDelegate onFinishExecute)
        {
            ProcessingDict.Remove(guid);

            if (onFinishExecute != null)
                onFinishExecute.Execute();
        }
    }
}
