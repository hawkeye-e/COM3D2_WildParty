using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    internal class IKRectify
    {
        public string ScenarioID;
        public List<string> ScriptName;
        public string LabelName;
        

        public string Type;
        public string IKType;
        public List<List<string>> MatchingCondition;
        public List<List<string>> TagData;

        public ScriptManagerFast.KagTagSupportFast GetTagDataInKagTagSupportFormat()
        {
            ScriptManagerFast.KagTagSupportFast result = new ScriptManagerFast.KagTagSupportFast();

            if(TagData != null)
            {
                foreach(var tagRow in TagData)
                {
                    result.AddTagProperty(tagRow[0], tagRow[1]);
                }
            }

            return result;
        }

        internal class RectifyType
        {
            public const string Replace = "Replace";
            public const string Add = "Add";
        }

        internal class IKRectifyType
        {
            public const string IKAttachBone = "ikattachbone";
            public const string IKAttachPoint = "ikattachpoint";
        }
    }


}
