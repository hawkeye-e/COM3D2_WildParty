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
        internal List<UnityEngine.Transform> transformInQuestion = new List<UnityEngine.Transform>();

        internal List<Maid> DebugMaidList = new List<Maid>();
        internal List<Maid> DebugManList = new List<Maid>();

        internal float DebugX = 0f;
        internal float DebugY = 0f;
        internal float DebugZ = 0f;

        internal bool killme = false;

        internal Dictionary<string, List<string>> ScriptInfoCapture = new Dictionary<string, List<string>>();

        internal Maid DummyMan;

        internal string[] DebugVoiceFileInQuestion = {


"H0_GP02C_24538.ogg",
"H0_PMD_27316.ogg",
"H0_X1Kai_63369.ogg",
"H0_PMD_27120.ogg",

"H0_X1Kai_63370.ogg",
"H0_PMD_27215.ogg",
"H0_X1Kai_63305.ogg",

"H0_GP_16937.ogg",
"H0_X1Kai_63387.ogg",


        };
    }
}
