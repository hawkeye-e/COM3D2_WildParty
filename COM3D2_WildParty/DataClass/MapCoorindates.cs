using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    internal class MapCoorindates
    {
        public string MapID;
        public string DisplayName;
        public bool IsRandom;           //True: The group or character is randomly assigned to any coordinates in the list. False: it is fixed and should follow the position order
        public List<CoordinateListInfo> CoordinateList;
        public List<CoordinatesInfo> SpecialCoordinates;

        internal class CoordinateListInfo
        {
            public int MaxGroup;
            public List<CoordinatesInfo> Coordinates;
        }

        internal class CoordinatesInfo : PosRot
        {
            public int ArrayPosition = -1;
            public string Type ="";
        }

    }
}
