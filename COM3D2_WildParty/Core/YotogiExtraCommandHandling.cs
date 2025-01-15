using System;
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
                var skillList = ModUseData.ValidOrgySkillList[personality][groupType];

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
                var skillList = ModUseData.ValidOrgySkillList[personality][groupType];

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
                var allowMapList = ModUseData.ScenarioList.Where(x => x.ScenarioID == StateManager.Instance.UndergoingModEventID).First().AllowMap;
                var formationOption = allowMapList.Where(x => x.MapID == YotogiStageSelectManager.SelectedStage.stageData.id).First().FormationOption;

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
    }
}
