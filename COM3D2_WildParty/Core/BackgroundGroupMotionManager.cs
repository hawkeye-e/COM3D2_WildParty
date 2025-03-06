using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;

namespace COM3D2.WildParty.Plugin.Core
{
    internal class BackgroundGroupMotionManager
    {
        private static ManualLogSource Log = WildParty.Log;

        internal static void CheckReviewForEachGroup(ADVKagManager instance, List<PartyGroup> groupList)
        {
            if (groupList == null)
                return;
            if (StateManager.Instance.ModEventProgress != Constant.EventProgress.YotogiPlay)
                return;

            //Skip group zero as it is under player control
            for (int i = 1; i < groupList.Count; i++)
            {
                if (groupList[i].CurrentSexState == SexState.StateType.NormalPlay)
                    HandleGroupForStateNormalPlay(groupList[i]);
                else if (groupList[i].CurrentSexState == SexState.StateType.Orgasm)
                    HandleGroupForStateOrgasm(groupList[i]);
                else if (groupList[i].CurrentSexState == SexState.StateType.OrgasmEnd)
                    HandleGroupForStateOrgasmEnd(instance, groupList[i]);
                else if (groupList[i].CurrentSexState == SexState.StateType.OrgasmWait)
                    HandleGroupForStateOrgasmWait(groupList[i]);
                else if (groupList[i].CurrentSexState == SexState.StateType.ChangePosition)
                    HandleGroupForStateChangePosition(groupList[i]);
                else if (groupList[i].CurrentSexState == SexState.StateType.Insert)
                    HandleGroupForStateInsert(groupList[i]);
                else if (groupList[i].CurrentSexState == SexState.StateType.InsertEnd)
                    HandleGroupForStateInsertEnd(groupList[i]);
                else if (groupList[i].CurrentSexState == SexState.StateType.ChangeMan)
                    HandleGroupForStateChangeMan(groupList[i]);
                else if (groupList[i].CurrentSexState == SexState.StateType.ChangeManFromQueue)
                    HandleGroupForStateChangeManFromQueue(groupList[i]);
            }
        }

        //Handle the sensual value of the group and determine if it is estrus or not
        private static void ProcessSensual(PartyGroup group)
        {
            if (group.IsEstrus)
            {
                //int decayRate = RNG.Random.Next(ConfigurableValue.YotogiSimulation.MinSensualRateDecay, ConfigurableValue.YotogiSimulation.MaxSensualRateDecay);
                int decayRate = RNG.Random.Next(Config.MinSensualRateDecay, Config.MaxSensualRateDecay);
                group.SensualRate -= decayRate;

                if (group.SensualRate <= 0)
                {
                    //reach the bottom and no longer estrus mode
                    group.SensualRate = 0;
                    group.IsEstrus = false;
                    group.RequestMotionLabelChange();
                }
            }
            else
            {
                //int increaseRate = RNG.Random.Next(ConfigurableValue.YotogiSimulation.MinSensualRateIncrease, ConfigurableValue.YotogiSimulation.MaxSensualRateIncrease);
                int increaseRate = RNG.Random.Next(Config.MinSensualRateIncrease, Config.MaxSensualRateIncrease);
                group.SensualRate += increaseRate;

                if (group.SensualRate >= Constant.SensualRateMaxCap)
                {
                    //reach max value and switch to estrus mode
                    group.SensualRate = Constant.SensualRateMaxCap;
                    group.IsEstrus = true;
                    group.RequestMotionLabelChange();
                }
            }


        }

