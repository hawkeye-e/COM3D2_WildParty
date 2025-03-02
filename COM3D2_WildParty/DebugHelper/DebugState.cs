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

        internal Dictionary<string, List<string>> ScriptInfoCapture = new Dictionary<string, List<string>>();


        internal string[] DebugVoiceFileInQuestion = {


"H0_GP02C_24681.ogg",
"H0_X1Kai_63361.ogg",
"H0_X1Kai_63365.ogg",

"H0_X1Kai_63375.ogg",
"H0_X1Kai_63376.ogg",
"H0_X1Kai_63377.ogg",
"H0_X1Kai_63378.ogg"


        };
    }
}
