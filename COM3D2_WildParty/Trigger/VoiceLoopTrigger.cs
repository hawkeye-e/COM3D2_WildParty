using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin.Trigger
{
    //When the target starts a looped voice, execute the event deletgate
    internal class VoiceLoopTrigger
    {
        public Maid TargetMaid;
        public EventDelegate ToBeExecuted;
    }
}
