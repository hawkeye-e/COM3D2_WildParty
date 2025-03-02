using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.WildParty.Plugin
{
    internal class ClothesSet
    {
        public bool RequireNude = false;        //True: Apply FullNude first before applying this clothes set; False: just apply the parts mentioned

        public Dictionary<string, string> Slots;
        
    }
}
