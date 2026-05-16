using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin
{
    internal class ScenarioManifest
    {
        //Referring Special/ScenarioIDList.cs
        public int ID { get; set; }

        //Key: FileType, Values: List of relative path of the files
        public Dictionary<string, List<string>> Files { get; set; }
        
        //For special case handling
        public string SpecialFlag { get; set; } 
    }
}
