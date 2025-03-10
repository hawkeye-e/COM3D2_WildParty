﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BepInEx.Logging;
using HarmonyLib;

namespace COM3D2.WildParty.Plugin.Core
{
    class YotogiExtraCommandHandling
    {
        private static ManualLogSource Log = WildParty.Log;

        //Need to update this function if a new type of yotogi button is added.
        internal static EventDelegate GetButtonCallbackFromString(string buttonID)
        {
            switch (buttonID)
            {
                case Constant.ModYotogiCommandButtonID.ChangePosition:
                    return new EventDelegate(Orgy_ChangePosition_ShowPositionList);
                case Constant.ModYotogiCommandButtonID.ChangePositionAll:
                    return new EventDelegate(Orgy_MassChangePosition_ShowPositionList);
                case Constant.ModYotogiCommandButtonID.ChangeFormation:
                    return new EventDelegate(Orgy_ShowFormationOption);
                case Constant.ModYotogiCommandButtonID.ChangePartner:
                    return new EventDelegate(Orgy_ShowMaidList);
                case Constant.ModYotogiCommandButtonID.FetishOrgy:
                    return new EventDelegate(Orgy_AddFetish_Orgy);

                case Constant.ModYotogiCommandButtonID.ChangeFormationHaremKing:
                    return new EventDelegate(HaremKing_ShowFormationOption);
                case Constant.ModYotogiCommandButtonID.ChangeMaidHaremKing:
                    return new EventDelegate(HaremKing_ShowMaidList);
                case Constant.ModYotogiCommandButtonID.MoveLeftHaremKing:
                    return new EventDelegate(HaremKing_MoveLeft);
                case Constant.ModYotogiCommandButtonID.MoveRightHaremKing:
                    return new EventDelegate(HaremKing_MoveRight);
                default:
                    return null;
            }
        }

        public static void Orgy_ChangePosition_ShowPositionList()
        {
            StateManager.Instance.ExtraCommandWindow.ResetScrollPosition();

            if (CheckRequireExtraCommandWindowPopulate(CustomGameObject.YotogiExtraCommandWindow.Mode.PositionList))
            {
                //Load the position list based on the info of maid zero
                int personality = StateManager.Instance.PartyGroupList[0].Maid1.status.personal.id;
                string groupType = StateManager.Instance.PartyGroupList[0].GroupType;
                var skillList = ModUseData.ValidSkillList[personality][groupType];

                List<GameObject> buttons = new List<GameObject>();

                foreach (var skill in skillList)
                {
                    var cmd = CloneCommandButton(skill.DisplayName, new EventDelegate(() => YotogiExtraCommandCallbacks.ChangeMainGroupSkill_Callback(skill.YotogiSkillID)));
                    var btn = cmd.GetComponent<UIButton>();
                    if (skill.SexPosID == StateManager.Instance.PartyGroupList[0].SexPosID)
                    {
                        btn.isEnabled = false;
                        btn.SetState(UIButtonColor.State.Disabled, true);
                    }

                    buttons.Add(cmd);
                }
                StateManager.Instance.ExtraCommandWindow.ShowContent(buttons, CustomGameObject.YotogiExtraCommandWindow.Mode.PositionList);

                StateManager.Instance.ExtraCommandWindow.SetVisible(true);
            }
        }

        public static void Orgy_MassChangePosition_ShowPositionList()
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
                List<string> formationOption;
                if (scenario.AllowMap != null)
                    formationOption = scenario.AllowMap.Where(x => x.MapID == YotogiStageSelectManager.SelectedStage.stageData.id).First().FormationOption;
                else
                    formationOption = scenario.DefaultMap.FormationOption;


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

        public static void Orgy_ShowMaidList()
        {
            StateManager.Instance.ExtraCommandWindow.ResetScrollPosition();

            if (CheckRequireExtraCommandWindowPopulate(CustomGameObject.YotogiExtraCommandWindow.Mode.MaidList))
            {
                //Load the position list based on the info of maid zero

                List<GameObject> buttons = new List<GameObject>();

                foreach (var maid in StateManager.Instance.SelectedMaidsList)
                {

                    var cmd = CloneCommandButton(Util.GetMaidDisplayName(maid), new EventDelegate(() => YotogiExtraCommandCallbacks.ChangeTargetMaid_Callback(maid.status.guid)));
                    buttons.Add(cmd);
                }
                StateManager.Instance.ExtraCommandWindow.ShowContent(buttons, CustomGameObject.YotogiExtraCommandWindow.Mode.MaidList);

                StateManager.Instance.ExtraCommandWindow.SetVisible(true);
            }
        }

