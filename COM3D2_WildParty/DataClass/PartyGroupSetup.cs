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
        public int BackgroundSexPosID = -1;
        public bool IsRandomAssign;
        public bool IsShuffleMaidList;
        public bool IsShuffleManList;
        public List<int> ExcludePersonality;
        public List<GroupSetupData> GroupSetup;
        public int ExtraManCount = 0;                       //For Shared list. If the logic of yotogi requires assigning extra man to each group, use the one inside GroupSetup
        public MovingQueueSetupData MovingQueueSetup;       //For gangbang queue type use
        public CameraDataInJson CameraSetup = null;                //If this is not setup, will use the calculation method

        internal class GroupSetupData
        {
            public int ArrayPosition;
            public int MaidCount;
            public int ManCount;
            public int ExtraManCount = 0;
            public bool IsAutomatedGroup = true;            //True: Use the BackgroundGroupMotionManager logic; False: Static motion
            public bool MaidFromNPC = false;                //True: Get the maid from NPCList; False: Get the maid from YotogiWorkingMaidList
            public bool IsVoicelessGroup = false;                //True: No Voice in yotogi scene; False: Play Voice according to personality id in yotogi scene
        }

        internal class MovingQueueSetupData
        {
            public string StandingAnimationFile;
            [JsonProperty]
            private string StandingMotionOffsetString;
            [JsonProperty]
            private string WalkingMotionOffsetString;
            [JsonProperty]
            private string RotationOffsetString;
            [JsonProperty]
            private string MaidMotionOffsetString;                //Used for fixing the issue of the man "walk over" the maid 


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
        }
    }
}
