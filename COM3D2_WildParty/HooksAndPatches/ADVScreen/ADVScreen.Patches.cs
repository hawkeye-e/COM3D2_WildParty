using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using UnityEngine;
using HarmonyLib;

namespace COM3D2.WildParty.Plugin.HooksAndPatches.ADVScreen
{
    internal class Patches
    {
        
        internal static void CheckStartADVScene()
        {
            if (StateManager.Instance.UndergoingModEventID > 0 && StateManager.Instance.ModEventProgress == Constant.EventProgress.Init)
            {
                var scenario = ModUseData.ScenarioList.Where(x => x.ScenarioID == StateManager.Instance.UndergoingModEventID).First();
                StateManager.Instance.ModEventProgress = Constant.EventProgress.IntroADV;
                StateManager.Instance.CurrentADVStepID = scenario.ADVEntryStep;
                GameMain.Instance.CharacterMgr.GetMaid(0).Visible = false;
                GameMain.Instance.LoadScene(Constant.SceneType.ADV);
            }
        }


        internal static void SpoofGetTagList(ref Dictionary<string, string> result)
        {
            if (StateManager.Instance.SpoofTagListResult)
            {
                result = new Dictionary<string, string>();
                foreach (var kvp in StateManager.Instance.TagBackUp)
                    result.Add(kvp.Key, kvp.Value);
            }
        }

        internal static void SpoofGetTagListEnd()
        {
            if (StateManager.Instance.SpoofTagListResult)
            {
                StateManager.Instance.SpoofTagListResult = false;
                StateManager.Instance.TagBackUp = null;
            }
        }

        internal static void CheckRequireEnterModScenario(CharacterSelectMain instance)
        {
            if (StateManager.Instance.RequireCheckModdedSceneFlag)
            {
                //Prevent selecting a sub chara with mod schedule causing the game to enter the mod scenario flow
                if (instance.parent_mgr is SceneCharacterSelect)
                {
                    if (((SceneCharacterSelect)instance.parent_mgr).select_type != SceneCharacterSelect.SelectType.NewYotogi)
                        return;
                }
                else
                    return;

                //After the player select a maid for some schedule, check if the maid is assigned with schedule task added by mod

                Maid selectedMaid = Traverse.Create(instance).Field(Constant.DefinedClassFieldNames.CharacterSelectMainSelectedMaid).GetValue<Maid>();

                if (selectedMaid != null)
                {
                    List<int> moddedScenarioIDs = ModUseData.ScenarioList.Select(x => x.ScenarioID).ToList();
                    int selectedMaidScheduleID = Schedule.ScheduleAPI.GetScheduleId(selectedMaid, GameMain.Instance.CharacterMgr.status.isDaytime);

                    if (moddedScenarioIDs.Contains(selectedMaidScheduleID))
                    {
                        ModUseData.InitDataForScenario(selectedMaidScheduleID);

                        //Hit, set the flag so that the system will goes through the mod adv scene
                        var scenario = ModUseData.ScenarioList.Where(x => x.ScenarioID == selectedMaidScheduleID).First();

                        StateManager.Instance.UndergoingModEventID = selectedMaidScheduleID;
                        StateManager.Instance.ModEventProgress = Constant.EventProgress.Init;

                        StateManager.Instance.ExtraCommandWindow = new CustomGameObject.YotogiExtraCommandWindow(StateManager.Instance.ExtraCommandWindowMasterCopy.transform.gameObject);                        
                    }
                }
            }
        }

        internal static void CheckWaitForFullLoadDone(ADVKagManager instance)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                //Checking if all the game objects we need are all loaded
                if (StateManager.Instance.WaitForFullLoadList.Count > 0)
                {
                    int i = 0;
                    while (i < StateManager.Instance.WaitForFullLoadList.Count)
                    {
                        if (!StateManager.Instance.WaitForFullLoadList[i].IsAllProcPropBusy)
                            StateManager.Instance.WaitForFullLoadList.RemoveAt(i);
                        else
                            i++;
                    }

                }

                if (StateManager.Instance.WaitForFullLoadList.Count == 0)
                    Core.CustomADVProcessManager.ProcessADVStep(instance);
            }
        }

        internal static void HandleModADVScenarioUserInput()
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (StateManager.Instance.WaitForUserClick && !StateManager.Instance.WaitForMotionChange)
                {
                    Core.CustomADVProcessManager.ADVSceneProceedToNextStep();
                }
            }
        }

        internal static void ReturnToMainTitleHandling()
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                Core.ModEventCleanUp.ResetModEvent();
                StateManager.Instance.ModEventProgress = Constant.EventProgress.None;
            }
        }

        internal static bool CheckAudioChoppingPlayback(SoundMgr soungMgr, string strFileName, int voicePitch, AudioSourceMgr.Type soundType)
        {
            if (Helper.AudioChoppingManager.SubClipLibrary.ContainsKey(strFileName))
            {
                //if exists, play the voice stored in the memory and skip the normal flow
                soungMgr.m_AudioDummyVoice.Pitch = SoundMgr.ConvertToAudioSourcePitch(voicePitch);
                soungMgr.m_AudioDummyVoice.audiosource.outputAudioMixerGroup = soungMgr.mix_mgr[soundType];
                soungMgr.m_AudioDummyVoice.audiosource.clip = Helper.AudioChoppingManager.SubClipLibrary[strFileName];
                soungMgr.m_AudioDummyVoice.audiosource.Play();
                return false;
            }

            return true;
        }

        internal static bool IsAllowHideCharacters()
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                ADVStep currentStep = ModUseData.ADVStepData[StateManager.Instance.UndergoingModEventID][StateManager.Instance.CurrentADVStepID];

                if (currentStep.YotogiSetup != null)
                {
                    if (currentStep.YotogiSetup.IsKeepCharacterVisibleAfterYotogi)
                        return false;
                }
            }

            return true;
        }

        internal static void StartIndividualOffsetHandling(BaseKagManager baseKagManager)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (baseKagManager is MotionKagManager)
                {
                    MotionKagManager motionKagManager = (MotionKagManager)baseKagManager;
                    StartMotionKagIndividualOffsetHandling(motionKagManager);
                }
            }
        }

        internal static void StartMotionKagIndividualOffsetHandling(MotionKagManager motionKagManager)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (motionKagManager.main_maid != null)
                {
                    PartyGroup group = Util.GetPartyGroupByCharacter(motionKagManager.main_maid);
                    if (group != null)
                    {
                        StateManager.Instance.IsMotionKagSetPosition = true;
                        StateManager.Instance.CurrentMotionKagHandlingGroup = group;
                    }
                }
            }
        }

        internal static void EndIndividualOffsetHandling()
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                StateManager.Instance.IsMotionKagSetPosition = false;
                StateManager.Instance.CurrentMotionKagHandlingGroup = null;
            }
        }

        internal static bool IsPlaySE(string fileName)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (Util.GetUndergoingScenario().IgnoreEjaculationSE)
                {
                    if (Constant.EjaculateSEFileArray.Contains(fileName))
                        return false;
                }
            }
            return true;
        }
    }
}