        public static void Orgy_AddFetish_Orgy()
        {
            Maid maid = StateManager.Instance.PartyGroupList[0].Maid1;
            int fetishID = Util.GetFetishIDByButtonID(Constant.ModYotogiCommandButtonID.FetishOrgy);
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


        public static void HaremKing_ShowFormationOption()
        {
            StateManager.Instance.ExtraCommandWindow.ResetScrollPosition();

            if (CheckRequireExtraCommandWindowPopulate(CustomGameObject.YotogiExtraCommandWindow.Mode.FormationList))
            {
                //Load the formation list based on the current stage
                Scenario scenario = ModUseData.ScenarioList.Where(x => x.ScenarioID == StateManager.Instance.UndergoingModEventID).First();
                List<string> formationOption;
                if (scenario.AllowMap != null)
                    formationOption = scenario.AllowMap.Where(x => x.MapID == YotogiStageSelectManager.SelectedStage.stageData.id).First().FormationOption;
                else
                    formationOption = scenario.DefaultMap.FormationOption;
                

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

        //Function to set all yotogi command disabled
        private static void BlockAllYotogiCommands()
        {
            var commandList = StateManager.Instance.YotogiCommandFactory;
            for (int i = 0; i < commandList.transform.childCount; i++)
            {
                Transform childTransform = commandList.transform.GetChild(i);
                UIButton childButton = childTransform.GetComponent<UIButton>();
                if (childButton != null)
                {
                    childButton.enabled = false;
                    childButton.isEnabled = false;
                    childButton.SetState(UIButtonColor.State.Disabled, true);
                }
            }
        }

        private static void ResetMotionToWaiting(PartyGroup group, bool isMovingRight)
        {
            if(ModUseData.MapCoordinateList[PartyGroup.CurrentFormation].SpecialSetting != null)
            {
                MapCoorindates.ManualMovementSettingInfo motionSettings = null;
                if (isMovingRight)
                    motionSettings = ModUseData.MapCoordinateList[PartyGroup.CurrentFormation].SpecialSetting.MoveRightSetting;
                else
                    motionSettings = ModUseData.MapCoordinateList[PartyGroup.CurrentFormation].SpecialSetting.MoveLeftSetting;

                if(motionSettings != null)
                    if(motionSettings.PreMoveMotion != null)
                        if(motionSettings.PreMoveMotion.WaitingMotionBeforeMovement != null)
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
            BlockAllYotogiCommands();

            //Since some of the yotogi motion will constantly update the animation, we need to set the motion script of the main group to the static waiting before we apply any change
            ResetMotionToWaiting(StateManager.Instance.PartyGroupList[0], false);

            int currentMaidIndex = Util.GetIndexPositionInWorkingYotogiArrayForMaid(StateManager.Instance.PartyGroupList[0].Maid1);
            PartyGroup targetMaidGroup = Util.GetPartyGroupByGUID(StateManager.Instance.YotogiWorkingMaidList[currentMaidIndex - 1].status.guid);

            SetupPreSwapMotionEndTrigger(-1, false);

            YotogiExtraCommandCallbacks.PlayPreMovementMotion(false, -1, StateManager.Instance.PartyGroupList[0], targetMaidGroup);

        }

        public static void HaremKing_MoveRight()
        {
            if (!IsMainGroupChangeMemberIndexValid(1))
                return;

            StateManager.Instance.ExtraCommandWindow.SetVisible(false);

            //We dont want the user to be able to click any command when moving which will mess up animation and the logic flow
            BlockAllYotogiCommands();

            //Since some of the yotogi motion will constantly update the animation, we need to set the motion script of the main group to the static waiting before we apply any change
            ResetMotionToWaiting(StateManager.Instance.PartyGroupList[0], true);


            //Setup trigger to be executed when target animation starts
            int currentMaidIndex = Util.GetIndexPositionInWorkingYotogiArrayForMaid(StateManager.Instance.PartyGroupList[0].Maid1);
            PartyGroup targetMaidGroup = Util.GetPartyGroupByGUID(StateManager.Instance.YotogiWorkingMaidList[currentMaidIndex + 1].status.guid);

            SetupPreSwapMotionEndTrigger(1, true);

            //Load movement motion for main group
            YotogiExtraCommandCallbacks.PlayPreMovementMotion(true, 1, StateManager.Instance.PartyGroupList[0], targetMaidGroup);
        }

        private static void SetupPreSwapMotionEndTrigger(int indexOffset, bool isMovingRight)
        {
            //Setup trigger to be executed when target animation starts
            int currentMaidIndex = Util.GetIndexPositionInWorkingYotogiArrayForMaid(StateManager.Instance.PartyGroupList[0].Maid1);
            PartyGroup targetMaidGroup = Util.GetPartyGroupByGUID(StateManager.Instance.YotogiWorkingMaidList[currentMaidIndex + indexOffset].status.guid);

            StateManager.Instance.WaitingAnimationTrigger = new AnimationEndTrigger();
            StateManager.Instance.WaitingAnimationTrigger.TargetGUID = StateManager.Instance.PartyGroupList[0].Man1.status.guid;
            StateManager.Instance.WaitingAnimationTrigger.ToBeExecuted = new EventDelegate(() => YotogiExtraCommandCallbacks.HaremKing_SwapMainGroupMaid(currentMaidIndex, currentMaidIndex + indexOffset, isMovingRight));
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
            foreach (var commandBtn in injectedButtons)
            {
                //check if fulfill criteria and update the status of the button accordingly
                if (commandBtn.Data.Type == ExtraYotogiCommandData.CommandType.Fetish)
                {
                    Fetish fetishInfo = ModUseData.FetishList.Where(x => x.ID == commandBtn.Data.FetishID).First();
                    Maid maid = StateManager.Instance.PartyGroupList[0].Maid1;
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


                    if (!isAllFulfilled)
                    {
                        button.enabled = isAllFulfilled;
                        button.isEnabled = isAllFulfilled;
                        button.SetState(UIButtonColor.State.Disabled, true);
                    }
                    else
                    {
                        button.enabled = isAllFulfilled;
                        button.isEnabled = isAllFulfilled;
                        button.SetState(UIButtonColor.State.Normal, true);
                    }
                }
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
            return template.Replace(Constant.JsonReplaceTextLabels.ManCount, fetishInfo.Conditions.ManCount.ToString())
                           .Replace(Constant.JsonReplaceTextLabels.OrgasmCount, fetishInfo.Conditions.OrgasmCount.ToString());
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
