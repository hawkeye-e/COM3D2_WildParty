using System;
using System.Collections.Generic;
using System.IO;
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
            RenderMaidAfterInit(maid);
            StateManager.Instance.WaitForFullLoadList.Add(maid);
        }

        internal static Maid InitMan(int manSlot, List<string> manTypeKeyList)
        {
            Maid man;

            man = GameMain.Instance.CharacterMgr.AddStockMan();

            man.gameObject.transform.SetParent(GameMain.Instance.CharacterMgr.GetMaid(0).gameObject.transform.parent, false);
            man.gameObject.name = Constant.DefinedGameObjectNames.ModAddedManGameObjectPrefix + manSlot;

            RandomizeManBody(man, manTypeKeyList);

            RenderMaidAfterInit(man);

            StateManager.Instance.WaitForFullLoadList.Add(man);

            return man;
        }

        internal static Maid InitNPCMaid(string presetName, bool isEmptyLastName)
        {
            //Maid maid = GameMain.Instance.CharacterMgr.AddStockNpcMaid();
            Maid maid = GameMain.Instance.CharacterMgr.AddStockMaid();

            foreach (var kvp in CharacterMgr.npcDatas)
            {
                if (kvp.Value.presetFileName == presetName && isEmptyLastName == string.IsNullOrEmpty(kvp.Value.lastName))
                {
                    kvp.Value.Apply(maid);
                    break;
                }
            }

            RenderMaidAfterInit(maid);
            StateManager.Instance.WaitForFullLoadList.Add(maid);

            return maid;
        }

        internal static Maid InitModNPCFemale(string NPCID)
        {
            ModNPCFemale npcData = ModUseData.ModNPCFemaleList[NPCID];
            
            Maid maid = GameMain.Instance.CharacterMgr.AddStockMaid();
#if COM3D2_5
#if UNITY_2022_3
            CharacterMgr.Preset preset = LoadPreset(npcData.PresetFile.V2_5);
#endif
#endif

#if COM3D2
            CharacterMgr.Preset preset = LoadPreset(npcData.PresetFile.V2);
#endif

            GameMain.Instance.CharacterMgr.PresetSet(maid, preset);

            if (maid != null)
            {
                maid.status.SetPersonal(npcData.Personality);
                maid.VoicePitch = npcData.VoicePitch;

                Traverse.Create(maid.status).Field(Constant.DefinedClassFieldNames.MaidStatusFirstName).SetValue(npcData.FirstName);
                Traverse.Create(maid.status).Field(Constant.DefinedClassFieldNames.MaidStatusLastName).SetValue(npcData.LastName);
                Traverse.Create(maid.status).Field(Constant.DefinedClassFieldNames.MaidStatusNickName).SetValue(npcData.NickName);

                maid.status.isNickNameCall = false;
                maid.status.isFirstNameCall = false;
                if (npcData.WayToCall == ModNPC.CallType.NickName)
                    maid.status.isNickNameCall = true;
                else if (npcData.WayToCall == ModNPC.CallType.FirstName)
                    maid.status.isFirstNameCall = true;

                RenderMaidAfterInit(maid);

                StateManager.Instance.WaitForFullLoadList.Add(maid);
            }

            return maid;
        }

        internal static Maid InitModNPCMale(string NPCID)
        {
            ModNPCMale npcData = ModUseData.ModNPCMaleList[NPCID];

            Maid man = GameMain.Instance.CharacterMgr.AddStockMan();

            string[] npcColor = npcData.Color.Split(',');
            Color manColor = new Color(float.Parse(npcColor[0].Trim()) / 256f, float.Parse(npcColor[1].Trim()) / 256f, float.Parse(npcColor[2].Trim()) / 256f);
            SetManBody(man, manColor, npcData.BodySize, npcData.Head, npcData.Clothed);

            Traverse.Create(man.status).Field(Constant.DefinedClassFieldNames.MaidStatusFirstName).SetValue(npcData.FirstName);
            Traverse.Create(man.status).Field(Constant.DefinedClassFieldNames.MaidStatusLastName).SetValue(npcData.LastName);
            Traverse.Create(man.status).Field(Constant.DefinedClassFieldNames.MaidStatusNickName).SetValue(npcData.NickName);

            man.status.isNickNameCall = false;
            man.status.isFirstNameCall = false;
            if (npcData.WayToCall == ModNPC.CallType.NickName)
                man.status.isNickNameCall = true;
            else if (npcData.WayToCall == ModNPC.CallType.FirstName)
                man.status.isFirstNameCall = true;

            RenderMaidAfterInit(man);

            StateManager.Instance.WaitForFullLoadList.Add(man);

            ManClothingInfo manClothingInfo = new ManClothingInfo();
            manClothingInfo.IsNude = false;
            manClothingInfo.Clothed = npcData.Clothed.Trim();
            manClothingInfo.Nude = npcData.Nude.Trim();
            StateManager.Instance.ManClothingList.Add(man.status.guid, manClothingInfo);


            return man;
        }

        internal static void RenderMaidAfterInit(Maid maid)
        {
            if (maid != null)
            {
                maid.gameObject.name = maid.status.fullNameJpStyle;
                maid.transform.localPosition = new Vector3(-999f, -999f, -999f);
                maid.Visible = true;
                maid.DutPropAll();
                maid.AllProcPropSeqStart();
            }
        }

        //Randomize the body of the temporary man
        private static void RandomizeManBody(Maid man, List<string> manTypeKeyList)
        {
            string pickedType = manTypeKeyList[RNG.Random.Next(manTypeKeyList.Count)];
            ManBodyInfo pickedInfo = ModUseData.ManBodyInfoList[pickedType];
            ManBodyInfo.BodyInfo pickedBodyInfo = pickedInfo.Body[RNG.Random.Next(pickedInfo.Body.Count)];

            //Body color
            Color manColor = new Color(RNG.Random.Next(256) / 256f, RNG.Random.Next(256) / 256f, RNG.Random.Next(256) / 256f);

            //How slim or fat
            int hara = pickedBodyInfo.Min + RNG.Random.Next(pickedBodyInfo.Max - pickedBodyInfo.Min);


            //head
            int head = RNG.Random.Next(pickedInfo.Head.Count);
            string strHeadFileName = pickedInfo.Head[head].Trim();

            //body
            string strBodyFileName = pickedBodyInfo.Clothed.Trim();

            SetManBody(man, manColor, hara, strHeadFileName, strBodyFileName);

            //put the info into the dictionary
            ManClothingInfo manClothingInfo = new ManClothingInfo();
            manClothingInfo.IsNude = false;
            manClothingInfo.Clothed = pickedBodyInfo.Clothed.Trim();
            manClothingInfo.Nude = pickedBodyInfo.Nude.Trim();
            StateManager.Instance.ManClothingList.Add(man.status.guid, manClothingInfo);
        }

        private static void SetManBody(Maid man, Color manColor, int bodySize, string headFile, string bodyFile)
        {
            //Body color
            man.ManColor = manColor;

            //How slim or fat
            man.SetProp(MPN.Hara, bodySize);

            int ridHead = Path.GetFileName(headFile).ToLower().GetHashCode();
            man.SetProp(MPN.head, headFile, ridHead);

            int ridBody = Path.GetFileName(bodyFile).ToLower().GetHashCode();
            man.SetProp(MPN.body, bodyFile, ridBody);
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

            Scenario scenario = ModUseData.ScenarioList.Where(x => x.ScenarioID == StateManager.Instance.UndergoingModEventID).First();
            bool isLockParameters = scenario.LockParameters;

            int maidCounter = 1;
            Maid mainMaid = GameMain.Instance.CharacterMgr.GetMaid(0);

            StateManager.Instance.SelectedMaidsList.Add(mainMaid);
            StateManager.Instance.YotogiProgressInfoList.Add(mainMaid.status.guid, new YotogiProgressInfo());
            mainMaid.status.enabledYotogiStatusLock = isLockParameters;

            if (scenario.IsGroupEvent)
            {
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

            //Backup all maid clothes information. We will use this info to restore the clothing information when the mod event is reset
            foreach(Maid maid in StateManager.Instance.SelectedMaidsList)
                BackupMaidClothesInfo(maid);

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
                if(ModUseData.PartyGroupSetupList[formationID].IsLesbianSetup)
                    AssignPartyGroupingRandomCaseLesbian(retainMainZero);
                else
                    AssignPartyGroupingRandom(retainMainZero);
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
            List<Maid> workingMaidList = new List<Maid>(StateManager.Instance.YotogiWorkingMaidList);
            
            //we will keep the man[0] in the same position
            Maid firstMan = StateManager.Instance.YotogiWorkingManList[0];
            StateManager.Instance.YotogiWorkingManList.Remove(firstMan);

            
            //Shuffle the man list
            if (ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].IsShuffleManList)
                StateManager.Instance.YotogiWorkingManList = ShuffleMaidOrManList(StateManager.Instance.YotogiWorkingManList);
            StateManager.Instance.YotogiWorkingManList.Insert(0, firstMan);

            
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

            int numOfMale = StateManager.Instance.YotogiWorkingManList.Count;
            int numOfFemale = workingMaidList.Count;

            
            //First we assign each group 1 man and 1 maid
            for (int i = 0; i < Math.Min(numOfMale, numOfFemale); i++)
            {
                PartyGroup newGroup = new PartyGroup();
                newGroup.Man1 = StateManager.Instance.YotogiWorkingManList[i];
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
                            targetGroup.Man2 = StateManager.Instance.YotogiWorkingManList[i];
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

        }

        //This function is designed for lesbian (Yuri) case only. ***Maid with converted man structure is NOT considered as lesbian case and should go through normal Man+Maid setup.***
        internal static void AssignPartyGroupingRandomCaseLesbian(bool retainGroupZero = false)
        {
            StateManager.Instance.PartyGroupList.Clear();

            //Keep the master list unchanged so that the chosen maid will remain the same in the ADV
            List<Maid> workingMaidList = new List<Maid>(StateManager.Instance.YotogiWorkingMaidList);

            //Shuffle the maid list
            if (retainGroupZero)
            {
                Maid firstMaid = GameMain.Instance.CharacterMgr.GetMaid(0);
                Maid secondMaid = GameMain.Instance.CharacterMgr.GetMaid(1);

                workingMaidList.Remove(firstMaid);
                workingMaidList.Remove(secondMaid);

                if (ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].IsShuffleMaidList)
                    workingMaidList = ShuffleMaidOrManList(workingMaidList);

                workingMaidList.Insert(0, firstMaid);
                workingMaidList.Insert(1, secondMaid);
            }
            else
            {
                workingMaidList = ShuffleMaidOrManList(workingMaidList);
            }

            StateManager.Instance.YotogiWorkingMaidList = workingMaidList;

            int numOfFemale = workingMaidList.Count;


            //For the case of lesbian, only FF is possible
            for (int i = 0; i < numOfFemale / 2; i++)
            {
                PartyGroup newGroup = new PartyGroup();
                newGroup.Maid1 = workingMaidList[i*2];
                newGroup.Maid2 = workingMaidList[i*2+1];
                StateManager.Instance.PartyGroupList.Add(newGroup);
            }
        }

        internal static void AssignPartyGroupingBySetupInfo(string formationID, bool retainMaidZero = false)
        {
            PartyGroupSetup setupInfo = ModUseData.PartyGroupSetupList[formationID];
            
            StateManager.Instance.PartyGroupList.Clear();

            //Keep the master list unchanged so that the chosen maid will remain the same in the ADV
            List<Maid> workingMaidList = new List<Maid>(StateManager.Instance.YotogiWorkingMaidList);

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
                StateManager.Instance.YotogiWorkingManList = ShuffleMaidOrManList(StateManager.Instance.YotogiWorkingManList);

            StateManager.Instance.YotogiWorkingMaidList = workingMaidList;

            int maidRunningNumber = 0;
            int manRunningNumber = 0;
            int NPCFemaleRunningNumber = 0;
            
            foreach (var groupSetupInfo in setupInfo.GroupSetup.OrderBy(x => x.ArrayPosition))
            {
                if (!groupSetupInfo.MaidFromNPC)
                {
                    if (maidRunningNumber >= StateManager.Instance.YotogiWorkingMaidList.Count)
                        continue;
                }
                else
                {
                    if (NPCFemaleRunningNumber >= StateManager.Instance.NPCList.Count)
                        continue;
                }

                PartyGroup newGroup = new PartyGroup();

                for (int i = 0; i < groupSetupInfo.MaidCount; i++)
                {
                    if (!groupSetupInfo.MaidFromNPC)
                        newGroup.SetMaidAtIndex(i, workingMaidList[maidRunningNumber++]);
                    else
                        newGroup.SetMaidAtIndex(i, StateManager.Instance.NPCList[NPCFemaleRunningNumber++]);
                }
                for (int i = 0; i < groupSetupInfo.ManCount; i++)
                    newGroup.SetManAtIndex(i, StateManager.Instance.YotogiWorkingManList[manRunningNumber++]);



                newGroup.IsAutomatedGroup = groupSetupInfo.IsAutomatedGroup;
                newGroup.IsVoicelessGroup = groupSetupInfo.IsVoicelessGroup;
                

                StateManager.Instance.PartyGroupList.Add(newGroup);
                
            }
            
            //Shared extra man handling
            PartyGroup.SharedExtraManList = new Dictionary<int, Maid>();
            for (int i = 0; i < setupInfo.ExtraManCount; i++)
            {
                if (manRunningNumber >= StateManager.Instance.YotogiWorkingManList.Count)
                    PartyGroup.SharedExtraManList.Add(i, null);
                else
                    PartyGroup.SharedExtraManList.Add(i, StateManager.Instance.YotogiWorkingManList[manRunningNumber++]);
            }
            
            //Extra man for each group handling
            foreach (var groupSetupInfo in setupInfo.GroupSetup.OrderBy(x => x.ArrayPosition))
            {
                for (int i = 0; i < groupSetupInfo.ExtraManCount; i++)
                {
                    if (manRunningNumber >= StateManager.Instance.YotogiWorkingManList.Count)
                        StateManager.Instance.PartyGroupList[groupSetupInfo.ArrayPosition].ExtraManList.Add(i, null);
                    else
                        StateManager.Instance.PartyGroupList[groupSetupInfo.ArrayPosition].ExtraManList.Add(i, StateManager.Instance.YotogiWorkingManList[manRunningNumber++]);
                }
            }
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

            int group1Index = group1.GetMaidOrManIndex(maid1);
            int group2Index = group1.GetMaidOrManIndex(maid2);

            //do the swapping
            
            ReplaceTargetMaid(group1, group1Index, maid2);
            ReplaceTargetMaid(group2, group2Index, maid1);
            
        }

        private static void ReplaceTargetMaid(PartyGroup group, int index, Maid newMaid)
        {
            if (!newMaid.boMAN)
                group.SetMaidAtIndex(index, newMaid);
            else
                group.SetManAtIndex(index, newMaid);
            
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
        internal static void SetGroupZeroActive()
        {
            
            //back up the maid[0], maid[1], and man[1], we will have to set the gameobject visible later. No need for man[0] since he will be the temp protagonist in the yotogi scenario.
            Maid backupMaid0 = GameMain.Instance.CharacterMgr.GetMaid(0);
            Maid backupMaid1 = GameMain.Instance.CharacterMgr.GetMaid(1);

            UnlinkMaid(backupMaid0);
            UnlinkMaid(backupMaid1);
            
            PartyGroup group = StateManager.Instance.PartyGroupList[0];
            PlayableSkill.SkillItem skill = Util.GetGroupCurrentSkill(group);
            
            //Set the spoof flag so that the while object doesnt go through the whole initialization process again
            StateManager.Instance.SpoofActivateMaidObjectFlag = true;

            //assign the selected maid and man to the system array.
            for (int i = 0; i < skill.MaidIndex.Count; i++)
                if (group.GetMaidAtIndex(i) != null)
                {
                    group.GetMaidAtIndex(i).ActiveSlotNo = skill.MaidIndex[i];
                    GameMain.Instance.CharacterMgr.SetActiveMaid(group.GetMaidAtIndex(i), skill.MaidIndex[i]);
                }
            
            for (int i = 0; i < skill.ManIndex.Count; i++)
                if (group.GetManAtIndex(i) != null)
                {
                    GameMain.Instance.CharacterMgr.SetActiveMan(group.GetManAtIndex(i), skill.ManIndex[i]);
                }
            
            //Check if there is any null in the man list, need to fill something back there to avoid error
            List<Maid> currentManArray = new List<Maid>();
            for (int i = 0; i < GameMain.Instance.CharacterMgr.GetManCount(); i++)
            {
                Maid man = GameMain.Instance.CharacterMgr.GetMan(i);
                if (man != null)
                    currentManArray.Add(man);
            }
            
            for (int i = 0; i < GameMain.Instance.CharacterMgr.GetManCount(); i++)
            {
                Maid man = GameMain.Instance.CharacterMgr.GetMan(i);
                if (man == null)
                {
                    //pull a random man from the man list and fill it in. The null place is not used by main group so it should be fine to fill in any man?
                    Maid randomMan = StateManager.Instance.MenList.Where(x => !currentManArray.Contains(x)).First();
                    GameMain.Instance.CharacterMgr.SetActiveMan(randomMan, i);
                }
            }
            
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
                SetCharacterVoiceEntry(group.Maid1, PersonalityVoice.VoiceEntryType.NormalPlay, excitementLevel, motionLabelEntry.VoiceType1, group.IsEstrus, group.IsVoicelessGroup);

                //record the type for setting face anime
                group.Maid1VoiceType = motionLabelEntry.VoiceType1;
            }

            if (group.Maid2 != null)
            {
                if (!string.IsNullOrEmpty(motionLabelEntry.VoiceType2))
                {
                    SetCharacterVoiceEntry(group.Maid2, PersonalityVoice.VoiceEntryType.NormalPlay, excitementLevel, motionLabelEntry.VoiceType2, group.IsEstrus, group.IsVoicelessGroup);

                    group.Maid2VoiceType = motionLabelEntry.VoiceType2;
                }
            }

        }

        internal static void SetCharacterVoiceEntry(Maid maid, PersonalityVoice.VoiceEntryType voiceEntryType, int excitementLevel, string voiceType, bool isEstrus, bool isVoiceless)
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

            if (!isVoiceless)
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
                    Helper.AudioChoppingManager.PlaySubClip(maid, "", voiceEntry.FileName, voiceEntry.StartTime, voiceEntry.EndTime);
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
            foreach (Maid maid in StateManager.Instance.SelectedMaidsList)
                StopMaidSound(maid);
            if (StateManager.Instance.NPCList != null)
                foreach (Maid npc in StateManager.Instance.NPCList)
                    StopMaidSound(npc);
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

        /*
         * Due to TagTexMulAdd will call GetMaidAndMan function which will mess up the spoofing logic for other parts, I dont directly call TagTexMulAdd but follow the logic inside.
           Drawback: There are some special handlings on CRC body in TagTexMulAdd, so the effect wont apply in 3.0 version(I have no idea whether this mod will work or not in v3.0 anyway)
         */
        internal static void AddSemenTexture(Maid maid, SemenPattern pattern)
        {
            if (maid == null)
                return;

            List<string> slotToBeProc = new List<string>();

            for (int i = 0; i < pattern.SplashCount; i++)
            {
                int xValue = RNG.Random.Next(pattern.XRange[i].MinValue, pattern.XRange[i].MaxValue);
                int yValue = RNG.Random.Next(pattern.YRange[i].MinValue, pattern.YRange[i].MaxValue);
                float rotValue = RNG.Random.Next((int)(pattern.RotRange[i].MinValue * 100), (int)(pattern.RotRange[i].MaxValue * 100)) / 100.0f;
                float scaleValue = RNG.Random.Next((int)(pattern.Scale[i].MinValue * 100), (int)(pattern.Scale[i].MaxValue * 100)) / 100.0f;

                int rnd = RNG.Random.Next(pattern.FileName[i].Count);
                string fileName = pattern.FileName[i][rnd];

                foreach (var propName in pattern.PropName)
                {
                    maid.body0.MulTexSet(pattern.Slotname, pattern.MatNo, propName, pattern.LayerNo, fileName, pattern.BlendMode, pattern.Add,
                        xValue, yValue, rotValue, scaleValue, pattern.NoTransform, pattern.SubProp, pattern.Alpha, pattern.TargetBodyTexSize);
                }
                if (!slotToBeProc.Contains(pattern.Slotname))
                    slotToBeProc.Add(pattern.Slotname);
            }

            foreach (var slot in slotToBeProc)
                maid.body0.MulTexProc(slot);
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
            }
            else if (charaInfo.ListType == IKAttachInfo.ArrayListType.Maid)
            {
                return StateManager.Instance.YotogiWorkingMaidList[charaInfo.ArrayIndex];
            }
            else if (charaInfo.ListType == IKAttachInfo.ArrayListType.Man)
            {
                return StateManager.Instance.YotogiWorkingManList[charaInfo.ArrayIndex];
            }

            return null;
        }

