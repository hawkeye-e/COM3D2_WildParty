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
            group.GenerateNextReviewTime();

            var motionItem = ModUseData.BackgroundOrgyMotionList[group.GroupType].Where(x => x.ID == sexPosID).First();
            int excitementLevel = group.ExcitementLevel;


            if (isRandomizeMotionLabelGroup || group.CurrentLabelGroupID < 0)
            {
                //need to pick a label randomly
                List<int> labelOptionList = motionItem.ValidLabels.Select(x => x.LabelGroupID).Distinct().ToList();

                int rndLabel = RNG.Random.Next(labelOptionList.Count);

                group.CurrentLabelGroupID = labelOptionList[rndLabel];
            }

            var label = motionItem.ValidLabels.Where(x => x.LabelGroupID == group.CurrentLabelGroupID && x.ExcitementLevel == excitementLevel).First();

            GameMain.Instance.ScriptMgr.LoadMotionScript(1, false,
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

            GameMain.Instance.ScriptMgr.LoadMotionScript(1, false,
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
                ValidCoordinates.Add(coords.Coordinates[i]);
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

            for (int i = 0; i < StateManager.Instance.PartyGroupList.Count; i++)
            {
                var currentGroup = StateManager.Instance.PartyGroupList[i];

                //Get a valid position
                currentGroup.SetGroupPosition(ValidCoordinates[i].Pos, ValidCoordinates[i].Rot);
            }

            //Set special position
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

        internal static void SetGroupToScene(bool playAudio = true)
        {
            var mainMaid = GameMain.Instance.CharacterMgr.GetMaid(0);

            var coordsGroup = ModUseData.MapCoordinateList[PartyGroup.CurrentFormation];

            SetGroupFormation(PartyGroup.CurrentFormation);

            for (int i = 0; i < StateManager.Instance.PartyGroupList.Count; i++)
            {
                var currentGroup = StateManager.Instance.PartyGroupList[i];

                if (i > 0)
                {
                    var lstMotion = ModUseData.BackgroundOrgyMotionList[currentGroup.GroupType];
                    int rndMotion = RNG.Random.Next(lstMotion.Count);

                    ChangeBackgroundGroupSexPosition(currentGroup, lstMotion[rndMotion].ID, true, playAudio);
                }
            }

            //Handle the case of unassigned maid
            //TODO: action controlled by json file?
            if (PartyGroup.UnassignedMaid != null)
            {
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
                maid.body0.LoadAnime(mMotionData.Tag, GameUty.FileSystem, mMotionData.FileName, false, true);
                maid.body0.ReloadAnimation();
            }
            else
            {

                GameMain.Instance.ScriptMgr.LoadMotionScript(1, false,
                mMotionData.MotionFileName, mMotionData.Label,
                maid.status.guid, "", false, false, false, false);

                var voiceData = ModUseData.PersonalityVoiceList[maid.status.personal.id].NormalPlayVoice.Where(x => x.MotionType == mMotionData.VoiceType && x.ExcitementLevel == 2).First();
                maid.AudioMan.LoadPlay(voiceData.NormalFile, 0, false, true);
                maid.AudioMan.standAloneVoice = true;

            }
        }

        internal static void ChangeMainGroupSkill(string skillID)
        {
            //Mark the flag to ensure the extra command is added properly
            MarkInjectedButtonsDirty();

            int personality = StateManager.Instance.PartyGroupList[0].Maid1.status.personal.id;
            string groupType = StateManager.Instance.PartyGroupList[0].GroupType;
            var selectedSkill = ModUseData.ValidOrgySkillList[personality][groupType].Where(x => x.YotogiSkillID == skillID).First();

            StateManager.Instance.PartyGroupList[0].SexPosID = selectedSkill.SexPosID;
            StateManager.Instance.PartyGroupList[0].GroupOffsetVector = Vector3.zero;

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

            //Load the motion for the main group
            BackgroundGroupMotion.MotionItem motionItem = null;
            foreach (var kvp in ModUseData.BackgroundOrgyMotionList)
            {
                var hit = kvp.Value.Where(x => x.ID == selectedSkill.SexPosID).ToList();
                if (hit.Count > 0)
                {
                    motionItem = hit.First();
                    break;
                }
            }

            MotionSpecialLabel waitingLabel = motionItem.SpecialLabels.Where(x => x.Type == MotionSpecialLabel.LabelType.Waiting).First();

            GameMain.Instance.ScriptMgr.LoadMotionScript(0, false,
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

        private static void MarkInjectedButtonsDirty()
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
            int rnd = random.Next(ModUseData.BackgroundOrgyMotionList[groupType].Count);
            return ModUseData.BackgroundOrgyMotionList[groupType][rnd];
        }

        internal static PlayableSkill.SkillItem GetRandomSkill(int personality, string groupType)
        {
            System.Random random = new System.Random();
            int rnd = random.Next(ModUseData.ValidOrgySkillList[personality][groupType].Count);
            return ModUseData.ValidOrgySkillList[personality][groupType][rnd];
        }

        internal static void PlayRoomBGM(YotogiManager manager)
        {
            ScriptManagerFast.KagTagSupportFast tagBGM = TagSupportData.ConvertDictionaryToTagSupportType(TagSupportData.GetTagPlayBGMRoom());
            manager.TagPlayBGMRoom(tagBGM);
        }

        internal static void YotogiSkillCall(YotogiManager manager)
        {
            PlayableSkill.SkillItem initialSkill = GetRandomSkill(StateManager.Instance.PartyGroupList[0].Maid1.status.personal.id, StateManager.Instance.PartyGroupList[0].GroupType);

            StateManager.Instance.PartyGroupList[0].SexPosID = initialSkill.SexPosID;
            ScriptManagerFast.KagTagSupportFast tag = TagSupportData.ConvertDictionaryToTagSupportType(TagSupportData.GetTagForYotogiSkillPlay(initialSkill.YotogiSkillID));
            manager.TagYotogiCall(tag);
        }

        internal static void InitYotogiData()
        {
            CharacterHandling.AssignPartyGrouping();
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
            System.Random random = new System.Random();
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
