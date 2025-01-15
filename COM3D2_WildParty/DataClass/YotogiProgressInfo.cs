using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin
{
    //This class is supposed to store the info we need for determining the fetish condition
    class YotogiProgressInfo
    {
        //Key: man GUID; Value: number of orgasm times
        public Dictionary<string, int> ManOrgasmInfo;       //Store the info of the men who have orgasm with the maid
        public List<int> CustomFetishEarned;

        public YotogiProgressInfo()
        {
            ManOrgasmInfo = new Dictionary<string, int>();
            CustomFetishEarned = new List<int>();
        }
    }
}
