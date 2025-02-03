using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

namespace COM3D2.WildParty.Plugin
{
    //Base command which will be added to YotogiCommandFactory
    internal class ExtraYotogiCommandData
    {
        public string Name;
        public CommandType Type;
        public int FetishID;
        public List<ConditionCheck> ConditionCheckTexts;
        public List<string> FormationConstraint = null;   //If this list is null or empty, there is no restriction. Otherwise it contains the list of formation that allows to use this button.

        public class ConditionCheck
        {
            public string Field;
            public string DisplayText;
        }

        public enum CommandType
        {
            Common,
            Fetish
        }
    }
}
