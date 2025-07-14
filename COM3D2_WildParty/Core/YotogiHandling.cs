using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx.Logging;
using HarmonyLib;
using COM3D2.WildParty.Plugin.Trigger;

namespace COM3D2.WildParty.Plugin.Core
{
    internal class YotogiHandling
    {
        //Update the parameter window(screen top and right) for the case of partner exchange
        internal static void UpdateParameterView(Maid maid)
        {
            if (maid != null)
            {
                if (StateManager.Instance.YotogiManager != null)
                {
                    Traverse.Create(StateManager.Instance.YotogiManager).Field(Constant.DefinedClassFieldNames.YotogiManagerMaidField).SetValue(maid);
                    Traverse.Create(StateManager.Instance.YotogiManager.play_mgr).Field(Constant.DefinedClassFieldNames.YotogiPlayManagerMaidField).SetValue(maid);
                }
            }
        }


        //Function for setting the common sex motion
        internal static void ChangeBackgroundGroupSexPosition(PartyGroup group, int sexPosID, bool isRandomizeMotionLabelGroup, bool playAudio = true, bool isSmoothAnimation = true)
        {
            //Reset some of the data
            group.DetachAllIK();
            group.GroupOffsetVector = Vector3.zero;
            group.CurrentSexState = SexState.StateType.NormalPlay;

            if (group.IsAutomatedGroup)
                group.GenerateNextReviewTime();

            var motionItem = ModUseData.BackgroundMotionList[group.GroupType].Where(x => x.ID == sexPosID).First();
            int excitementLevel = group.ExcitementLevel;


            if (isRandomizeMotionLabelGroup || group.CurrentLabelGroupID < 0)
            {
                //need to pick a label randomly
                List<int> labelOptionList = motionItem.ValidLabels.Select(x => x.LabelGroupID).Distinct().ToList();

                int rndLabel = RNG.Random.Next(labelOptionList.Count);

                group.CurrentLabelGroupID = labelOptionList[rndLabel];
            }

            var label = motionItem.ValidLabels.Where(x => x.LabelGroupID == group.CurrentLabelGroupID && x.ExcitementLevel == excitementLevel).First();

            group.SexPosID = motionItem.ID;
            string scriptFileName = motionItem.FileName;
            if (!string.IsNullOrEmpty(label.SpecialScriptName)) 
                scriptFileName = label.SpecialScriptName;

            CharacterHandling.LoadMotionScript(Util.GetPartyGroupIndex(group), false,
                scriptFileName, label.LabelName,
                group.Maid1.status.guid, group.Man1?.status.guid, false, false, true, false);
            
            group.ReloadAnimation(isSmoothAnimation);

            

            if (playAudio)
                CharacterHandling.SetGroupVoice(group);

            CharacterHandling.SetGroupFace(group);

            group.SetGroupPosition();

        }

        //Function for setting special motion (eg orgasm)
        internal static MotionSpecialLabel ChangeBackgroundGroupMotionWithSpecificLabel(PartyGroup group, string type)
        {
            //Reset data
            group.GroupOffsetVector = Vector3.zero;

            BackgroundGroupMotion.MotionItem motionItem = Util.GetMotionItemBySexPosID(group.SexPosID);

            List<MotionSpecialLabel> lstSpecialLabel = motionItem.SpecialLabels.Where(x => x.Type == type).ToList();

            MotionSpecialLabel pickedLabel = lstSpecialLabel[RNG.Random.Next(lstSpecialLabel.Count)];

            CharacterHandling.LoadMotionScript(Util.GetPartyGroupIndex(group), false,
                motionItem.FileName, pickedLabel.Label,
                group.Maid1.status.guid, group.Man1?.status.guid, false, false, true, false);

            if(type == SexState.StateType.Waiting)
                CheckYotogiMiscSetup(group);

            return pickedLabel;
        }


