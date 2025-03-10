﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin.Core
{
    //Separate into another file specific for the special handling steps
    internal partial class CustomADVProcessManager
    {

        //for calling other functions that require special coding in certain step
        private static void ProcessADVSpecial(ADVKagManager instance, ADVStep step)
        {
            switch (step.ID)
            {
                case "org0004":
                    ProcessADV_OrgyParty_NumberOfGuests(instance, step);
                    break;
                case "orgy_wg_CameraDecision":
                    ProcessADV_OrgyParty_CameraDecision(instance, step);
                    break;
                case "orgy_start_yotogiplay":
                    ProcessADV_Step_org0101_YotogiPlay(instance, step);
                    break;
                case "haremking_start_yotogiplay":
                    ProcessADV_Step_HaremKing_YotogiPlay(instance, step);
                    break;
            }
        }

        //Display only the valid options of number of guests to make the yotogi scene simple
        private static void ProcessADV_OrgyParty_NumberOfGuests(ADVKagManager instance, ADVStep step)
        {
            //calculate the valid number of men
            int maidCount = CharacterHandling.GetMaidCountForEventID(StateManager.Instance.UndergoingModEventID, GameMain.Instance.CharacterMgr.status.isDaytime);
            
            int minGuestMan = Math.Max(1, (int)Math.Ceiling(maidCount / 2.0));      
            int maxGuestMan = Math.Min(maidCount * 2, ConfigurableValue.MaxOrgyPartyManCount);
            
            List<KeyValuePair<string, KeyValuePair<string, bool>>> lstNumofMan = new List<KeyValuePair<string, KeyValuePair<string, bool>>>();
            for (int i = minGuestMan; i <= maxGuestMan; i++)
            {
                lstNumofMan.Add(new KeyValuePair<string, KeyValuePair<string, bool>>(step.ChoiceData[i - 1].Value, new KeyValuePair<string, bool>(step.ChoiceData[i - 1].Key, true)));
            }

            Action<string, string> onClickCallBack = delegate (string displayText, string value)
            {
                StateManager.Instance.MaxManUsed = int.Parse(value);

                //proceed to next step
                GameMain.Instance.MainCamera.FadeOut(f_dg: delegate ()
                {
                    ADVSceneProceedToNextStep();
                });


            };
            
            instance.MessageWindowMgr.CreateSelectButtons(lstNumofMan, onClickCallBack);
        }

        //Change the camera based on the number of maids selected
        private static void ProcessADV_OrgyParty_CameraDecision(ADVKagManager instance, ADVStep step)
        {
            if (StateManager.Instance.SelectedMaidsList.Count <= 10)
                ADVSceneProceedToNextStep("orgy_wg_10Maid");
            else if (StateManager.Instance.SelectedMaidsList.Count <= 20)
                ADVSceneProceedToNextStep("orgy_wg_20Maid");
            else if (StateManager.Instance.SelectedMaidsList.Count <= 30)
                ADVSceneProceedToNextStep("orgy_wg_30Maid");
            else
                ADVSceneProceedToNextStep("orgy_wg_40Maid");
        }


        private static void ProcessADV_Step_org0101_YotogiPlay(ADVKagManager instance, ADVStep step)
        {
            StateManager.Instance.ModEventProgress = Constant.EventProgress.YotogiInit;

            GameMain.Instance.MainCamera.FadeOut(f_dg: delegate
            {
                CharacterHandling.SetDefaultGroupFormation();

                CharacterHandling.AssignPartyGroupingRandom();

                YotogiHandling.InitYotogiData();


                GameMain.Instance.LoadScene(step.Tag);
            });

        }

        private static void ProcessADV_Step_HaremKing_YotogiPlay(ADVKagManager instance, ADVStep step)
        {
            StateManager.Instance.ModEventProgress = Constant.EventProgress.YotogiInit;

            GameMain.Instance.MainCamera.FadeOut(f_dg: delegate
            {
                //CharacterHandling.AssignPartyGrouping_HaremKing();
                CharacterHandling.SetDefaultGroupFormation();

                CharacterHandling.AssignPartyGrouping(PartyGroup.CurrentFormation);

                YotogiHandling.InitYotogiData();


                GameMain.Instance.LoadScene(step.Tag);
            });

        }
        
    }
}
