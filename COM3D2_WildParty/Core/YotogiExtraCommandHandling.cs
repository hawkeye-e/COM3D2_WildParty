using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.WildParty.Plugin.Core
{
    class YotogiExtraCommandHandling
    {
        private static ManualLogSource Log = WildParty.Log;

        //Need to update this function if a new type of yotogi button is added.
        internal static EventDelegate GetButtonCallbackFromString(string buttonID)
        {
            string cmdCode = ModUseData.ExtraYotogiCommandDataList[buttonID].CommandCode;

            switch (cmdCode)
            {
                case Constant.ModYotogiCommandButtonCode.ChangePosition:
                    return new EventDelegate(ChangePosition_ShowPositionList);
                case Constant.ModYotogiCommandButtonCode.ChangePositionAll:
                    return new EventDelegate(MassChangePosition_ShowPositionList);
                case Constant.ModYotogiCommandButtonCode.ChangeFormation:
                    return new EventDelegate(Orgy_ShowFormationOption);
                case Constant.ModYotogiCommandButtonCode.ChangePartner:
                    return new EventDelegate(ChangePartner_ShowMaidList);

                case Constant.ModYotogiCommandButtonCode.AddFetish:
                    return new EventDelegate(() => AddFetish(buttonID));

                case Constant.ModYotogiCommandButtonCode.ChangeFormationAsPositionChange:
                    return new EventDelegate(ShowFormationOption_AsChangePositionCommand);
                case Constant.ModYotogiCommandButtonCode.ChangeMaidHaremKing:
                    return new EventDelegate(HaremKing_ShowMaidList);
                case Constant.ModYotogiCommandButtonCode.MoveLeftHaremKing:
                    return new EventDelegate(HaremKing_MoveLeft);
                case Constant.ModYotogiCommandButtonCode.MoveRightHaremKing:
                    return new EventDelegate(HaremKing_MoveRight);

                case Constant.ModYotogiCommandButtonCode.OrgasmMotion:
                    return new EventDelegate(() => OrgasmCommandOnClick(buttonID));

                case Constant.ModYotogiCommandButtonCode.ChangeMaidAnotherGBDesire:
                    return new EventDelegate(GBClub_ShowMaidList);
                case Constant.ModYotogiCommandButtonCode.SwapMaidWithinGroup:
                    return new EventDelegate(SwapMaidWithinGroup);

                case Constant.ModYotogiCommandButtonCode.ChangeControllingMaid:
                    return new EventDelegate(ChangeControllingMaid_ShowMaidList);
                case Constant.ModYotogiCommandButtonCode.ShuffleMaidYuri:
                    return new EventDelegate(ShuffleMaidYuri);

                case Constant.ModYotogiCommandButtonCode.ChainedAction:
                    return new EventDelegate(() => ProcessChainedAction(buttonID));

                case Constant.ModYotogiCommandButtonCode.ExchangeStrapOn:
                    return new EventDelegate(ExchangeStrapOnYuri);
                default:
                    return null;
            }
        }

        public static void ChangePosition_ShowPositionList()
        {
            StateManager.Instance.ExtraCommandWindow.ResetScrollPosition();

            if (CheckRequireExtraCommandWindowPopulate(CustomGameObject.YotogiExtraCommandWindow.Mode.PositionList))
            {
                //Load the position list based on the info of maid zero
                int personality = StateManager.Instance.PartyGroupList[0].Maid1.status.personal.id;

                List<string> possibleGroupTypes = new List<string> { StateManager.Instance.PartyGroupList[0].GroupType };
                if (Util.GetUndergoingScenario().YotogiSetup.Where(x => x.Phase == StateManager.Instance.YotogiPhase).First().FlexibleManCountInYotogi)
                    possibleGroupTypes = StateManager.Instance.PartyGroupList[0].GetPossibleGroupType();


                List<GameObject> buttons = new List<GameObject>();
                foreach (var groupType in possibleGroupTypes)
                {
                    if (!ModUseData.ValidSkillList[personality].ContainsKey(groupType))
                        continue;
                    var skillList = ModUseData.ValidSkillList[personality][groupType].Where(x => x.Phase == StateManager.Instance.YotogiPhase);

                    foreach (var skill in skillList)
                    {
                        var cmd = CloneCommandButton(skill.DisplayName, new EventDelegate(() => YotogiExtraCommandCallbacks.ChangeMainGroupSkill_Callback(skill.SexPosID, skill.YotogiSkillID)));
                        var btn = cmd.GetComponent<UIButton>();
                        if (skill.SexPosID == StateManager.Instance.PartyGroupList[0].SexPosID)
                        {
                            btn.isEnabled = false;
                            btn.SetState(UIButtonColor.State.Disabled, true);
                        }

                        buttons.Add(cmd);
                    }
                }
                StateManager.Instance.ExtraCommandWindow.ShowContent(buttons, CustomGameObject.YotogiExtraCommandWindow.Mode.PositionList);

                StateManager.Instance.ExtraCommandWindow.SetVisible(true);
            }
        }

        public static void MassChangePosition_ShowPositionList()
        {
            StateManager.Instance.ExtraCommandWindow.ResetScrollPosition();

            if (CheckRequireExtraCommandWindowPopulate(CustomGameObject.YotogiExtraCommandWindow.Mode.MassPositionList))
            {
                //Load the position list based on the info of maid zero
                int personality = StateManager.Instance.PartyGroupList[0].Maid1.status.personal.id;
                string groupType = StateManager.Instance.PartyGroupList[0].GroupType;
                var skillList = ModUseData.ValidSkillList[personality][groupType];

                List<GameObject> buttons = new List<GameObject>();

                foreach (var skill in skillList)
                {
                    var cmd = CloneCommandButton(skill.DisplayName, new EventDelegate(() => YotogiExtraCommandCallbacks.MassChangeGroupSkill_Callback(skill.YotogiSkillID)));
                    buttons.Add(cmd);
                }
                StateManager.Instance.ExtraCommandWindow.ShowContent(buttons, CustomGameObject.YotogiExtraCommandWindow.Mode.MassPositionList);

                StateManager.Instance.ExtraCommandWindow.SetVisible(true);
            }
        }

        public static void Orgy_ShowFormationOption()
        {
            StateManager.Instance.ExtraCommandWindow.ResetScrollPosition();

            if (CheckRequireExtraCommandWindowPopulate(CustomGameObject.YotogiExtraCommandWindow.Mode.FormationList))
            {
                //Load the formation list based on the current stage
                Scenario scenario = ModUseData.ScenarioList.Where(x => x.ScenarioID == StateManager.Instance.UndergoingModEventID).First();
                Scenario.YotogiSetupInfo yotogiSetup = scenario.YotogiSetup.Where(x => x.Phase == StateManager.Instance.YotogiPhase).First();
                List<string> formationOption;
                if (yotogiSetup.AllowMap != null)
                    formationOption = yotogiSetup.AllowMap.Where(x => x.MapID == YotogiStageSelectManager.SelectedStage.stageData.id).First().FormationOption;
                else
                    formationOption = yotogiSetup.DefaultMap.FormationOption;


                List<GameObject> buttons = new List<GameObject>();

                foreach (var fid in formationOption)
                {
                    var coord = ModUseData.MapCoordinateList[fid];

                    var cmd = CloneCommandButton(coord.DisplayName, new EventDelegate(() => YotogiExtraCommandCallbacks.ChangeFormation_Callback(fid)));
                    var btn = cmd.GetComponent<UIButton>();
                    if (fid == PartyGroup.CurrentFormation)
                    {
                        btn.isEnabled = false;
                        btn.SetState(UIButtonColor.State.Disabled, true);
                    }

                    buttons.Add(cmd);
                }
                StateManager.Instance.ExtraCommandWindow.ShowContent(buttons, CustomGameObject.YotogiExtraCommandWindow.Mode.FormationList);

                StateManager.Instance.ExtraCommandWindow.SetVisible(true);
            }
        }

        public static void ChangePartner_ShowMaidList()
        {
            StateManager.Instance.ExtraCommandWindow.ResetScrollPosition();

            if (CheckRequireExtraCommandWindowPopulate(CustomGameObject.YotogiExtraCommandWindow.Mode.MaidList))
            {
                //Load the position list based on the info of maid zero

                List<GameObject> buttons = new List<GameObject>();

                foreach (var maid in StateManager.Instance.YotogiWorkingMaidList)
                {

                    var cmd = CloneCommandButton(Util.GetMaidDisplayName(maid), new EventDelegate(() => YotogiExtraCommandCallbacks.ChangeTargetMaid_Callback(maid.status.guid)));
                    buttons.Add(cmd);
                }
                StateManager.Instance.ExtraCommandWindow.ShowContent(buttons, CustomGameObject.YotogiExtraCommandWindow.Mode.MaidList);

                StateManager.Instance.ExtraCommandWindow.SetVisible(true);
            }
        }

        public static void AddFetish(string buttonID)
        {
            Maid maid = StateManager.Instance.PartyGroupList[0].Maid1;
            int fetishID = Util.GetFetishIDByButtonID(buttonID);
            CharacterHandling.AddFetish(maid, fetishID);
            CheckExtraYotogiCommandCondition(StateManager.Instance.InjectedButtons);
        }

        public static void HaremKing_ShowMaidList()
        {
            StateManager.Instance.ExtraCommandWindow.ResetScrollPosition();

            if (CheckRequireExtraCommandWindowPopulate(CustomGameObject.YotogiExtraCommandWindow.Mode.MaidList))
            {
                //Load the position list based on the info of maid zero

                List<GameObject> buttons = new List<GameObject>();

                foreach (var maid in StateManager.Instance.SelectedMaidsList)
                {
                    if (IsShowButtonPerFormation(PartyGroup.CurrentFormation, maid))
                    {

                        var cmd = CloneCommandButton(Util.GetMaidDisplayName(maid),
                        new EventDelegate(() => YotogiExtraCommandCallbacks.ChangeTargetMaid_Callback(maid.status.guid, PartyGroup.CurrentFormation, StateManager.Instance.PartyGroupList[0].SexPosID))
                        );
                        buttons.Add(cmd);
                    }
                }
                StateManager.Instance.ExtraCommandWindow.ShowContent(buttons, CustomGameObject.YotogiExtraCommandWindow.Mode.MaidList);

                StateManager.Instance.ExtraCommandWindow.SetVisible(true);
            }
        }


        public static void ShowFormationOption_AsChangePositionCommand()
        {
            StateManager.Instance.ExtraCommandWindow.ResetScrollPosition();

            if (CheckRequireExtraCommandWindowPopulate(CustomGameObject.YotogiExtraCommandWindow.Mode.FormationList))
            {
                //Load the formation list based on the current stage
                Scenario scenario = ModUseData.ScenarioList.Where(x => x.ScenarioID == StateManager.Instance.UndergoingModEventID).First();
                Scenario.YotogiSetupInfo yotogiSetup = scenario.YotogiSetup.Where(x => x.Phase == StateManager.Instance.YotogiPhase).First();
                List<string> formationOption;
                if (yotogiSetup.AllowMap != null)
                    formationOption = yotogiSetup.AllowMap.Where(x => x.MapID == YotogiStageSelectManager.SelectedStage.stageData.id).First().FormationOption;
                else
                    formationOption = yotogiSetup.DefaultMap.FormationOption;


                List<GameObject> buttons = new List<GameObject>();

                foreach (var fid in formationOption)
                {
                    var coord = ModUseData.MapCoordinateList[fid];

                    if (IsShowButtonPerFormation(fid, StateManager.Instance.PartyGroupList[0].Maid1))
                    {
                        var cmd = CloneCommandButton(coord.DisplayName, new EventDelegate(() => YotogiExtraCommandCallbacks.ChangeFormationWithNewGroup_Callback(fid)));
                        var btn = cmd.GetComponent<UIButton>();
                        if (fid == PartyGroup.CurrentFormation)
                        {
                            btn.isEnabled = false;
                            btn.SetState(UIButtonColor.State.Disabled, true);
                        }

                        buttons.Add(cmd);
                    }
                }
                StateManager.Instance.ExtraCommandWindow.ShowContent(buttons, CustomGameObject.YotogiExtraCommandWindow.Mode.FormationList);

                StateManager.Instance.ExtraCommandWindow.SetVisible(true);
            }
        }


        private static void ResetMotionToWaiting(PartyGroup group, bool isMovingRight)
        {
            if (ModUseData.MapCoordinateList[PartyGroup.CurrentFormation].SpecialSetting != null)
            {
                MapCoorindates.ManualMovementSettingInfo motionSettings = null;
                if (isMovingRight)
                    motionSettings = ModUseData.MapCoordinateList[PartyGroup.CurrentFormation].SpecialSetting.MoveRightSetting;
                else
                    motionSettings = ModUseData.MapCoordinateList[PartyGroup.CurrentFormation].SpecialSetting.MoveLeftSetting;

                if (motionSettings != null)
                    if (motionSettings.PreMoveMotion != null)
                        if (motionSettings.PreMoveMotion.WaitingMotionBeforeMovement != null)
                            if (motionSettings.PreMoveMotion.WaitingMotionBeforeMovement.Motion != null)
                            {
                                CharacterHandling.LoadMotionScript(0, false, motionSettings.PreMoveMotion.WaitingMotionBeforeMovement.Motion.ScriptFile,
                                    motionSettings.PreMoveMotion.WaitingMotionBeforeMovement.Motion.ScriptLabel,
                                    group.Maid1.status.guid, group.Man1.status.guid, false, false, false, false);
                            }
            }

        }

        public static void HaremKing_MoveLeft()
        {
            if (!IsMainGroupChangeMemberIndexValid(-1))
                return;

            StateManager.Instance.ExtraCommandWindow.SetVisible(false);

            //We dont want the user to be able to click any command when moving which will mess up animation and the logic flow
            YotogiHandling.BlockAllYotogiCommands();

            //Since some of the yotogi motion will constantly update the animation, we need to set the motion script of the main group to the static waiting before we apply any change
            ResetMotionToWaiting(StateManager.Instance.PartyGroupList[0], false);

            int currentMaidIndex = Util.GetIndexPositionInWorkingYotogiArrayForMaid(StateManager.Instance.PartyGroupList[0].Maid1);
            PartyGroup targetMaidGroup = Util.GetPartyGroupByGUID(StateManager.Instance.YotogiWorkingMaidList[currentMaidIndex - 1].status.guid);

            YotogiExtraCommandCallbacks.PlayPreMovementMotion(false, -1, StateManager.Instance.PartyGroupList[0], targetMaidGroup);

            SetupPreSwapMotionEndTrigger(-1, false);
        }

        public static void HaremKing_MoveRight()
        {
            if (!IsMainGroupChangeMemberIndexValid(1))
                return;

            StateManager.Instance.ExtraCommandWindow.SetVisible(false);

            //We dont want the user to be able to click any command when moving which will mess up animation and the logic flow
            YotogiHandling.BlockAllYotogiCommands();

            //Since some of the yotogi motion will constantly update the animation, we need to set the motion script of the main group to the static waiting before we apply any change
            ResetMotionToWaiting(StateManager.Instance.PartyGroupList[0], true);


            //Setup trigger to be executed when target animation starts
            int currentMaidIndex = Util.GetIndexPositionInWorkingYotogiArrayForMaid(StateManager.Instance.PartyGroupList[0].Maid1);
            PartyGroup targetMaidGroup = Util.GetPartyGroupByGUID(StateManager.Instance.YotogiWorkingMaidList[currentMaidIndex + 1].status.guid);

            //Load movement motion for main group
            YotogiExtraCommandCallbacks.PlayPreMovementMotion(true, 1, StateManager.Instance.PartyGroupList[0], targetMaidGroup);

            SetupPreSwapMotionEndTrigger(1, true);
        }

        public static void OrgasmCommandOnClick(string buttonID)
        {
            PartyGroup mainGroup = StateManager.Instance.PartyGroupList[0];

            //Find the orgasm type from resources by using button ID
            string orgasmType = ModUseData.ExtraYotogiCommandDataList[buttonID].OrgasmSetting.Type;

            //We dont want the user to be able to click any command when processing the the modded orgasm logic which will mess up animation and the logic flow
            YotogiHandling.BlockAllYotogiCommands();

            //Update the player state
            Traverse.Create(StateManager.Instance.YotogiManager.play_mgr).Field(Constant.DefinedClassFieldNames.YotogiPlayManagerPlayerState).SetValue(YotogiPlay.PlayerState.Normal);

            //deduct the excite value
            bool isEstrus = Traverse.Create(StateManager.Instance.YotogiManager.play_mgr).Field(Constant.DefinedClassFieldNames.YotogiPlayManagerEstrusMode).GetValue<bool>();
            YotogiParamBasicBar paramBasicBar = Traverse.Create(StateManager.Instance.YotogiManager.play_mgr).Field(Constant.DefinedClassFieldNames.YotogiPlayManagerParamBasicBar).GetValue<YotogiParamBasicBar>();
            if (isEstrus)
            {
                mainGroup.Maid1.status.currentSensual -= ModUseData.ExtraYotogiCommandDataList[buttonID].OrgasmSetting.ExciteDecay;
                paramBasicBar.SetCurrentSensual(mainGroup.Maid1.status.currentSensual, true);
            }
            else
            {
                mainGroup.Maid1.status.currentExcite -= ModUseData.ExtraYotogiCommandDataList[buttonID].OrgasmSetting.ExciteDecay;
                paramBasicBar.SetCurrentExcite(mainGroup.Maid1.status.currentExcite, true);
            }


            //Use the orgasm type to locate the special label
            BackgroundGroupMotion.MotionItem motionItem = Util.GetMotionItemOfGroup(mainGroup);
            MotionSpecialLabel spLabel = motionItem.SpecialLabels.Where(x => x.SexPosID == mainGroup.SexPosID && x.OrgasmType == orgasmType).FirstOrDefault();

            if (spLabel != null)
            {
                mainGroup.CurrentOrgasmLabelRecord = spLabel;

                CharacterHandling.LoadMotionScript(0, false, motionItem.FileName, spLabel.Label);

                Trigger.AnimationEndTrigger trigger = new Trigger.AnimationEndTrigger(mainGroup.Maid1, new EventDelegate(() => OrgasmCommandFinishFollowUp()), ConfigurableValue.OrgasmFinishFollowUpBaseExtraWaitingTimeInSecond + RNG.Random.Next(ConfigurableValue.OrgasmFinishFollowUpVariableExtraWaitingTimeInSecond));
                StateManager.Instance.AnimationChangeTrigger = trigger;

                var clip = mainGroup.Maid1.body0.m_Animation.GetClip(mainGroup.CurrentMaid1AnimationClipName);

                //play the audio specfic
                mainGroup.IsMaid1OrgasmScreamSet = CharacterHandling.SetOrgasmScreamVoice(mainGroup.Maid1, spLabel.VoiceType1);
                mainGroup.IsMaid2OrgasmScreamSet = CharacterHandling.SetOrgasmScreamVoice(mainGroup.Maid2, spLabel.VoiceType2);

                YotogiHandling.AddOrgasmCountForGroup(mainGroup);
                //
            }

            StateManager.Instance.PartyGroupList[0].BlockMotionScriptChange = true;
        }

        private static void OrgasmCommandFinishFollowUp()
        {
            StateManager.Instance.PartyGroupList[0].BlockMotionScriptChange = false;

            //Get the next sex state
            string nextState = ModUseData.SexStateList[SexState.StateType.OrgasmEnd].NextStates[0];

            PartyGroup mainGroup = StateManager.Instance.PartyGroupList[0];
            BackgroundGroupMotionManager.ProcessSemenForGroup(mainGroup);

            if (nextState == SexState.StateType.OrgasmWait)
            {
                //Common rule
                //Currently not in use
            }
            else if (nextState == SexState.StateType.ChangeMan)
            {
                //Gangbang rule
                //randomly pick up the man from extra man list and swap with the main group

                YotogiHandling.ChangeManMembersShareListType(mainGroup);

            }

            StateManager.Instance.YotogiManager.play_mgr.UpdateCommand();
        }

        public static void GBClub_ShowMaidList()
        {
            StateManager.Instance.ExtraCommandWindow.ResetScrollPosition();

            if (CheckRequireExtraCommandWindowPopulate(CustomGameObject.YotogiExtraCommandWindow.Mode.MaidList))
            {
                //Load the position list based on the info of maid zero

                List<GameObject> buttons = new List<GameObject>();

                foreach (var maid in StateManager.Instance.SelectedMaidsList)
                {
                    if (IsShowButtonPerFormation(PartyGroup.CurrentFormation, maid))
                    {
                        bool isManSwap = Util.GetUndergoingScenario().YotogiSetup.Where(x => x.Phase == StateManager.Instance.YotogiPhase).First().IsMainManOwner;
                        var cmd = CloneCommandButton(Util.GetMaidDisplayName(maid),
                        new EventDelegate(() => YotogiExtraCommandCallbacks.ChangeTargetGroup_Callback(maid.status.guid, isManSwap, true))
                        );

                        var btn = cmd.GetComponent<UIButton>();
                        if (StateManager.Instance.PartyGroupList[0].Maid1 == maid)
                        {
                            btn.isEnabled = false;
                            btn.SetState(UIButtonColor.State.Disabled, true);
                        }

                        buttons.Add(cmd);
                    }
                }
                StateManager.Instance.ExtraCommandWindow.ShowContent(buttons, CustomGameObject.YotogiExtraCommandWindow.Mode.MaidList);

                StateManager.Instance.ExtraCommandWindow.SetVisible(true);
            }
        }

        public static void SwapMaidWithinGroup()
        {
            GameMain.Instance.MainCamera.FadeOut(ConfigurableValue.CameraFadeTime, f_dg: delegate
            {
                PartyGroup group = StateManager.Instance.PartyGroupList[0];

                CharacterHandling.AssignPartyGrouping_SwapMember(group.Maid1, group.Maid2);
                CharacterHandling.SetGroupZeroActive();

                YotogiHandling.UpdateParameterView(group.Maid1);

                //need to update the main group
                var initialSkill = YotogiHandling.GetSkill(group.Maid1.status.personal.id, group.GroupType, group.SexPosID);
                CharacterHandling.CleanseCharacterMgrArray();
                YotogiHandling.ChangeMainGroupSkill(initialSkill.YotogiSkillID);

                CameraHandling.SetCameraLookAt(group.Maid1);
            });
            GameMain.Instance.MainCamera.FadeIn(ConfigurableValue.CameraFadeTime);
        }

        public static void ChangeControllingMaid_ShowMaidList()
        {
            StateManager.Instance.ExtraCommandWindow.ResetScrollPosition();

            if (CheckRequireExtraCommandWindowPopulate(CustomGameObject.YotogiExtraCommandWindow.Mode.MaidAsManList))
            {
                //Load the position list based on the info of maid zero
                List<GameObject> buttons = new List<GameObject>();

                foreach (var maid in StateManager.Instance.YotogiWorkingManList)
                {
                    var cmd = CloneCommandButton(Util.GetMaidDisplayName(maid), new EventDelegate(() => YotogiExtraCommandCallbacks.ChangeTargetGroup_Callback(maid.status.guid, false, false)));
                    buttons.Add(cmd);
                }
                StateManager.Instance.ExtraCommandWindow.ShowContent(buttons, CustomGameObject.YotogiExtraCommandWindow.Mode.MaidAsManList);

                StateManager.Instance.ExtraCommandWindow.SetVisible(true);
            }
        }

        public static void ShuffleMaidYuri()
        {
            GameMain.Instance.MainCamera.FadeOut(ConfigurableValue.CameraFadeTime, f_dg: delegate
            {
                //stop all maid motion
                foreach (PartyGroup group in StateManager.Instance.PartyGroupList)
                    group.StopNextReviewTime();
                
                CharacterHandling.StopCurrentAnimation();
                GameMain.Instance.ScriptMgr.StopMotionScript();

                //restore all maid back to female structure
                foreach (PartyGroup group in StateManager.Instance.PartyGroupList)
                {
                    group.DetachAllIK();
                    for (int i = 0; i < group.ManCount; i++)
                        if (group.GetManAtIndex(i) != null)
                            CharacterHandling.RecoverMaidFromManStructure(group.GetManAtIndex(i));
                }

                //shuffle and convert maid to man
                ADVStep currentStep = ModUseData.ADVStepData[StateManager.Instance.UndergoingModEventID][StateManager.Instance.CurrentADVStepID];
                YotogiHandling.InitArrayForYotogiUsed(currentStep.YotogiSetup.MaidConvertToMan.RatioPercent);

                CharacterHandling.AssignPartyGroupingRandom();

                YotogiHandling.SetupYotogiSceneInitialSkill(Util.GetCurrentDefaultSexPosID());
                CharacterHandling.SetGroupZeroActive();

                BackgroundGroupMotionManager.InitNextReviewTimer();
                YotogiHandling.UpdateParameterView(StateManager.Instance.PartyGroupList[0].Maid1);

                //need to update the main group
                var initialSkill = YotogiHandling.GetSkill(StateManager.Instance.PartyGroupList[0].Maid1.status.personal.id, StateManager.Instance.PartyGroupList[0].GroupType);
                CharacterHandling.CleanseCharacterMgrArray();
                YotogiHandling.ChangeMainGroupSkill(initialSkill.YotogiSkillID);

                YotogiHandling.SetGroupToScene();

                Util.ResetAllGroupPosition();

                CameraHandling.SetCameraLookAt(StateManager.Instance.PartyGroupList[0].Maid1);

                StateManager.Instance.ExtraCommandWindow.SetVisible(false);

                GameMain.Instance.MainCamera.FadeIn(ConfigurableValue.CameraFadeTime);
            });
        }

        public static void ExchangeStrapOnYuri()
        {
            GameMain.Instance.MainCamera.FadeOut(ConfigurableValue.CameraFadeTime, f_dg: delegate
            {
                //stop all maid motion
                CharacterHandling.StopCurrentAnimation();
                GameMain.Instance.ScriptMgr.StopMotionScript();

                PartyGroup group = StateManager.Instance.PartyGroupList[0];
                group.DetachAllIK();

                //Locate the maid with strap-on and the maid without
                //Assumption: This is a MMF situation so the maid without strap-on must be the only female in the MMF group
                Maid maidNoStrapOn = group.Maid1;
                Maid maidWithStrapOn = null;
                for (int i = 0; i < group.ManCount; i++)
                    if (group.GetManAtIndex(i) != null)
                        if (Util.IsManAConvertedMaid(group.GetManAtIndex(i)))
                            maidWithStrapOn = group.GetManAtIndex(i);

                //restore the maid back to female structure
                CharacterHandling.RecoverMaidFromManStructure(maidWithStrapOn);

                //convert the maid without strap to man
                CharacterHandling.ConvertMaidToManStructure(maidNoStrapOn, StateManager.Instance.PairedManForMaidList[maidNoStrapOn]);


                //Update working maid and working man list
                StateManager.Instance.YotogiWorkingManList.Remove(maidWithStrapOn);
                StateManager.Instance.YotogiWorkingMaidList.Remove(maidNoStrapOn);

                StateManager.Instance.YotogiWorkingManList.Add(maidNoStrapOn);
                StateManager.Instance.YotogiWorkingMaidList.Add(maidWithStrapOn);


                //set the default skill
                
                CharacterHandling.AssignPartyGroupingBySetupInfo(PartyGroup.CurrentFormation);

                //Update the variable reference again
                group = StateManager.Instance.PartyGroupList[0];

                YotogiHandling.SetupYotogiSceneInitialSkill(Util.GetCurrentDefaultSexPosID());
                
                CharacterHandling.SetGroupZeroActive();

                //need to update the main group
                var initialSkill = YotogiHandling.GetSkill(StateManager.Instance.PartyGroupList[0].Maid1.status.personal.id, StateManager.Instance.PartyGroupList[0].GroupType, group.SexPosID);
                CharacterHandling.CleanseCharacterMgrArray();

                //Check if the strap on maid is in the correct position
                YotogiHandling.CheckPreChangeSkillYotogiMiscSetup(StateManager.Instance.PartyGroupList[0], initialSkill.YotogiSkillID);

                YotogiHandling.UpdateParameterView(StateManager.Instance.PartyGroupList[0].Maid1);

                YotogiHandling.ChangeMainGroupSkill(initialSkill.YotogiSkillID);

                YotogiHandling.SetGroupToScene();

                Util.ResetAllGroupPosition();

                CameraHandling.SetCameraLookAt(StateManager.Instance.PartyGroupList[0].Maid1);

                StateManager.Instance.ExtraCommandWindow.SetVisible(false);

                GameMain.Instance.MainCamera.FadeIn(ConfigurableValue.CameraFadeTime);
            });
        }

        private static void SetupPreSwapMotionEndTrigger(int indexOffset, bool isMovingRight)
        {
            foreach (PartyGroup group in StateManager.Instance.PartyGroupList)
                group.BlockMotionScriptChange = true;

            //Setup trigger to be executed when target animation starts
            int currentMaidIndex = Util.GetIndexPositionInWorkingYotogiArrayForMaid(StateManager.Instance.PartyGroupList[0].Maid1);
            PartyGroup targetMaidGroup = Util.GetPartyGroupByGUID(StateManager.Instance.YotogiWorkingMaidList[currentMaidIndex + indexOffset].status.guid);

            Trigger.AnimationEndTrigger trigger = new Trigger.AnimationEndTrigger(StateManager.Instance.PartyGroupList[0].Man1, new EventDelegate(() => YotogiExtraCommandCallbacks.HaremKing_SwapMainGroupMaid(currentMaidIndex, currentMaidIndex + indexOffset, isMovingRight)));
            StateManager.Instance.WaitingAnimationTrigger = trigger;
        }

        internal static void ProcessChainedAction(string buttonID)
        {
            string chainedActionCode = ModUseData.ExtraYotogiCommandDataList[buttonID].ChainedActionCode;

            StateManager.Instance.ExtraCommandWindow.SetVisible(false);

            //We dont want the user to be able to click any command when moving which will mess up animation and the logic flow
            YotogiHandling.BlockAllYotogiCommands();

            Dictionary<string, object> parameters = PrepareChainedActionParameters(buttonID);

            CommandChainedActionManager.ProcessChainedMotion(chainedActionCode, parameters, new EventDelegate(FinishProcessChainedAction));
        }

        private static Dictionary<string, object> PrepareChainedActionParameters(string buttonID)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            if(ModUseData.ExtraYotogiCommandDataList[buttonID].Parameters != null)
            {
                foreach(ExtraYotogiCommandData.CommandParameters paramSetting in ModUseData.ExtraYotogiCommandDataList[buttonID].Parameters)
                {
                    if(paramSetting.Name == ExtraYotogiCommandData.SpecialParameterNames.PrimaryGroup)
                    {
                        parameters.Add(paramSetting.Name, StateManager.Instance.PartyGroupList[Convert.ToInt32(paramSetting.Value)]);
                    }
                    else if (paramSetting.Name == ExtraYotogiCommandData.SpecialParameterNames.NextExtraMan_Group)
                    {
                        Maid man = StateManager.Instance.PartyGroupList[Convert.ToInt32(paramSetting.Value)].GetCurrentExtraMaid();
                        parameters.Add(paramSetting.Name, man);
                    }
                    else
                    {
                        //default handling
                        parameters.Add(paramSetting.Name, paramSetting.Value);
                    }
                }
            }

            

            return parameters;
        }

        private static void FinishProcessChainedAction()
        {
            StateManager.Instance.YotogiManager.play_mgr.UpdateCommand();
        }



        private static bool IsMainGroupChangeMemberIndexValid(int indexOffset)
        {
            int currentIndex = Util.GetIndexPositionInWorkingYotogiArrayForMaid(StateManager.Instance.PartyGroupList[0].Maid1);
            if (currentIndex < 0)
                return false;

            currentIndex += indexOffset;

            //Check if it will be out of range
            if (currentIndex >= StateManager.Instance.YotogiWorkingMaidList.Count || currentIndex < 0)
            {
                return false;
            }
            return true;
        }


        internal static void CheckExtraYotogiCommandCondition(List<CustomGameObject.InjectYotogiCommand> injectedButtons)
        {
            Maid maid = StateManager.Instance.PartyGroupList[0].Maid1;
            var playerState = Traverse.Create(StateManager.Instance.YotogiManager.play_mgr).Field(Constant.DefinedClassFieldNames.YotogiPlayManagerPlayerState).GetValue<YotogiPlay.PlayerState>();

            foreach (var commandBtn in injectedButtons)
            {
                //check if fulfill criteria and update the status of the button accordingly
                if (commandBtn.Data.Type == ExtraYotogiCommandData.CommandType.Fetish)
                {
                    Fetish fetishInfo = ModUseData.FetishList.Where(x => x.ID == commandBtn.Data.FetishID).First();

                    YotogiProgressInfo progressInfo = StateManager.Instance.YotogiProgressInfoList[maid.status.guid];
                    var button = commandBtn.Button.GetComponent<UIButton>();

                    //Check if it should be displayed
                    if (maid.status.propensitys.ContainsKey(fetishInfo.ID))
                    {
                        commandBtn.Button.transform.localScale = Vector3.zero;
                        //no need to do the rest of the following checking as this button should not be displayed
                        continue;
                    }
                    else
                    {
                        commandBtn.Button.transform.localScale = Vector3.one;
                    }


                    //check if it should be enabled
                    bool isAllFulfilled = true;

                    isAllFulfilled = isAllFulfilled && (progressInfo.ManOrgasmInfo.Count >= fetishInfo.Conditions.ManCount);
                    isAllFulfilled = isAllFulfilled && (progressInfo.ManOrgasmInfo.Sum(x => x.Value) >= fetishInfo.Conditions.OrgasmCount);

                    isAllFulfilled = isAllFulfilled && (progressInfo.MaidOrgasmCount >= fetishInfo.Conditions.MaidOrgasmCount);
                    isAllFulfilled = isAllFulfilled && (progressInfo.SexPositionOrgasmInfo.Count >= fetishInfo.Conditions.PositionOrgasmCount);

                    if (fetishInfo.Conditions.SexPosRequired != null)
                    {
                        int matchCount = fetishInfo.Conditions.SexPosRequired.Intersect(progressInfo.SexPositionOrgasmInfo.Keys).Count();
                        isAllFulfilled = isAllFulfilled && matchCount == fetishInfo.Conditions.SexPosRequired.Count;
                    }

                    UpdateCommandButtonState(button, isAllFulfilled);
                }
                else if (commandBtn.Data.Type == ExtraYotogiCommandData.CommandType.Orgasm)
                {
                    bool isEnable = maid.status.currentExcite >= commandBtn.Data.OrgasmSetting.MinExcite && playerState == YotogiPlay.PlayerState.Insert;
                    UpdateCommandButtonState(commandBtn.Button.GetComponent<UIButton>(), isEnable);
                }
                else
                {
                    if (commandBtn.Data.TriggerConditions != null && Config.IsTriggerConditionOn)
                    {
                        bool isAllFulfilled = true;
                        YotogiProgressInfo progressInfo = StateManager.Instance.YotogiProgressInfoList[maid.status.guid];
                        
                        if (commandBtn.Data.TriggerConditions.CurrentCommandIDs != null)
                        {
                            bool isOK = false;
                            if (commandBtn.Data.TriggerConditions.CurrentCommandIDs.Contains(progressInfo.CurrentCommandID))
                                isOK = true;
                            
                            isAllFulfilled = isAllFulfilled && isOK;
                        }
                        
                        if (commandBtn.Data.TriggerConditions.RequireCommandClicks != null)
                        {
                            bool isOK = true;
                            
                            foreach (var clickRequirement in commandBtn.Data.TriggerConditions.RequireCommandClicks)
                            {
                                bool hasMatch = false;
                                foreach(var commandID in clickRequirement.CommandIDs)
                                {
                                    if (progressInfo.CommandClicked.ContainsKey(commandID))
                                        if (progressInfo.CommandClicked[commandID] >= clickRequirement.Count)
                                            hasMatch = true;
                                }
                                isOK = isOK && hasMatch;
                            }

                            isAllFulfilled = isAllFulfilled && isOK;
                        }

                        if (commandBtn.Data.TriggerConditions.ExciteSetting != null)
                        {
                            if (maid.status.currentExcite < commandBtn.Data.TriggerConditions.ExciteSetting.MinExcite || maid.status.currentExcite > commandBtn.Data.TriggerConditions.ExciteSetting.MaxExcite)
                                isAllFulfilled = false;
                        }

                        UpdateCommandButtonState(commandBtn.Button.GetComponent<UIButton>(), isAllFulfilled);
                    }
                    else
                        UpdateCommandButtonState(commandBtn.Button.GetComponent<UIButton>(), true);
                }
            }
        }

        private static void UpdateCommandButtonState(UIButton button, bool isEnable)
        {
            button.enabled = isEnable;
            button.isEnabled = isEnable;
            if (!isEnable)
            {
                button.SetState(UIButtonColor.State.Disabled, true);
            }
            else
            {
                button.SetState(UIButtonColor.State.Normal, true);
            }
        }


        private static GameObject CloneCommandButton(string text, EventDelegate eventDelegate)
        {
            var commandList = StateManager.Instance.YotogiCommandFactory;

            var lastChildTransform = commandList.transform.GetChild(commandList.transform.childCount - 1);
            //the button should always exist in our case but just in case
            if (lastChildTransform.GetComponent<UIButton>() != null)
            {
                var clone = GameObject.Instantiate(lastChildTransform.gameObject);
                clone.name = text;

                var label = clone.GetComponentInChildren<UILabel>();
                label.text = text;
                label.color = Constant.CommandButtonTextColor.Normal;

                var button = clone.GetComponent<UIButton>();
                button.onClick = new List<EventDelegate>();
                button.onClick.Add(eventDelegate);
                button.enabled = true;
                button.isEnabled = true;
                button.defaultColor = Constant.CommandButtonColor.Normal;
                button.transform.localScale = Vector3.one;                      //the last one could be our own fetish button and are set to zero scale to be "invisible"

                button.SetState(UIButtonColor.State.Normal, true);

                //while it is not strictly necessary, change the name of the component so that it wont be a mess when debugging
                button.name = Constant.DefinedGameObjectNames.ModButtonPrefix + text;

                return clone;
            }
            return null;
        }

        internal static GameObject InjectCommandButton(string text, EventDelegate eventDelegate, Transform attachedTo)
        {
            var button = CloneCommandButton(text, eventDelegate);
            button.transform.localPosition += new Vector3(0f, -30f, 5f);
            button.transform.SetParent(attachedTo, false);

            return button;
        }

        private static bool CheckRequireExtraCommandWindowPopulate(CustomGameObject.YotogiExtraCommandWindow.Mode mode)
        {

            if (StateManager.Instance.ExtraCommandWindow.GetMode() != CustomGameObject.YotogiExtraCommandWindow.Mode.Hidden)
            {
                if (mode == StateManager.Instance.ExtraCommandWindow.GetMode())
                {
                    //same mode, hide the button
                    StateManager.Instance.ExtraCommandWindow.SetVisible(false);
                    return false;
                }
                else
                {
                    //different mode, keep it showing and inform things need to be done
                    return true;
                }
            }
            else
            {
                //the windows is not shown
                StateManager.Instance.ExtraCommandWindow.SetVisible(true);
                return true;
            }
        }


        internal static string ReplaceFetishConditionText(string template, Fetish fetishInfo)
        {
            Maid maid = StateManager.Instance.PartyGroupList[0].Maid1;
            YotogiProgressInfo progressInfo = StateManager.Instance.YotogiProgressInfoList[maid.status.guid];

            int sexPosRequired = 0;
            int currentSexPosCount = 0;
            if (fetishInfo.Conditions.SexPosRequired != null)
            {
                sexPosRequired = fetishInfo.Conditions.SexPosRequired.Count;
                currentSexPosCount = fetishInfo.Conditions.SexPosRequired.Intersect(progressInfo.SexPositionOrgasmInfo.Keys).Count();
            }

            return template.Replace(Constant.JsonReplaceTextLabels.ManCount, fetishInfo.Conditions.ManCount.ToString())
                           .Replace(Constant.JsonReplaceTextLabels.OrgasmCount, fetishInfo.Conditions.OrgasmCount.ToString())
                           .Replace(Constant.JsonReplaceTextLabels.CurrentManCount, progressInfo.ManOrgasmInfo.Count.ToString())
                           .Replace(Constant.JsonReplaceTextLabels.CurrentOrgasmCount, progressInfo.ManOrgasmInfo.Sum(x => x.Value).ToString())
                           .Replace(Constant.JsonReplaceTextLabels.MaidOrgasmCount, fetishInfo.Conditions.MaidOrgasmCount.ToString())
                           .Replace(Constant.JsonReplaceTextLabels.CurrentMaidOrgasmCount, progressInfo.MaidOrgasmCount.ToString())
                           .Replace(Constant.JsonReplaceTextLabels.PositionOrgasmCount, fetishInfo.Conditions.PositionOrgasmCount.ToString())
                           .Replace(Constant.JsonReplaceTextLabels.CurrentPositionOrgasmCount, progressInfo.SexPositionOrgasmInfo.Count.ToString())
                           .Replace(Constant.JsonReplaceTextLabels.SexPosRequired, sexPosRequired.ToString())
                           .Replace(Constant.JsonReplaceTextLabels.CurrentSexPosCount, currentSexPosCount.ToString());
        }

        internal static bool IsThisConditionFulfilled(string field, Fetish fetishInfo)
        {
            Maid maid = StateManager.Instance.PartyGroupList[0].Maid1;
            YotogiProgressInfo progressInfo = StateManager.Instance.YotogiProgressInfoList[maid.status.guid];
            if (field == Constant.YotogiProgressFieldName.ManCount)
                return (progressInfo.ManOrgasmInfo.Count >= fetishInfo.Conditions.ManCount);
            else if (field == Constant.YotogiProgressFieldName.OrgasmCount)
                return (progressInfo.ManOrgasmInfo.Sum(x => x.Value) >= fetishInfo.Conditions.OrgasmCount);

            return false;
        }

        //Code follows YotogiPlayManager.OnMouseCommand
        internal static void ShowExecConditionPanel(GameObject commandButton, List<KeyValuePair<string[], Color>> displayConditions)
        {
            Traverse yotogiPlayMgr = Traverse.Create(StateManager.Instance.YotogiManager.play_mgr);
            UIWFConditionList commandExecConditionList = yotogiPlayMgr.Field(Constant.DefinedClassFieldNames.YotogiPlayManagerCommandExecConditionList).GetValue<UIWFConditionList>();
            GameObject commandExecConditionsPanel = yotogiPlayMgr.Field(Constant.DefinedClassFieldNames.YotogiPlayManagerCommandExecConditionPanel).GetValue<GameObject>();

            UIWidget widget = UTY.GetChildObject(commandExecConditionsPanel, Constant.DefinedGameObjectNames.YotogiPlayManagerCommandExecConditionPanelBackground).GetComponent<UIWidget>();
            commandExecConditionList.resizeUIEvent.Clear();
            EventDelegate.Add(commandExecConditionList.resizeUIEvent, delegate
            {
                widget.width = commandExecConditionList.width + 25;
                widget.height = commandExecConditionList.height + 65;
            });
            commandExecConditionList.SetTexts(displayConditions.ToArray());
            Transform parent = commandExecConditionsPanel.transform.parent;
            commandExecConditionsPanel.transform.SetParent(commandButton.transform, worldPositionStays: false);
            commandExecConditionsPanel.transform.localPosition = Vector3.zero;
            commandExecConditionsPanel.transform.SetParent(parent, worldPositionStays: true);
            Vector3 localPosition = commandExecConditionsPanel.transform.localPosition;
            localPosition = new Vector3(localPosition.x + 194f, localPosition.y + 12f, localPosition.z);
            commandExecConditionsPanel.transform.localPosition = localPosition;
            commandExecConditionsPanel.SetActive(value: true);
        }

        //Code follows YotogiPlayManager.OnMouseCommand
        internal static void HideExecConditionPanel()
        {
            Traverse yotogiPlayMgr = Traverse.Create(StateManager.Instance.YotogiManager.play_mgr);
            GameObject commandExecConditionsPanel = yotogiPlayMgr.Field(Constant.DefinedClassFieldNames.YotogiPlayManagerCommandExecConditionPanel).GetValue<GameObject>();
            commandExecConditionsPanel.SetActive(value: false);
        }

        private static bool IsShowButtonPerFormation(string formationID, Maid maid)
        {
            bool isShowButton = true;
            var setupInfo = ModUseData.PartyGroupSetupList[formationID];
            if (setupInfo.ExcludePersonality != null)
                if (setupInfo.ExcludePersonality.Contains(maid.status.personal.id))
                    isShowButton = false;
            return isShowButton;
        }
    }
}
