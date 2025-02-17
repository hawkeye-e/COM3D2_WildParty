using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx.Logging;
using HarmonyLib;

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
        internal static void ChangeBackgroundGroupSexPosition(PartyGroup group, int sexPosID, bool isRandomizeMotionLabelGroup, bool playAudio = true)
        {
            //Reset some of the data
            group.GroupOffsetVector = Vector3.zero;
            group.CurrentSexState = SexState.StateType.NormalPlay;

            if(group.IsAutomatedGroup)
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

            CharacterHandling.LoadMotionScript(1, false,
                motionItem.FileName, label.LabelName,
                group.Maid1.status.guid, group.Man1.status.guid, false, false, true, false);

            group.ReloadAnimation();

            group.SexPosID = motionItem.ID;
            
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

            BackgroundGroupMotion.MotionItem motionItem = Util.GetMotionItemOfGroup(group);

            List<MotionSpecialLabel> lstSpecialLabel = motionItem.SpecialLabels.Where(x => x.Type == type).ToList();

            MotionSpecialLabel pickedLabel = lstSpecialLabel[RNG.Random.Next(lstSpecialLabel.Count)];

            CharacterHandling.LoadMotionScript(1, false,
                motionItem.FileName, pickedLabel.Label,
                group.Maid1.status.guid, group.Man1.status.guid, false, false, true, false);

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
            }

            Vector3 positionOffset = Vector3.zero;
            Quaternion forceRotation = ValidCoordinates[0].Rot;
            if (coordsGroup.SpecialSetting != null)
                if (coordsGroup.SpecialSetting.MainGroupMotionOffset != null)
                {
                    positionOffset = coordsGroup.SpecialSetting.MainGroupMotionOffset.Pos;
                    forceRotation = coordsGroup.SpecialSetting.MainGroupMotionOffset.Rot;
                }

            for (int i = 0; i < StateManager.Instance.PartyGroupList.Count; i++)
            {
                var currentGroup = StateManager.Instance.PartyGroupList[i];

                //Get a valid position
                if (i == 0)
                    currentGroup.SetGroupPosition(ValidCoordinates[i].Pos + positionOffset, forceRotation);
                else
                    currentGroup.SetGroupPosition(ValidCoordinates[i].Pos, ValidCoordinates[i].Rot);
                
            }

            //Set individual position
            if (coords.IndividualCoordinates != null)
            {
                foreach (var item in coords.IndividualCoordinates)
                {
                    if (item.Type == Constant.IndividualCoordinateType.Maid)
                    {
                        if (StateManager.Instance.YotogiWorkingMaidList.Count > item.ArrayPosition)
                        {
                            StateManager.Instance.YotogiWorkingMaidList[item.ArrayPosition].transform.localPosition = Vector3.zero;
                            StateManager.Instance.YotogiWorkingMaidList[item.ArrayPosition].transform.position = item.Pos;
                            StateManager.Instance.YotogiWorkingMaidList[item.ArrayPosition].transform.rotation = item.Rot;
                        }
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
                    }
                    else if (item.Type == Constant.SpecialCoordinateType.UnassignedMaid)
                    {
                        if (PartyGroup.UnassignedMaid != null)
                        {
                            PartyGroup.UnassignedMaid.transform.localPosition = Vector3.zero;
                            PartyGroup.UnassignedMaid.transform.position = item.Pos;
                            PartyGroup.UnassignedMaid.transform.rotation = item.Rot;
                        }
                    }
                }
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

                if (i > 0)
                {
                    currentGroup.IsIndependentExcitement = coordinateListInfo.GroupCoordinates[i].IsIndependentExcitement;
                    if (!currentGroup.IsAutomatedGroup)
                        currentGroup.StopNextReviewTime();

                    if (coordinateListInfo.GroupCoordinates[i].ForceSexPos == null)
                    {
                        //Randomize motion case
                        var lstMotion = ModUseData.BackgroundMotionList[currentGroup.GroupType];
                        int rndMotion = RNG.Random.Next(lstMotion.Count);

                        ChangeBackgroundGroupSexPosition(currentGroup, lstMotion[rndMotion].ID, true, playAudio);
                    }
                    else
                    {
                        currentGroup.ForceSexPos = new ForceSexPosInfo();
                        currentGroup.ForceSexPos.NormalPlay = coordinateListInfo.GroupCoordinates[i].ForceSexPos.NormalPlay;
                        currentGroup.ForceSexPos.Waiting = coordinateListInfo.GroupCoordinates[i].ForceSexPos.Waiting;
                        currentGroup.ForceSexPos.Orgasm = coordinateListInfo.GroupCoordinates[i].ForceSexPos.Orgasm;

                        //A SexPosID is assigned to this group
                        ChangeBackgroundGroupSexPosition(currentGroup, currentGroup.ForceSexPos.Waiting, true, playAudio);
                    }

                    if (!coordinateListInfo.GroupCoordinates[i].IsManVisible)
                    {
                        if(currentGroup.Man1 != null)
                            currentGroup.Man1.transform.localScale = Vector3.zero;
                        if (currentGroup.Man2 != null)
                            currentGroup.Man2.transform.localScale = Vector3.zero;
                    }

                    
                }

                if (coordinateListInfo.GroupCoordinates[i].EyeSight != null)
                {
                    currentGroup.ForceEyeSight = new List<EyeSightSetting>();
                    foreach (var eyeSightSetting in coordinateListInfo.GroupCoordinates[i].EyeSight)
                    {
                        currentGroup.ForceEyeSight.Add(eyeSightSetting);
                        CustomADVProcessManager.SetCharacterEyeSight(currentGroup, eyeSightSetting);
                    }
                }

                if (coordinateListInfo.GroupCoordinates[i].IKAttach != null)
                {
                    currentGroup.ForceIKAttach = new List<IKAttachInfo>();
                    foreach (var ikInfo in coordinateListInfo.GroupCoordinates[i].IKAttach)
                    {
                        currentGroup.ForceIKAttach.Add(ikInfo);
                        CharacterHandling.IKAttachBone(ikInfo);
                    }
                }
            }

            //Set Individual Motion
            
            if(coordinateListInfo.IndividualCoordinates != null)
            {
                foreach (var item in coordinateListInfo.IndividualCoordinates)
                {
                    if (item.Type == Constant.IndividualCoordinateType.Maid)
                    {
                        if (StateManager.Instance.YotogiWorkingMaidList.Count > item.ArrayPosition)
                        {
                            Maid maid = StateManager.Instance.YotogiWorkingMaidList[item.ArrayPosition];

                            if (item.Motion != null)
                            {
                                if (!string.IsNullOrEmpty(item.Motion.ScriptFile) && !string.IsNullOrEmpty(item.Motion.ScriptLabel))
                                {
                                    CharacterHandling.LoadMotionScript(1, false,
                                       item.Motion.ScriptFile, item.Motion.ScriptLabel,
                                       maid.status.guid, "", false, false, false, false);
                                }

                                if (!string.IsNullOrEmpty(item.Motion.MotionFile) && !string.IsNullOrEmpty(item.Motion.MotionTag))
                                {
                                    CharacterHandling.PlayAnimation(maid, item.Motion.MotionFile, item.Motion.MotionTag, item.Motion.IsLoopMotion);
                                }
                            }

                            if (!string.IsNullOrEmpty(item.FaceAnime))
                                CustomADVProcessManager.SetFaceAnimeToMaid(maid, item.FaceAnime);

                            if (item.EyeSight != null)
                            {
                                foreach (var eyeSightSetting in item.EyeSight)
                                {
                                    CustomADVProcessManager.SetCharacterEyeSight(maid, eyeSightSetting);
                                }
                            }
                        }
                    }
                }
            }

            //Handle the case of unassigned maid
            if (PartyGroup.UnassignedMaid != null)
            {
                if(coordsGroup.SpecialCoordinates.Where(x => x.Type == Constant.SpecialCoordinateType.UnassignedMaid).First().IsMasturbation)
                    SetMasturbMotionToCharacter(PartyGroup.UnassignedMaid, MasturbationMotion.Type.MaidOnFloor);
            }

            //for some unknown reason the setpos above may not work, try to set it here once more
            Util.ResetAllGroupPosition();

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
                maid.body0.SetChinkoVisible(true);
                CharacterHandling.PlayAnimation(maid, mMotionData.FileName, mMotionData.Tag);
            }
            else
            {

                CharacterHandling.LoadMotionScript(1, false,
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
            //We dont want the game to deactivate any maid object
            StateManager.Instance.SpoofActivateMaidObjectFlag = true;

            //Mark the flag to ensure the extra command is added properly
            MarkInjectedButtonsDirty();
            
            int personality = StateManager.Instance.PartyGroupList[0].Maid1.status.personal.id;
            string groupType = StateManager.Instance.PartyGroupList[0].GroupType;
            var selectedSkill = ModUseData.ValidSkillList[personality][groupType].Where(x => x.YotogiSkillID == skillID).First();

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
            
            //refresh the command
            StateManager.Instance.YotogiManager.play_mgr.NextSkill();

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
            
            MotionSpecialLabel waitingLabel = motionItem.SpecialLabels.Where(x => x.Type == MotionSpecialLabel.LabelType.Waiting).First();
            
            if (loadMotionScript)
                CharacterHandling.LoadMotionScript(0, false,
                motionItem.FileName, waitingLabel.Label,
                "", "", false, false, false, false);
            
            bool isEstrus = Traverse.Create(StateManager.Instance.YotogiManager.play_mgr).Field(Constant.DefinedClassFieldNames.YotogiPlayManagerEstrusMode).GetValue<bool>();
            CharacterHandling.SetCharacterVoiceEntry(StateManager.Instance.PartyGroupList[0].Maid1, PersonalityVoice.VoiceEntryType.Waiting, StateManager.Instance.PartyGroupList[0].ExcitementLevel, waitingLabel.VoiceType1, isEstrus);
            CharacterHandling.SetCharacterVoiceEntry(StateManager.Instance.PartyGroupList[0].Maid2, PersonalityVoice.VoiceEntryType.Waiting, StateManager.Instance.PartyGroupList[0].ExcitementLevel, waitingLabel.VoiceType2, isEstrus);
            
            StateManager.Instance.PartyGroupList[0].Maid1VoiceType = waitingLabel.VoiceType1;
            if (StateManager.Instance.PartyGroupList[0].Maid2 != null)
                StateManager.Instance.PartyGroupList[0].Maid2VoiceType = waitingLabel.VoiceType2;
            
            CharacterHandling.SetGroupFace(StateManager.Instance.PartyGroupList[0]);
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
            BackgroundGroupMotionManager.InitNextReviewTimer();
            YotogiHandling.UpdateParameterView(StateManager.Instance.PartyGroupList[0].Maid1);

            RandomizeMaidExcitement(StateManager.Instance.SelectedMaidsList);

            foreach (var man in StateManager.Instance.MenList)
            {
                man.body0.SetChinkoVisible(true);
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
            YotogiProgressInfo info = StateManager.Instance.YotogiProgressInfoList[maid.status.guid];
            if (!info.ManOrgasmInfo.ContainsKey(man.status.guid))
                info.ManOrgasmInfo.Add(man.status.guid, 1);
            else
                info.ManOrgasmInfo[man.status.guid] += 1;
        }

        internal static void AddManOrgasmCountForGroup(PartyGroup group)
        {
            AddManOrgasmCountToMaid(group.Maid1, group.Man1);
            AddManOrgasmCountToMaid(group.Maid1, group.Man2);
            AddManOrgasmCountToMaid(group.Maid2, group.Man1);
            //unlikely to have a 2M2F case but put it here anyway.
            AddManOrgasmCountToMaid(group.Maid2, group.Man2);
        }
    }
}
