using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx.Logging;
using HarmonyLib;

namespace COM3D2.WildParty.Plugin.Core
{
    internal class CharacterHandling
    {
        private static ManualLogSource Log = WildParty.Log;

        internal static void InitMaid(Maid maid)
        {
            //For maid, no need to create new. we take it from stock
            maid.gameObject.transform.SetParent(GameMain.Instance.CharacterMgr.GetMaid(0).gameObject.transform.parent, false);
            maid.gameObject.name = maid.status.fullNameJpStyle;
            maid.transform.localPosition = new Vector3(-999f, -999f, -999f);
            maid.Visible = true;
            maid.DutPropAll();
            maid.AllProcPropSeqStart();

            StateManager.Instance.WaitForFullLoadList.Add(maid);
        }

        internal static Maid InitMan(int manSlot)
        {
            Maid man;

            man = GameMain.Instance.CharacterMgr.AddStockMan();

            man.gameObject.transform.SetParent(GameMain.Instance.CharacterMgr.GetMaid(0).gameObject.transform.parent, false);
            man.gameObject.name = Constant.DefinedGameObjectNames.ModAddedManGameObjectPrefix + manSlot;

            RandomizeManBody(man);

            man.Visible = true;
            man.DutPropAll();
            man.AllProcPropSeqStart();

            man.transform.localPosition = new Vector3(-999f, -999f, -999f);

            StateManager.Instance.WaitForFullLoadList.Add(man);

            return man;
        }

        internal static Maid InitNPCMaid(string presetName)
        {
            //Maid maid = GameMain.Instance.CharacterMgr.AddStockNpcMaid();
            Maid maid = GameMain.Instance.CharacterMgr.AddStockMaid();
            
            foreach (var kvp in CharacterMgr.npcDatas)
            {
                if (kvp.Value.presetFileName == presetName)
                {
                    kvp.Value.Apply(maid);
                    break;
                }
            }
            if (maid != null)
            {
                maid.gameObject.name = maid.status.fullNameJpStyle;
                maid.transform.localPosition = new Vector3(-999f, -999f, -999f);
                maid.Visible = true;
                maid.DutPropAll();
                maid.AllProcPropSeqStart();
            }

            return maid;
        }

        //Randomize the body of the temporary man
        private static void RandomizeManBody(Maid man)
        {
            //Body color
            man.ManColor = new Color(RNG.Random.Next(256) / 256f, RNG.Random.Next(256) / 256f, RNG.Random.Next(256) / 256f);

            //How slim or fat
            MaidProp prop = man.GetProp(MPN.Hara);
            int hara = RNG.Random.Next(100);
            man.SetProp(MPN.Hara, hara);


            //head
            int head = RNG.Random.Next(ModUseData.ManBodyPartList[Constant.ManBodyOptionType.Head].Count);
            string strFileName = ModUseData.ManBodyPartList[Constant.ManBodyOptionType.Head][head].Trim();

            int ridHead = System.IO.Path.GetFileName(strFileName).ToLower().GetHashCode();

            man.SetProp(MPN.head, strFileName, ridHead);

            //body
            int body = RNG.Random.Next(ModUseData.ManBodyPartList[Constant.ManBodyOptionType.Body].Count);
            strFileName = ModUseData.ManBodyPartList[Constant.ManBodyOptionType.Body][body].Trim();

            int ridBody = System.IO.Path.GetFileName(strFileName).ToLower().GetHashCode();
            man.SetProp(MPN.body, strFileName, ridBody);
        }

        internal static void BackupManOrder()
        {
            for (int i = 0; i < GameMain.Instance.CharacterMgr.GetManCount(); i++)
            {
                Maid tmp = GameMain.Instance.CharacterMgr.GetMan(i);
                if (tmp != null)
                {
                    StateManager.Instance.OriginalManOrderList.Add(tmp);
                }
            }
        }

        internal static int GetMaidCountForEventID(int modEventID, bool isDayTime)
        {
            int maidCount = 0;
            for (int i = 0; i < Constant.ScheduleSlotsCount; i++)
            {
                Maid scheduleSlot = GameMain.Instance.CharacterMgr.status.GetScheduleSlot(i);
                if (scheduleSlot != null)
                {
                    if (Schedule.ScheduleAPI.GetScheduleId(scheduleSlot, isDayTime) == modEventID)
                    {
                        maidCount++;
                    }
                }

            }
            return maidCount;
        }