        private static void HandleGroupForStateNormalPlay(PartyGroup group)
        {
            if (DateTime.Now > group.NextActionReviewTime)
            {
                int oldExciteValue = group.Maid1.status.currentExcite;
                //Randomly increase the excitement rate
                //group.Maid1.status.currentExcite += RNG.Random.Next(ConfigurableValue.YotogiSimulation.MinExcitementRateIncrease, ConfigurableValue.YotogiSimulation.MaxExcitementRateIncrease);
                group.Maid1.status.currentExcite += RNG.Random.Next(Config.MinExcitementRateIncrease, Config.MaxExcitementRateIncrease);
                group.Maid1.status.currentExcite = Math.Min(group.Maid1.status.currentExcite, Constant.ExcitementRateMaxCap);

                ProcessSensual(group);

                //Determine if orgasm
                if (Util.IsBackgroundMaidOrgasm(group.Maid1))
                {

                    //Set the next review time to Max to prevent the group enter the regular time check flow.
                    group.StopNextReviewTime();

                    //Change to next possible state
                    group.CurrentSexState = GetNextSexState(group.CurrentSexState);
                }
                else
                {
                    bool requireAnimationChange = false;

                    bool isChangeMotionLabel = group.IsChangeMotionLabel();
                    if (isChangeMotionLabel)
                        requireAnimationChange = true;

                    //If excitement level change the motion has to be updated
                    if (Util.IsExcitementLevelChanged(oldExciteValue, group.Maid1.status.currentExcite))
                        requireAnimationChange = true;

                    //Update animation if necessary
                    if (requireAnimationChange)
                    {
                        YotogiHandling.ChangeBackgroundGroupSexPosition(group, group.SexPosID, isChangeMotionLabel);
                    }

                    //Assign a new review time
                    group.GenerateNextReviewTime();
                }

            }
        }



        private static void HandleGroupForStateOrgasm(PartyGroup group)
        {
            //Re-Initiate the excitement rate
            group.Maid1.status.currentExcite = Util.GetRandomExcitementRate();

            //play the orgasm animation 
            MotionSpecialLabel pickedLabel = YotogiHandling.ChangeBackgroundGroupMotionWithSpecificLabel(group, MotionSpecialLabel.LabelType.Orgasm);
            group.CurrentOrgasmLabelRecord = pickedLabel;

            //set to review this group after the orgasm animation ended
            var clip = group.Maid1.body0.m_Animation.GetClip(group.CurrentMaid1AnimationClipName);
            group.RequestNextReviewTimeAfter(clip.length);

            //play the audio specfic
            if (!group.IsVoicelessGroup)
            {
                group.IsMaid1OrgasmScreamSet = CharacterHandling.SetOrgasmScreamVoice(group.Maid1, pickedLabel.VoiceType1);
                group.IsMaid2OrgasmScreamSet = CharacterHandling.SetOrgasmScreamVoice(group.Maid2, pickedLabel.VoiceType2);
            }

            //also update the progress info
            YotogiHandling.AddManOrgasmCountForGroup(group);

            group.CurrentSexState = GetNextSexState(group.CurrentSexState);
        }

        /*
         * Due to TagTexMulAdd will call GetMaidAndMan function which will mess up the spoofing logic for other parts, I dont directly call TagTexMulAdd but follow the logic inside.
           Drawback: There are some special handlings on CRC body in TagTexMulAdd, so the effect wont apply in 3.0 version(I have no idea whether this mod will work or not in v3.0 anyway)
         */
        private static void ProcessSemen(Maid maid, SemenPattern pattern)
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

        //Mainly for adding the semen texture
        private static void HandleGroupForStateOrgasmEnd(ADVKagManager instance, PartyGroup group)
        {
            bool voicePlayed = true;
            if (group.IsMaid1OrgasmScreamSet)
                voicePlayed = voicePlayed && !group.Maid1.AudioMan.isPlay();
            if (group.Maid2 != null && group.IsMaid2OrgasmScreamSet)
                voicePlayed = voicePlayed && !group.Maid2.AudioMan.isPlay();

            if (voicePlayed)
            {
                //the orgasm voice is all finished for this group
                group.IsMaid1OrgasmScreamSet = false;
                group.IsMaid2OrgasmScreamSet = false;

                //add semen
                ProcessSemenForGroup(group);

                group.CurrentSexState = GetNextSexState(group.CurrentSexState);
            }
        }

