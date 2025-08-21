using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    internal class WorldObject
    {
        public bool IsCustom = false;
        public string Src;
        public string ObjectID;                 //Used for removing the object
        public PosRot PosRot;
        public float Scale = 1;                 //negative for unchanged
    }

}