        internal static void InitSelectedMaids()
        {
            //prepare the maids for the scene
            StateManager.Instance.SelectedMaidsList = new List<Maid>();
            StateManager.Instance.YotogiProgressInfoList = new Dictionary<string, YotogiProgressInfo>();

            List<Maid> lstMaid = new List<Maid>();
            for (int i = 0; i < Constant.ScheduleSlotsCount; i++)
            {
                Maid scheduleSlot = GameMain.Instance.CharacterMgr.status.GetScheduleSlot(i);
                if (scheduleSlot != null)
                    lstMaid.Add(scheduleSlot);
            }

            bool isLockParameters = ModUseData.ScenarioList.Where(x => x.ScenarioID == StateManager.Instance.UndergoingModEventID).First().LockParameters;

            int maidCounter = 1;
            Maid mainMaid = GameMain.Instance.CharacterMgr.GetMaid(0);

            StateManager.Instance.SelectedMaidsList.Add(mainMaid);
            StateManager.Instance.YotogiProgressInfoList.Add(mainMaid.status.guid, new YotogiProgressInfo());
            foreach (var maid in lstMaid)
            {
                if (Schedule.ScheduleAPI.GetScheduleId(maid, GameMain.Instance.CharacterMgr.status.isDaytime) == StateManager.Instance.UndergoingModEventID)
                {
                    if (maid.status.guid != mainMaid.status.guid)
                    {
                        //this maid is set to the the event
                        StateManager.Instance.SelectedMaidsList.Add(maid);
                        StateManager.Instance.YotogiProgressInfoList.Add(maid.status.guid, new YotogiProgressInfo());
                        InitMaid(maid);

                        //Based on the scenario setting also set whether the maid could have her parameters changed in yotogi
                        maid.status.enabledYotogiStatusLock = isLockParameters;

                        maidCounter++;
                    }
                }
            }


        }


        internal static List<Maid> ShuffleMaidOrManList(List<Maid> list)
        {
            List<Maid> result = new List<Maid>();
            while (list.Count > 0)
            {
                int rnd = RNG.Random.Next(list.Count);
                Maid m = list[rnd];
                list.RemoveAt(rnd);
                result.Add(m);
            }

            return result;
        }

        internal static void AssignPartyGrouping(string formationID, bool retainMainZero = false)
        {
            if (ModUseData.PartyGroupSetupList[formationID].IsRandomAssign)
            {
                AssignPartyGroupingRandom(true);
            }
            else
            {
                AssignPartyGroupingBySetupInfo(formationID, retainMainZero);
            }
        }

        internal static void AssignPartyGroupingRandom(bool retainMaidZero = false)
        {
            StateManager.Instance.PartyGroupList.Clear();
            PartyGroup.UnassignedMaid = null;

            //Keep the master list unchanged so that the chosen maid will remain the same in the ADV
            List<Maid> workingMaidList = new List<Maid>(StateManager.Instance.SelectedMaidsList);

            //we will keep the man[0] in the same position
            Maid firstMan = StateManager.Instance.MenList[0];
            StateManager.Instance.MenList.Remove(firstMan);


            //Shuffle the man list
            if (ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].IsShuffleManList)
                StateManager.Instance.MenList = ShuffleMaidOrManList(StateManager.Instance.MenList);
            StateManager.Instance.MenList.Insert(0, firstMan);


            //Shuffle the maid list
            if (retainMaidZero)
            {
                Maid firstMaid = GameMain.Instance.CharacterMgr.GetMaid(0);
                workingMaidList.Remove(firstMaid);

                if (ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].IsShuffleMaidList)
                    workingMaidList = ShuffleMaidOrManList(workingMaidList);

                workingMaidList.Insert(0, firstMaid);
            }
            else
            {
                workingMaidList = ShuffleMaidOrManList(workingMaidList);
            }

            StateManager.Instance.YotogiWorkingMaidList = workingMaidList;
            StateManager.Instance.YotogiWorkingManList = StateManager.Instance.MenList;

            int numOfMale = StateManager.Instance.MaxManUsed;
            int numOfFemale = workingMaidList.Count;


