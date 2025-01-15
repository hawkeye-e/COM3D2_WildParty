using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin.DebugHelper
{
    public class CaptureDialogueAndText
    {
        public static Dictionary<string, DebugCaptureDialogueAndText> DialoguesDictionary = new Dictionary<string, DebugCaptureDialogueAndText>();

        public class DebugCaptureDialogueAndText
        {
            public string Speaker;
            public string text;
        }
    }
}
