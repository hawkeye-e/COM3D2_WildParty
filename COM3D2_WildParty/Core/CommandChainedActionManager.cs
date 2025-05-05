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
            internal EventDelegate OnFinishExecute = null;
        }

        internal static void ProcessChainedMotion(string chainedActionCode, EventDelegate onFinishExecute = null)
        {
            if (!ModUseData.CommandChainedActionList.ContainsKey(chainedActionCode))
                return;

            ProcessingDetail processingDetail = new ProcessingDetail();
            processingDetail.ChainedActionCode = chainedActionCode;
            processingDetail.CurrentStepID = ModUseData.CommandChainedActionList[chainedActionCode].FirstStep;
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
                
                CharacterHandling.ApplyMotionInfoToCharacter(targetMaid, stepInfo.MotionInfo);
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
                Quaternion newRotation = targetMaid.transform.rotation * Quaternion.Euler(Vector3.up * stepInfo.OffsetInfo.Offset.Rot.y);

                if (stepInfo.OffsetInfo.IsSmoothAnimation)
                {

                    Util.SmoothMoveMaidPosition(targetMaid, targetMaid.transform.position + stepInfo.OffsetInfo.Offset.Pos, newRotation);
                }
                else {
                    targetMaid.transform.position += stepInfo.OffsetInfo.Offset.Pos;
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
                            newPosition = ((PartyGroup)destinationTarget).GroupPosition;
                            newRotation = ((PartyGroup)destinationTarget).GroupRotation;
                        }
                    }

                    //Apply the change
                    if (stepInfo.PositionInfo.IsSmoothAnimation)
                        Util.SmoothMoveMaidPosition(targetMaid, newPosition, newRotation);
                    else
                    {
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
                if (stepInfo.RemoveExtraManInfo.Type == CommandChainedAction.ExtraManType.Group)
                    extraManList = StateManager.Instance.PartyGroupList[stepInfo.RemoveExtraManInfo.TargetGroupPosition].ExtraManList;
                else if (stepInfo.RemoveExtraManInfo.Type == CommandChainedAction.ExtraManType.Global)
                    extraManList = PartyGroup.SharedExtraManList;
                
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
                Vector3 destinationVector = Vector3.zero;
                if (destinationTarget is Maid)
                    destinationVector = ((Maid)destinationTarget).transform.position;
                else if (destinationTarget is PartyGroup)
                    destinationVector = ((PartyGroup)destinationTarget).GroupPosition;

                float angle = manToMove.transform.rotation.eulerAngles.y;
                Vector3 maidMotionOffsetRespectToRotation = Quaternion.AngleAxis(angle, Vector3.up) * stepInfo.ManWalkSetupInfo.MaidMotionOffset;
                float distance = Vector3.Distance(manToMove.transform.position, destinationVector + maidMotionOffsetRespectToRotation);

                HardCodeMotion.ManWalkController.StandingMotionType standingMotionType = new HardCodeMotion.ManWalkController.StandingMotionType();
                standingMotionType.AnimationFileName = stepInfo.ManWalkSetupInfo.StandingAnimationFile;
                standingMotionType.StandingMotionOffset = stepInfo.ManWalkSetupInfo.StandingMotionOffset;
                standingMotionType.WalkingMotionOffset = stepInfo.ManWalkSetupInfo.WalkingMotionOffset;
                standingMotionType.RotationOffset = stepInfo.ManWalkSetupInfo.RotationOffset;

                HardCodeMotion.ManWalkController.MoveForward(manToMove, distance, standingMotionType, new EventDelegate(() => AdvanceToNextActionStep(guid, stepInfo)));
            }
        }

        private static void ProcessChainedMotion_SetGroupMember(string guid, CommandChainedAction.StepDetail stepInfo)
        {
            PartyGroup group = StateManager.Instance.PartyGroupList[stepInfo.SetGroupMemberInfo.TargetGroupPosition];

            ProcessSetGroupMemberRequest(guid, group, true, 0, stepInfo.SetGroupMemberInfo.Man1);
            ProcessSetGroupMemberRequest(guid, group, true, 1, stepInfo.SetGroupMemberInfo.Man2);
            ProcessSetGroupMemberRequest(guid, group, true, 2, stepInfo.SetGroupMemberInfo.Man3);
            ProcessSetGroupMemberRequest(guid, group, false, 0, stepInfo.SetGroupMemberInfo.Maid1);
            ProcessSetGroupMemberRequest(guid, group, false, 1, stepInfo.SetGroupMemberInfo.Maid2);

            if (group == StateManager.Instance.PartyGroupList[0])
            {
                //need update some info if it is main group
                CharacterHandling.SetGroupZeroActive();

                YotogiHandling.UpdateParameterView(group.Maid1);

                //need to update the main group
                var initialSkill = YotogiHandling.GetSkill(group.Maid1.status.personal.id, group.GroupType, group.SexPosID);
                CharacterHandling.CleanseCharacterMgrArray();
                YotogiHandling.ChangeMainGroupSkill(initialSkill.YotogiSkillID);
            }
            AdvanceToNextActionStep(guid, stepInfo);
        }

        private static void ProcessSetGroupMemberRequest(string guid, PartyGroup group, bool isMan, int positionIndex, CommandChainedAction.TargetInfo targetInfo)
        {
            if (targetInfo == null)
                return;

            var target = GetTarget(guid, targetInfo);

            if (target is Maid)
            {
                if (isMan)
                    group.SetManAtIndex(positionIndex, (Maid)target);
                else
                    group.SetMaidAtIndex(positionIndex, (Maid)target);
            }
        }





        private static object GetTarget(string guid, CommandChainedAction.TargetInfo stepInfo)
        {
            if (stepInfo.Type == CommandChainedAction.TargetType.Group)
                return StateManager.Instance.PartyGroupList[stepInfo.TargetGroupPosition];
            else if (stepInfo.Type == CommandChainedAction.TargetType.GroupMaid)
                return StateManager.Instance.PartyGroupList[stepInfo.TargetGroupPosition].GetMaidAtIndex(stepInfo.TargetMaidPosition);
            else if (stepInfo.Type == CommandChainedAction.TargetType.GroupMan)
                return StateManager.Instance.PartyGroupList[stepInfo.TargetGroupPosition].GetManAtIndex(stepInfo.TargetMaidPosition);
            else if (stepInfo.Type == CommandChainedAction.TargetType.Maid)
                return StateManager.Instance.YotogiWorkingMaidList[stepInfo.TargetMaidPosition];
            else if (stepInfo.Type == CommandChainedAction.TargetType.Man)
                return StateManager.Instance.YotogiWorkingManList[stepInfo.TargetMaidPosition];
            else if (stepInfo.Type == CommandChainedAction.TargetType.TargetGroup)
                return null;    //TODO: set target
            else if (stepInfo.Type == CommandChainedAction.TargetType.TargetMaid)
                return ProcessingDict[guid].ProcessTargetMaid;

            return null;
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