            //First we assign each group 1 man and 1 maid
            for (int i = 0; i < Math.Min(numOfMale, numOfFemale); i++)
            {
                PartyGroup newGroup = new PartyGroup();
                newGroup.Man1 = StateManager.Instance.MenList[i];
                newGroup.Maid1 = workingMaidList[i];
                StateManager.Instance.PartyGroupList.Add(newGroup);
            }

            bool isMoreMan = true;
            if (numOfFemale > numOfMale) isMoreMan = false;

            for (int i = Math.Min(numOfFemale, numOfMale); i < Math.Max(numOfFemale, numOfMale); i++)
            {
                //randomly pick a group and assign extra man or maid there
                int rnd = RNG.Random.Next(StateManager.Instance.PartyGroupList.Count);
                if (isMoreMan)
                {
                    bool notAssigned = true;
                    while (notAssigned)
                    {
                        var targetGroup = StateManager.Instance.PartyGroupList[rnd];
                        if (targetGroup.Man2 == null)
                        {
                            targetGroup.Man2 = StateManager.Instance.MenList[i];
                            notAssigned = false;
                        }
                        else
                        {
                            rnd++;
                            //just in case
                            if (rnd >= StateManager.Instance.PartyGroupList.Count) rnd = 0;
                        }
                    }
                }
                else
                {
                    bool notAssigned = true;

                    while (notAssigned)
                    {
                        //Special checking: if the first maid in group 0 is one of the CharacterEX personalities, 
                        //we need to avoid a second maid is assigned to the group due to lack of FFM positions 
                        if (rnd == 0 && Util.IsExPackPersonality(StateManager.Instance.PartyGroupList[rnd].Maid1))
                        {
                            rnd = 1;
                            //There is a possibility that a maid unable to assigned to any group due to this handling, check if this is the case and break the loop
                            bool isAllFull = true;
                            for (int exCheck = 1; exCheck < StateManager.Instance.PartyGroupList.Count; exCheck++)
                            {
                                if (StateManager.Instance.PartyGroupList[exCheck].Maid2 == null)
                                {
                                    isAllFull = false;
                                    break;
                                }
                            }
                            if (isAllFull)
                            {
                                PartyGroup.UnassignedMaid = workingMaidList[i];
                                break;
                            }
                        }

                        var targetGroup = StateManager.Instance.PartyGroupList[rnd];
                        if (targetGroup.Maid2 == null)
                        {
                            targetGroup.Maid2 = workingMaidList[i];
                            notAssigned = false;
                        }
                        else
                        {
                            rnd++;
                            //just in case
                            if (rnd >= StateManager.Instance.PartyGroupList.Count) rnd = 0;
                        }


                    }
                }
            }

