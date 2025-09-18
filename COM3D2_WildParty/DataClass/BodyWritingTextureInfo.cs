using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.WildParty.Plugin
{
    public class BodyWritingTextureInfo
    {
        public string Type;
        public List<TextureInfo> TextureData;

        public class TextureInfo
        {
            //public string BodyPart;             //For readiblity in json only?
            public string Slot;
            public List<string> Files;
        }
    }
}