#if COM3D2_5
#if UNITY_2022_3
        internal static void IKAttachBone(KagTagSupport tag_data, Maid source, Maid target)
        {
            string ik_name = tag_data.GetTagProperty("srcbone").AsString();
            kt.ik.IKAttachParam iKAttachParam = kt.ik.IKScriptHelper.GetIKAttachParam(tag_data, source, target);
            iKAttachParam.targetBoneName = tag_data.GetTagProperty("targetbone").AsString();
            source.body0.fullBodyIK.IKAttach(ik_name, iKAttachParam);
        }
#endif
#endif

#if COM3D2
        //Code copied from Kiss to evade the GetMaidAndMan
        internal static void IKAttachBone(KagTagSupport tag_data, Maid source, Maid target)
        {
            string ik_name = tag_data.GetTagProperty("srcbone").AsString();
            string targetBoneName = tag_data.GetTagProperty("targetbone").AsString();

            IKCtrlData iKData = source.IKCtrl.GetIKData(ik_name);

            Vector3 offset = Vector3.zero;
            if (tag_data.IsValid("offsetx"))
            {
                offset.x = tag_data.GetTagProperty("offsetx").AsReal();
            }
            if (tag_data.IsValid("offsety"))
            {
                offset.y = tag_data.GetTagProperty("offsety").AsReal();
            }
            if (tag_data.IsValid("offsetz"))
            {
                offset.z = tag_data.GetTagProperty("offsetz").AsReal();
            }

            IKCtrlData.IKAttachType iKAttachType = IKCtrlData.IKAttachType.Point;
            if (tag_data.IsValid("attach_type"))
            {
                iKAttachType = (IKCtrlData.IKAttachType)Enum.Parse(typeof(IKCtrlData.IKAttachType), tag_data.GetTagProperty("attach_type").AsString());
            }
            if (iKAttachType == IKCtrlData.IKAttachType.NewPoint)
            {
                if (tag_data.IsValid("offset_world"))
                {
                    iKData.posOffsetType = IKCtrlData.PosOffsetType.OffsetWorld;
                }
                else if (tag_data.IsValid("offset_bone"))
                {
                    iKData.posOffsetType = IKCtrlData.PosOffsetType.OffsetBone;
                }
            }
            iKData.GetIKEnable(iKAttachType).Recet();
            if (iKAttachType == IKCtrlData.IKAttachType.NewPoint && iKData is HandFootIKData)
            {
                HandFootIKData handFootIKData = iKData as HandFootIKData;
                handFootIKData.SetPullState(!tag_data.IsValid("pull_off"));
            }

            source.IKTargetToBone(ik_name, target, targetBoneName, offset, iKAttachType, false, false);

        }
