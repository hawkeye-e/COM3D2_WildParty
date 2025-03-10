﻿using System;
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

        internal class GroupSetupData
        {
            public int ArrayPosition;
            public int MaidCount;
            public int ManCount;
            public bool IsAutomatedGroup = true;            //True: Use the BackgroundGroupMotionManager logic; False: Static motion
        }
    }
}
