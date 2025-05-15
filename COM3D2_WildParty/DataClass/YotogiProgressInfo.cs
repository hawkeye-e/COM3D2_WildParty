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

        public int CurrentCommandID;
        //Key: CommandID, Value: Click Count
        public Dictionary<int, int> CommandClicked;

        public YotogiProgressInfo()
        {
            ManOrgasmInfo = new Dictionary<string, int>();
            SexPositionOrgasmInfo = new Dictionary<int, int>();
            CustomFetishEarned = new List<int>();
            CommandClicked = new Dictionary<int, int>();

            MaidOrgasmCount = 0;
            CurrentCommandID = -1;
        }
    }
}