            //now for group 0, we have to set the call the SetActive function from the original code
            SetGroupZeroActive();
        }

        internal static void AssignPartyGroupingBySetupInfo(string formationID, bool retainMaidZero = false)
        {
            PartyGroupSetup setupInfo = ModUseData.PartyGroupSetupList[formationID];

            StateManager.Instance.PartyGroupList.Clear();

            //Keep the master list unchanged so that the chosen maid will remain the same in the ADV
            List<Maid> workingMaidList = new List<Maid>(StateManager.Instance.SelectedMaidsList);

            if (setupInfo.IsShuffleMaidList)
            {
                if (retainMaidZero)
                {
                    Maid firstMaid = GameMain.Instance.CharacterMgr.GetMaid(0);
                    workingMaidList.Remove(firstMaid);
                    workingMaidList = ShuffleMaidOrManList(workingMaidList);
                    workingMaidList.Insert(0, firstMaid);
                }
                else
                    workingMaidList = ShuffleMaidOrManList(workingMaidList);
            }
            if (setupInfo.IsShuffleManList)
                StateManager.Instance.MenList = ShuffleMaidOrManList(StateManager.Instance.MenList);

            StateManager.Instance.YotogiWorkingMaidList = workingMaidList;
            StateManager.Instance.YotogiWorkingManList = StateManager.Instance.MenList;

            int maidRunningNumber = 0;
            int manRunningNumber = 0;

            foreach(var groupSetupInfo in setupInfo.GroupSetup.OrderBy(x => x.ArrayPosition))
            {
                PartyGroup newGroup = new PartyGroup();
                if(groupSetupInfo.MaidCount  >= 1 )
                    newGroup.Maid1 = workingMaidList[maidRunningNumber++];
                if (groupSetupInfo.MaidCount >= 2)
                    newGroup.Maid2 = workingMaidList[maidRunningNumber++];
                if (groupSetupInfo.ManCount >= 1)
                    newGroup.Man1 = StateManager.Instance.MenList[manRunningNumber++];
                if (groupSetupInfo.ManCount >= 2)
                    newGroup.Man2 = StateManager.Instance.MenList[manRunningNumber++];

                newGroup.IsAutomatedGroup = groupSetupInfo.IsAutomatedGroup;
                
                StateManager.Instance.PartyGroupList.Add(newGroup);
            }

            SetGroupZeroActive();
        }

        internal static void AssignPartyGrouping_SwapMember(Maid maid1, Maid maid2)
        {
            //If the sex is not the same, do nothing
            if (maid1.boMAN != maid2.boMAN)
                return;

            PartyGroup group1 = Util.GetPartyGroupByCharacter(maid1);
            PartyGroup group2 = Util.GetPartyGroupByCharacter(maid2);

            if (group1 == null || group2 == null)
                return;

            //do the swapping
            
            ReplaceTargetMaid(group1, maid1, maid2);
            ReplaceTargetMaid(group2, maid2, maid1);
            
        }

        private static void ReplaceTargetMaid(PartyGroup group, Maid toBeReplaced, Maid newMaid)
        {
            if (!toBeReplaced.boMAN)
            {
                if (group.Maid1.status.guid == toBeReplaced.status.guid)
                    group.Maid1 = newMaid;
                else if (group.Maid2.status.guid == toBeReplaced.status.guid)
                    group.Maid2 = newMaid;
            }
            else
            {
                if (group.Man1.status.guid == toBeReplaced.status.guid)
                    group.Man1 = newMaid;
                else if (group.Man2.status.guid == toBeReplaced.status.guid)
                    group.Man2 = newMaid;
            }
        }

        internal static void CleanseCharacterMgrArray()
        {
            var groupZero = StateManager.Instance.PartyGroupList[0];
            if (groupZero.Maid2 == null)
            {
                //it could be the case from 2 maids down to 1 maid, we will need to cleanse the array
                var objActiveMaidArray = Traverse.Create(GameMain.Instance.CharacterMgr).Field("m_objActiveMaid").GetValue<GameObject[]>();
                var objMaidArray = Traverse.Create(GameMain.Instance.CharacterMgr).Field("m_gcActiveMaid").GetValue<Maid[]>();

                for (int i = 1; i < objActiveMaidArray.Length; i++)
                    objActiveMaidArray[i] = null;
                for (int i = 1; i < objMaidArray.Length; i++)
                    objMaidArray[i] = null;

            }


        }

        //Special handling for the User control group to make sure the system is referencing the correct maids and men
        private static void SetGroupZeroActive()
        {
            //back up the maid[0], maid[1], and man[1], we will have to set the gameobject visible later. No need for man[0] since he will be the temp protagonist in the yotogi scenario.
            Maid backupMaid0 = GameMain.Instance.CharacterMgr.GetMaid(0);
            Maid backupMaid1 = GameMain.Instance.CharacterMgr.GetMaid(1);
            Maid backupMan1 = GameMain.Instance.CharacterMgr.GetMan(1);

            UnlinkMaid(backupMaid0);
            UnlinkMaid(backupMaid1);

            PartyGroup group = StateManager.Instance.PartyGroupList[0];

            //Set the spoof flag so that the while object doesnt go through the whole initialization process again
            StateManager.Instance.SpoofActivateMaidObjectFlag = true;

            //assign the selected maid and man to the system array.
            GameMain.Instance.CharacterMgr.SetActiveMaid(group.Maid1, 0);
            GameMain.Instance.CharacterMgr.SetActiveMan(group.Man1, 0);
            if (group.Maid2 != null)
                GameMain.Instance.CharacterMgr.SetActiveMaid(group.Maid2, 1);
            if (group.Man2 != null)
                GameMain.Instance.CharacterMgr.SetActiveMan(group.Man2, 1);

            StateManager.Instance.SpoofActivateMaidObjectFlag = false;
        }

        private static void UnlinkMaid(Maid maid)
        {
            if (maid != null)
            {
                maid.ActiveSlotNo = -1;
            }
        }




        internal static void SetGroupVoice(PartyGroup group)
        {
            //stop the current audio
            group.StopAudio();

            int excitementLevel = group.ExcitementLevel;

            var lstLabels = ModUseData.BackgroundMotionList[group.GroupType].Where(x => x.ID == group.SexPosID).First().ValidLabels;

            var motionLabelEntry = lstLabels.Where(x => x.ExcitementLevel == excitementLevel && x.LabelGroupID == group.CurrentLabelGroupID).First();

            if (!string.IsNullOrEmpty(motionLabelEntry.VoiceType1))
            {
                SetCharacterVoiceEntry(group.Maid1, PersonalityVoice.VoiceEntryType.NormalPlay, excitementLevel, motionLabelEntry.VoiceType1, group.IsEstrus);

                //record the type for setting face anime
                group.Maid1VoiceType = motionLabelEntry.VoiceType1;
            }

            if (group.Maid2 != null)
            {
                if (!string.IsNullOrEmpty(motionLabelEntry.VoiceType2))
                {
                    SetCharacterVoiceEntry(group.Maid2, PersonalityVoice.VoiceEntryType.NormalPlay, excitementLevel, motionLabelEntry.VoiceType2, group.IsEstrus);

                    group.Maid2VoiceType = motionLabelEntry.VoiceType2;
                }
            }

        }

        internal static void SetCharacterVoiceEntry(Maid maid, PersonalityVoice.VoiceEntryType voiceEntryType, int excitementLevel, string voiceType, bool isEstrus)
        {
            if (maid == null)
                return;

            List<PersonalityVoice.VoiceEntry> voiceEntries = null;
            switch (voiceEntryType)
            {
                case PersonalityVoice.VoiceEntryType.NormalPlay:
                case PersonalityVoice.VoiceEntryType.Waiting:
                    voiceEntries = ModUseData.PersonalityVoiceList[maid.status.personal.id].NormalPlayVoice;
                    break;
                case PersonalityVoice.VoiceEntryType.OrgasmWait:
                    voiceEntries = ModUseData.PersonalityVoiceList[maid.status.personal.id].OrgasmWait;
                    break;
                case PersonalityVoice.VoiceEntryType.Insert:
                    voiceEntries = ModUseData.PersonalityVoiceList[maid.status.personal.id].InsertVoice;
                    break;
            }

            var targetVoiceList = voiceEntries.Where(x => x.ExcitementLevel == excitementLevel && x.MotionType == voiceType).ToList();

            if (targetVoiceList.Count == 0)
                return;
            int rnd = RNG.Random.Next(targetVoiceList.Count);

            var voiceFile = (isEstrus ? targetVoiceList[rnd].EstrusFile : targetVoiceList[rnd].NormalFile);

            SetCharacterVoice(maid, voiceFile);
        }

        internal static void SetCharacterVoice(Maid maid, string voiceFile, bool isLoop = true)
        {
            if (maid == null)
                return;
            maid.AudioMan.LoadPlay(voiceFile, 0, false, isLoop);
            maid.AudioMan.standAloneVoice = true;
        }


        //Result: True: voice is set; False: voice is not set
        internal static bool SetOrgasmScreamVoice(Maid maid, string voiceType)
        {
            if (maid == null)
                return false;

            //Not all participants will orgasm (eg for harem play there are cases only one of the maid will orgasm and the other wont)
            if (!string.IsNullOrEmpty(voiceType))
            {
                var voiceList = ModUseData.PersonalityVoiceList[maid.status.personal.id].OrgasmScream.Where(x => x.Type == voiceType && x.Personality == Util.GetPersonalityNameByValue(maid.status.personal.id)).ToList();

                if (voiceList != null && voiceList.Count > 0)
                {
                    PersonalityVoice.OrgasmScreamEntry voiceEntry = voiceList.First();
                    Helper.AudioChoppingManager.PlaySubClip(maid, "", voiceEntry.FileName, 0, voiceEntry.ChoppingTime);
                    return true;
                }
            }

            return false;
        }

        internal static void SetCharacterOpenMouth(Maid maid, string voiceType)
        {
            bool isOpenMouth = Constant.FellatioLabel.Contains(voiceType);
            maid.OpenMouth(isOpenMouth);
        }


        //this has to be after SetGroupVoice
        internal static void SetGroupFace(PartyGroup group)
        {
            //int excitementLevel = GetExcitementLevelByRate(group.ExcitementRate);
            int excitementLevel = group.ExcitementLevel;
            List<string> lstMaid1Face;
            string maid1FaceBlend;

            if (!string.IsNullOrEmpty(group.Maid1VoiceType))
            {

                if (group.IsEstrus)
                {
                    lstMaid1Face = ModUseData.VoiceFaceList[group.Maid1VoiceType][excitementLevel].EstrusFaceFiles;
                    maid1FaceBlend = ModUseData.VoiceFaceList[group.Maid1VoiceType][excitementLevel].EstrusFaceBlend;
                }
                else
                {
                    lstMaid1Face = ModUseData.VoiceFaceList[group.Maid1VoiceType][excitementLevel].NormalFaceFiles;
                    maid1FaceBlend = ModUseData.VoiceFaceList[group.Maid1VoiceType][excitementLevel].NormalFaceBlend;
                }

                int maid1FaceID = RNG.Random.Next(lstMaid1Face.Count);
                SetCharacterFace(group.Maid1, lstMaid1Face[maid1FaceID].Trim(), maid1FaceBlend);

                SetCharacterOpenMouth(group.Maid1, group.Maid1VoiceType);
            }

            if (group.Maid2 != null && !string.IsNullOrEmpty(group.Maid2VoiceType))
            {
                List<string> lstMaid2Face;
                string maid2FaceBlend;

                if (group.IsEstrus)
                {
                    lstMaid2Face = ModUseData.VoiceFaceList[group.Maid2VoiceType][excitementLevel].EstrusFaceFiles;
                    maid2FaceBlend = ModUseData.VoiceFaceList[group.Maid2VoiceType][excitementLevel].EstrusFaceBlend;
                }
                else
                {
                    lstMaid2Face = ModUseData.VoiceFaceList[group.Maid2VoiceType][excitementLevel].NormalFaceFiles;
                    maid2FaceBlend = ModUseData.VoiceFaceList[group.Maid2VoiceType][excitementLevel].NormalFaceBlend;
                }

                int maid2FaceID = RNG.Random.Next(lstMaid2Face.Count);

                SetCharacterFace(group.Maid2, lstMaid2Face[maid2FaceID].Trim(), maid2FaceBlend);

                SetCharacterOpenMouth(group.Maid2, group.Maid2VoiceType);
            }
        }

        private static void SetCharacterFace(Maid maid, string faceAnime, string faceBlend)
        {
            if (maid == null)
                return;
            maid.FaceAnime(faceAnime);
            maid.FaceBlend(faceBlend);
        }

        internal static void StopCurrentAnimation()
        {
            GameMain.Instance.ScriptMgr.StopMotionScript();
            foreach (var group in StateManager.Instance.PartyGroupList)
            {
                StopCurrentAnimation(group);
            }
        }

        internal static void StopCurrentAnimation(PartyGroup group)
        {
            GameMain.Instance.ScriptMgr.StopMotionScript();
            
            StopMaidAnimeAndSound(group.Maid1);
            StopMaidAnimeAndSound(group.Maid2);
            StopMaidAnimeAndSound(group.Man1);
            StopMaidAnimeAndSound(group.Man2);
        }

        internal static void StopMaidAnimeAndSound(Maid maid)
        {
            if (maid != null)
            {
                maid.StopAnime();
                if (maid.AudioMan != null)
                {
                    maid.AudioMan.standAloneVoice = false;
                    maid.AudioMan.Stop();
                }
            }
        }

        internal static void StopAllMaidSound()
        {
            foreach(Maid maid in StateManager.Instance.SelectedMaidsList)
                StopMaidSound(maid);
        }

        internal static void StopMaidSound(Maid maid)
        {
            if (maid != null)
            {
                if (maid.AudioMan != null)
                {
                    maid.AudioMan.standAloneVoice = false;
                    maid.AudioMan.Stop();
                }
            }
        }

        internal static void RemoveSemenTexture(Maid maid)
        {
            if (maid != null)
            {
                maid.body0.MulTexRemove(SemenPattern.SlotType.Body, SemenPattern.MaterialType.Body, SemenPattern.PropType.MainTexture, SemenPattern.SemenLayer);
                maid.body0.MulTexRemove(SemenPattern.SlotType.Body, SemenPattern.MaterialType.Body, SemenPattern.PropType.ShadowTexture, SemenPattern.SemenLayer);
                maid.body0.MulTexRemove(SemenPattern.SlotType.Head, SemenPattern.MaterialType.Head, SemenPattern.PropType.MainTexture, SemenPattern.SemenLayer);
                maid.body0.MulTexRemove(SemenPattern.SlotType.Head, SemenPattern.MaterialType.Head, SemenPattern.PropType.ShadowTexture, SemenPattern.SemenLayer);
                maid.body0.MulTexProc(SemenPattern.SlotType.Body);
                maid.body0.MulTexProc(SemenPattern.SlotType.Head);
            }
        }

        internal static void AddFetish(Maid maid, int fetishID)
        {
            if (maid == null)
                return;
            if (!maid.status.propensitys.ContainsKey(fetishID))
            {
                maid.status.AddPropensity(fetishID);
                StateManager.Instance.YotogiProgressInfoList[maid.status.guid].CustomFetishEarned.Add(fetishID);
            }
        }

        internal static void IKAttachBone(IKAttachInfo ikInfo)
        {
            Maid srcMaid = GetMaidForIKCharaInfo(ikInfo.Source);
            Maid targetMaid = GetMaidForIKCharaInfo(ikInfo.Target);

            IKAttachBone(
                TagSupportData.CreateAttachBoneTag(ikInfo.Pos.x, ikInfo.Pos.y, ikInfo.Pos.z, ikInfo.AttachType, ikInfo.Source.Bone, ikInfo.Target.Bone, ikInfo.PullOff),
                srcMaid, targetMaid
            );
        }

        private static Maid GetMaidForIKCharaInfo(IKAttachInfo.IKCharaInfo charaInfo)
        {
            if (charaInfo.ListType == IKAttachInfo.ArrayListType.Group)
            {
                PartyGroup group = StateManager.Instance.PartyGroupList[charaInfo.ArrayIndex];
                if (charaInfo.MemberType == IKAttachInfo.GroupMemberType.Maid1)
                    return group.Maid1;
                else if (charaInfo.MemberType == IKAttachInfo.GroupMemberType.Maid2)
                    return group.Maid2;
                else if (charaInfo.MemberType == IKAttachInfo.GroupMemberType.Man1)
                    return group.Man1;
                else if (charaInfo.MemberType == IKAttachInfo.GroupMemberType.Man2)
                    return group.Man2;
            }else if(charaInfo.ListType == IKAttachInfo.ArrayListType.Maid)
            {
                return StateManager.Instance.YotogiWorkingMaidList[charaInfo.ArrayIndex];
            }
            else if (charaInfo.ListType == IKAttachInfo.ArrayListType.Man)
            {
                return StateManager.Instance.YotogiWorkingManList[charaInfo.ArrayIndex];
            }

            return null;
        }

        internal static void IKAttachBone(KagTagSupport tag_data, Maid source, Maid target)
        {
            string ik_name = tag_data.GetTagProperty("srcbone").AsString();
            kt.ik.IKAttachParam iKAttachParam = kt.ik.IKScriptHelper.GetIKAttachParam(tag_data, source, target);
            iKAttachParam.targetBoneName = tag_data.GetTagProperty("targetbone").AsString();
            source.body0.fullBodyIK.IKAttach(ik_name, iKAttachParam);
        }

        internal static void SetDefaultGroupFormation()
        {
            var scenario = ModUseData.ScenarioList.Where(x => x.ScenarioID == StateManager.Instance.UndergoingModEventID).First();
            if (scenario.AllowMap != null)
            {
                PartyGroup.CurrentFormation = scenario.AllowMap.Where(x => x.MapID == YotogiStageSelectManager.SelectedStage.stageData.id).First().DefaultFormation;
            }
            else if (scenario.DefaultMap != null)
            {
                PartyGroup.CurrentFormation = scenario.DefaultMap.DefaultFormation;
            }
        }

        internal static void PlayAnimation(Maid maid, string fileName, string tag, bool isLoop = true, bool isBlend = false, bool isQueued = false)
        {
            if (maid == null)
                return;

            float fade = 0f;
            if (isBlend)
                fade = 0.5f;

            maid.body0.LoadAnime(tag, GameUty.FileSystem, fileName, false, isLoop);
            maid.body0.CrossFade(maid.body0.LastAnimeFN, GameUty.FileSystem, additive: false, loop: isLoop, boAddQue: isQueued, fade: fade);
        }
    }
}