        internal static void SetGroupFormation(string formationID)
        {
            MapCoorindates coordsGroup = ModUseData.MapCoordinateList[formationID];
            
            //Pick the one that fit the most group
            MapCoorindates.CoordinateListInfo coords = coordsGroup.CoordinateList.Where(x => x.MaxGroup >= StateManager.Instance.PartyGroupList.Count).OrderBy(x => x.MaxGroup).First();

            List<MapCoorindates.CoordinatesInfo> ValidCoordinates = new List<MapCoorindates.CoordinatesInfo>();
            for (int i = 0; i < StateManager.Instance.PartyGroupList.Count; i++)
                ValidCoordinates.Add(coords.GroupCoordinates[i]);
            if (coordsGroup.IsRandom)
            {
                List<MapCoorindates.CoordinatesInfo> shuffled = new List<MapCoorindates.CoordinatesInfo>();
                while (ValidCoordinates.Count > 0)
                {
                    int rnd = RNG.Random.Next(ValidCoordinates.Count);
                    shuffled.Add(ValidCoordinates[rnd]);
                    ValidCoordinates.RemoveAt(rnd);
                }
                ValidCoordinates = shuffled;
                for (int i = 0; i < ValidCoordinates.Count; i++)
                    ValidCoordinates[i].ArrayPosition = i;
            }

            Vector3 positionOffset = Vector3.zero;
            Quaternion forceRotation = ValidCoordinates.Where(x => x.ArrayPosition == 0).First().Rot;
            if (coordsGroup.SpecialSetting != null)
                if (coordsGroup.SpecialSetting.MainGroupMotionOffset != null)
                {
                    positionOffset = coordsGroup.SpecialSetting.MainGroupMotionOffset.Pos;
                    forceRotation = coordsGroup.SpecialSetting.MainGroupMotionOffset.Rot;
                }

            for (int i = 0; i < StateManager.Instance.PartyGroupList.Count; i++)
            {
                var currentGroup = StateManager.Instance.PartyGroupList[i];
                var coord = ValidCoordinates.Where(x => x.ArrayPosition == i).First();

                //Get a valid position
                if (i == 0)
                    currentGroup.SetGroupPosition(coord.Pos + positionOffset, forceRotation);
                else
                    currentGroup.SetGroupPosition(coord.Pos, coord.Rot);

            }

            //Set individual position
            if (coords.IndividualCoordinates != null)
            {
                foreach (var item in coords.IndividualCoordinates)
                {
                    Maid targetMaid = null;
                    if (item.Type == Constant.IndividualCoordinateType.Maid)
                    {
                        if (StateManager.Instance.YotogiWorkingMaidList.Count > item.ArrayPosition)
                            targetMaid = StateManager.Instance.YotogiWorkingMaidList[item.ArrayPosition];
                    }
                    else if (item.Type == Constant.IndividualCoordinateType.Man)
                    {
                        if (StateManager.Instance.YotogiWorkingMaidList.Count > item.ArrayPosition)
                            targetMaid = StateManager.Instance.YotogiWorkingManList[item.ArrayPosition];
                    }
                    else if (item.Type == Constant.IndividualCoordinateType.ADVMaid)
                    {
                        if (StateManager.Instance.SelectedMaidsList.Count > item.ArrayPosition)
                            targetMaid = StateManager.Instance.SelectedMaidsList[item.ArrayPosition];
                    }
                    else if (item.Type == Constant.IndividualCoordinateType.ADVMan)
                    {
                        if (StateManager.Instance.MenList.Count > item.ArrayPosition)
                            targetMaid = StateManager.Instance.MenList[item.ArrayPosition];
                    }
                    else if (item.Type == Constant.IndividualCoordinateType.NPCMale)
                    {
                        if (StateManager.Instance.NPCManList.Count > item.ArrayPosition)
                            targetMaid = StateManager.Instance.NPCManList[item.ArrayPosition];
                    }
                    else if (item.Type == Constant.IndividualCoordinateType.NPCFemale)
                    {
                        if (StateManager.Instance.NPCManList.Count > item.ArrayPosition)
                            targetMaid = StateManager.Instance.NPCList[item.ArrayPosition];
                    }
                    else if (item.Type == Constant.IndividualCoordinateType.Owner)
                        targetMaid = StateManager.Instance.ClubOwner;


                    if(targetMaid != null)
                    {
                        targetMaid.transform.localPosition = Vector3.zero;
                        targetMaid.transform.position = item.Pos;
                        targetMaid.transform.rotation = item.Rot;
                        targetMaid.body0.SetBoneHitHeightY(item.Pos.y);
                    }
                }
            }

            //Set special position
            if (coordsGroup.SpecialCoordinates != null)
            {
                foreach (var item in coordsGroup.SpecialCoordinates)
                {
                    if (item.Type == Constant.SpecialCoordinateType.Owner)
                    {
                        StateManager.Instance.ClubOwner.transform.localPosition = Vector3.zero;
                        StateManager.Instance.ClubOwner.transform.position = item.Pos;
                        StateManager.Instance.ClubOwner.transform.rotation = item.Rot;
                        StateManager.Instance.ClubOwner.body0.SetBoneHitHeightY(item.Pos.y);
                    }
                    else if (item.Type == Constant.SpecialCoordinateType.UnassignedMaid)
                    {
                        if (PartyGroup.UnassignedMaid != null)
                        {
                            PartyGroup.UnassignedMaid.transform.localPosition = Vector3.zero;
                            PartyGroup.UnassignedMaid.transform.position = item.Pos;
                            PartyGroup.UnassignedMaid.transform.rotation = item.Rot;
                            PartyGroup.UnassignedMaid.body0.SetBoneHitHeightY(item.Pos.y);
                        }
                    }
                }
            }


            //For extra man assigned to the group
            if (coords.ExtraManInfo != null)
            {
                PartyGroup.SharedExtraManSetupInfo = new List<MapCoorindates.CoordinatesInfo>();

                foreach (var extraManInfo in coords.ExtraManInfo)
                {
                    if (extraManInfo.GroupIndex < 0)
                        PartyGroup.SharedExtraManSetupInfo.Add(extraManInfo);
                    else
                    {
                        StateManager.Instance.PartyGroupList[extraManInfo.GroupIndex].ExtraManSetupInfo.Insert(extraManInfo.ArrayPosition, extraManInfo);
                    }
                }
                PartyGroup.SetSharedExtraManPosition();
            }
        }

        internal static void SetGroupToScene(bool playAudio = true)
        {
            var coordsGroup = ModUseData.MapCoordinateList[PartyGroup.CurrentFormation];

            SetGroupFormation(PartyGroup.CurrentFormation);

            MapCoorindates.CoordinateListInfo coordinateListInfo = coordsGroup.CoordinateList.Where(x => x.MaxGroup >= StateManager.Instance.PartyGroupList.Count).OrderBy(x => x.MaxGroup).First();
            
            for (int i = 0; i < StateManager.Instance.PartyGroupList.Count; i++)
            {
                var currentGroup = StateManager.Instance.PartyGroupList[i];
                MapCoorindates.CoordinatesInfo groupCoordinateInfo = coordinateListInfo.GroupCoordinates.Where(x => x.ArrayPosition == i).First();

                if (i > 0)
                {
                    currentGroup.IsIndependentExcitement = groupCoordinateInfo.IsIndependentExcitement;
                    if (!currentGroup.IsAutomatedGroup)
                        currentGroup.StopNextReviewTime();
                    
                    if (groupCoordinateInfo.ForceSexPos == null)
                    {
                        if (ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].BackgroundSexPosID < 0)
                        {
                            //Randomize motion case
                            var lstMotion = ModUseData.BackgroundMotionList[currentGroup.GroupType].Where(x => x.Phase == StateManager.Instance.YotogiPhase && x.IsBGGroupUse).ToList();
                            int rndMotion = RNG.Random.Next(lstMotion.Count);

                            ChangeBackgroundGroupSexPosition(currentGroup, lstMotion[rndMotion].ID, true, playAudio);
                        }
                        else
                        {
                            //A background sex group ID is set, use that instead
                            ChangeBackgroundGroupSexPosition(currentGroup, ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].BackgroundSexPosID, true, playAudio);
                        }
                    }
                    else
                    {
                        currentGroup.ForceSexPos = new ForceSexPosInfo();
                        currentGroup.ForceSexPos.NormalPlay = groupCoordinateInfo.ForceSexPos.NormalPlay;
                        currentGroup.ForceSexPos.Waiting = groupCoordinateInfo.ForceSexPos.Waiting;
                        currentGroup.ForceSexPos.Orgasm = groupCoordinateInfo.ForceSexPos.Orgasm;

                        //A SexPosID is assigned to this group
                        ChangeBackgroundGroupSexPosition(currentGroup, currentGroup.ForceSexPos.Waiting, true, playAudio);
                    }
                    
                    CheckYotogiMiscSetup(currentGroup);
                    
                    if (!groupCoordinateInfo.IsManVisible)
                    {
                        if (currentGroup.Man1 != null)
                            currentGroup.Man1.transform.localScale = Vector3.zero;
                        if (currentGroup.Man2 != null)
                            currentGroup.Man2.transform.localScale = Vector3.zero;
                    }
                    

                }

                if (groupCoordinateInfo.EyeSight != null)
                {
                    currentGroup.ForceEyeSight = new List<EyeSightSetting>();
                    foreach (var eyeSightSetting in groupCoordinateInfo.EyeSight)
                    {
                        currentGroup.ForceEyeSight.Add(eyeSightSetting);
                        CustomADVProcessManager.SetCharacterEyeSight(currentGroup, eyeSightSetting);
                    }
                }
                
