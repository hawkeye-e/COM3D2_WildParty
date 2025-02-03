using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin.DebugHelper
{
    class DebugState
    {
        internal static DebugState Instance;

        internal bool IsCtrlPressed = false;
        internal float DebugSubclipAudioStartTime = 1f;
        internal float DebugSubclipAudioEndTime = 2f;
        internal int DebugUseVoiceFileArrayIndex = 0;
        internal int DebugUseCurrentGroupIndex = 0;
        internal bool DebugOneOffResetMarriageFlag = true;

        internal List<Maid> DebugMaidList = new List<Maid>();
        internal List<Maid> DebugManList = new List<Maid>();

        internal float DebugX = 0f;
        internal float DebugY = 0f;
        internal float DebugZ = 0f;

        internal bool killme = false;

        internal string[] DebugVoiceFileInQuestion = {

            "H13_tsou_06143.ogg"
                ,"H13_02762.ogg"
                ,"H13_04671.ogg"
                ,"H13_04183.ogg"

                ,"H13_04171.ogg"

                ,"H13_04197.ogg",
            "H13_02595.ogg",
            "H8_GP01ADD_60676.ogg",
            "H8_X01_54923.ogg"

        };
    }
}
