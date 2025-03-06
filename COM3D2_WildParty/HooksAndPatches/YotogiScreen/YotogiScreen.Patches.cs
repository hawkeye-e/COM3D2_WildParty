using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace COM3D2.WildParty.Plugin.HooksAndPatches.YotogiScreen
{
    internal class Patches
    {
        internal static Maid[] SpoofUndressingMaidArray(UndressingManager instance)
        {
            Maid[] backup = null;
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                var maidDataTraverse = Traverse.Create(instance).Field(Constant.DefinedClassFieldNames.UndressingManagerAllMaidArray);
                backup = maidDataTraverse.GetValue<Maid[]>();

                var spoofMaidArray = StateManager.Instance.SelectedMaidsList.ToArray();
                maidDataTraverse.SetValue(spoofMaidArray);
            }

            return backup;
        }

        internal static void RecoverUndressingMaidArray(UndressingManager instance, Maid[] original)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                Traverse.Create(instance).Field(Constant.DefinedClassFieldNames.UndressingManagerAllMaidArray).SetValue(original);
            }
        }

        internal static void RemoveAllSemen()
        {
            if (StateManager.Instance.UndergoingModEventID > 0 && StateManager.Instance.SelectedMaidsList != null)
            {
                foreach (var maid in StateManager.Instance.SelectedMaidsList)
                {
                    if (maid != null)
                    {
                        Core.CharacterHandling.RemoveSemenTexture(maid);
                    }
                }

                foreach (var maid in StateManager.Instance.NPCList)
                {
                    if (maid != null)
                    {
                        Core.CharacterHandling.RemoveSemenTexture(maid);
                    }
                }
            }
        }


        internal static bool CanEarnExperience(int modEventID)
        {
            if (modEventID > 0)
            {
                //if it is the mod scenario, we may not want to show the skill icon as the skills used may not be a yotogi skill that can gain any experiences
                Scenario scenario = ModUseData.ScenarioList.Where(x => x.ScenarioID == modEventID).First();
                if (!scenario.CanGainSkillExp)
                {
                    return false;
                }

            }
            return true;
        }

        internal static void HandleYotogiPlayEnd()
        {
            //Since the value of the Next Label is removed when changing skill by our mod, here we fix it before the system process the click action
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (StateManager.Instance.IsFinalYotogi)
                {
                    StateManager.Instance.ModEventProgress = Constant.EventProgress.YotogiEnd;
                    StateManager.Instance.YotogiManager.null_mgr.SetNextLabel(Constant.NextButtonLabel.YotogiPlayEnd);
                }
                else
                {
                    StateManager.Instance.ModEventProgress = Constant.EventProgress.ADV;
                }
                
                //To make the result window display the proper fetish added message
                foreach (int fetish in StateManager.Instance.YotogiProgressInfoList[StateManager.Instance.YotogiManager.maid.status.guid].CustomFetishEarned)
                    StateManager.Instance.YotogiManager.acquired_propensityid_list.Add(fetish);
            }
        }

        //Preseve the ExtraCommandWindow from being destroyed.
        internal static void PreserveExtraCommandWindow()
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                StateManager.Instance.ExtraCommandWindow.transform.SetParent(null);
                StateManager.Instance.ExtraCommandWindow.SetVisible(false);
                StateManager.Instance.ExtraCommandWindow.SetActive(true);
            }
        }

        internal static void DisplayExecConditionPanel(UIButtonColor instance, bool isOver)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                foreach (var btn in StateManager.Instance.InjectedButtons)
                {
                    if (btn.Button.gameObject.GetInstanceID() == instance.gameObject.GetInstanceID())
                    {
                        UIButton button = btn.Button.GetComponent<UIButton>();
                        //only try to show panel if it is not enabled yet
                        if (isOver && !button.isEnabled)
                        {
                            if (btn.Data.Type == ExtraYotogiCommandData.CommandType.Fetish)
                            {

                                List<KeyValuePair<string[], Color>> displayConditions = new List<KeyValuePair<string[], Color>>();

                                Fetish fetishInfo = ModUseData.FetishList.Where(x => x.ID == btn.Data.FetishID).First();
                                foreach (ExtraYotogiCommandData.ConditionCheck condition in btn.Data.ConditionCheckTexts)
                                {
                                    string[] displayText = new string[1];
                                    displayText[0] = Core.YotogiExtraCommandHandling.ReplaceFetishConditionText(condition.DisplayText, fetishInfo);

                                    Color displayColor;
                                    if (Core.YotogiExtraCommandHandling.IsThisConditionFulfilled(condition.Field, fetishInfo))
                                        displayColor = Color.white;
                                    else
                                        displayColor = Color.gray;

                                    displayConditions.Add(new KeyValuePair<string[], Color>(displayText, displayColor));
                                }


                                Core.YotogiExtraCommandHandling.ShowExecConditionPanel(btn.Button, displayConditions);
                            }
                        }
                        else
                        {
                            Core.YotogiExtraCommandHandling.HideExecConditionPanel();
                        }
                    }
                }
            }
        }

        internal static void InitExtraCommandButtons()
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                InjectCommandButtons();
                Core.YotogiExtraCommandHandling.CheckExtraYotogiCommandCondition(StateManager.Instance.InjectedButtons);
                AttachExtraCommandWindow();
            }
        }

        internal static void InjectCommandButtons()
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (StateManager.Instance.YotogiCommandFactory != null && StateManager.Instance.RequireInjectCommandButtons)
                {
                    StateManager.Instance.RequireInjectCommandButtons = false;

                    GameObject btn;

                    foreach (var commandType in Util.GetUndergoingScenario().YotogiSetup.Where(x => x.Phase == StateManager.Instance.YotogiPhase).First().ExtraYotogiCommands)
                    {
                        ExtraYotogiCommandData commandInfo = ModUseData.ExtraYotogiCommandDataList[commandType];
                        
                        if (commandInfo.Constraint != null && commandInfo.Constraint.Count > 0)
                        {
                            var constraintInfo = commandInfo.Constraint.Where(x => x.EventID == StateManager.Instance.UndergoingModEventID).FirstOrDefault();
                            if(constraintInfo != null)
                            {
                                if (!constraintInfo.SexPosIDs.Contains(StateManager.Instance.PartyGroupList[0].SexPosID))
                                    continue;
                            }
    
                        }

                        btn = Core.YotogiExtraCommandHandling.InjectCommandButton(commandInfo.Name, Core.YotogiExtraCommandHandling.GetButtonCallbackFromString(commandType), StateManager.Instance.YotogiCommandFactory.transform);

                        CustomGameObject.InjectYotogiCommand newCommand = new CustomGameObject.InjectYotogiCommand();
                        newCommand.Button = btn;
                        newCommand.Data = commandInfo;

                        StateManager.Instance.InjectedButtons.Add(newCommand);
                    }
                }
            }
        }


        private static void AttachExtraCommandWindow()
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                
                if (StateManager.Instance.YotogiCommandFactory != null && StateManager.Instance.ExtraCommandWindow.IsRequireInit())
                {
                    //locate the transform of the Skill Viewer panel, we will attach the extra command window there
                    var skillViewerTransform = StateManager.Instance.YotogiCommandFactory.transform;
                    while (true)
                    {
                        if (skillViewerTransform == null)
                            break;
                        if (skillViewerTransform.name == Constant.DefinedGameObjectNames.SkillViewerPanel)
                            break;

                        skillViewerTransform = skillViewerTransform.parent;
                    }


                    StateManager.Instance.ExtraCommandWindow.transform.SetParent(skillViewerTransform.parent, false);

                    StateManager.Instance.ExtraCommandWindow.InitStructureAfterAttaching();
                }
            }
        }

        internal static void InitForYotogiScene(YotogiManager instance)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (StateManager.Instance.ModEventProgress == Constant.EventProgress.YotogiInit)
                {
                    //Update the flags
                    StateManager.Instance.ModEventProgress = Constant.EventProgress.YotogiPlay;
                    StateManager.Instance.RequireInjectCommandButtons = true;
                    StateManager.Instance.YotogiManager = instance;

                    Scenario scenario = ModUseData.ScenarioList.Where(x => x.ScenarioID == StateManager.Instance.UndergoingModEventID).First();
                    Scenario.YotogiSetupInfo yotogiSetup = scenario.YotogiSetup.Where(x => x.Phase == StateManager.Instance.YotogiPhase).First();

                    Core.YotogiHandling.YotogiSkillCall(instance, ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].DefaultSexPosID);

                    if (yotogiSetup.AllowMap != null)
                    {
                        Core.YotogiHandling.PlayRoomBGM(instance);
                    }
                    else if(yotogiSetup.DefaultMap != null)
                    {
                        //Set the dummy stage to prevent crash and also spoof flag to avoid changing bg
                        StateManager.Instance.SpoofChangeBackgroundFlag = true;
                        YotogiStageSelectManager.SelectStage(YotogiStage.GetAllDatas(true)[0], null, GameMain.Instance.CharacterMgr.status.isDaytime);

                        if (GameMain.Instance.CharacterMgr.status.isDaytime)
                            GameMain.Instance.BgMgr.ChangeBg(yotogiSetup.DefaultMap.DayMapID);
                        else
                            GameMain.Instance.BgMgr.ChangeBg(yotogiSetup.DefaultMap.NightMapID);
                        GameMain.Instance.SoundMgr.PlayBGM(yotogiSetup.DefaultMap.BGM, 1);

                    }

                    Core.YotogiHandling.SetGroupToScene();

                    //assign the club owner motion
                    MapCoorindates coordInfo = ModUseData.MapCoordinateList[PartyGroup.CurrentFormation];
                    if (coordInfo.SpecialCoordinates != null)
                    {
                        var ownerAction = coordInfo.SpecialCoordinates.Where(x => x.Type == Constant.SpecialCoordinateType.Owner).FirstOrDefault();
                        if (ownerAction != null)
                            if (ownerAction.IsMasturbation)
                                Core.YotogiHandling.SetMasturbMotionToCharacter(StateManager.Instance.ClubOwner, MasturbationMotion.Type.ManKneeDown, true);
                    }
                }
                else
                {
                    //This is for the case of showing the yotogi result

                    Util.SetAllPartyMemberVisibility(false);

                    //need to set the label to make the next button working
                    instance.null_mgr.SetNextLabel(Constant.OrgySceneLabel.StageSelected);
                }
            }
        }


        internal static void ApplyUnlimitedMind(Maid maid)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (ModUseData.ScenarioList.Where(x => x.ScenarioID == StateManager.Instance.UndergoingModEventID).First().UnlimitedMind)
                {
                    maid.status.currentMind = maid.status.maxMind;
                }
            }
        }

        //Process when the player clicks Next button in the yotogi play scene
        internal static void ProcessYotogiPlayEnd(WfScreenChildren instance)
        {
            if (instance.GetType() == typeof(YotogiPlayManager) && StateManager.Instance.ModEventProgress == Constant.EventProgress.YotogiEnd)
            {
                //terminate all the automated process of background groups
                foreach (var group in StateManager.Instance.PartyGroupList)
                {
                    group.StopAudio();
                    group.StopNextReviewTime();
                }

                //To fix the invalid status that causing the scene unable to go through the finish process when player click "next" in the yotogi play scene.
                if (instance.fade_status != WfScreenChildren.FadeStatus.Wait)
                {
                    Traverse.Create(instance).Field(Constant.DefinedClassFieldNames.WfScreenChildrenFadeStatus).SetValue(WfScreenChildren.FadeStatus.Wait);
                }
            }
        }


        //Process when the player clicks Next button in the yotogi result scene
        internal static void ProcessYotogiResultEnd(WfScreenChildren instance)
        {
            if (instance.GetType() == typeof(YotogiResultManager) && StateManager.Instance.ModEventProgress == Constant.EventProgress.YotogiEnd)
            {
                //try to connect to the next step
                StateManager.Instance.ModEventProgress = Constant.EventProgress.PostYotogiADV;
                Core.CustomADVProcessManager.ADVSceneProceedToNextStep();
                GameMain.Instance.LoadScene(Constant.SceneType.ADV);
            }
        }

        internal static void SetCameraFocusOnDefaultMaid()
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (StateManager.Instance.ModEventProgress == Constant.EventProgress.YotogiPlay)
                {
                    if (!StateManager.Instance.ForceNoCameraResetAfterFadeIn)
                    {
                        if (ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].CameraSetup != null)
                        {
                            Core.CameraHandling.SetCameraLookAtFixedPoint(ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].CameraSetup);
                        }
                        else
                        {
                            if (StateManager.Instance.PartyGroupList != null && StateManager.Instance.PartyGroupList.Count > 0)
                                Core.CameraHandling.SetCameraLookAt(StateManager.Instance.PartyGroupList[0].Maid1);
                        }
                    }
                }
            }
        }



        internal static void ResetYotogiPlaySkillArray(YotogiManager instance, Yotogis.Skill.Data skillData)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                KeyValuePair<Yotogis.Skill.Data, bool>[] array = new KeyValuePair<Yotogis.Skill.Data, bool>[1];
                array[0] = new KeyValuePair<Yotogis.Skill.Data, bool>(skillData, true);
                instance.SetPlaySkillArray(array);
            }
        }

        internal static bool IsAllowChangeBackground()
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (StateManager.Instance.ModEventProgress == Constant.EventProgress.YotogiEnd)
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool CheckJumpLabelStageSelected(string labelName)
        {
            //Connect the scene after stage selected 
            if (labelName == Constant.OrgySceneLabel.StageSelected)
            {
                Core.CustomADVProcessManager.ADVSceneProceedToNextStep();

                return false;
            }
            return true;
        }

        internal static bool CheckJumpLabelSessionResultEnd(BaseKagManager instance, string labelName)
        {
            if (labelName == Constant.GameSceneLabel.NoonResultEnd || labelName == Constant.GameSceneLabel.NightResultEnd)
            {
                //No idea when the external dll change to point to another .ks file. Load the correct script file once again to make sure the game jump back to the main flow correctly.
                instance.LoadScriptFile(Constant.MainEventScriptFile);

                //Reset the last flag
                StateManager.Instance.ModEventProgress = Constant.EventProgress.None;
            }
            return true;
        }

        internal static void FilterStageSelection(YotogiStageSelectManager.StageExpansionPack stagePack, ref bool enabled)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                Scenario scenario = ModUseData.ScenarioList.Where(x => x.ScenarioID == StateManager.Instance.UndergoingModEventID).First();
                Scenario.YotogiSetupInfo yotogiSetup = scenario.YotogiSetup.Where(x => x.Phase == StateManager.Instance.YotogiPhase).First();
                var allowMaps = yotogiSetup.AllowMap.Select(x => x.MapID).ToList();
                enabled = allowMaps.Contains(stagePack.stageData.id);
            }
        }

        internal static void StartSpoofingLoadMotionScript(string label_name, string maid_guid, string man_guid)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                StateManager.Instance.processingMaidGUID = maid_guid;
                StateManager.Instance.processingManGUID = man_guid;

                if (maid_guid == "")
                {
                    //this is group zero. we try mark down the label name here
                    if (StateManager.Instance.PartyGroupList != null)
                        if (StateManager.Instance.PartyGroupList.Count > 0)
                            StateManager.Instance.PartyGroupList[0].CurrentLabelName = label_name;

                }
            }
        }

        internal static void EndSpoofingLoadMotionScript()
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                StateManager.Instance.processingMaidGUID = "";
                StateManager.Instance.processingManGUID = "";
            }
        }

        internal static void LocateCommandMaskGroupTransform()
        {
            if (StateManager.Instance.CommandMaskGroupTransform == null)
            {
                if (StateManager.Instance.YotogiManager != null)
                    if (StateManager.Instance.YotogiManager.play_mgr != null)
                    {
                        var pc = Traverse.Create(StateManager.Instance.YotogiManager.play_mgr).Field(Constant.DefinedClassFieldNames.YotogiPlayManagerPositionChanger).GetValue<YotogiPositionChanger>();
                        StateManager.Instance.CommandMaskGroupTransform = pc.m_CommandViewer.transform.Find(Constant.DefinedSearchPath.MaskGroupPathFromCommandViewer);
                    }
            }
        }

        internal static void CheckHideExtraCommandWindow(GameObject target, System.Collections.Hashtable args)
        {
            if (StateManager.Instance.CommandMaskGroupTransform != null)
            {
                //If the target object matches the main command window, hide the extra command window if the main one is sliding out
                if (StateManager.Instance.CommandMaskGroupTransform.GetInstanceID() == target.transform.GetInstanceID())
                {
                    if (args.Contains("position") && args["position"].GetType() == typeof(Vector3))
                    {
                        Vector3 movingTo = (Vector3)args["position"];
                        if (movingTo.x < 0)
                        {
                            //it is closing
                            StateManager.Instance.ExtraCommandWindow.SetVisible(false);
                        }
                    }
                }
            }
        }

        internal static void SetDesiredSkillName(ref string skillName)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (StateManager.Instance.PartyGroupList.Count > 0)
                {
                    var skill = Util.GetGroupCurrentSkill(StateManager.Instance.PartyGroupList[0]);
                    skillName = skill.DisplayName;
                }
            }
        }

        internal static void SetDesiredSkillNameForCommandCategory()
        {
            if (StateManager.Instance.CommandLabel != null)
            {
                if (StateManager.Instance.UndergoingModEventID > 0)
                {
                    if (StateManager.Instance.PartyGroupList.Count > 0)
                    {
                        var skill = Util.GetGroupCurrentSkill(StateManager.Instance.PartyGroupList[0]);
                        StateManager.Instance.CommandLabel.text = skill.DisplayName;
                    }
                }
            }
        }

        internal static void ReplaceNotSuitableVoice(AudioSourceMgr asMgr, ref string f_strFileName, ref bool f_bLoop)
        {
            if (StateManager.Instance.UndergoingModEventID > 0 && StateManager.Instance.ModEventProgress == Constant.EventProgress.YotogiPlay)
            {
                if (StateManager.Instance.PartyGroupList != null && StateManager.Instance.PartyGroupList.Count > 0 && !StateManager.Instance.SpoofAudioLoadPlay)
                {
                    bool isGroupZero = false;
                    isGroupZero = isGroupZero || asMgr.GetInstanceID() == StateManager.Instance.PartyGroupList[0].Maid1.AudioMan.GetInstanceID();
                    if (StateManager.Instance.PartyGroupList[0].Maid2 != null)
                        isGroupZero = isGroupZero || asMgr.GetInstanceID() == StateManager.Instance.PartyGroupList[0].Maid2.AudioMan.GetInstanceID();

                    if (!f_bLoop)
                    {
                        if (isGroupZero)
                        {
                            var currentSkill = ModUseData.ValidSkillList[StateManager.Instance.PartyGroupList[0].Maid1.status.personal.id][StateManager.Instance.PartyGroupList[0].GroupType].Where(x => x.SexPosID == StateManager.Instance.PartyGroupList[0].SexPosID).First();

                            if (!currentSkill.IsDialogueAllowed)
                            {
                                
                                //Replace the dialogue that doesnt fit the sitation with moaning (eg For FFM harem, the dialogue is always mentioning the master which doesnt fit for the orgy party)

                                var group0 = StateManager.Instance.PartyGroupList[0];
                                //int eLevel = GetExcitementLevelByRate(GameMain.Instance.CharacterMgr.GetMaid(0).status.currentExcite);
                                int eLevel = StateManager.Instance.PartyGroupList[0].ExcitementLevel;

                                Maid targetMaid;
                                bool isMaid1 = true;
                                if (asMgr.GetInstanceID() == group0.Maid1.AudioMan.GetInstanceID())
                                {
                                    targetMaid = group0.Maid1;
                                    isMaid1 = true;
                                }
                                else
                                {
                                    targetMaid = group0.Maid2;
                                    isMaid1 = false;
                                }


                                //Find the current motion type (Piston, Fellatio etc), and simply load the corresponding voice
                                BackgroundGroupMotion.MotionItem motionItem = Util.GetMotionItemOfGroup(group0);
                                MotionSpecialLabel spLabel = Util.GetCurrentMotionSpecialLabel(group0, group0.CurrentLabelName);

                                bool isEstrus = Traverse.Create(StateManager.Instance.YotogiManager.play_mgr).Field(Constant.DefinedClassFieldNames.YotogiPlayManagerEstrusMode).GetValue<bool>();
                                
                                if (spLabel != null)
                                {
                                    if (spLabel.Type == MotionSpecialLabel.LabelType.Orgasm)
                                    {
                                        //after the orgasm scream the maid will usually speak a few sentences, have to replace it as it is far from the situation we want
                                        string voiceType = isMaid1 ? spLabel.VoiceType1 : spLabel.VoiceType2;

                                        var voiceList = ModUseData.PersonalityVoiceList[targetMaid.status.personal.id].OrgasmScream.Where(x => x.Type == voiceType && x.Personality == Util.GetPersonalityNameByValue(targetMaid.status.personal.id)).ToList();
                                        
                                        if (voiceList != null && voiceList.Count > 0)
                                        {
                                            //we want to replace it with a chopped audio clip, do not let the system to continue processing in the original way
                                            int rnd = RNG.Random.Next(voiceList.Count);
                                            PersonalityVoice.OrgasmScreamEntry voiceEntry = voiceList[rnd];
                                            Helper.AudioChoppingManager.PlaySubClip(targetMaid, "", voiceEntry.FileName, voiceEntry.StartTime, voiceEntry.EndTime);

                                            TimeEndTrigger trigger = new TimeEndTrigger();
                                            trigger.DueTime = DateTime.Now.AddSeconds(voiceEntry.EndTime - voiceEntry.StartTime + ConfigurableValue.ReplacedOrgasmWaitBufferTime);
                                            trigger.ToBeExecuted = new EventDelegate(() => PlayOrgasmWaitVoiceChoppedCase(targetMaid, group0));
                                            StateManager.Instance.TimeEndTriggerList.Add(trigger);

                                            f_strFileName = "";
                                            f_bLoop = false;
                                        }

                                    }
                                    else if (spLabel.Type == MotionSpecialLabel.LabelType.Waiting)
                                    {
                                        //at the beginning of the yotogi skill the maid may speak something depending on the excitement level. 
                                        var voiceEntries = ModUseData.PersonalityVoiceList[targetMaid.status.personal.id].NormalPlayVoice
                                                                                .Where(x => x.MotionType == MotionSpecialLabel.LabelType.Waiting && x.ExcitementLevel == eLevel).ToList();
                                        if (voiceEntries != null && voiceEntries.Count > 0)
                                        {
                                            PersonalityVoice.VoiceEntry pickedEntry = voiceEntries[RNG.Random.Next(voiceEntries.Count)];
                                            f_strFileName = isEstrus ? pickedEntry.EstrusFile : pickedEntry.NormalFile;
                                            f_bLoop = true;
                                        }
                                    }
                                    else if (spLabel.Type == MotionSpecialLabel.LabelType.Insert)
                                    {
                                        //The insert motion is short and probably wont say anything, but to play safe we replace it too
                                        string voiceType = isMaid1 ? spLabel.VoiceType1 : spLabel.VoiceType2;

                                        var voiceEntries = ModUseData.PersonalityVoiceList[targetMaid.status.personal.id].InsertVoice
                                                                                .Where(x => x.MotionType == voiceType && x.ExcitementLevel == eLevel).ToList();

                                        if (voiceEntries != null && voiceEntries.Count > 0)
                                        {
                                            PersonalityVoice.VoiceEntry pickedEntry = voiceEntries.First();
                                            f_strFileName = isEstrus ? pickedEntry.EstrusFile : pickedEntry.NormalFile;
                                            f_bLoop = false;
                                        }
                                    }

                                }
                                else
                                {
                                    //normal play label
                                    List<BackgroundGroupMotion.MotionLabel> motionLabels = Util.GetCurrentMotionLabel(group0, group0.CurrentLabelName);
                                    if (motionLabels != null)
                                    {

                                        BackgroundGroupMotion.MotionLabel pickedLabel = motionLabels[RNG.Random.Next(motionLabels.Count)];
                                        string voiceType = isMaid1 ? pickedLabel.VoiceType1 : pickedLabel.VoiceType2;

                                        var voiceEntries = ModUseData.PersonalityVoiceList[targetMaid.status.personal.id].NormalPlayVoice
                                                                                .Where(x => x.MotionType == voiceType && x.ExcitementLevel == eLevel).ToList();

                                        if (voiceEntries != null && voiceEntries.Count > 0)
                                        {
                                            PersonalityVoice.VoiceEntry pickedEntry = voiceEntries[RNG.Random.Next(voiceEntries.Count)];
                                            f_strFileName = isEstrus ? pickedEntry.EstrusFile : pickedEntry.NormalFile;
                                            f_bLoop = true;
                                        }
                                    }
                                }

                            }
                        }
                    }

                    //f_bLoop = false means it is a possible dialogue sound file 
                    float vol = 1f;
                    if (!f_bLoop && isGroupZero)
                        vol = 0.2f;

                    //Try to fade the voice for all background group to make the main group dialogue audible, and resume the volume if the main group start playing repeating voice file
                    if (StateManager.Instance.PartyGroupList != null)
                    {
                        for (int i = 1; i < StateManager.Instance.PartyGroupList.Count; i++)
                        {
                            if (StateManager.Instance.PartyGroupList[i].Maid1 != null)
                                if (StateManager.Instance.PartyGroupList[i].Maid1.AudioMan != null)
                                    StateManager.Instance.PartyGroupList[i].Maid1.AudioMan.audiosource.volume = vol;
                            if (StateManager.Instance.PartyGroupList[i].Maid2 != null)
                                if (StateManager.Instance.PartyGroupList[i].Maid2.AudioMan != null)
                                    StateManager.Instance.PartyGroupList[i].Maid2.AudioMan.audiosource.volume = vol;
                        }
                    }

                }

            }
        }

        internal static void ForceGroupPosition()
        {
            if (StateManager.Instance.UndergoingModEventID > 0 && StateManager.Instance.ModEventProgress == Constant.EventProgress.YotogiPlay)
            {
                Util.ForceGroupPosition();
            }
        }

        internal static void FixGroupOffsetVector(ref Vector3 f_vecLocalPos)
        {
            //The vector is for fixing the y position of different motion loaded
            //add this vector value to the group position vector, and keep the tranform position of AllOffset gameobject at zero 

            //if (StateManager.Instance.UndergoingModEventID > 0 && StateManager.Instance.ModEventProgress == Constant.EventProgress.YotogiPlay)
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                //identify the group and set the vector
                PartyGroup group = Util.GetPartyGroupByGUID(StateManager.Instance.processingMaidGUID);
                if (group != null)
                {
                    group.GroupOffsetVector = f_vecLocalPos;
                }
                f_vecLocalPos = Vector3.zero;
            }
        }

        internal static void StoreAnimationClipNameForGroup(TBody tbody, string tag)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (StateManager.Instance.PartyGroupList != null)
                {
                    foreach (var group in StateManager.Instance.PartyGroupList)
                    {
                        if (group.Maid1.status.guid == tbody.maid.status.guid)
                        {
                            group.CurrentMaid1AnimationClipName = tag;
                        }
                    }
                }

            }
        }

        internal static void AddOrgasmCountForGroup()
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
                Core.YotogiHandling.AddManOrgasmCountForGroup(StateManager.Instance.PartyGroupList[0]);
        }

        internal static void HideDefaultPositionChangeButton(YotogiPlayManager playMgr)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                YotogiPositionChanger ypc = Traverse.Create(playMgr).Field(Constant.DefinedClassFieldNames.YotogiPlayManagerPositionChanger).GetValue<YotogiPositionChanger>();
                GameObject go = Traverse.Create(ypc).Field(Constant.DefinedClassFieldNames.YotogiPlayManagerPositionChangerButton).GetValue<GameObject>();

                go.transform.localScale = Vector3.zero;
            }
        }

        internal static void ResetMaidVisibility()
        {
            Util.SetAllMaidVisiblility(true);
        }

        internal static void ApplyForceSetting(Maid maid)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (maid == null)
                    return;

                PartyGroup group = Util.GetPartyGroupByGUID(maid.status.guid);

                if (group != null)
                {


                    foreach (var eyeSightSetting in group.ForceEyeSight)
                    {
                        Core.CustomADVProcessManager.SetCharacterEyeSight(group, eyeSightSetting);
                    }
                    if (PartyGroup.CurrentMainGroupMotionType == ForceSexPosInfo.Type.NormalPlay)
                    {
                        foreach (var ikInfo in group.ForceIKAttach)
                        {
                            Core.CharacterHandling.IKAttachBone(ikInfo);
                        }
                    }
                }
            }
        }

        internal static void RecordCommandTypeClicked(Yotogis.Skill.Data.Command.Data command_data)
        {
            switch (command_data.basic.command_type)
            {
                case Yotogi.SkillCommandType.挿入:
                case Yotogi.SkillCommandType.単発_挿入:
                case Yotogi.SkillCommandType.単発:
                case Yotogi.SkillCommandType.継続:
                    PartyGroup.CurrentMainGroupMotionType = ForceSexPosInfo.Type.NormalPlay;
                    break;
                case Yotogi.SkillCommandType.絶頂:
                    PartyGroup.CurrentMainGroupMotionType = ForceSexPosInfo.Type.Orgasm;
                    break;
                case Yotogi.SkillCommandType.止める:
                    PartyGroup.CurrentMainGroupMotionType = ForceSexPosInfo.Type.Waiting;
                    break;
            }
        }

        internal static void ApplyLinkedGroupMotionUponCommandClicked(Yotogis.Skill.Data.Command.Data command_data)
        {
            switch (command_data.basic.command_type)
            {
                case Yotogi.SkillCommandType.挿入:
                case Yotogi.SkillCommandType.単発_挿入:
                case Yotogi.SkillCommandType.単発:
                case Yotogi.SkillCommandType.継続:
                    ApplyLinkedGroupMotion(ForceSexPosInfo.Type.NormalPlay);
                    break;
                case Yotogi.SkillCommandType.絶頂:
                    ApplyLinkedGroupMotion(ForceSexPosInfo.Type.Orgasm);
                    break;
                case Yotogi.SkillCommandType.止める:
                    ApplyLinkedGroupMotion(ForceSexPosInfo.Type.Waiting);
                    break;
            }
        }

        private static void ApplyLinkedGroupMotion(ForceSexPosInfo.Type type)
        {
            foreach (var group in StateManager.Instance.PartyGroupList)
            {
                if (!group.IsIndependentExcitement)
                {

                    //sync the excitement value etc
                    group.Maid1.status.currentExcite = StateManager.Instance.YotogiManager.maid.status.currentExcite;
                    group.Maid1.status.currentSensual = StateManager.Instance.YotogiManager.maid.status.currentSensual;

                    //set motion
                    if (group.ForceSexPos != null)
                    {
                        int sexPosID = group.ForceSexPos.NormalPlay;
                        if (type == ForceSexPosInfo.Type.Waiting)
                            sexPosID = group.ForceSexPos.Waiting;
                        else if (type == ForceSexPosInfo.Type.Orgasm)
                            sexPosID = group.ForceSexPos.Orgasm;

                        Core.YotogiHandling.ChangeBackgroundGroupSexPosition(group, sexPosID, true, true);
                    }

                }
            }
        }

        internal static void ForceChangeManUponCommandClicked(Yotogis.Skill.Data.Command.Data command_data)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                Scenario.YotogiSetupInfo yotogiSetup = Util.GetUndergoingScenario().YotogiSetup.Where(x => x.Phase == StateManager.Instance.YotogiPhase).First();
                if (yotogiSetup.ForceChangeManWhenOrgasm)
                {
                    if (command_data.basic.command_type == Yotogi.SkillCommandType.絶頂)
                    {
                        Core.YotogiHandling.BlockAllYotogiCommands();

                        EventDelegate toBeExec;
                        if(StateManager.Instance.PartyGroupList[0].ExtraManList.Count > 0)
                            toBeExec = new EventDelegate(() => ForceChangeManQueueTypeTriggerExecution());
                        else
                            toBeExec = new EventDelegate(() => ForceChangeManShareListTypeTriggerExecution(yotogiSetup.IsMainManOwner));

                        VoiceLoopTrigger trigger = new VoiceLoopTrigger();
                        trigger.TargetMaid = StateManager.Instance.PartyGroupList[0].Maid1;
                        trigger.ToBeExecuted = toBeExec;
                        StateManager.Instance.VoiceLoopTrigger = trigger;
                    }
                }
            }
        }

        private static void ForceChangeManShareListTypeTriggerExecution(bool isMainManOwner)
        {
            Core.YotogiHandling.ChangeManMembersShareListType(StateManager.Instance.PartyGroupList[0], isMainManOwner);
            StateManager.Instance.YotogiManager.play_mgr.UpdateCommand();
        }

        private static void ForceChangeManQueueTypeTriggerExecution()
        {
            Core.YotogiHandling.ChangeManMembersQueueType(StateManager.Instance.PartyGroupList[0]);
        }

        internal static void CheckVoiceloopTrigger(AudioSourceMgr audioMgr, bool isLoop)
        {
            if(StateManager.Instance.UndergoingModEventID > 0)
            {
                if(StateManager.Instance.VoiceLoopTrigger != null)
                {
                    if (isLoop && StateManager.Instance.VoiceLoopTrigger.TargetMaid.AudioMan.GetInstanceID() == audioMgr.GetInstanceID())
                    {
                        EventDelegate dg = StateManager.Instance.VoiceLoopTrigger.ToBeExecuted;
                        StateManager.Instance.VoiceLoopTrigger = null;
                        dg.Execute();
                    }
                }
                
            }
        }

        internal static void CheckMaidAnimationTrigger(Maid maid)
        {
            if (StateManager.Instance.WaitingAnimationTrigger != null)
            {
                if (StateManager.Instance.WaitingAnimationTrigger.TargetGUID == maid.status.guid)
                {
                    if (maid.body0.GetAnimation() != null)
                        if (!maid.body0.GetAnimation().isPlaying)
                        {
                            StateManager.Instance.WaitingAnimationTrigger.ToBeExecuted.Execute();

                            //remove the trigger
                            StateManager.Instance.WaitingAnimationTrigger = null;
                        }

                }
            }
        }

        internal static void CheckAnimationChangeTrigger(Maid maid)
        {
            //check if the maid is same as animation change trigger
            if (StateManager.Instance.AnimationChangeTrigger != null)
                if (StateManager.Instance.AnimationChangeTrigger.TargetGUID == maid.status.guid)
                {
                    TimeEndTrigger trigger = new TimeEndTrigger();
                    trigger.DueTime = DateTime.Now.AddSeconds(StateManager.Instance.AnimationChangeTrigger.ExtraWaitingTimeInSecond);
                    trigger.ToBeExecuted = StateManager.Instance.AnimationChangeTrigger.ToBeExecuted;
                    StateManager.Instance.AnimationChangeTrigger = null;

                    StateManager.Instance.TimeEndTriggerList.Add(trigger);
                }
        }

        internal static bool HandleCameraReset()
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].CameraSetup != null)
                    Core.CameraHandling.SetCameraLookAtFixedPoint(ModUseData.PartyGroupSetupList[PartyGroup.CurrentFormation].CameraSetup);
                else
                    Core.CameraHandling.SetCameraLookAt(GameMain.Instance.CharacterMgr.GetMaid(0));
                return false;
            }
            return true;
        }

        internal static void LoadADVSceneAfterYotogi(WfScreenChildren instance)
        {
            if (instance.GetType() == typeof(YotogiPlayManager) && StateManager.Instance.ModEventProgress == Constant.EventProgress.ADV)
            {
                Util.ClearGenericCollection(StateManager.Instance.InjectedButtons);
                StateManager.Instance.InjectedButtons = new List<CustomGameObject.InjectYotogiCommand>();
                StateManager.Instance.ExtraCommandWindow = new CustomGameObject.YotogiExtraCommandWindow(StateManager.Instance.ExtraCommandWindowMasterCopy.transform.gameObject);

                Core.CustomADVProcessManager.ADVSceneProceedToNextStep();
                
                //To fix the invalid status that causing the scene unable to go through the finish process when player click "next" in the yotogi play scene.
                if (instance.fade_status != WfScreenChildren.FadeStatus.Wait)
                {
                    Traverse.Create(instance).Field(Constant.DefinedClassFieldNames.WfScreenChildrenFadeStatus).SetValue(WfScreenChildren.FadeStatus.Wait);
                }
                
                GameMain.Instance.LoadScene(Constant.SceneType.ADV);
            }
        }

        internal static void BackupModAddedExtraObjects(BgMgr bgMgr, List<KeyValuePair<string, GameObject>> extraObjectsBackupList)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (StateManager.Instance.ModEventProgress == Constant.EventProgress.YotogiPlay)
                {
                    List<string> lstAddedObjectNames = new List<string>();
                    if (StateManager.Instance.PartyGroupList != null)
                        foreach (var group in StateManager.Instance.PartyGroupList)
                            if (group.ExtraObjects != null)
                                foreach (var itemName in group.ExtraObjects.Keys)
                                    lstAddedObjectNames.Add(itemName);

                    //Put the extra objects that requested by the mod to state and temporarily removed from the BgMgr so that the system will not destory them
                    foreach (var attachObjName in bgMgr.m_DicAttachObj.Select(x => x.Key).ToList())
                    {
                        if (lstAddedObjectNames.Contains(attachObjName))
                        {
                            extraObjectsBackupList.Add(new KeyValuePair<string, GameObject>(attachObjName, bgMgr.m_DicAttachObj[attachObjName]));
                            bgMgr.m_DicAttachObj.Remove(attachObjName);
                        }
                    }
                }
            }
        }

        internal static void RestoreModAddedExtraObjects(BgMgr bgMgr, List<KeyValuePair<string, GameObject>> extraObjectsBackupList)
        {
            if (StateManager.Instance.UndergoingModEventID > 0)
            {
                if (StateManager.Instance.ModEventProgress == Constant.EventProgress.YotogiPlay)
                {
                    //put the extra objects back to the BgMgr list
                    foreach (var kvp in extraObjectsBackupList)
                    {
                        bgMgr.m_DicAttachObj.Add(kvp.Key, kvp.Value);
                    }
                }
            }
        }

        private static void PlayOrgasmWaitVoiceChoppedCase(Maid maid, PartyGroup group)
        {
            BackgroundGroupMotion.MotionItem motionItem = Util.GetMotionItemBySexPosID(group.SexPosID);

            List<MotionSpecialLabel> lstSpecialLabel = motionItem.SpecialLabels.Where(x => x.Type == MotionSpecialLabel.LabelType.Orgasm).ToList();

            MotionSpecialLabel pickedLabel = lstSpecialLabel[RNG.Random.Next(lstSpecialLabel.Count)];

            string labelName;
            if (group.Maid1 == maid)
                labelName = pickedLabel.WaitLabel1;
            else
                labelName = pickedLabel.WaitLabel2;

            bool isEstrus = Traverse.Create(StateManager.Instance.YotogiManager.play_mgr).Field(Constant.DefinedClassFieldNames.YotogiPlayManagerEstrusMode).GetValue<bool>();

            Core.CharacterHandling.SetCharacterVoiceEntry(maid, PersonalityVoice.VoiceEntryType.OrgasmWait, group.ExcitementLevel, labelName, isEstrus, false);
        }
    }
}
