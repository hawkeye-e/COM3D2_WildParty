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
        public PosRotVectorFormat Offset;
        public List<ExtraObjectInfo> ExtraObjects;
        public List<ExtraObjectOnCharacterInfo> ExtraObjectsOnCharacter;
        public string RequiredFormationID = "";                             //if this value is supplied, for the main group to change to this formation id
        public YuriMMFSetupInfo YuriMMFSetup = null;

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

        internal class YuriMMFSetupInfo
        {
            public int ConvertedMaidPosition;                           //This is specified for MMF yotogi skill in yuri scenario (ie. one of the maid is wearing strap-on), indicate the man index position of the converted maid
        }
    }
}