#endif

        internal static void SetDefaultGroupFormation()
        {
            var scenario = ModUseData.ScenarioList.Where(x => x.ScenarioID == StateManager.Instance.UndergoingModEventID).First();
            var yotogiSetup = scenario.YotogiSetup.Where(x => x.Phase == StateManager.Instance.YotogiPhase).First();
            if (yotogiSetup.AllowMap != null)
            {
                PartyGroup.CurrentFormation = yotogiSetup.AllowMap.Where(x => x.MapID == YotogiStageSelectManager.SelectedStage.stageData.id).First().DefaultFormation;
            }
            else if (yotogiSetup.DefaultMap != null)
            {
                PartyGroup.CurrentFormation = yotogiSetup.DefaultMap.DefaultFormation;
            }
        }

        internal static void PlayAnimation(Maid maid, string fileName, string tag, bool isLoop = true, bool isBlend = false, bool isQueued = false)
        {
            if (maid == null)
                return;

            float fade = 0f;
            if (isBlend)
                fade = ConfigurableValue.AnimationBlendTime;
            else
                maid.body0.StopAnime();
            
            maid.body0.LoadAnime(tag, GameUty.FileSystem, fileName, false, isLoop);
            maid.body0.CrossFade(maid.body0.LastAnimeFN, GameUty.FileSystem, additive: false, loop: isLoop, boAddQue: isQueued, fade: fade);
        }

        //function definition copied from KISS code. All load motion script call from the mod should call here so that there is no need to handle the V2 compatible version everywhere
        internal static void LoadMotionScript(int sloat, bool is_next, string file_name, string label_name = "", string maid_guid = "", string man_guid = "", bool face_fix = false, bool valid_pos = true, bool disable_diff_pos = false, bool body_mix_ok = false)
        {
#if COM3D2_5
#if UNITY_2022_3
            GameMain.Instance.ScriptMgr.LoadMotionScript(sloat, is_next, file_name, label_name, maid_guid, man_guid, face_fix, valid_pos, disable_diff_pos, body_mix_ok);
#endif
#endif

#if COM3D2
            GameMain.Instance.ScriptMgr.LoadMotionScript(sloat, is_next, file_name, label_name, maid_guid, man_guid, face_fix, valid_pos, disable_diff_pos);
#endif
        }

        internal static void ApplyMotionInfoToCharacter(Maid maid, MotionInfo motionInfo)
        {
            if (motionInfo == null || maid == null)
                return;

            if (!string.IsNullOrEmpty(motionInfo.RandomMotion))
            {
                //Replace the variable if random motion is set
                motionInfo = RandomList.Motion.GetRandomMotionByCode(motionInfo.RandomMotion, maid.boMAN);
            }

            if (!string.IsNullOrEmpty(motionInfo.ScriptFile))
            {
                string maidGUID = "";
                string manGUID = "";

                if (maid.boMAN)
                    manGUID = maid.status.guid;
                else
                    maidGUID = maid.status.guid;

                LoadMotionScript(0, false, motionInfo.ScriptFile, motionInfo.ScriptLabel, maidGUID, manGUID, false, false, false, false);
            }
            else
            {
                PlayAnimation(maid, motionInfo.MotionFile, motionInfo.MotionTag, motionInfo.IsLoopMotion, motionInfo.IsBlend, motionInfo.IsQueued);
            }
        }

        internal static CharacterMgr.Preset LoadPreset(string presetFileName)
        {
            byte[] presetData = NPCPresetMapping.GetPresetResources(presetFileName);

            BinaryReader binaryReader = new BinaryReader(new MemoryStream(presetData));
#if COM3D2_5
#if UNITY_2022_3
            CharacterMgr.Preset result = CharacterMgr.PresetLoad(binaryReader, Path.GetFileName(presetFileName));
#endif
#endif

#if COM3D2
            CharacterMgr.Preset result = GameMain.Instance.CharacterMgr.PresetLoad(binaryReader, Path.GetFileName(presetFileName));
#endif
            binaryReader.Close();

            return result;
        }

        internal static void SetManClothing(Maid man, bool isNude)
        {
            if (man == null)
                return;
            if (!man.boMAN)
                return;

            //Penis
            man.body0.SetChinkoVisible(isNude);

            //Clothes
            if (StateManager.Instance.ManClothingList.ContainsKey(man.status.guid))
            {

                if (StateManager.Instance.ManClothingList[man.status.guid].IsNude == isNude)
                    return;
                else
                {
                    string fileName;
                    if (isNude)
                        fileName = StateManager.Instance.ManClothingList[man.status.guid].Nude;
                    else
                        fileName = StateManager.Instance.ManClothingList[man.status.guid].Clothed;

                    int ridBody = Path.GetFileName(fileName).ToLower().GetHashCode();
                    man.SetProp(MPN.body, fileName, ridBody);
                    man.AllProcProp();
                }
            }
        }

        internal static void AttachObjectToCharacter(Maid maid, List<ExtraItemObject> extraItemObjects)
        {
            if (maid == null)
                return;

            if (extraItemObjects != null)
            {
                //the objects dont show up if it is not reset first...
                foreach (var extraItem in extraItemObjects)
                    maid.ResetProp(extraItem.Target, true);
                maid.AllProcProp();

                foreach (var extraItem in extraItemObjects)
                {
                    maid.SetProp(extraItem.Target, extraItem.ItemFile, 0, extraItem.IsTemp);
                }
                maid.AllProcProp();
            }
        }

        internal static void RemoveObjectFromCharacter(Maid maid, List<string> positionToRemove)
        {
            if (positionToRemove != null)
            {
                foreach (var position in positionToRemove)
                    maid.ResetProp(position, true);
                maid.AllProcProp();
            }
        }

        internal static void SetFemaleClothing(Maid maid, string clothesSetID)
        {
            if (maid == null || string.IsNullOrEmpty(clothesSetID))
                return;

            if (clothesSetID == Constant.ClothesSetResetCode)
            {
                RestoreMaidClothesInfo(maid);
            }
            else
            {
                ClothesSet targetSet = ModUseData.ClothesSetList[clothesSetID];
                ClothesSet nudeSet = ModUseData.ClothesSetList[ModResources.TextResource.NudeClothesSetID];

                for (int i = 0; i < Constant.DressingClothingTagArray.Length; i++)
                {
                    string slotName = Constant.DressingClothingTagArray[i];
                    string fileName = "";
                    if (targetSet.RequireNude)
                        fileName = nudeSet.Slots[slotName];
                    if (targetSet.Slots.ContainsKey(slotName))
                        fileName = targetSet.Slots[slotName];

                    maid.ResetProp(slotName, true);
                    maid.AllProcProp();
                    maid.SetProp(slotName, fileName, 0, false);
                    maid.AllProcProp();
                }

                if (targetSet.NonClothesSlots != null)
                {
                    foreach(var item in targetSet.NonClothesSlots)
                        maid.SetProp(item.Key, item.Value, 0, true);
                    
                    maid.AllProcProp();
                }
            }
        }

        internal static void RestoreMaidClothesInfo(Maid maid)
        {
            if (maid == null)
                return;
            if (!StateManager.Instance.BackupMaidClothingList.ContainsKey(maid.status.guid))
                return;

            Dictionary<string, string> maidClothesDict = StateManager.Instance.BackupMaidClothingList[maid.status.guid];
            foreach(var kvp in maidClothesDict)
            {
                maid.SetProp(kvp.Key, kvp.Value, 0, false);
            }
            maid.AllProcProp();
        }

        private static void BackupMaidClothesInfo(Maid maid)
        {
            Dictionary<string, string> maidClothesDict = new Dictionary<string, string>();

            for (int i = 0; i < Constant.DressingClothingTagArray.Length; i++)
            {
                maidClothesDict.Add(Constant.DressingClothingTagArray[i], maid.GetProp(Constant.DressingClothingTagArray[i]).strFileName);
            }

            if (!StateManager.Instance.BackupMaidClothingList.ContainsKey(maid.status.guid))
                StateManager.Instance.BackupMaidClothingList.Add(maid.status.guid, maidClothesDict);            
            else
                StateManager.Instance.BackupMaidClothingList[maid.status.guid] = maidClothesDict;
        }

        internal static void AddCharacterEffect(Maid maid, List<string> effectList)
        {
            if (maid == null)
                return;
            if (effectList == null)
                return;

            foreach(string effectID in effectList)
            {
                if (!ModUseData.CharacterEffectList.ContainsKey(effectID))
                    continue;

                CharacterEffect effect = ModUseData.CharacterEffectList[effectID];
                maid.AddPrefab(effect.Prefab, effect.Name, effect.TargetBone, effect.Offset.Pos, effect.Offset.Rot);
            }
        }

        internal static void RemoveCharacterEffect(Maid maid, List<string> effectList)
        {
            if (maid == null)
                return;
            if (effectList == null)
                return;

            foreach (string effectID in effectList)
            {
                if (!ModUseData.CharacterEffectList.ContainsKey(effectID))
                    continue;
                CharacterEffect effect = ModUseData.CharacterEffectList[effectID];
                maid.DelPrefab(effect.Name);
            }
        }

        internal static void ConvertMaidToManStructure(Maid maid, Maid dummyMan)
        {
            SetFemaleClothing(maid, Constant.ClothesSetStrapOnDildo);
            maid.AllProcProp();

            Helper.BoneNameConverter.ConvertFemaleStructureToMale(maid, dummyMan);
            maid.boMAN = true;

            maid.SetProp(MPN.DouPer, 50, true);     //leg length
            maid.SetProp(MPN.sintyou, 50, true);    //height
            maid.SetProp(MPN.UdeScl, 50, true);     //arm length
            maid.SetProp(MPN.kata, 50, true);       //shoulder

            maid.AllProcProp();
        }

        internal static void RecoverMaidFromManStructure(Maid maid)
        {
            Helper.BoneNameConverter.RecoverConvertedMaidStructure(maid);
            maid.boMAN = false;
            maid.ResetProp(MPN.DouPer);
            maid.ResetProp(MPN.sintyou);
            maid.ResetProp(MPN.UdeScl);
            maid.ResetProp(MPN.kata);
            maid.ResetProp(MPN.kousoku_lower);
            maid.AllProcProp();

            RestoreMaidClothesInfo(maid);
        }
    }
}
