using System;
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
            }
        }

        private static void ProcessADVLoadYotogiScene(ADVKagManager instance, ADVStep step)
        {
            StateManager.Instance.YotogiPhase = step.YotogiSetup.Phase;
            StateManager.Instance.IsFinalYotogi = step.YotogiSetup.IsFinalYotogi;
            StateManager.Instance.IsYotogiUseModSemenPattern = step.YotogiSetup.UseModSemenPattern;

            if (!step.YotogiSetup.IsClubOwnerMainCharacter)
            {
                //the Man[0] is replaced by a customer and will use his view to proceed the yotogi scene
                StateManager.Instance.SpoofActivateMaidObjectFlag = true;
                GameMain.Instance.CharacterMgr.SetActiveMan(StateManager.Instance.MenList[0], 0);
                StateManager.Instance.SpoofActivateMaidObjectFlag = false;
            }
            else
            {
                //The owner is the main character, put him as the first element of the men list 
                if(StateManager.Instance.ClubOwner.status.guid != StateManager.Instance.MenList[0].status.guid)
                    StateManager.Instance.MenList.Insert(0, StateManager.Instance.ClubOwner);

                StateManager.Instance.SpoofActivateMaidObjectFlag = true;
                GameMain.Instance.CharacterMgr.SetActiveMan(StateManager.Instance.ClubOwner, 0);
                StateManager.Instance.SpoofActivateMaidObjectFlag = false;
            }

            switch (step.YotogiSetup.YotogiCode)
            {
                case Constant.YotogiSceneCode.OrgyParty:
                    ProcessADV_Step_org0101_YotogiPlay(instance, step);
                    break;
                case Constant.YotogiSceneCode.HaremKing:
                    ProcessADV_Step_HaremKing_YotogiPlay(instance, step);
                    break;
                case Constant.YotogiSceneCode.HappyGBClub:
                    ProcessADV_Step_HappyGBClub_YotogiPlay(instance, step);
                    break;
                case Constant.YotogiSceneCode.LilyBloomingParadise:
                    ProcessADV_Step_LilyBloomingParadise_YotogiPlay(instance, step);
                    break;
                case Constant.YotogiSceneCode.ImmoralVillage:
                    ProcessADV_Step_ImmoralVillage_YotogiPlay(instance, step);
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
                ModUseData.ReloadCoordinateData(StateManager.Instance.UndergoingModEventID);

                YotogiHandling.InitArrayForYotogiUsed();

                CharacterHandling.SetDefaultGroupFormation();

                CharacterHandling.AssignPartyGroupingRandom();

                YotogiHandling.SetupYotogiSceneInitialSkill(Util.GetCurrentDefaultSexPosID());

                CharacterHandling.SetGroupZeroActive();

                YotogiHandling.InitYotogiData();


                GameMain.Instance.LoadScene(step.Tag);
            });

        }

        private static void ProcessADV_Step_HaremKing_YotogiPlay(ADVKagManager instance, ADVStep step)
        {
            StateManager.Instance.ModEventProgress = Constant.EventProgress.YotogiInit;

            GameMain.Instance.MainCamera.FadeOut(f_dg: delegate
            {
                ModUseData.ReloadCoordinateData(StateManager.Instance.UndergoingModEventID);

                YotogiHandling.InitArrayForYotogiUsed();

                CharacterHandling.SetDefaultGroupFormation();

                CharacterHandling.AssignPartyGrouping(PartyGroup.CurrentFormation);

                YotogiHandling.SetupYotogiSceneInitialSkill(Util.GetCurrentDefaultSexPosID());

                CharacterHandling.SetGroupZeroActive();

                YotogiHandling.InitYotogiData();


                GameMain.Instance.LoadScene(step.Tag);
            });

        }

        private static void ProcessADV_Step_HappyGBClub_YotogiPlay(ADVKagManager instance, ADVStep step)
        {
            StateManager.Instance.ModEventProgress = Constant.EventProgress.YotogiInit;

            GameMain.Instance.MainCamera.FadeOut(f_dg: delegate
            {
                ModUseData.ReloadCoordinateData(StateManager.Instance.UndergoingModEventID);

                YotogiHandling.InitArrayForYotogiUsed();

                CharacterHandling.SetDefaultGroupFormation();
                
                CharacterHandling.AssignPartyGrouping(PartyGroup.CurrentFormation);

                YotogiHandling.SetupYotogiSceneInitialSkill(Util.GetCurrentDefaultSexPosID());

                CharacterHandling.SetGroupZeroActive();
                
                YotogiHandling.InitYotogiData();
                

                GameMain.Instance.LoadScene(step.Tag);
            });

        }


        private static void ProcessADV_Step_LilyBloomingParadise_YotogiPlay(ADVKagManager instance, ADVStep step)
        {
            StateManager.Instance.ModEventProgress = Constant.EventProgress.YotogiInit;

            GameMain.Instance.MainCamera.FadeOut(f_dg: delegate
            {
                ModUseData.ReloadCoordinateData(StateManager.Instance.UndergoingModEventID);
                
                if (step.YotogiSetup.MaidConvertToMan != null)
                    YotogiHandling.InitArrayForYotogiUsed(step.YotogiSetup.MaidConvertToMan.RatioPercent);
                else
                    YotogiHandling.InitArrayForYotogiUsed();
                
                CharacterHandling.SetDefaultGroupFormation();
                
                CharacterHandling.AssignPartyGrouping(PartyGroup.CurrentFormation);

                YotogiHandling.SetupYotogiSceneInitialSkill(Util.GetCurrentDefaultSexPosID());

                CharacterHandling.SetGroupZeroActive();

                YotogiHandling.InitYotogiData();


                GameMain.Instance.LoadScene(step.Tag);
            });

        }

        private static void ProcessADV_Step_ImmoralVillage_YotogiPlay(ADVKagManager instance, ADVStep step)
        {
            StateManager.Instance.ModEventProgress = Constant.EventProgress.YotogiInit;

            GameMain.Instance.MainCamera.FadeOut(f_dg: delegate
            {
                ModUseData.ReloadCoordinateData(StateManager.Instance.UndergoingModEventID);

                YotogiHandling.InitArrayForYotogiUsed();

                CharacterHandling.SetDefaultGroupFormation();

                CharacterHandling.AssignPartyGrouping(PartyGroup.CurrentFormation);

                YotogiHandling.SetupYotogiSceneInitialSkill(Util.GetCurrentDefaultSexPosID());

                CharacterHandling.SetGroupZeroActive();

                YotogiHandling.InitYotogiData();


                GameMain.Instance.LoadScene(step.Tag);
            });

        }
        
    }
}