        internal static void ProcessSemenForGroup(PartyGroup group)
        {
            BackgroundGroupMotion.MotionItem motionItem = Util.GetMotionItemOfGroup(group);

            if (ModUseData.SemenPatternList.ContainsKey(group.CurrentOrgasmLabelRecord.SemenType1))
                ProcessSemen(Util.GetSemenTarget(group, group.CurrentOrgasmLabelRecord.SemenTarget1), ModUseData.SemenPatternList[group.CurrentOrgasmLabelRecord.SemenType1]);

            if (!string.IsNullOrEmpty(group.CurrentOrgasmLabelRecord.SemenType2))
            {
                if (ModUseData.SemenPatternList.ContainsKey(group.CurrentOrgasmLabelRecord.SemenType2))
                    ProcessSemen(Util.GetSemenTarget(group, group.CurrentOrgasmLabelRecord.SemenTarget2), ModUseData.SemenPatternList[group.CurrentOrgasmLabelRecord.SemenType2]);
            }

            if (!string.IsNullOrEmpty(group.CurrentOrgasmLabelRecord.SemenType3))
            {
                if (ModUseData.SemenPatternList.ContainsKey(group.CurrentOrgasmLabelRecord.SemenType3))
                    ProcessSemen(Util.GetSemenTarget(group, group.CurrentOrgasmLabelRecord.SemenTarget3), ModUseData.SemenPatternList[group.CurrentOrgasmLabelRecord.SemenType3]);
            }
        }

        //The whole orgasm motion is ended
        private static void HandleGroupForStateOrgasmWait(PartyGroup group)
        {
            if (DateTime.Now > group.NextActionReviewTime)
            {
                //When the orgasm motion ended, Change the voice to orgasm wait
                BackgroundGroupMotion.MotionItem motionItem = Util.GetMotionItemOfGroup(group);
                
                int excitementLevel = group.ExcitementLevel;

                CharacterHandling.SetCharacterVoiceEntry(group.Maid1, PersonalityVoice.VoiceEntryType.OrgasmWait, excitementLevel, group.CurrentOrgasmLabelRecord.WaitLabel1, group.IsEstrus, group.IsVoicelessGroup);
                group.Maid1VoiceType = group.CurrentOrgasmLabelRecord.WaitLabel1;

                if (group.Maid2 != null)
                {
                    CharacterHandling.SetCharacterVoiceEntry(group.Maid2, PersonalityVoice.VoiceEntryType.OrgasmWait, excitementLevel, group.CurrentOrgasmLabelRecord.WaitLabel2, group.IsEstrus, group.IsVoicelessGroup);
                    group.Maid2VoiceType = group.CurrentOrgasmLabelRecord.WaitLabel2;
                }

                CharacterHandling.SetGroupFace(group);

                //assign a random review time to make differences in a short waiting for different group, and set to next state
                //group.GenerateNextReviewTime(ConfigurableValue.YotogiSimulation.MaxTimeToResumeSexAfterOrgasm);
                group.GenerateNextReviewTime(Config.MaxTimeToResumeSexAfterOrgasm);
                group.CurrentSexState = GetNextSexState(group.CurrentSexState);
            }
        }

        private static void HandleGroupForStateChangePosition(PartyGroup group)
        {
            if (DateTime.Now > group.NextActionReviewTime)
            {
                List<string> possibleGroupTypes = new List<string> { group.GroupType };
                List<BackgroundGroupMotion.MotionItem> lstMotion = new List<BackgroundGroupMotion.MotionItem>();
                if (Util.GetUndergoingScenario().YotogiSetup.Where(x => x.Phase == StateManager.Instance.YotogiPhase).First().FlexibleManCountInYotogi)
                    possibleGroupTypes = group.GetPossibleGroupType();

                foreach (var groupType in possibleGroupTypes)
                {
                    lstMotion.AddRange(ModUseData.BackgroundMotionList[groupType].Where(x => x.Phase == StateManager.Instance.YotogiPhase && x.IsBGGroupUse).ToList());
                }
                
                int rndMotion = RNG.Random.Next(lstMotion.Count);

                string targetGroupType = "";
                foreach (var groupType in possibleGroupTypes)
                {
                    if(ModUseData.BackgroundMotionList[groupType].Any(x => x.ID == lstMotion[rndMotion].ID))
                    {
                        targetGroupType = groupType;
                        break;
                    }
                }

                if(group.GroupType != targetGroupType)
                    YotogiHandling.ConvertToGroupType(group, targetGroupType, lstMotion[rndMotion].ID);

                group.SexPosID = lstMotion[rndMotion].ID;
                YotogiHandling.ChangeBackgroundGroupMotionWithSpecificLabel(group, SexState.StateType.Waiting);
                //we have to request a new label for the new motion when enter normal play
                group.RequestMotionLabelChange();

                group.CurrentSexState = GetNextSexState(group.CurrentSexState);
            }
        }

