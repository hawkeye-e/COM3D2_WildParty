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
        public Dictionary<string, int> ManOrgasmInfo;       //Store the info of the men who has orgasm with the maid.
        public int MaidOrgasmCount;                   //Store how many times the maid has orgasm
        public Dictionary<int, int> SexPositionOrgasmInfo;                  //Key stored: SexPosID, Value: orgasm count
        public List<int> CustomFetishEarned;

        public YotogiProgressInfo()
        {
            ManOrgasmInfo = new Dictionary<string, int>();
            SexPositionOrgasmInfo = new Dictionary<int, int>();
            CustomFetishEarned = new List<int>();

            MaidOrgasmCount = 0;
        }
    }
}
