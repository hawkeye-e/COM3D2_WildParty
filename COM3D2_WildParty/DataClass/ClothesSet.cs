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
        public Dictionary<string, string> NonClothesSlots;  //If some of the parts need to apply on the slots that are not used in normal character customization, use this variable instead.
    }
}
