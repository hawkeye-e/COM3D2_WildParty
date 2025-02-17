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



"H4_pink_62255.ogg",
"H4_pink_62259.ogg",
"H5_pink_62233.ogg",
"H5_pink_62249.ogg",
"H5_pink_62235.ogg",
"H5_pink_62251.ogg",
"H5_pink_62274.ogg",
"H5_pink_62306.ogg",
"H5_pink_62237.ogg",
"H5_pink_62239.ogg"


        };
    }
}
