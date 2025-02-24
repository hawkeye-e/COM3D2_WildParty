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
        public int ExtraManCount = 0;

        internal class GroupSetupData
        {
            public int ArrayPosition;
            public int MaidCount;
            public int ManCount;
            public bool IsAutomatedGroup = true;            //True: Use the BackgroundGroupMotionManager logic; False: Static motion
            public bool MaidFromNPC = false;                //True: Get the maid from NPCList; False: Get the maid from YotogiWorkingMaidList
            public bool IsVoicelessGroup = false;                //True: No Voice in yotogi scene; False: Play Voice according to personality id in yotogi scene
        }
    }
}