        private static void HandleGroupForStateInsert(PartyGroup group)
        {
            //This state may not exist label for certain motion, need to handle that
            if (DateTime.Now > group.NextActionReviewTime)
            {
                group.CurrentOrgasmLabelRecord = null;

                //Check if the current motion does have insert state, skip if it doesnt
                BackgroundGroupMotion.MotionItem motionItem = Util.GetMotionItemOfGroup(group);
                MotionSpecialLabel insertLabel = motionItem.SpecialLabels.Where(x => x.Type == MotionSpecialLabel.LabelType.Insert).FirstOrDefault();

                if (insertLabel == null)
                {
                    //no insert state, skip to the next state
                    group.CurrentSexState = GetNextSexState(group.CurrentSexState);
                    return;
                }

                //play the insert motion
                YotogiHandling.ChangeBackgroundGroupMotionWithSpecificLabel(group, SexState.StateType.Insert);

                //load the voice
                CharacterHandling.SetCharacterVoiceEntry(group.Maid1, PersonalityVoice.VoiceEntryType.Insert, group.ExcitementLevel, insertLabel.VoiceType1, group.IsEstrus, group.IsVoicelessGroup);
                group.Maid1VoiceType = insertLabel.VoiceType1;

                if (group.Maid2 != null)
                {
                    CharacterHandling.SetCharacterVoiceEntry(group.Maid2, PersonalityVoice.VoiceEntryType.Insert, group.ExcitementLevel, insertLabel.VoiceType2, group.IsEstrus, group.IsVoicelessGroup);
                    group.Maid2VoiceType = insertLabel.VoiceType2;
                }

                //set face
                CharacterHandling.SetGroupFace(group);

                //set next review time to be clip end
                var clip = group.Maid1.body0.m_Animation.GetClip(group.CurrentMaid1AnimationClipName);
                group.RequestNextReviewTimeAfter(clip.length);

                group.CurrentSexState = GetNextSexState(group.CurrentSexState);
            }
        }

        private static void HandleGroupForStateInsertEnd(PartyGroup group)
        {
            if (DateTime.Now > group.NextActionReviewTime)
            {
                //switch to normal play
                group.CurrentSexState = GetNextSexState(group.CurrentSexState);
            }
        }

        private static void HandleGroupForStateChangeMan(PartyGroup group)
        {
            if (DateTime.Now > group.NextActionReviewTime)
            {
                YotogiHandling.ChangeManMembersShareListType(group);

                YotogiHandling.ChangeBackgroundGroupMotionWithSpecificLabel(group, MotionSpecialLabel.LabelType.Waiting);
                group.CurrentSexState = GetNextSexState(group.CurrentSexState);
            }
        }

        private static void HandleGroupForStateChangeManFromQueue(PartyGroup group)
        {
            if (DateTime.Now > group.NextActionReviewTime)
            {
                YotogiHandling.ChangeManMembersQueueType(group, new EventDelegate(() => OnChangeManMembersQueueTypeFinish(group)) );

                //The next review time will be resumed after all change man member process finished
                group.StopNextReviewTime();
            }
        }

        private static void OnChangeManMembersQueueTypeFinish(PartyGroup group)
        {
            group.GenerateNextReviewTime(0);
            group.CurrentSexState = GetNextSexState(group.CurrentSexState);
        }


        internal static string GetNextSexState(string currentState)
        {
            int rnd = RNG.Random.Next(ModUseData.SexStateList[currentState].NextStates.Count);
            return ModUseData.SexStateList[currentState].NextStates[rnd];
        }

        internal static void InitNextReviewTimer()
        {
            //For group zero, the player control all the action so there is no review time
            StateManager.Instance.PartyGroupList[0].NextActionReviewTime = DateTime.MaxValue;

            //Assign a review time for each group except group zero
            for (int i = 1; i < StateManager.Instance.PartyGroupList.Count; i++)
            {
                if (StateManager.Instance.PartyGroupList[i].IsAutomatedGroup)
                    StateManager.Instance.PartyGroupList[i].GenerateNextReviewTime();
                else
                    StateManager.Instance.PartyGroupList[i].NextActionReviewTime = DateTime.MaxValue;
            }
        }
    }
}