                if (groupCoordinateInfo.IKAttach != null)
                {
                    currentGroup.ForceIKAttach = new List<IKAttachInfo>();
                    foreach (var ikInfo in groupCoordinateInfo.IKAttach)
                    {
                        currentGroup.ForceIKAttach.Add(ikInfo);
                        CharacterHandling.IKAttachBone(ikInfo);
                    }
                }
            }

            //Set Individual Motion
            
            if (coordinateListInfo.IndividualCoordinates != null)
            {
                foreach (var item in coordinateListInfo.IndividualCoordinates)
                {
                    Maid targetMaid = null;
                    if (item.Type == Constant.IndividualCoordinateType.Maid)
                    {
                        if (StateManager.Instance.YotogiWorkingMaidList.Count > item.ArrayPosition)
                            targetMaid = StateManager.Instance.YotogiWorkingMaidList[item.ArrayPosition];
                    }
                    else if (item.Type == Constant.IndividualCoordinateType.Man)
                    {
                        if (StateManager.Instance.YotogiWorkingMaidList.Count > item.ArrayPosition)
                            targetMaid = StateManager.Instance.YotogiWorkingManList[item.ArrayPosition];
                    }
                    else if (item.Type == Constant.IndividualCoordinateType.ADVMaid)
                    {
                        if (StateManager.Instance.SelectedMaidsList.Count > item.ArrayPosition)
                            targetMaid = StateManager.Instance.SelectedMaidsList[item.ArrayPosition];
                    }
                    else if (item.Type == Constant.IndividualCoordinateType.ADVMan)
                    {
                        if (StateManager.Instance.MenList.Count > item.ArrayPosition)
                            targetMaid = StateManager.Instance.MenList[item.ArrayPosition];
                    }
                    else if (item.Type == Constant.IndividualCoordinateType.NPCMale)
                    {
                        if (StateManager.Instance.NPCManList.Count > item.ArrayPosition)
                            targetMaid = StateManager.Instance.NPCManList[item.ArrayPosition];
                    }
                    else if (item.Type == Constant.IndividualCoordinateType.NPCFemale)
                    {
                        if (StateManager.Instance.NPCManList.Count > item.ArrayPosition)
                            targetMaid = StateManager.Instance.NPCList[item.ArrayPosition];
                    }
                    else if (item.Type == Constant.IndividualCoordinateType.Owner)
                        targetMaid = StateManager.Instance.ClubOwner;
                    
                    if (targetMaid != null)
                    {
                        CharacterHandling.ApplyMotionInfoToCharacter(targetMaid, item.Motion);

                        if (!string.IsNullOrEmpty(item.FaceAnime))
                            CustomADVProcessManager.SetFaceAnimeToMaid(targetMaid, item.FaceAnime);

                        if (item.EyeSight != null)
                        {
                            foreach (var eyeSightSetting in item.EyeSight)
                            {
                                CustomADVProcessManager.SetCharacterEyeSight(targetMaid, eyeSightSetting);
                            }
                        }

                        CharacterHandling.AttachObjectToCharacter(targetMaid, item.ExtraItemObjects);
                    }
                }
            }

            if (coordinateListInfo.ExtraManInfo != null)
            {
                foreach(var extraManInfo in coordinateListInfo.ExtraManInfo)
                {
                    if (extraManInfo.GroupIndex < 0)
                    {
                        //Shared list
                        if (PartyGroup.SharedExtraManList.Count > extraManInfo.ArrayPosition)
                        {
                            CharacterHandling.ApplyMotionInfoToCharacter(PartyGroup.SharedExtraManList[extraManInfo.ArrayPosition], extraManInfo.Motion);
                        }
                    }
                    else
                    {
                        if (StateManager.Instance.PartyGroupList[extraManInfo.GroupIndex].ExtraManList.Count > extraManInfo.ArrayPosition)
                        {
                            CharacterHandling.ApplyMotionInfoToCharacter(StateManager.Instance.PartyGroupList[extraManInfo.GroupIndex].ExtraManList[extraManInfo.ArrayPosition], extraManInfo.Motion);
                        }
                    }
                }
            }

            //Handle the case of unassigned maid
            if (PartyGroup.UnassignedMaid != null)
            {
                if (coordsGroup.SpecialCoordinates.Where(x => x.Type == Constant.SpecialCoordinateType.UnassignedMaid).First().IsMasturbation)
                    SetMasturbMotionToCharacter(PartyGroup.UnassignedMaid, MasturbationMotion.Type.MaidOnFloor);
            }

            //for some unknown reason the setpos above may not work, try to set it here once more
            Util.ResetAllGroupPosition();

