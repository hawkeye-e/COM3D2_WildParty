using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    internal class PartyGroupSetup
    {
        public int DefaultSexPosID = -1;
        public string SexStateRule = "";                    //Empty if no change
        public SpecialRuleSet SpecialRules = null;
        public List<SpecialCaseDefaultSexPosID> SpecialCaseDefaultSexPosIDList = null;
        public int BackgroundSexPosID = -1;
        public bool IsRandomAssign;
        public bool IsShuffleMaidList;
        public bool IsShuffleManList;
        public bool IsLesbianSetup = false;
        public bool IsSyncBackgroundSexPos = false;         //If set to true, the background group will follow the sex pos id of the main group
        public List<int> ExcludePersonality;
        public List<GroupSetupData> GroupSetup;
        public int ExtraManCount = 0;                       //For Shared list. If the logic of yotogi requires assigning extra man to each group, use the one inside GroupSetup
        public int ExtraManSlotCount = -1;
        public int BackgroundManCount = 0;
        public int BackgroundManSlotCount = -1;
        public List<MovingQueueSetupData> MovingQueueSetup;       //For gangbang queue type use
        public List<MovingQueueSetupData> GroupManMovementSetup;    //For "train of asses"

        internal class GroupSetupData
        {
            public int ArrayPosition;
            public int MaidCount;
            public int ManCount;
            public int ExtraManCount = 0;
            public int ExtraManSlotCount = -1;
            public bool IsAutomatedGroup = true;            //True: Use the BackgroundGroupMotionManager logic; False: Static motion
            public bool MaidFromNPC = false;                //True: Get the maid from NPCList; False: Get the maid from YotogiWorkingMaidList
            public bool IsVoicelessGroup = false;           //True: No Voice in yotogi scene; False: Play Voice according to personality id in yotogi scene
            public string ClothesSet = "";                  //ID from ClothesSet.json ; Empty string: no change;
            public bool IsApplyEyeMask = false;             //True: Apply eye mask to the character if the user select to turn it on
        }

        internal class MovingQueueSetupData
        {
            public string ID = "";                          //If a yotogi skill need to use a different movement setup, specified the ID in YotogiMiscHandling.json
            public string StandingAnimationFile;
            [JsonProperty]
            private string StandingMotionOffsetString;
            [JsonProperty]
            private string WalkingMotionOffsetString;
            [JsonProperty]
            private string RotationOffsetString;
            [JsonProperty]
            private string MaidMotionOffsetString;                //Used for fixing the issue of the man "walk over" the maid 
            [JsonProperty]
            private string ExitingManOffsetString = "";                //Used for fixing the issue of the man "walk over" the maid 


            public Vector3 StandingMotionOffset
            {
                get { return Util.ParseVector3RawString(StandingMotionOffsetString); }
            }
            public Vector3 WalkingMotionOffset
            {
                get { return Util.ParseVector3RawString(WalkingMotionOffsetString); }
            }

            public Vector3 RotationOffset
            {
                get { return Util.ParseVector3RawString(RotationOffsetString); }
            }

            public Vector3 MaidMotionOffset
            {
                get { return Util.ParseVector3RawString(MaidMotionOffsetString); }
            }

            public Vector3 ExitingManOffset
            {
                get { return Util.ParseVector3RawString(ExitingManOffsetString); }
            }
        }


        internal class SpecialCaseDefaultSexPosID
        {
            public List<int> Personality;
            public int DefaultSexPosID;

        }

        internal class SpecialRuleSet
        {
            public ChangeManWhenOrgasmRuleSet ChangeManWhenOrgasmRule;

            internal class ChangeManWhenOrgasmRuleSet
            {
                public bool ForceChangeManWhenOrgasm;
            }
        }
    }
}
