using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin
{
    //When the target starts the defined animation name, execute the event deletgate
    internal class TimeEndTrigger
    {
        public DateTime DueTime;
        public EventDelegate ToBeExecuted;
    }
}