            MapCoorindates.CoordinatesInfo groupZeroCoordinateInfo = coordinateListInfo.GroupCoordinates.Where(x => x.ArrayPosition == 0).First();
            if (groupZeroCoordinateInfo.CameraSetup != null)
            {
                CameraHandling.SetCameraLookAtFixedPoint(groupZeroCoordinateInfo.CameraSetup);
            }
            else
                CameraHandling.SetCameraLookAt(StateManager.Instance.PartyGroupList[0].Maid1);
        }

        internal static void SetMasturbMotionToCharacter(Maid maid, string type, bool isMan = false)
        {
            if (maid == null)
                return;

            //Just in case it is set to not visible in adv scene
            maid.Visible = true;

            var mMotionData = ModUseData.MasturbationMotionList[type];
            if (isMan)
            {
                CharacterHandling.SetManClothing(maid, true);
                CharacterHandling.PlayAnimation(maid, mMotionData.FileName, mMotionData.Tag);
            }
            else
            {

                CharacterHandling.LoadMotionScript(0, false,
                mMotionData.MotionFileName, mMotionData.Label,
                maid.status.guid, "", false, false, false, false);

                var voiceData = ModUseData.PersonalityVoiceList[maid.status.personal.id].NormalPlayVoice.Where(x => x.MotionType == mMotionData.VoiceType && x.ExcitementLevel == 2).First();
                maid.AudioMan.LoadPlay(voiceData.NormalFile, 0, false, true);
                maid.AudioMan.standAloneVoice = true;

            }
        }


        /*
         * skillID: The id defined by the game (different per personality). Not the same as the sexPosID defined by the mod
         * loadMotionScript: In some cases (eg Move left/right button), the animation is already loaded for the characters. Calling the LoadMotionScript may cause the character flicks for unknown reason. Set it to false to avoid it.
         */
        internal static void ChangeMainGroupSkill(string skillID, bool loadMotionScript = true, bool resetEstrus = false)
        {
            StateManager.Instance.PartyGroupList[0].DetachAllIK();

            //We dont want the game to deactivate any maid object
            StateManager.Instance.SpoofActivateMaidObjectFlag = true;
            
            //Mark the flag to ensure the extra command is added properly
            MarkInjectedButtonsDirty();

            int personality = StateManager.Instance.PartyGroupList[0].Maid1.status.personal.id;
            string targetGroupType = Util.GetSkillGroupType(StateManager.Instance.PartyGroupList[0], skillID);
            var selectedSkill = ModUseData.ValidSkillList[personality][targetGroupType].Where(x => x.YotogiSkillID == skillID).First();
            
            CheckSpecialRuleOnYotogiSkill(StateManager.Instance.PartyGroupList[0], selectedSkill.SexPosID);
            
            //Check if change of number of man is needed

            if (targetGroupType != StateManager.Instance.PartyGroupList[0].GroupType)
            {
                ConvertToGroupType(StateManager.Instance.PartyGroupList[0], targetGroupType, selectedSkill.SexPosID);
            }

            
            string groupType = StateManager.Instance.PartyGroupList[0].GroupType;

            StateManager.Instance.PartyGroupList[0].SexPosID = selectedSkill.SexPosID;
            StateManager.Instance.PartyGroupList[0].GroupOffsetVector = Vector3.zero;

            if (resetEstrus)
                Traverse.Create(StateManager.Instance.YotogiManager.play_mgr).Field(Constant.DefinedClassFieldNames.YotogiPlayManagerEstrusMode).SetValue(false);
            
            var data = Yotogis.Skill.Get(int.Parse(selectedSkill.YotogiSkillID));

            //create a new skill array with only the selected skill
            KeyValuePair<Yotogis.Skill.Data, bool>[] kvps = new KeyValuePair<Yotogis.Skill.Data, bool>[1];
            kvps[0] = new KeyValuePair<Yotogis.Skill.Data, bool>(data, true);
            StateManager.Instance.YotogiManager.SetPlaySkillArray(kvps);
            

            //reset the playing skill no
            StateManager.Instance.YotogiManager.null_mgr.SetNextLabel("");
            StateManager.Instance.YotogiManager.play_mgr.AdvanceSkillMove();

            //reset the player state
            Traverse.Create(StateManager.Instance.YotogiManager.play_mgr).Field(Constant.DefinedClassFieldNames.YotogiPlayManagerPlayerState).SetValue(YotogiPlay.PlayerState.Normal);
            
            //refresh the command
            StateManager.Instance.YotogiManager.play_mgr.NextSkill();

            //Reset the Current Command Click in ProgressInfo
            foreach (var progressInfo in StateManager.Instance.YotogiProgressInfoList)
                progressInfo.Value.CurrentCommandID = -1;

            StateManager.Instance.SpoofActivateMaidObjectFlag = false;

            //Load the motion for the main group
            BackgroundGroupMotion.MotionItem motionItem = null;
            foreach (var kvp in ModUseData.BackgroundMotionList)
            {
                var hit = kvp.Value.Where(x => x.ID == selectedSkill.SexPosID).ToList();
                if (hit.Count > 0)
                {
                    motionItem = hit.First();
                    break;
                }
            }
            
            YotogiHandling.CheckYotogiMiscSetup(StateManager.Instance.PartyGroupList[0]);

            SetGroupWaitingMotion(StateManager.Instance.PartyGroupList[0], motionItem, loadMotionScript);
        }

        //A function for handling some very special case which is difficult to put in json
        private static void CheckSpecialRuleOnYotogiSkill(PartyGroup group, int targetSexPosID)
        {
            if(StateManager.Instance.UndergoingModEventID == ScenarioIDList.HappyGBClubScenarioID && targetSexPosID == 2)
            {
                group.ForceCharacterVisibleOnPositionChange = false;
                group.Man2.Visible = false;
            }
            else
                group.ForceCharacterVisibleOnPositionChange = true;
        }

        internal static void SetGroupWaitingMotion(PartyGroup group, BackgroundGroupMotion.MotionItem motionItem, bool loadMotionScript = true)
        {

            
            MotionSpecialLabel waitingLabel = motionItem.SpecialLabels.Where(x => x.Type == MotionSpecialLabel.LabelType.Waiting).First();

            string maidGUID = "";
            string manGUID = "";
            if(group != StateManager.Instance.PartyGroupList[0])
            {
                maidGUID = group.Maid1.status.guid;
                manGUID = group.Man1.status.guid;
            }
            
            if (loadMotionScript)
                CharacterHandling.LoadMotionScript(Util.GetPartyGroupIndex(group), false,
                motionItem.FileName, waitingLabel.Label,
                maidGUID, manGUID, false, false, false, false);
            
            bool isEstrus = Traverse.Create(StateManager.Instance.YotogiManager.play_mgr).Field(Constant.DefinedClassFieldNames.YotogiPlayManagerEstrusMode).GetValue<bool>();
            CharacterHandling.SetCharacterVoiceEntry(group.Maid1, PersonalityVoice.VoiceEntryType.Waiting, group.ExcitementLevel, waitingLabel.VoiceType1, isEstrus, group.IsVoicelessGroup);
            CharacterHandling.SetCharacterVoiceEntry(group.Maid2, PersonalityVoice.VoiceEntryType.Waiting, group.ExcitementLevel, waitingLabel.VoiceType2, isEstrus, group.IsVoicelessGroup);
            
            group.Maid1VoiceType = waitingLabel.VoiceType1;
            if (group.Maid2 != null)
                group.Maid2VoiceType = waitingLabel.VoiceType2;
            
            CharacterHandling.SetGroupFace(group);
        }

        internal static void MarkInjectedButtonsDirty()
        {
            StateManager.Instance.YotogiCommandFactory = null;
            StateManager.Instance.RequireInjectCommandButtons = true;

            foreach (var btn in StateManager.Instance.InjectedButtons)
            {
                btn.Button.SetActive(false);
                btn.Button.transform.SetParent(null);
                btn.Button.transform.localScale = Vector3.zero;
                GameObject.Destroy(btn.Button);
            }

            StateManager.Instance.InjectedButtons.Clear();
        }

        internal static BackgroundGroupMotion.MotionItem GetRandomMotion(string groupType)
        {
            System.Random random = new System.Random();
            int rnd = random.Next(ModUseData.BackgroundMotionList[groupType].Count);
            return ModUseData.BackgroundMotionList[groupType][rnd];
        }

        internal static PlayableSkill.SkillItem GetSkill(int personality, string groupType, int sexPosID = -1)
        {
            if (sexPosID < 0)
            {
                System.Random random = new System.Random();
                int rnd = random.Next(ModUseData.ValidSkillList[personality][groupType].Count);
                return ModUseData.ValidSkillList[personality][groupType][rnd];
            }
            else
            {
                return ModUseData.ValidSkillList[personality][groupType].Where(x => x.SexPosID == sexPosID).First();
            }
        }

        internal static void PlayRoomBGM(YotogiManager manager)
        {
            ScriptManagerFast.KagTagSupportFast tagBGM = TagSupportData.ConvertDictionaryToTagSupportType(TagSupportData.GetTagPlayBGMRoom());
            manager.TagPlayBGMRoom(tagBGM);
        }

        internal static void SetupYotogiSceneInitialSkill(int defaultSexPosID)
        {            
            PlayableSkill.SkillItem initialSkill;
            initialSkill = GetSkill(StateManager.Instance.PartyGroupList[0].Maid1.status.personal.id, StateManager.Instance.PartyGroupList[0].GroupType, defaultSexPosID);

            StateManager.Instance.PartyGroupList[0].SexPosID = initialSkill.SexPosID;
        }

        internal static void YotogiSkillCall(YotogiManager manager, int defaultSexPosID)
        {
            PlayableSkill.SkillItem initialSkill = Util.GetGroupCurrentSkill(StateManager.Instance.PartyGroupList[0]);
            ScriptManagerFast.KagTagSupportFast tag = TagSupportData.ConvertDictionaryToTagSupportType(TagSupportData.GetTagForYotogiSkillPlay(initialSkill.YotogiSkillID));
            manager.TagYotogiCall(tag);
        }

        internal static void InitYotogiData()
        {
            //Load up new sex state rules
            string sexStateRule = Util.GetUndergoingScenario().YotogiSetup.Where(x => x.Phase == StateManager.Instance.YotogiPhase).First().SexStateRule;
            ModUseData.LoadSexStateRule(sexStateRule);

            BackgroundGroupMotionManager.InitNextReviewTimer();
            YotogiHandling.UpdateParameterView(StateManager.Instance.PartyGroupList[0].Maid1);

            RandomizeMaidExcitement(StateManager.Instance.SelectedMaidsList);

            foreach (var man in StateManager.Instance.MenList)
            {
                CharacterHandling.SetManClothing(man, true);
            }
        }

        internal static void InitArrayForYotogiUsed()
        {
            StateManager.Instance.YotogiWorkingMaidList = new List<Maid>(StateManager.Instance.SelectedMaidsList);
            StateManager.Instance.YotogiWorkingManList = new List<Maid>(StateManager.Instance.MenList);
        }

        //This function is for yotogi scene that maid will act as a man
        internal static void InitArrayForYotogiUsed(int maidConvertToManRatioInPercentage)
        {
            if(maidConvertToManRatioInPercentage > 0)
            {
                StateManager.Instance.YotogiWorkingMaidList = new List<Maid>();
                StateManager.Instance.YotogiWorkingManList = new List<Maid>();

                List<Maid> shuffleList = new List<Maid>(StateManager.Instance.SelectedMaidsList);
                shuffleList = CharacterHandling.ShuffleMaidOrManList(shuffleList);
                int convertedMaidCount = shuffleList.Count * maidConvertToManRatioInPercentage / 100;
                
                for (int i = 0; i < convertedMaidCount; i++)
                {
                    CharacterHandling.ConvertMaidToManStructure(shuffleList[i], StateManager.Instance.PairedManForMaidList[shuffleList[i]]);
                    StateManager.Instance.YotogiWorkingManList.Add(shuffleList[i]);

                    RemoveMaidFromMaidArray(shuffleList[i]);
                }

                for (int i = convertedMaidCount; i < shuffleList.Count; i++)
                {
                    StateManager.Instance.YotogiWorkingMaidList.Add(shuffleList[i]);
                    RemoveMaidFromMaidArray(shuffleList[i]);
                }
            }
            else
            {
                InitArrayForYotogiUsed();
            }
        }

        //Having the maid in the maid array while it is not in main group may result in wrong animation. This function aims to resolve this issue.
        private static void RemoveMaidFromMaidArray(Maid maid)
        {
            for (int j = 0; j < GameMain.Instance.CharacterMgr.GetMaidCount(); j++)
            {
                Maid arrayMaid = GameMain.Instance.CharacterMgr.GetMaid(j);

                if (arrayMaid != null)
                {

                    if (arrayMaid.GetInstanceID() == maid.GetInstanceID())
                    {
                        //this maid has converted to man structure, need to replace it in the array or else error will be thrown in YotogiPlayManager.OnCall

                        Maid dummyMaid = GameMain.Instance.CharacterMgr.GetStockNpcMaid(0);
                        StateManager.Instance.SpoofActivateMaidObjectFlag = true;
                        GameMain.Instance.CharacterMgr.SetActiveMaid(dummyMaid, j);
                        StateManager.Instance.SpoofActivateMaidObjectFlag = false;

                    }
                }
            }
        }

        internal static void RandomizeMaidExcitement(List<Maid> maidList, bool isMaidZeroChange = false)
        {
            string maid_0_guid = GameMain.Instance.CharacterMgr.GetMaid(0).status.guid;
            foreach (var maid in maidList)
            {
                if (maid.status.guid == maid_0_guid)
                {
                    if (isMaidZeroChange)
                        maid.status.currentExcite = Util.GetRandomExcitementRate();
                }
                else
                {
                    maid.status.currentExcite = Util.GetRandomExcitementRate();
                }

            }
        }

        internal static void AddManOrgasmCountToMaid(Maid maid, Maid man)
        {
            if (maid == null || man == null)
                return;
            if (!StateManager.Instance.YotogiProgressInfoList.ContainsKey(maid.status.guid))
                return;

            YotogiProgressInfo info = StateManager.Instance.YotogiProgressInfoList[maid.status.guid];
            if (!info.ManOrgasmInfo.ContainsKey(man.status.guid))
                info.ManOrgasmInfo.Add(man.status.guid, 1);
            else
                info.ManOrgasmInfo[man.status.guid] += 1;
        }

        internal static void AddPositionOrgasmCountToMaid(PartyGroup group, Maid maid)
        {
            if (!StateManager.Instance.YotogiProgressInfoList.ContainsKey(maid.status.guid))
                return;

            YotogiProgressInfo info = StateManager.Instance.YotogiProgressInfoList[maid.status.guid];
            
            if (!info.SexPositionOrgasmInfo.ContainsKey(group.SexPosID))
                info.SexPositionOrgasmInfo.Add(group.SexPosID, 1);
            else
                info.SexPositionOrgasmInfo[group.SexPosID] += 1;

            info.MaidOrgasmCount += 1;
        }

        internal static void AddOrgasmCountForGroup(PartyGroup group)
        {
            for (int i = 0; i < group.MaidCount; i++)
            {
                AddPositionOrgasmCountToMaid(group, group.GetMaidAtIndex(i));
                for (int j = 0; j < group.ManCount; j++)
                    AddManOrgasmCountToMaid(group.GetMaidAtIndex(i), group.GetManAtIndex(j));
            }
        }

        internal static void ConvertToGroupType(PartyGroup group, string targetGroupType, int sexPosID)
        {
            int targetManCount = Util.GetGroupTypeMemberCount(targetGroupType, true);
            if(targetManCount > group.ManCount)
            {
                //Need more man for the group, pull from extra man list
                group.DetachAllIK();
                BackgroundGroupMotion.MotionItem motionItem = Util.GetMotionItemBySexPosID(sexPosID);
                while (group.ManCount < targetManCount)
                {
                    int indexToAdd = group.ManCount;
                    Maid manToAdd = null;
                    while(manToAdd == null)
                    {
                        manToAdd = PartyGroup.SharedExtraManList[RNG.Random.Next(PartyGroup.SharedExtraManList.Count)];
                    }

                    foreach (var kvp in PartyGroup.SharedExtraManList)
                        if (kvp.Value == manToAdd)
                        {
                            PartyGroup.SharedExtraManList[kvp.Key] = null;
                            break;
                        }

                    group.SetManAtIndex(indexToAdd, manToAdd);
                    
                    if (group == StateManager.Instance.PartyGroupList[0])
                    {
                        StateManager.Instance.SpoofActivateMaidObjectFlag = true;
                        GameMain.Instance.CharacterMgr.SetActiveMan(manToAdd, motionItem.ManIndex[indexToAdd]);
                        StateManager.Instance.SpoofActivateMaidObjectFlag = false;
                    }
                    
                    group.SetGroupPosition();
                }
            }
            else if(targetManCount < group.ManCount)
            {
                group.DetachAllIK();
                //Remove man from group and put it back to extra man list
                while(group.ManCount > targetManCount)
                {
                    int indexToRemove = group.ManCount - 1;
                    Maid man = group.GetManAtIndex(indexToRemove);
                    group.SetManAtIndex(indexToRemove, null);
                    List<int> emptySpot = PartyGroup.GetExtraManEmptySpotList();
                    int rndIndex = RNG.Random.Next(emptySpot.Count);
                    if (emptySpot.Count > 0)
                    {
                        PartyGroup.SharedExtraManList[emptySpot[rndIndex]] = man;
                        
                        var setupInfo = PartyGroup.SharedExtraManSetupInfo.Where(x => x.ArrayPosition == emptySpot[rndIndex]).First();
                        
                        man.transform.position = setupInfo.Pos;
                        man.transform.rotation = setupInfo.Rot;
                        man.body0.SetBoneHitHeightY(setupInfo.Pos.y);
                        CharacterHandling.ApplyMotionInfoToCharacter(man, setupInfo.Motion);
                    }
                }
            }
        }

        internal static void CheckTimeEndTrigger()
        {
            if (StateManager.Instance.ModEventProgress != Constant.EventProgress.YotogiPlay)
                return;

            if (StateManager.Instance.TimeEndTriggerList != null)
            {
                for(int i= StateManager.Instance.TimeEndTriggerList.Count - 1; i>=0; i--)
                {
                    if (DateTime.Now > StateManager.Instance.TimeEndTriggerList[i].DueTime)
                    {
                        var delegateToExecute = StateManager.Instance.TimeEndTriggerList[i].ToBeExecuted;
                        StateManager.Instance.TimeEndTriggerList.RemoveAt(i);
                        delegateToExecute.Execute();
                    }
                }

                
            }
        }

        internal static void CheckManWalkTrigger(Maid maid)
        {
            if (maid.boMAN)
                HardCodeMotion.ManWalkController.CheckManWalkingDistanceTrigger(maid);
        }

        internal static void ChangeManMembersShareListType(PartyGroup group, bool isKeepFirstMan = false)
        {
            BackgroundGroupMotion.MotionItem motionItem = Util.GetMotionItemBySexPosID(group.SexPosID);

            //Locate group index
            int groupIndex = 0;
            for(int i=0; i < StateManager.Instance.PartyGroupList.Count; i++)
            {
                if (StateManager.Instance.PartyGroupList[i] == group)
                {
                    groupIndex = i;
                    break;
                }
            }

            //Remove all existng IK 
            group.DetachAllIK();

            int manStartCounter = 0;
            if (isKeepFirstMan)
                manStartCounter = 1;
            
            for (int i = manStartCounter; i < group.ManCount; i++)
            {
                Maid toBeExtra = group.GetManAtIndex(i);
                int rnd = 0;
                Maid toBeMain = null;
                while (toBeMain == null)
                {
                    rnd = RNG.Random.Next(PartyGroup.SharedExtraManList.Count);
                    toBeMain = PartyGroup.SharedExtraManList[rnd];
                }
                
                PartyGroup.SharedExtraManList[rnd] = toBeExtra;

                group.SetManAtIndex(i, toBeMain);

                if (groupIndex == 0)
                {
                    StateManager.Instance.SpoofActivateMaidObjectFlag = true;
                    GameMain.Instance.CharacterMgr.SetActiveMan(toBeMain, motionItem.ManIndex[i]);
                    StateManager.Instance.SpoofActivateMaidObjectFlag = false;
                }

            }

            //reload motion for the group
            
            MotionSpecialLabel waitingLabel = motionItem.SpecialLabels.Where(x => x.Type == MotionSpecialLabel.LabelType.Waiting).First();

            //Change skill will reset the current process of the yotogi...
            YotogiHandling.SetGroupWaitingMotion(group, motionItem);

            foreach (var setupInfo in PartyGroup.SharedExtraManSetupInfo)
            {
                if (PartyGroup.SharedExtraManList.Count > setupInfo.ArrayPosition)
                {
                    Maid man = PartyGroup.SharedExtraManList[setupInfo.ArrayPosition];
                    CharacterHandling.ApplyMotionInfoToCharacter(man, setupInfo.Motion);
                }
            }

            //reassign position for the group
            MapCoorindates.CoordinateListInfo coordinateListInfo = ModUseData.MapCoordinateList[PartyGroup.CurrentFormation].CoordinateList.Where(x => x.MaxGroup >= StateManager.Instance.PartyGroupList.Count).OrderBy(x => x.MaxGroup).First();
            MapCoorindates.CoordinatesInfo coordinatesInfo = coordinateListInfo.GroupCoordinates.Where(x => x.ArrayPosition == groupIndex).First();
            group.SetGroupPosition(coordinatesInfo.Pos, coordinatesInfo.Rot);
        }

        //Check the YotogiMiscSetup to see if extra handling is required
        //internal static void CheckYotogiMiscSetup(string maidGUID)
        internal static void CheckYotogiMiscSetup(PartyGroup group)
        {
            if (StateManager.Instance.ModEventProgress == Constant.EventProgress.YotogiPlay)
            {
                if (group != null)
                {
                    ResetYotogiMiscSetup(group);
                    
                    if (ModUseData.YotogiMiscSetupList.ContainsKey(StateManager.Instance.UndergoingModEventID))
                    {       
                        YotogiMiscSetup miscSetup = ModUseData.YotogiMiscSetupList[StateManager.Instance.UndergoingModEventID].Where(x => x.SexPosIDs.Contains(group.SexPosID)).FirstOrDefault();
                        if (miscSetup != null)
                        {
                            ApplyYotogiMiscSetup(group, miscSetup);
                        }
                    }
                }
            }
        }

        internal static void ResetYotogiMiscSetup(PartyGroup group)
        {
            group.GroupOffsetVector2 = Vector3.zero;
            group.GroupRotationOffset = Vector3.zero;
            if (group.ExtraObjects != null)
            {
                while (group.ExtraObjects.Count > 0)
                {
                    string objName = group.ExtraObjects.Keys.First();
                    
                    GameObject obj = group.ExtraObjects[objName];
                    if (obj != null)
                    {
                        GameMain.Instance.BgMgr.DelPrefabFromBg(objName);
                    }
                    group.ExtraObjects.Remove(objName);
                    
                }

                while (group.ExtraCharacterObjects.Count > 0)
                {
                    Maid target = group.ExtraCharacterObjects.Keys.First();
                    CharacterHandling.RemoveObjectFromCharacter(target, group.ExtraCharacterObjects[target]);
                    
                    group.ExtraCharacterObjects.Remove(target);
                }
            }

        }

        internal static void ApplyYotogiMiscSetup(PartyGroup group, YotogiMiscSetup setupInfo)
        {
            if (setupInfo.Offset != null)
            {
                group.GroupOffsetVector2 = setupInfo.Offset.Pos;
                group.GroupRotationOffset = setupInfo.Offset.Rot;
            }
            if (setupInfo.ExtraObjects != null)
            {
                foreach (var extraObjectSetup in setupInfo.ExtraObjects)
                {
                    string objName = extraObjectSetup.Name + "_" + group.Maid1.status.guid;
                    GameObject addedObject = GameMain.Instance.BgMgr.AddPrefabToBg(extraObjectSetup.Src, objName, extraObjectSetup.Dest, Vector3.zero, Vector3.zero);
                    
                    if (addedObject != null)
                    {
                        MapCoorindates.CoordinatesInfo coordinatesInfo = Util.GetGroupCoordinateInfo(group);
                        
                        //we only care about the rotation in y-plane as x plane and z plane rotation makes no sense for yotogi motion.
                        float angle = group.Maid1.transform.rotation.eulerAngles.y;
                        Vector3 offsetRespectToRotation = Quaternion.AngleAxis(angle, Vector3.up) * extraObjectSetup.Offset.Pos;
                        
                        addedObject.transform.position = coordinatesInfo.Pos + offsetRespectToRotation;
                        addedObject.transform.rotation = Quaternion.Euler(coordinatesInfo.Rot.eulerAngles + extraObjectSetup.Offset.Rot);
                        
                        group.ExtraObjects.Add(objName, addedObject);
                    }
                }
            }

            if (setupInfo.ExtraObjectsOnCharacter != null)
            {
                foreach (var extraObjectOnCharSetup in setupInfo.ExtraObjectsOnCharacter)
                {
                    Maid target = null;
                    if (extraObjectOnCharSetup.CharacterType == Constant.CharacterType.Maid)
                        target = group.GetMaidAtIndex(extraObjectOnCharSetup.ArrayPosition);
                    else if (extraObjectOnCharSetup.CharacterType == Constant.CharacterType.Man)
                        target = group.GetManAtIndex(extraObjectOnCharSetup.ArrayPosition);
                    CharacterHandling.AttachObjectToCharacter(target, new List<ExtraItemObject> { extraObjectOnCharSetup.ItemInfo });

                    if(!group.ExtraCharacterObjects.ContainsKey(target))
                        group.ExtraCharacterObjects.Add(target, new List<string>());
                    group.ExtraCharacterObjects[target].Add(extraObjectOnCharSetup.ItemInfo.Target);
                }
            }
        }

        //Function to set all yotogi command disabled
        internal static void BlockAllYotogiCommands()
        {
            var commandList = StateManager.Instance.YotogiCommandFactory;
            for (int i = 0; i < commandList.transform.childCount; i++)
            {
                Transform childTransform = commandList.transform.GetChild(i);
                UIButton childButton = childTransform.GetComponent<UIButton>();
                if (childButton != null)
                {
                    childButton.isEnabled = false;
                    childButton.SetState(UIButtonColor.State.Disabled, true);
                }
            }
        }

        //Handles 1 to 1 queueing case only
        internal static void ChangeManMembersQueueType(PartyGroup group, EventDelegate OnChangeManMemberFinish = null)
        {
            //set to stand up motion
            string animFile = ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].MovingQueueSetup.StandingAnimationFile;
            CharacterHandling.PlayAnimation(group.Man1, animFile, animFile, isBlend: true);

            TimeEndTrigger trigger = new TimeEndTrigger();
            trigger.DueTime = DateTime.Now.AddMilliseconds(ConfigurableValue.AnimationBlendTime * 1000);
            trigger.ToBeExecuted = new EventDelegate(() => ChangeManMembersQueueType_MoveQueueMembers(group, OnChangeManMemberFinish));
            StateManager.Instance.TimeEndTriggerList.Add(trigger);
        }

        internal static void ChangeManMembersQueueType_MoveQueueMembers(PartyGroup currentGroup, EventDelegate OnChangeManMemberFinish = null)
        {
            //assign the group.Man1 to the end of the queue of a random group
            int minGroupLength = StateManager.Instance.PartyGroupList.Min(x => x.ExtraManList.Count(y => y.Value != null));

            List<PartyGroup> validGroup = StateManager.Instance.PartyGroupList.Where(
                x => (x.ExtraManList.Count(y => y.Value != null) <= minGroupLength + ConfigurableValue.GangBangReQueueMinLengthDiffLimit)
                    && (!x.ExtraManList.ContainsKey(x.ExtraManSetupInfo.Max(y => y.ArrayPosition)))
                ).ToList();
            
            int rnd = RNG.Random.Next(validGroup.Count);
            PartyGroup groupToJoin = validGroup[rnd];

            List<int> occupiedPos = new List<int>();
            foreach (var kvp in groupToJoin.ExtraManList)
                if(kvp.Value != null)
                    occupiedPos.Add(kvp.Key);
            
            int lastPos = groupToJoin.ExtraManSetupInfo.Where(x => !occupiedPos.Contains(x.ArrayPosition) && x.ArrayPosition > 0).Min(y => y.ArrayPosition);
            
            if(groupToJoin.ExtraManList.ContainsKey(lastPos))
                groupToJoin.ExtraManList[lastPos] = currentGroup.Man1;
            else
                groupToJoin.ExtraManList.Add(lastPos, currentGroup.Man1);
            groupToJoin.SetExtraManPosition(lastPos);

            //connect the first man in queue to the maid
            Maid firstManInQueue = currentGroup.ExtraManList[0];
            ChangeManMembersQueueType_SwapMan(currentGroup, firstManInQueue);

            //move the first member of the queue to maid location
            PartyGroupSetup.MovingQueueSetupData queueSetup = ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].MovingQueueSetup;
            HardCodeMotion.ManWalkController.StandingMotionType standingMotionType = new HardCodeMotion.ManWalkController.StandingMotionType();
            standingMotionType.AnimationFileName = queueSetup.StandingAnimationFile;
            standingMotionType.StandingMotionOffset = queueSetup.StandingMotionOffset;
            standingMotionType.WalkingMotionOffset = queueSetup.WalkingMotionOffset;
            standingMotionType.RotationOffset = queueSetup.RotationOffset;
            
            //need to add the offset value of regarding to the motion so that the man doesnt not walk over the maid visually
            float angle = firstManInQueue.transform.rotation.eulerAngles.y;
            Vector3 maidMotionOffsetRespectToRotation = Quaternion.AngleAxis(angle, Vector3.up) * queueSetup.MaidMotionOffset;
            
            float firstManDistance = Vector3.Distance(currentGroup.ExtraManSetupInfo[0].Pos, currentGroup.Maid1.transform.position + maidMotionOffsetRespectToRotation);
            HardCodeMotion.ManWalkController.MoveForward(firstManInQueue, firstManDistance, standingMotionType,
                new EventDelegate(() => ChangeManMembersQueueType_FirstManMoveFinish(currentGroup, firstManInQueue, OnChangeManMemberFinish))
                );
            currentGroup.MovingGroupMemberList.Add(firstManInQueue);

            //move the queue forward in this group
            List<int> currentGroupKeyList = currentGroup.ExtraManList.Keys.OrderBy(x => x).ToList();
            for (int i = 0; i < currentGroupKeyList.Count; i++)
            {
                Maid man = currentGroup.ExtraManList[currentGroupKeyList[i]];

                if (man != null)
                {
                    var currentInfo = currentGroup.ExtraManSetupInfo[currentGroupKeyList[i]];
                    var targetInfo = currentGroup.ExtraManSetupInfo[currentGroupKeyList[i] - 1];

                    int originalIndex = currentGroupKeyList[i];
                    int targetIndex = currentGroupKeyList[i] - 1;

                    float distance = Vector3.Distance(currentInfo.Pos, targetInfo.Pos);

                    HardCodeMotion.ManWalkController.MoveForward(man, distance, standingMotionType,
                        new EventDelegate(() => ChangeManMembersQueueType_UpdateQueueOnMoveFinish(currentGroup, man, originalIndex, targetIndex))
                        );

                    currentGroup.MovingExtraManIndexList.Add(currentGroupKeyList[i]);
                }
            }

            
        }

        private static void ChangeManMembersQueueType_FirstManMoveFinish(PartyGroup group, Maid man, EventDelegate OnChangeManMemberFinish = null)
        {
            BackgroundGroupMotion.MotionItem motionItem = Util.GetMotionItemBySexPosID(group.SexPosID);

            group.MovingGroupMemberList.Remove(man);

            int groupIndex = 0;
            for (int i = 0; i < StateManager.Instance.PartyGroupList.Count; i++)
                if (StateManager.Instance.PartyGroupList[i] == group)
                {
                    groupIndex = i;
                    break;
                }

            //reload motion for the group
            MotionSpecialLabel waitingLabel = motionItem.SpecialLabels.Where(x => x.Type == MotionSpecialLabel.LabelType.Waiting).First();
            
            //Change skill will reset the current process of the yotogi...
            YotogiHandling.SetGroupWaitingMotion(group, motionItem);

            //reassign position for the group
            MapCoorindates.CoordinateListInfo coordinateListInfo = ModUseData.MapCoordinateList[PartyGroup.CurrentFormation].CoordinateList.Where(x => x.MaxGroup >= StateManager.Instance.PartyGroupList.Count).OrderBy(x => x.MaxGroup).First();
            MapCoorindates.CoordinatesInfo coordinatesInfo = coordinateListInfo.GroupCoordinates.Where(x => x.ArrayPosition == groupIndex).First();
            group.SetGroupPosition(coordinatesInfo.Pos, coordinatesInfo.Rot);

            if (group == StateManager.Instance.PartyGroupList[0])
            {
                StateManager.Instance.YotogiManager.play_mgr.UpdateCommand();
            }

            if (OnChangeManMemberFinish != null)
                OnChangeManMemberFinish.Execute();
        }

        private static void ChangeManMembersQueueType_SwapMan(PartyGroup group, Maid man)
        {
            BackgroundGroupMotion.MotionItem motionItem = Util.GetMotionItemBySexPosID(group.SexPosID);

            if (group.ExtraManList.ContainsKey(0))
                if (group.ExtraManList[0] != null)
                    if (group.ExtraManList[0].status.guid == man.status.guid)
                        group.ExtraManList[0] = null;

            //Remove all existng IK 
            group.DetachAllIK();
            group.SetManAtIndex(0, man);

            if (group == StateManager.Instance.PartyGroupList[0])
            {
                StateManager.Instance.SpoofActivateMaidObjectFlag = true;
                GameMain.Instance.CharacterMgr.SetActiveMan(man, motionItem.ManIndex[0]);
                StateManager.Instance.SpoofActivateMaidObjectFlag = false;
            }
        }

        private static void ChangeManMembersQueueType_UpdateQueueOnMoveFinish(PartyGroup group, Maid man, int originalIndex, int targetIndex)
        {
            if (group.ExtraManList.ContainsKey(originalIndex))
                if (group.ExtraManList[originalIndex] != null)
                    if(group.ExtraManList[originalIndex].status.guid == man.status.guid)
                        group.ExtraManList[originalIndex] = null;

            if (group.ExtraManList.ContainsKey(targetIndex))
                group.ExtraManList[targetIndex] = man;
            else
                group.ExtraManList.Add(targetIndex, man);
            
            group.SetExtraManPosition(targetIndex);
            
            group.MovingExtraManIndexList.Remove(originalIndex);

        }
    }
}
