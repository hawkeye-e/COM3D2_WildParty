using BepInEx.Logging;
using COM3D2.WildParty.Plugin.Trigger;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace COM3D2.WildParty.Plugin.Core
{
    internal partial class CustomADVProcessManager
    {
        private static ManualLogSource Log = WildParty.Log;

        internal static void ProcessADVStep(ADVKagManager instance)
        {
            if (StateManager.Instance.UndergoingModEventID > 0 && StateManager.Instance.ModEventProgress != Constant.EventProgress.Init)
            {
                //If there is no such step found (wrong setup), terminate the mod event.
                if (!ModUseData.ADVStepData[StateManager.Instance.UndergoingModEventID].ContainsKey(StateManager.Instance.CurrentADVStepID))
                {
                    StateManager.Instance.UndergoingModEventID = -1;
                    return;
                }

                CheckCameraPanFinish();
                CheckTimeWaitFinish();
                CheckSystemFadeOutFinish();


                //We dont want to process this over and over again if we are waiting for user input
                if (StateManager.Instance.ProcessedADVStepID != StateManager.Instance.CurrentADVStepID)
                {

                    //ADVStep thisStep = StateManager.Instance.dicADVSceneSteps[StateManager.Instance.CurrentADVStepID];
                    ADVStep thisStep = ModUseData.ADVStepData[StateManager.Instance.UndergoingModEventID][StateManager.Instance.CurrentADVStepID];
                    switch (thisStep.Type)
                    {
                        case Constant.ADVType.ChangeBGM:
                            ProcessADVChangeBGM(instance, thisStep);
                            break;
                        case Constant.ADVType.PlaySE:
                            ProcessADVPlaySE(instance, thisStep);
                            break;
                        case Constant.ADVType.ChangeBackground:
                            ProcessADVChangeBackground(instance, thisStep);
                            break;
                        case Constant.ADVType.ChangeCamera:
                            ProcessADVChangeCamera(instance, thisStep);
                            break;
                        case Constant.ADVType.CloseMsgPanel:
                            ProcessADVCloseMessagePanel(instance, thisStep);
                            break;
                        case Constant.ADVType.LoadScene:
                            ProcessADVLoadScene(instance, thisStep);
                            break;
                        case Constant.ADVType.Talk:
                            ProcessADVTalk(instance, thisStep);
                            break;
                        case Constant.ADVType.Chara:
                            ProcessADVCharacter(instance, thisStep);
                            break;
                        case Constant.ADVType.Group:
                            ProcessADVGroup(instance, thisStep);
                            break;
                        case Constant.ADVType.CharaInit:
                            ProcessADVCharaInit(instance, thisStep);
                            break;
                        case Constant.ADVType.BranchByPersonality:
                            ProcessADVBranchByPersonality(instance, thisStep);
                            break;
                        case Constant.ADVType.BranchByMap:
                            ProcessADVBranchByMap(instance, thisStep);
                            break;
                        case Constant.ADVType.Pick:
                            ProcessADVRandomPick(instance, thisStep);
                            break;
                        case Constant.ADVType.MakeGroup:
                            ProcessADVMakeGroup(instance, thisStep);
                            break;
                        case Constant.ADVType.DismissGroup:
                            ProcessADVDismissGroup(instance, thisStep);
                            break;
                        case Constant.ADVType.AddTexture:
                            ProcessADVAddTexture(instance, thisStep);
                            break;
                        case Constant.ADVType.RemoveTexture:
                            ProcessADVRemoveTexture(instance, thisStep);
                            break;
                        case Constant.ADVType.AddObject:
                            ProcessADVAddObject(instance, thisStep);
                            break;
                        case Constant.ADVType.RemoveObject:
                            ProcessADVRemoveObject(instance, thisStep);
                            break;
                        case Constant.ADVType.Shuffle:
                            ProcessADVShuffle(instance, thisStep);
                            break;
                        case Constant.ADVType.ListUpdate:
                            ProcessADVListUpdate(instance, thisStep);
                            break;
                        case Constant.ADVType.TimeWait:
                            ProcessADVTimeWait(instance, thisStep);
                            break;
                        case Constant.ADVType.ConvertSex:
                            ProcessADVConvertSex(instance, thisStep);
                            break;
                        case Constant.ADVType.Special:
                            ProcessADVSpecial(instance, thisStep);
                            break;
                        case Constant.ADVType.LoadYotogi:
                            ProcessADVLoadYotogiScene(instance, thisStep);
                            break;
                        case Constant.ADVType.ADVEnd:
                            ProcessADVEnd(instance);
                            break;
                    }


                    if (thisStep.FadeData != null)
                    {
                        if (thisStep.FadeData.IsFadeIn)
                            GameMain.Instance.MainCamera.FadeIn(f_fTime: thisStep.FadeData.Time);
                        else if (thisStep.FadeData.IsFadeOut)
                        {
                            CameraMain.dgOnCompleteFade dg = null;
                            if (thisStep.WaitingType == Constant.WaitingType.FadeOut)
                            {
                                dg = delegate
                                {
                                    ADVSceneProceedToNextStep();
                                };
                            }
                            CharacterHandling.StopAllMaidSound();
                            GameMain.Instance.MainCamera.FadeOut(f_dg: dg, f_fTime: thisStep.FadeData.Time , f_color: thisStep.FadeData.Color);
                        }
                    }



                    switch (thisStep.WaitingType)
                    {
                        case Constant.WaitingType.Auto:
                            //StateManager.Instance.CurrentADVStepID = thisStep.NextStepID;
                            ADVSceneProceedToNextStep();
                            break;
                        case Constant.WaitingType.Click:
                            StateManager.Instance.WaitForUserClick = true;
                            break;
                        case Constant.WaitingType.InputChoice:
                            StateManager.Instance.WaitForUserInput = true;
                            break;
                        case Constant.WaitingType.CameraPan:
                            StateManager.Instance.WaitForCameraPanFinish = true;
                            break;
                        case Constant.WaitingType.SystemFadeOut:
                            StateManager.Instance.WaitForSystemFadeOut = true;
                            break;
                    }

                    StateManager.Instance.ProcessedADVStepID = thisStep.ID;
                }
            }
        }

        private static void ProcessADVCharaInit(ADVKagManager instance, ADVStep step)
        {
            CharacterHandling.InitSelectedMaids();
            CharacterHandling.BackupManOrder();
            
            //Init paired man for maid
            if (step.CharaInitData.RequiresMaidPairMan)
                InitMaidPairMan();

            if (step.CharaInitData.ManRequired >= 0)
                StateManager.Instance.MaxManUsed = step.CharaInitData.ManRequired;

            List<string> validManTypes = GetValidManTypes(step.CharaInitData);

            //init man
            for (int i = 0; i < StateManager.Instance.MaxManUsed; i++)
            {
                var man = Core.CharacterHandling.InitMan(i, validManTypes);
                StateManager.Instance.MenList.Add(man);
            }
            
            //init the club owner
            StateManager.Instance.ClubOwner = StateManager.Instance.OriginalManOrderList[0];
            StateManager.Instance.ClubOwner.Visible = true;
            StateManager.Instance.ClubOwner.DutPropAll();
            StateManager.Instance.ClubOwner.AllProcPropSeqStart();
            StateManager.Instance.ClubOwner.transform.localPosition = new Vector3(-999f, -999f, -999f);
            
            StateManager.Instance.SpoofActivateMaidObjectFlag = true;
            if (!step.CharaInitData.IsClubOwnerADVMainCharacter)
            {
                //the Man[0] is replaced by a customer and will use his view to proceed the yotogi scene
                GameMain.Instance.CharacterMgr.SetActiveMan(StateManager.Instance.MenList[0], 0);
            }
            else
            {
                //The owner is the main character, put him as the first element of the men list 
                StateManager.Instance.MenList.Insert(0, StateManager.Instance.ClubOwner);
                GameMain.Instance.CharacterMgr.SetActiveMan(StateManager.Instance.ClubOwner, 0);
            }
            StateManager.Instance.SpoofActivateMaidObjectFlag = false;
            
            //init NPC
            StateManager.Instance.NPCList = new List<Maid>();
            if (step.CharaInitData.NPC != null)
            {
                foreach (var npcRequest in step.CharaInitData.NPC)
                {
                    Maid npc = CharacterHandling.InitNPCMaid(npcRequest.Preset, npcRequest.EmptyLastName);
                    StateManager.Instance.NPCList.Insert(npcRequest.Index, npc);

                }
            }
            
            if (step.CharaInitData.ModNPC != null)
            {
                foreach (var modNPCRequest in step.CharaInitData.ModNPC)
                {
                    Maid npc = null;
                    if (modNPCRequest.IsFemale)
                    {
                        npc = CharacterHandling.InitModNPCFemale(modNPCRequest.NPCID);
                        StateManager.Instance.NPCList.Insert(modNPCRequest.Index, npc);
                    }
                    else
                    {
                        npc = CharacterHandling.InitModNPCMale(modNPCRequest.NPCID);
                        StateManager.Instance.NPCManList.Insert(modNPCRequest.Index, npc);
                    }
                }
            }
        }

        private static void ProcessADVChangeBGM(ADVKagManager instance, ADVStep step)
        {
            GameMain.Instance.SoundMgr.PlayBGM(step.Tag, 1);
        }

        private static void ProcessADVPlaySE(ADVKagManager instance, ADVStep step)
        {
            if (step.SEData.Stop)
                GameMain.Instance.SoundMgr.StopSe();
            if(!string.IsNullOrEmpty(step.SEData.FileName))
                GameMain.Instance.SoundMgr.PlaySe(step.SEData.FileName, step.SEData.IsLoop);
        }

        private static void ProcessADVChangeBackground(ADVKagManager instance, ADVStep step)
        {
            //When change background, should also stop all voice to prevent the character is still talking in a completely new scene if the player is skipping part of the ADV
            CharacterHandling.StopAllMaidSound();

            if (!GameMain.Instance.CharacterMgr.status.isDaytime && !string.IsNullOrEmpty(step.TagForNight))
                instance.TagSetBg(CreateChangeBackgroundTag(step.TagForNight));
            else
                instance.TagSetBg(CreateChangeBackgroundTag(step.Tag));
        }

        private static void ProcessADVCloseMessagePanel(ADVKagManager instance, ADVStep step)
        {
            instance.MessageWindowMgr.CloseMessageWindowPanel();
        }

        private static void ProcessADVChangeCamera(ADVKagManager instance, ADVStep step)
        {
            if (step.CameraData.Type == ADVStep.Camera.CameraType.FixedPoint)
            {
                if (step.CameraData.MoveType == ADVStep.Camera.CameraMoveType.Instant)
                {
                    GameMain.Instance.MainCamera.SetPos(step.CameraData.FixedPointData.Pos);
                    GameMain.Instance.MainCamera.SetTargetPos(step.CameraData.FixedPointData.TargetPos);
                    GameMain.Instance.MainCamera.SetDistance(step.CameraData.FixedPointData.Distance);
                    GameMain.Instance.MainCamera.SetAroundAngle(step.CameraData.FixedPointData.AroundAngle);
                }
                else
                {
                    CameraHandling.AnimateCameraToLookAt(step.CameraData.FixedPointData.TargetPos, step.CameraData.FixedPointData.Distance,
                        step.CameraData.FixedPointData.AroundAngle.x, step.CameraData.FixedPointData.AroundAngle.y, step.CameraData.AnimationTime);
                }
            }
            else if (step.CameraData.Type == ADVStep.Camera.CameraType.LookAt)
            {

                Maid target = null;
                if (step.CameraData.LookAtData.Target == ADVStep.Camera.TargetType.Owner)
                    target = StateManager.Instance.ClubOwner;
                else
                {
                    int index = step.CameraData.LookAtData.ArrayPosition;
                    if (step.CameraData.LookAtData.UseRandomPick)
                        index = StateManager.Instance.RandomPickIndexList[step.CameraData.LookAtData.ArrayPosition];

                    if (step.CameraData.LookAtData.Target == ADVStep.Camera.TargetType.Maid)
                        target = StateManager.Instance.SelectedMaidsList[index];
                    else if (step.CameraData.LookAtData.Target == ADVStep.Camera.TargetType.Man)
                        target = StateManager.Instance.MenList[index];
                    else if (step.CameraData.LookAtData.Target == ADVStep.Camera.TargetType.GroupMaid1)
                        target = StateManager.Instance.PartyGroupList[index].Maid1;
                    else if (step.CameraData.LookAtData.Target == ADVStep.Camera.TargetType.GroupMaid2)
                        target = StateManager.Instance.PartyGroupList[index].Maid2;
                }

                if (step.CameraData.LookAtData.UseDefaultCameraWorkSetting)
                {
                    if (step.CameraData.MoveType == ADVStep.Camera.CameraMoveType.Instant)
                        CameraHandling.SetCameraLookAt(target);
                    else
                        CameraHandling.AnimateCameraToLookAt(target);
                }
                else
                {
                    Vector3 offset = new Vector3(step.CameraData.LookAtData.OffsetX, step.CameraData.LookAtData.OffsetY, step.CameraData.LookAtData.OffsetZ);
                    Vector3 targetOffset = new Vector3(step.CameraData.LookAtData.TargetOffsetX, step.CameraData.LookAtData.TargetOffsetY, step.CameraData.LookAtData.TargetOffsetZ);

                    if (step.CameraData.MoveType == ADVStep.Camera.CameraMoveType.Instant)
                        CameraHandling.SetCameraLookAt(target, offset, targetOffset, step.CameraData.LookAtData.Distance, step.CameraData.LookAtData.HorizontalRotation, step.CameraData.LookAtData.VerticalRotation);
                    else
                        CameraHandling.AnimateCameraToLookAt(target, offset, step.CameraData.LookAtData.Distance, step.CameraData.LookAtData.HorizontalRotation, step.CameraData.LookAtData.VerticalRotation);
                }

            }

            if(step.CameraData.LockCamera != null)
            {
                GameMain.Instance.MainCamera.SetControl(!step.CameraData.LockCamera.IsLock);
            }
            if(step.CameraData.BlurCamera != null)
            {
                GameMain.Instance.MainCamera.Blur(step.CameraData.BlurCamera.IsBlur);
            }
        }

        private static void ProcessADVLoadScene(ADVKagManager instance, ADVStep step)
        {
            //Special handling for loading up the background selection
            if (step.YotogiSetup != null)
                StateManager.Instance.YotogiPhase = step.YotogiSetup.Phase;
            GameMain.Instance.LoadScene(step.Tag);
        }



        private static void ProcessADVCharacter(ADVKagManager instance, ADVStep step)
        {
            if (step.CharaData != null)
            {
                for (int i = 0; i < step.CharaData.Length; i++)
                {

                    List<Maid> targetList;

                    if (step.CharaData[i].Type == Constant.TargetType.SingleMan)
                    {
                        if (step.CharaData[i].IsMaster)
                        {
                            targetList = new List<Maid>();
                            targetList.Add(StateManager.Instance.ClubOwner);
                        }
                        else
                            targetList = StateManager.Instance.MenList;

                    }
                    else if (step.CharaData[i].Type == Constant.TargetType.NPCFemale)
                        targetList = StateManager.Instance.NPCList;
                    else if (step.CharaData[i].Type == Constant.TargetType.NPCMale)
                        targetList = StateManager.Instance.NPCManList;
                    else
                        targetList = StateManager.Instance.SelectedMaidsList;
                    
                    if (step.CharaData[i].Type == Constant.TargetType.AllMaids)
                    {
                        foreach (var maid in StateManager.Instance.SelectedMaidsList)
                        {
                            SetADVCharaDataToCharacter(maid, step.CharaData[i], false);
                        }
                    }
                    else if (step.CharaData[i].Type == Constant.TargetType.AllMen)
                    {
                        foreach (var man in StateManager.Instance.MenList)
                        {
                            SetADVCharaDataToCharacter(man, step.CharaData[i], true);
                        }
                        if (StateManager.Instance.ClubOwner != null)
                        {
                            SetADVCharaDataToCharacter(StateManager.Instance.ClubOwner, step.CharaData[i], true);
                        }
                    }
                    else if (step.CharaData[i].Type == Constant.TargetType.AllNPCFemale)
                    {
                        foreach (var maid in StateManager.Instance.NPCList)
                        {
                            SetADVCharaDataToCharacter(maid, step.CharaData[i], false);
                        }
                    }
                    else if (step.CharaData[i].Type == Constant.TargetType.AllNPCMale)
                    {
                        foreach (var man in StateManager.Instance.NPCManList)
                        {
                            SetADVCharaDataToCharacter(man, step.CharaData[i], true);
                        }
                    }
                    else if (step.CharaData[i].Type == Constant.TargetType.PairedMan)
                    {
                        foreach (var man in StateManager.Instance.PairedManForMaidList.Values.ToList())
                        {
                            SetADVCharaDataToCharacter(man, step.CharaData[i], true);
                        }
                    }
                    else
                    {
                        int index = step.CharaData[i].ArrayPosition;
                        if (step.CharaData[i].UseBranchIndex)
                            index = StateManager.Instance.BranchIndex;
                        if (targetList.Count > index)
                        {
                            SetADVCharaDataToCharacter(targetList[index], step.CharaData[i], targetList[index].boMAN);

                        }
                    }

                }
            }
        }

        internal static void SetCharacterEyeSight(PartyGroup group, EyeSightSetting eyeSightSetting)
        {
            if (eyeSightSetting.SourceGroupMember == EyeSightSetting.GroupMemberType.Maid1)
                SetCharacterEyeSight(group.Maid1, eyeSightSetting);
            else if (eyeSightSetting.SourceGroupMember == EyeSightSetting.GroupMemberType.Maid2)
                SetCharacterEyeSight(group.Maid2, eyeSightSetting);
            else if (eyeSightSetting.SourceGroupMember == EyeSightSetting.GroupMemberType.Man1)
                SetCharacterEyeSight(group.Man1, eyeSightSetting);
            else if (eyeSightSetting.SourceGroupMember == EyeSightSetting.GroupMemberType.Man2)
                SetCharacterEyeSight(group.Man2, eyeSightSetting);
        }


        internal static void SetCharacterEyeSight(Maid maid, EyeSightSetting eyeSightSetting)
        {
            if (maid == null)
                return;
            if (eyeSightSetting == null)
                return;

            if (eyeSightSetting.Type == EyeSightSetting.EyeSightType.ToCamera)
            {
                maid.EyeToCamera((Maid.EyeMoveType)eyeSightSetting.EyeToCameraSetting.MoveType);
            }
            else if (eyeSightSetting.Type == EyeSightSetting.EyeSightType.ToChara)
            {
                Maid target = null;
                if (eyeSightSetting.EyeToCharaSetting.Type == EyeSightSetting.EyeToCharaSettingDetail.TargetType.ClubOwner)
                    target = StateManager.Instance.ClubOwner;
                else if (eyeSightSetting.EyeToCharaSetting.Type == EyeSightSetting.EyeToCharaSettingDetail.TargetType.Man)
                    target = Util.GetManFromList(eyeSightSetting.EyeToCharaSetting.ArrayPosition);
                else if (eyeSightSetting.EyeToCharaSetting.Type == EyeSightSetting.EyeToCharaSettingDetail.TargetType.Maid)
                    target = Util.GetMaidFromList(eyeSightSetting.EyeToCharaSetting.ArrayPosition);
                else if (eyeSightSetting.EyeToCharaSetting.Type == EyeSightSetting.EyeToCharaSettingDetail.TargetType.NPCMale)
                    target = Util.GetNPCMaleFromList(eyeSightSetting.EyeToCharaSetting.ArrayPosition);
                else if (eyeSightSetting.EyeToCharaSetting.Type == EyeSightSetting.EyeToCharaSettingDetail.TargetType.NPCFemale)
                    target = Util.GetNPCFemaleFromList(eyeSightSetting.EyeToCharaSetting.ArrayPosition);
                else if (eyeSightSetting.EyeToCharaSetting.Type == EyeSightSetting.EyeToCharaSettingDetail.TargetType.GroupMember)
                {
                    PartyGroup group = StateManager.Instance.PartyGroupList[eyeSightSetting.EyeToCharaSetting.ArrayPosition];
                    if (eyeSightSetting.EyeToCharaSetting.TargetGroupMember == EyeSightSetting.GroupMemberType.Maid1)
                        target = group.Maid1;
                    else if (eyeSightSetting.EyeToCharaSetting.TargetGroupMember == EyeSightSetting.GroupMemberType.Maid2)
                        target = group.Maid2;
                    else if (eyeSightSetting.EyeToCharaSetting.TargetGroupMember == EyeSightSetting.GroupMemberType.Man1)
                        target = group.Man1;
                    else if (eyeSightSetting.EyeToCharaSetting.TargetGroupMember == EyeSightSetting.GroupMemberType.Man2)
                        target = group.Man2;
                }


                if (target != null)
                {
                    //The logic in Maid.EyeToTarget is wrong. The parameter in the replace function for man case is swapped. We have to supply the name of the bone to avoid the problem.
                    string targetBone = Constant.DefinedGameObjectNames.MaidHeadBoneName;
                    if (target.boMAN)
                        targetBone = Constant.DefinedGameObjectNames.ManHeadBoneName;

                    maid.EyeToTarget(target, 0.5f, targetBone);
                }
            }
            else if (eyeSightSetting.Type == EyeSightSetting.EyeSightType.ToObject)
            {
                maid.EyeToTargetObject(eyeSightSetting.EyeToObjectSetting.Target.transform);
            }
            else if (eyeSightSetting.Type == EyeSightSetting.EyeSightType.Reset)
            {
                maid.EyeToReset();
            }
        }

        private static void SetADVCharaDataToCharacter(Maid maid, ADVStep.ShowChara charaData, bool isMan = false)
        {
            maid.Visible = charaData.Visible;
            if (charaData.Visible)
                maid.transform.localScale = Vector3.one;

            if (!isMan)
            {
                if (!string.IsNullOrEmpty(charaData.FaceAnime))
                {
                    SetFaceAnimeToMaid(maid, charaData.FaceAnime);
                }

                if (!string.IsNullOrEmpty(charaData.FaceBlend))
                {
                    maid.FaceBlend(charaData.FaceBlend);
                }

                maid.OpenMouth(charaData.OpenMouth);

                CharacterHandling.SetFemaleClothing(maid, charaData.ClothesSetID);

                if(charaData.Effect != null)
                {
                    CharacterHandling.AddCharacterEffect(maid, charaData.Effect.Add);
                    CharacterHandling.RemoveCharacterEffect(maid, charaData.Effect.Remove);
                }
            }
            else
            {
                CharacterHandling.SetManClothing(maid, charaData.IsManNude);
            }

            SetCharacterEyeSight(maid, charaData.EyeSight);

            CharacterHandling.ApplyMotionInfoToCharacter(maid, charaData.MotionInfo);

            if (charaData.ResetIK)
#if COM3D2_5
#if UNITY_2022_3   
                maid.body0.fullBodyIK.AllIKDetach();          
#endif
#endif
#if COM3D2
                maid.AllIKDetach();
#endif

            if (charaData.IKAttach != null)
                foreach (IKAttachInfo info in charaData.IKAttach)
                    CharacterHandling.IKAttachBone(info);

            if (charaData.PosRot != null)
            {
                Util.StopSmoothMove(maid);
                if (charaData.SmoothMovement != null)
                {
                    Util.SmoothMoveMaidPosition(maid, charaData.PosRot.Pos, charaData.PosRot.Rot, charaData.SmoothMovement.Time );
                }
                else
                {
                    maid.transform.localPosition = Vector3.zero;
                    maid.transform.position = charaData.PosRot.Pos;
                    maid.transform.localRotation = new Quaternion(0, 0, 0, 0);
                    maid.transform.rotation = charaData.PosRot.Rot;
                }
                //Need to call the following to fix the gravity
                maid.body0.SetBoneHitHeightY(charaData.PosRot.Pos.y);
            }

            if (charaData.ExtraObjectsInfo != null)
            {
                CharacterHandling.RemoveObjectFromCharacter(maid, charaData.ExtraObjectsInfo.RemoveObjects);
                CharacterHandling.AttachObjectToCharacter(maid, charaData.ExtraObjectsInfo.AddObjects);
            }

            if (charaData.WaitLoad)
                StateManager.Instance.WaitForFullLoadList.Add(maid);
        }

        private static void ProcessADVGroupIndividual(Maid maid, ADVStep.ShowGroupMotion.DetailSetup setupData)
        {
            if (maid == null || setupData == null)
                return;

            maid.Visible = setupData.Visible;
            if (maid.Visible)
            {
                CharacterHandling.SetManClothing(maid, setupData.IsManNude);
                maid.OpenMouth(setupData.OpenMouth);

                if (!string.IsNullOrEmpty(setupData.FaceAnime))
                {
                    SetFaceAnimeToMaid(maid, setupData.FaceAnime);
                }

                if (!string.IsNullOrEmpty(setupData.FaceBlend))
                    maid.FaceBlend(setupData.FaceBlend);

                SetCharacterEyeSight(maid, setupData.EyeSight);

                if (setupData.PosRot != null)
                {
                    maid.transform.localPosition = Vector3.zero;
                    maid.transform.position = setupData.PosRot.Pos;
                    maid.body0.SetBoneHitHeightY(setupData.PosRot.Pos.y);
                    maid.SetRot(Vector3.zero);
                    maid.transform.rotation = setupData.PosRot.Rot;
                }
            }
            else
            {
                maid.transform.localScale = Vector3.zero;
                CharacterHandling.SetManClothing(maid, false);
            }
        }

        internal static void SetFaceAnimeToMaid(Maid maid, string faceAnime)
        {
            if (maid == null)
                return;

            if (faceAnime == RandomList.FaceAnime.FaceAnimeCode.RandomSmile)
            {
                int rnd = RNG.Random.Next(RandomList.FaceAnime.SmileList.Length);
                maid.FaceAnime(RandomList.FaceAnime.SmileList[rnd]);
            }
            else if (faceAnime == RandomList.FaceAnime.FaceAnimeCode.RandomOh)
            {
                int rnd = RNG.Random.Next(RandomList.FaceAnime.OhList.Length);
                maid.FaceAnime(RandomList.FaceAnime.OhList[rnd]);
            }
            else if (faceAnime == RandomList.FaceAnime.FaceAnimeCode.RandomHorny)
            {
                int rnd = RNG.Random.Next(RandomList.FaceAnime.HornyList.Length);
                maid.FaceAnime(RandomList.FaceAnime.HornyList[rnd]);
            }
            else if (faceAnime == RandomList.FaceAnime.FaceAnimeCode.RandomRest)
            {
                int rnd = RNG.Random.Next(RandomList.FaceAnime.RestList.Length);
                maid.FaceAnime(RandomList.FaceAnime.RestList[rnd]);
            }
            else if (faceAnime == RandomList.FaceAnime.FaceAnimeCode.RandomMaidAsManHorny)
            {
                int rnd = RNG.Random.Next(RandomList.FaceAnime.MaidAsManHornyList.Length);
                maid.FaceAnime(RandomList.FaceAnime.MaidAsManHornyList[rnd]);
            }
            else if (faceAnime == RandomList.FaceAnime.FaceAnimeCode.RandomAngry)
            {
                int rnd = RNG.Random.Next(RandomList.FaceAnime.AngryList.Length);
                maid.FaceAnime(RandomList.FaceAnime.AngryList[rnd]);   
            }
            else
            {
                maid.FaceAnime(faceAnime);
            }
        }

        private static void ProcessADVGroup(ADVKagManager instance, ADVStep step)
        {
            if (step.GroupData != null)
            {
                for (int i = 0; i < step.GroupData.Length; i++)
                {
                    int targetGroupIndex = step.GroupData[i].ArrayPosition;
                    if (step.GroupData[i].UseRandomPick)
                        targetGroupIndex = StateManager.Instance.RandomPickIndexList[step.GroupData[i].ArrayPosition];

                    
                    PartyGroup group = StateManager.Instance.PartyGroupList[targetGroupIndex];
                    
                    //could be a empty group due to not enough character selected
                    if (group.GroupType != Constant.GroupType.Invalid)
                    {
                        if (!string.IsNullOrEmpty(step.GroupData[i].ScriptFile) && !string.IsNullOrEmpty(step.GroupData[i].ScriptLabel))
                        {
                            string manID = "";
                            if (group.Man1 != null)
                                manID = group.Man1.status.guid;
                            
                            if (step.GroupData[i].SexPosID >= 0)
                            {
                                group.SexPosID = step.GroupData[i].SexPosID;
                            }
                            
                            CharacterHandling.LoadMotionScript(0, false, step.GroupData[i].ScriptFile, step.GroupData[i].ScriptLabel, group.Maid1.status.guid, manID,
                                false, true, false, false);

                        }
                        
                        ProcessADVGroupIndividual(group.Maid1, step.GroupData[i].Maid1);
                        ProcessADVGroupIndividual(group.Maid2, step.GroupData[i].Maid2);
                        ProcessADVGroupIndividual(group.Man1, step.GroupData[i].Man1);
                        ProcessADVGroupIndividual(group.Man2, step.GroupData[i].Man2);
                        ProcessADVGroupIndividual(group.Man3, step.GroupData[i].Man3);
                        
                        if (step.GroupData[i].WaitLoad)
                        {
                            StateManager.Instance.WaitForFullLoadList.Add(group.Maid1);
                            if (group.Maid2 != null)
                                StateManager.Instance.WaitForFullLoadList.Add(group.Maid2);
                            if (group.Man1 != null)
                                StateManager.Instance.WaitForFullLoadList.Add(group.Man1);
                            if (group.Man2 != null)
                                StateManager.Instance.WaitForFullLoadList.Add(group.Man2);
                            if (group.Man3 != null)
                                StateManager.Instance.WaitForFullLoadList.Add(group.Man3);
                        }
                        
                        if (step.GroupData[i].PosRot != null)
                            group.SetGroupPosition(step.GroupData[i].PosRot.Pos, step.GroupData[i].PosRot.Rot);
                        
                        group.SetGroupPosition();
                        
                        if (step.GroupData[i].BlockInputUntilMotionChange && !Config.DebugIgnoreADVForceTimeWait)
                        {
                            StateManager.Instance.WaitForMotionChange = true;
                            AnimationEndTrigger trigger = new AnimationEndTrigger(group.Maid1, new EventDelegate(ADVMotionChangeComplete));
                            StateManager.Instance.AnimationChangeTrigger = trigger;
                        }
                    }
                }
            }
        }


        private static void ProcessADVTalk(ADVKagManager instance, ADVStep step)
        {
            string speakerName;
            int voicePitch = 0;
            List<Maid> lstMaidToSpeak = null;

            switch (step.TalkData.SpecificSpeaker)
            {
                case Constant.ADVTalkSpearkerType.All:
                    lstMaidToSpeak = StateManager.Instance.SelectedMaidsList;
                    speakerName = step.TalkData.SpeakerName;
                    break;
                case Constant.ADVTalkSpearkerType.Narrative:
                    speakerName = "";
                    break;
                case Constant.ADVTalkSpearkerType.Owner:
                    speakerName = GameMain.Instance.CharacterMgr.status.playerName;
                    break;
                case Constant.ADVTalkSpearkerType.SelectedMaid:
                    Maid mainMaid = StateManager.Instance.SelectedMaidsList[0];
                    lstMaidToSpeak = new List<Maid>() { mainMaid };
                    speakerName = mainMaid.status.callName;
                    break;
                case Constant.ADVTalkSpearkerType.Maid:
                    int index = step.TalkData.Index;
                    if (step.TalkData.UseBranchIndex)
                        index = StateManager.Instance.BranchIndex;
                    Maid maid = StateManager.Instance.SelectedMaidsList[index];
                    lstMaidToSpeak = new List<Maid>() { maid };
                    speakerName = maid.status.callName;
                    break;
                case Constant.ADVTalkSpearkerType.NPCFemale:
                    Maid npcMaid = StateManager.Instance.NPCList[step.TalkData.Index];
                    lstMaidToSpeak = new List<Maid>() { npcMaid };
                    speakerName = npcMaid.status.callName;
                    break;
                case Constant.ADVTalkSpearkerType.NPCMale:
                    if (Product.isJapan)
                        speakerName = StateManager.Instance.NPCManList[step.TalkData.Index].status.fullNameJpStyle;
                    else
                        speakerName = StateManager.Instance.NPCManList[step.TalkData.Index].status.fullNameEnStyle;
                    break;
                default:
                    speakerName = step.TalkData.SpeakerName;
                    break;
            }

            if (lstMaidToSpeak == null)
            {
                //there is no audio to be played in this step, narrative or a man speaking
                DisplayAdvText(instance, speakerName, step.TalkData.Text, "", voicePitch, AudioSourceMgr.Type.Voice);
            }
            else
            {
                CharacterHandling.StopAllMaidSound();

                bool isAudioChopped = false;
                ADVStep.Talk.Voice voiceInfo = null;

                //add audio to maids
                bool isAll = step.TalkData.SpecificSpeaker == Constant.ADVTalkSpearkerType.All;
                foreach (var maid in lstMaidToSpeak)
                {
                    //get the correct voice file by personality
                    if (step.TalkData.SpecificSpeaker == Constant.ADVTalkSpearkerType.NPCFemale)
                        voiceInfo = step.TalkData.VoiceData.First().Value;
                    else
                    {
                        if (step.TalkData.VoiceData.ContainsKey(Util.GetPersonalityNameByValue(maid.status.personal.id)))
                            voiceInfo = step.TalkData.VoiceData[Util.GetPersonalityNameByValue(maid.status.personal.id)];
                    }
                    
                    if (voiceInfo != null)
                    {
                        if (voiceInfo.IsChoppingAudio)
                        {
                            isAudioChopped = true;

                            Helper.AudioChoppingManager.PlaySubClip(maid, step.ID, voiceInfo.VoiceFile, voiceInfo.StartTime, voiceInfo.EndTime, isAll);
                        }
                        else
                        {
                            maid.AudioMan.LoadPlay(voiceInfo.VoiceFile, 0f, false);
                        }
                        maid.AudioMan.audiosource.volume = voiceInfo.Volume;
                    }
                }

                if (!isAll)
                {
                    string voiceFile = "";
                    if (voiceInfo != null)
                        voiceFile = voiceInfo.VoiceFile;
                    //if it is chopped we use the id instead to reload from our own subclip library
                    if (isAudioChopped)
                        voiceFile = step.ID;
                    //single maid speak and no chopping
                    voicePitch = lstMaidToSpeak[0].VoicePitch;
                    speakerName = lstMaidToSpeak[0].status.callName;
                    DisplayAdvText(instance, speakerName, step.TalkData.Text, voiceFile, voicePitch, AudioSourceMgr.Type.VoiceHeroine);
                }
                else
                {
                    //the case of hard to set the replay, skip it
                    DisplayAdvText(instance, speakerName, step.TalkData.Text, "", voicePitch, AudioSourceMgr.Type.VoiceHeroine);
                }

            }
        }


        private static string PrepareDialogueText(string text)
        {
            text = PrepareCharacterNameText(text);
            text = PrepareRandomGroupCharacterName(text);

            text = text.Replace(Constant.JsonReplaceTextLabels.ClubName, GameMain.Instance.CharacterMgr.status.clubName);
            text = text.Replace(Constant.JsonReplaceTextLabels.ClubOwnerName, GameMain.Instance.CharacterMgr.status.playerName);

            return text;
        }

        private static string PrepareRandomGroupCharacterName(string text)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(Constant.JsonReplaceTextLabels.RandomGroupRegex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var matches = regex.Matches(text);
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                int index = StateManager.Instance.RandomPickIndexList[int.Parse(match.Groups[1].Value)];
                PartyGroup group = StateManager.Instance.PartyGroupList[index];
                if (match.Groups[2].Value == "Maid1Name")
                    text = text.Replace(match.Groups[0].Value, group.Maid1.status.callName);
                else if (match.Groups[2].Value == "Maid2Name")
                    text = text.Replace(match.Groups[0].Value, group.Maid2.status.callName);
            }

            return text;
        }

        private static string PrepareCharacterNameText(string text)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(Constant.JsonReplaceTextLabels.CharacterNameRegex, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var matches = regex.Matches(text);
            foreach (System.Text.RegularExpressions.Match match in matches)
            {

                List<Maid> sourceList = null;
                if (match.Groups[1].Value == Constant.JsonReplaceTextLabels.CharacterNameSourceType.NPCMale)
                    sourceList = StateManager.Instance.NPCManList;
                else if (match.Groups[1].Value == Constant.JsonReplaceTextLabels.CharacterNameSourceType.NPCFemale)
                    sourceList = StateManager.Instance.NPCList;
                else if (match.Groups[1].Value == Constant.JsonReplaceTextLabels.CharacterNameSourceType.Maid)
                    sourceList = StateManager.Instance.SelectedMaidsList;

                if (sourceList == null)
                    return text;

                Maid maid = sourceList[int.Parse(match.Groups[2].Value)];

                string displayName = "";
                if (match.Groups[3].Value == Constant.JsonReplaceTextLabels.CharacterNameDisplayType.FullName)
                {
                    if (Product.isJapan)
                        displayName = maid.status.fullNameJpStyle;
                    else
                        displayName = maid.status.fullNameEnStyle;
                }
                else if (match.Groups[3].Value == Constant.JsonReplaceTextLabels.CharacterNameDisplayType.FirstName)
                    displayName = maid.status.firstName;
                else if (match.Groups[3].Value == Constant.JsonReplaceTextLabels.CharacterNameDisplayType.LastName)
                    displayName = maid.status.lastName;
                else if (match.Groups[3].Value == Constant.JsonReplaceTextLabels.CharacterNameDisplayType.NickName)
                    displayName = maid.status.nickName;
                else if (match.Groups[3].Value == Constant.JsonReplaceTextLabels.CharacterNameDisplayType.CallName)
                    displayName = maid.status.callName;

                text = text.Replace(match.Groups[0].Value, displayName);

            }

            return text;
        }

        private static void ProcessADVEnd(ADVKagManager instance)
        {
            //Add modded event flag requested by scenario, if any
            if (Util.GetUndergoingScenario().SetScenarioFlag != null)
                foreach (var flags in Util.GetUndergoingScenario().SetScenarioFlag)
                {
                    GameMain.Instance.CharacterMgr.status.AddFlag(flags.ID, flags.value);
                }

            //Add modded event flag to all individual maid, if any
            if (Util.GetUndergoingScenario().SetMaidFlag != null)
                foreach (Maid maid in StateManager.Instance.SelectedMaidsList)
                {
                    foreach (var flags in Util.GetUndergoingScenario().SetMaidFlag)
                    {
                        maid.status.AddFlag(flags.ID, flags.value);
                    }
                }

            //alter the maid status for all participants
            ModEventCleanUp.UpdateMaidStatusForModEvent(StateManager.Instance.SelectedMaidsList);

            //Remove all participants from action list
            ModEventCleanUp.RemoveMaidsFromSelectionList(StateManager.Instance.SelectedMaidsList);

            ModEventCleanUp.ResetModEvent();

            StateManager.Instance.SpoofTagListResult = true;
            ScriptManagerFast.KagTagSupportFast tagSceneCall = new ScriptManagerFast.KagTagSupportFast();
            StateManager.Instance.TagBackUp = new Dictionary<string, string>();
            Dictionary<string, string> tagList;
            if (SceneCharacterSelect.chara_guid_stock_list.Count > 0)
            {
                //goes back to the normal flow screen
                tagList = TagSupportData.GetTagForCharacterSelectScreen();

                //It is possible for the player to have setup more than 1 modded scenario in schedule, so set the flag to check it again
                StateManager.Instance.RequireCheckModdedSceneFlag = true;
            }
            else
            {
                //there is no more maid need to process, show the final result
                if (GameMain.Instance.CharacterMgr.status.isDaytime)
                    tagList = TagSupportData.GetTagForNoonTimeFinalResultScreen();
                else
                    tagList = TagSupportData.GetTagForNightTimeFinalResultScreen();
            }

            foreach (var kvp in tagList)
            {
                tagSceneCall.AddTagProperty(kvp.Key, kvp.Value);
                StateManager.Instance.TagBackUp.Add(kvp.Key, kvp.Value);
            }
            instance.TagSceneCall(tagSceneCall);
        }

        private static void ProcessADVBranchByPersonality(ADVKagManager instance, ADVStep step)
        {
            StateManager.Instance.BranchIndex = step.CharaData[0].ArrayPosition;

            string personalityName = Util.GetPersonalityNameByValue(StateManager.Instance.SelectedMaidsList[step.CharaData[0].ArrayPosition].status.personal.id);

            string nextStepID = step.NextStepID;
            nextStepID = nextStepID.Replace(Constant.JsonReplaceTextLabels.PersonalityType, personalityName);

            ADVSceneProceedToNextStep(nextStepID);
        }

        private static void ProcessADVBranchByMap(ADVKagManager instance, ADVStep step)
        {
            string nextStepID = step.NextStepID;
            nextStepID = nextStepID.Replace(Constant.JsonReplaceTextLabels.MapType, GameMain.Instance.BgMgr.GetBGName());

            ADVSceneProceedToNextStep(nextStepID);
        }

        private static void ProcessADVRandomPick(ADVKagManager instance, ADVStep step)
        {
            StateManager.Instance.RandomPickIndexList = new List<int>();
            int itemCount = 0;
            if (step.PickData.Type == ADVStep.RandomPick.PickType.Group)
                itemCount = StateManager.Instance.PartyGroupList.Count;
            else if (step.PickData.Type == ADVStep.RandomPick.PickType.Maid)
                itemCount = StateManager.Instance.SelectedMaidsList.Count;
            else if (step.PickData.Type == ADVStep.RandomPick.PickType.Man)
                itemCount = StateManager.Instance.MenList.Count;

            if (itemCount == 0)
                return;

            while (StateManager.Instance.RandomPickIndexList.Count < step.PickData.Num && StateManager.Instance.RandomPickIndexList.Count < itemCount)
            {
                int rnd = RNG.Random.Next(itemCount);
                if (!StateManager.Instance.RandomPickIndexList.Contains(rnd))
                    StateManager.Instance.RandomPickIndexList.Add(rnd);
            }
        }

        private static void ProcessADVMakeGroup(ADVKagManager instance, ADVStep step)
        {
            if (step.GroupFormat != null)
            {
                foreach (ADVStep.MakeGroupFormat groupInfo in step.GroupFormat)
                {
                    PartyGroup group;

                    while (StateManager.Instance.PartyGroupList.Count <= groupInfo.GroupIndex)
                        StateManager.Instance.PartyGroupList.Add(new PartyGroup());

                    group = new PartyGroup();
                    StateManager.Instance.PartyGroupList[groupInfo.GroupIndex] = group;

                    group.Maid1 = GetMaidForMakeGroupFormatRequest(groupInfo.Maid1, false);
                    group.Maid2 = GetMaidForMakeGroupFormatRequest(groupInfo.Maid2, false);

                    group.Man1 = GetMaidForMakeGroupFormatRequest(groupInfo.Man1, true);
                    group.Man2 = GetMaidForMakeGroupFormatRequest(groupInfo.Man2, true);
                    group.Man3 = GetMaidForMakeGroupFormatRequest(groupInfo.Man3, true);

                    group.GroupOffsetVector = Vector3.zero;
                    if(group.Maid1 != null)
                        group.SetGroupPosition(group.Maid1.transform.position, group.Maid1.transform.rotation);

                }
            }

        }

        private static Maid GetMaidForMakeGroupFormatRequest(ADVStep.MakeGroupFormat.GroupMemberInfo requestInfo, bool isMan)
        {
            if (requestInfo == null)
                return null;

            if (isMan)
            {
                if (requestInfo.Type == ADVStep.MakeGroupFormat.MemberType.Owner)
                    return StateManager.Instance.ClubOwner;
                List<Maid> targetList;
                if (requestInfo.Type == ADVStep.MakeGroupFormat.MemberType.NPC)
                    targetList = StateManager.Instance.NPCManList;
                else
                    targetList = StateManager.Instance.MenList;

                if (targetList.Count > requestInfo.ArrayPosition)
                    return targetList[requestInfo.ArrayPosition];
            }
            else
            {
                List<Maid> targetList;
                if (requestInfo.Type == ADVStep.MakeGroupFormat.MemberType.NPC)
                    targetList = StateManager.Instance.NPCList;
                else
                    targetList = StateManager.Instance.SelectedMaidsList;

                if (targetList.Count > requestInfo.ArrayPosition)
                    return targetList[requestInfo.ArrayPosition];
            }

            return null;
        }

        private static void ProcessADVDismissGroup(ADVKagManager instance, ADVStep step)
        {
            if (step.Tag == "ALL")
            {
                foreach (var group in StateManager.Instance.PartyGroupList)
                    group.DetachAllIK();
                StateManager.Instance.PartyGroupList.Clear();
            }
            else
            {
                int groupIndex = int.Parse(step.Tag);
                StateManager.Instance.PartyGroupList[groupIndex].DetachAllIK();
                StateManager.Instance.PartyGroupList.RemoveAt(groupIndex);
            }
        }

        private static void ProcessADVAddTexture(ADVKagManager instance, ADVStep step)
        {
            if (step.TextureData != null)
            {
                foreach (ADVStep.Texture data in step.TextureData)
                {
                    if (data.Type == Constant.TextureType.Semen)
                    {
                        if (data.TargetType == Constant.TargetType.AllMaids)
                        {
                            foreach (Maid maid in StateManager.Instance.SelectedMaidsList)
                            {
                                foreach (string bodyPart in data.BodyTarget)
                                    if (ModUseData.SemenPatternList.ContainsKey(bodyPart))
                                        CharacterHandling.AddSemenTexture(maid, ModUseData.SemenPatternList[bodyPart]);
                            }

                        }
                        else
                        {
                            Maid maid = null;
                            if (data.TargetType == Constant.TargetType.SingleMaid)
                                maid = StateManager.Instance.SelectedMaidsList[data.IndexPosition];
                            else if (data.TargetType == Constant.TargetType.NPCFemale)
                                maid = StateManager.Instance.NPCList[data.IndexPosition];

                            foreach (string bodyPart in data.BodyTarget)
                                if (ModUseData.SemenPatternList.ContainsKey(bodyPart))
                                    CharacterHandling.AddSemenTexture(maid, ModUseData.SemenPatternList[bodyPart]);
                        }


                    }
                }
            }
        }

        private static void ProcessADVRemoveTexture(ADVKagManager instance, ADVStep step)
        {
            if (step.TextureData != null)
            {
                foreach (ADVStep.Texture data in step.TextureData)
                {
                    if (data.Type == Constant.TextureType.Semen)
                    {
                        if (data.TargetType == Constant.TargetType.AllMaids)
                        {
                            foreach (Maid maid in StateManager.Instance.SelectedMaidsList)
                                CharacterHandling.RemoveSemenTexture(maid);
                        }
                        else
                        {
                            Maid maid = null;
                            if (data.TargetType == Constant.TargetType.SingleMaid)
                                maid = StateManager.Instance.SelectedMaidsList[data.IndexPosition];
                            else if (data.TargetType == Constant.TargetType.NPCFemale)
                                maid = StateManager.Instance.NPCList[data.IndexPosition];

                            CharacterHandling.RemoveSemenTexture(maid);
                        }
                    }
                }
            }
        }

        private static void ProcessADVAddObject(ADVKagManager instance, ADVStep step)
        {
            if(step.WorldObjectData != null)
            {
                foreach(var objData in step.WorldObjectData)
                {
                    GameObject addedObject = GameMain.Instance.BgMgr.AddPrefabToBg(objData.Src, objData.ObjectID, "", Vector3.zero, Vector3.zero);

                    if (addedObject != null)
                    {
                        addedObject.transform.position = objData.PosRot.Pos;
                        addedObject.transform.rotation = objData.PosRot.Rot;
                        addedObject.transform.localScale = new Vector3(objData.Scale, objData.Scale, objData.Scale);
                        StateManager.Instance.AddedGameObjectList.Add(objData.ObjectID, addedObject);
                    }
                }
            }
        }

        private static void ProcessADVRemoveObject(ADVKagManager instance, ADVStep step)
        {
            if (step.WorldObjectData != null)
            {
                foreach (var objData in step.WorldObjectData)
                {
                    GameMain.Instance.BgMgr.DelPrefabFromBg(objData.ObjectID);
                    
                    StateManager.Instance.AddedGameObjectList.Remove(objData.ObjectID);
                }
            }
        }

        private static void ProcessADVShuffle(ADVKagManager instance, ADVStep step)
        {
            if (step.ShuffleData != null)
            {
                List<Maid> targetList = null;
                if (step.ShuffleData.TargetList == Constant.TargetType.AllMaids)
                    targetList = StateManager.Instance.SelectedMaidsList;
                else if (step.ShuffleData.TargetList == Constant.TargetType.AllMen)
                    targetList = StateManager.Instance.MenList;
                else
                    return;

                List<Maid> shuffleList = new List<Maid>();
                Dictionary<int, Maid> keepPositionList = new Dictionary<int, Maid>();
                if (step.ShuffleData.KeepPosition != null)
                {
                    foreach (int index in step.ShuffleData.KeepPosition)
                        keepPositionList.Add(index, targetList[index]);

                    foreach (var kvp in keepPositionList)
                        targetList.Remove(kvp.Value);
                }

                targetList = CharacterHandling.ShuffleMaidOrManList(targetList);

                foreach (var kvp in keepPositionList.OrderByDescending(x => x.Key))
                {
                    targetList.Insert(kvp.Key, kvp.Value);
                }

                if (step.ShuffleData.TargetList == Constant.TargetType.AllMaids)
                    StateManager.Instance.SelectedMaidsList = targetList;
                else if (step.ShuffleData.TargetList == Constant.TargetType.AllMen)
                    StateManager.Instance.MenList = targetList;

            }
        }

        internal static void ProcessADVListUpdate(ADVKagManager instance, ADVStep step)
        {
            if (step.ListUpdateData != null)
            {
                if (step.ListUpdateData.Add != null)
                {
                    foreach (var addData in step.ListUpdateData.Add)
                    {
                        Maid maid = null;
                        List<Maid> targetList;
                        List<Maid> sourceList = null;
                        if (addData.Type == Constant.TargetType.ClubOwner)
                        {
                            maid = StateManager.Instance.ClubOwner;
                            targetList = StateManager.Instance.MenList;
                        }
                        else if (addData.Type == Constant.TargetType.SingleMan)
                        {
                            sourceList = StateManager.Instance.MenList;
                            targetList = StateManager.Instance.MenList;
                        }
                        else if (addData.Type == Constant.TargetType.NPCMale)
                        {
                            sourceList = StateManager.Instance.NPCManList;
                            targetList = StateManager.Instance.MenList;
                        }
                        else if (addData.Type == Constant.TargetType.ConvertedMaid)
                        {
                            sourceList = StateManager.Instance.SelectedMaidsList;
                            targetList = StateManager.Instance.MenList;
                        }
                        else
                        {
                            sourceList = StateManager.Instance.NPCList;
                            targetList = StateManager.Instance.SelectedMaidsList;
                        }

                        if (sourceList != null)
                        {
                            if (sourceList.Count <= addData.SrcPosition)
                                continue;
                            maid = sourceList[addData.SrcPosition];
                        }

                        if (maid == null)
                            continue;

                        if (targetList.Contains(maid))
                            targetList.Remove(maid);
                        targetList.Insert(addData.PositionToInsert, maid);
                    }
                }

                if (step.ListUpdateData.Remove != null)
                {
                    foreach (var removeData in step.ListUpdateData.Remove)
                    {
                        Maid maid = null;
                        List<Maid> targetList;
                        List<Maid> sourceList = null;
                        if (removeData.Type == Constant.TargetType.ClubOwner)
                        {
                            maid = StateManager.Instance.ClubOwner;
                            targetList = StateManager.Instance.MenList;
                        }
                        else if (removeData.Type == Constant.TargetType.SingleMan)
                        {
                            sourceList = StateManager.Instance.MenList;
                            targetList = StateManager.Instance.MenList;
                        }
                        else if (removeData.Type == Constant.TargetType.NPCMale)
                        {
                            sourceList = StateManager.Instance.NPCManList;
                            targetList = StateManager.Instance.MenList;
                        }
                        else if (removeData.Type == Constant.TargetType.ConvertedMaid)
                        {
                            sourceList = StateManager.Instance.SelectedMaidsList;
                            targetList = StateManager.Instance.MenList;
                        }
                        else
                        {
                            sourceList = StateManager.Instance.NPCList;
                            targetList = StateManager.Instance.SelectedMaidsList;
                        }

                        if (sourceList != null)
                        {
                            if (sourceList.Count <= removeData.SrcPosition)
                                continue;
                            maid = sourceList[removeData.SrcPosition];
                        }

                        if (maid == null)
                            continue;

                        targetList.Remove(maid);
                    }
                }
            }
        }

        internal static void ProcessADVTimeWait(ADVKagManager instance, ADVStep step)
        {
            Double secondToWait = Double.Parse(step.Tag);
            if (Config.DebugIgnoreADVForceTimeWait)
                secondToWait = 0;

            StateManager.Instance.ADVResumeTime = DateTime.Now.AddSeconds(secondToWait);
        }

        internal static void ProcessADVConvertSex(ADVKagManager instance, ADVStep step)
        {
            if (step.ConvertSexData == null)
                return;
            ConvertSex(step.ConvertSexData.ToMale, true);
            ConvertSex(step.ConvertSexData.ToFemale, false);
            RevertSexConversion(step.ConvertSexData.BackToMale, true);
            RevertSexConversion(step.ConvertSexData.BackToFemale, false);
        }

        private static void ConvertSex(List<ADVStep.ConvertSex.ConvertSexDetail> convertList, bool toMan)
        {
            if (convertList == null)
                return;

            foreach (ADVStep.ConvertSex.ConvertSexDetail info in convertList)
            {
                List<Maid> targetList = Util.GetTargetList(info.Type);
                if (targetList.Count > info.ArrayPosition)
                {
                    Maid target = targetList[info.ArrayPosition];
                    if (toMan)
                        CharacterHandling.ConvertMaidToManStructure(target, StateManager.Instance.PairedManForMaidList[target]);
                    else
                        CharacterHandling.ConvertManToFemaleStructure(target);

                    StateManager.Instance.WaitForFullLoadList.Add(target);
                }
            }
        }

        private static void RevertSexConversion(List<ADVStep.ConvertSex.ConvertSexDetail> convertList, bool revertToMan)
        {
            if (convertList == null)
                return;

            foreach (ADVStep.ConvertSex.ConvertSexDetail info in convertList)
            {
                List<Maid> targetList = Util.GetTargetList(info.Type);
                if (targetList.Count > info.ArrayPosition)
                {
                    Maid target = targetList[info.ArrayPosition];
                    if (revertToMan)
                        CharacterHandling.RecoverManFromFemaleStructure(target);
                    else
                        CharacterHandling.RecoverMaidFromManStructure(target);

                    StateManager.Instance.WaitForFullLoadList.Add(target);
                }
            }
        }


        internal static void ADVSceneProceedToNextStep(string nextStepID = "")
        {
            if (StateManager.Instance.UndergoingModEventID <= 0 || string.IsNullOrEmpty(StateManager.Instance.CurrentADVStepID))
                return;

            if (nextStepID == "")
                StateManager.Instance.CurrentADVStepID = ModUseData.ADVStepData[StateManager.Instance.UndergoingModEventID][StateManager.Instance.CurrentADVStepID].NextStepID;
            else
                StateManager.Instance.CurrentADVStepID = nextStepID;

            StateManager.Instance.WaitForUserClick = false;
            StateManager.Instance.WaitForUserInput = false;
            StateManager.Instance.WaitForCameraPanFinish = false;

        }

        private static void InitMaidPairMan()
        {
            StateManager.Instance.PairedManForMaidList = new Dictionary<Maid, Maid>();
            int counter = 100;
            List<Maid> m_listStockMan = Traverse.Create(GameMain.Instance.CharacterMgr).Field(Constant.DefinedClassFieldNames.CharacterMgrStockManList).GetValue<List<Maid>>();
            foreach (Maid maid in StateManager.Instance.SelectedMaidsList)
            {
                Maid man = CharacterHandling.InitMan(counter, ModUseData.ManBodyInfoList.Keys.ToList());
                StateManager.Instance.PairedManForMaidList.Add(maid, man);
                m_listStockMan.Remove(man);
                counter += 1;
            }   
        }

        private static List<string> GetValidManTypes(ADVStep.CharaInit charaInitInfo)
        {
            if (string.IsNullOrEmpty(charaInitInfo.ValidManConfigKey))
                return charaInitInfo.ValidManType;
            else
            {
                PropertyInfo prop = typeof(Config).GetProperty(charaInitInfo.ValidManConfigKey, BindingFlags.Static | BindingFlags.NonPublic);

                Config.ManTypeOption manTypeConfigValue = (Config.ManTypeOption)prop.GetValue(null, null);

                if (manTypeConfigValue == Config.ManTypeOption.Default)
                    return charaInitInfo.ValidManType;

                List<string> result = new List<string>();

                string[] typesInString = manTypeConfigValue.ToString().Split(',');
                foreach (string t in typesInString)
                    result.Add(t.Trim());

                return result;
            }
        }





        private static ScriptManagerFast.KagTagSupportFast CreateChangeBackgroundTag(string bgname)
        {
            ScriptManagerFast.KagTagSupportFast tag = new ScriptManagerFast.KagTagSupportFast();
            tag.AddTagProperty("file", bgname);
            tag.AddTagProperty("tagname", "setbg");
            return tag;
        }

        private static void DisplayAdvText(ADVKagManager advMgr, string speaker_name, string text, string voice_file, int pitch, AudioSourceMgr.Type type)
        {
            ScriptManagerFast.KagTagSupportFast tag = new ScriptManagerFast.KagTagSupportFast();
            tag.AddTagProperty("tagname", "talk");

            advMgr.MessageWindowMgr.SetText(speaker_name, PrepareDialogueText(text), voice_file, pitch, type);
            advMgr.TagTalk(tag);
        }

        private static void CheckCameraPanFinish()
        {
            //Check if camera pan is finish. The checking is put here due to the player could have chosen "skip" in the scene and thus the camera pan motion is not triggered
            if (StateManager.Instance.WaitForCameraPanFinish)
            {
                if (StateManager.Instance.TargetCameraAfterAnimation != null)
                {
                    //check the camera value against the target set
                    if (Util.NearlyEquals(GameMain.Instance.MainCamera.GetAroundAngle(), StateManager.Instance.TargetCameraAfterAnimation.AroundAngle)
                        && Util.NearlyEquals(GameMain.Instance.MainCamera.GetDistance(), StateManager.Instance.TargetCameraAfterAnimation.Distance)
                        && Util.NearlyEquals(GameMain.Instance.MainCamera.GetTargetPos(), StateManager.Instance.TargetCameraAfterAnimation.TargetPosition))
                    {
                        ADVSceneProceedToNextStep();
                    }
                }
            }
        }

        private static void CheckTimeWaitFinish()
        {
            if ((DateTime.Now > StateManager.Instance.ADVResumeTime && StateManager.Instance.ADVResumeTime != DateTime.MinValue))
            {
                StateManager.Instance.ADVResumeTime = DateTime.MinValue;
                ADVSceneProceedToNextStep();
            }
        }

        private static void CheckSystemFadeOutFinish()
        {
            if (StateManager.Instance.WaitForSystemFadeOut)
            {
                if (GameMain.Instance.MainCamera.GetFadeState() == CameraMain.FadeState.Out)
                {
                    StateManager.Instance.WaitForSystemFadeOut = false;
                    ADVSceneProceedToNextStep();
                }
            }
        }

        private static void ADVMotionChangeComplete()
        {
            StateManager.Instance.WaitForMotionChange = false;
        }
    }
}
