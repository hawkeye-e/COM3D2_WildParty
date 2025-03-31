using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    internal class YotogiMiscSetup
    {
        public List<int> SexPosIDs;
        public PosRot Offset;
        public List<ExtraObjectInfo> ExtraObjects;
        public List<ExtraObjectOnCharacterInfo> ExtraObjectsOnCharacter;

        internal class ExtraObjectInfo
        {
            public string Src;
            public string Name;
            public string Dest;
            public PosRotVectorFormat Offset;
        }
        
        internal class ExtraObjectOnCharacterInfo
        {
            public ExtraItemObject ItemInfo;
            public Constant.CharacterType CharacterType;        //Maid or Man only
            public int ArrayPosition;                           //position in the group
        }
    }
}
