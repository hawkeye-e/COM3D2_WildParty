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
                if (StateManager.Instance.WaitForUserClick)
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

                if (currentStep.PostYotogi != null)
                {
                    if (currentStep.PostYotogi.IsKeepCharacterVisible)
                        return false;
                }
            }

            return true;
        }
    }
}
