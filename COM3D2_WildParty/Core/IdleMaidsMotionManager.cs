using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;

namespace COM3D2.WildParty.Plugin.Core
{
    internal class IdleMaidsMotionManager
    {
        private static ManualLogSource Log = WildParty.Log;

        internal static void CheckReviewForIdleMaids(ADVKagManager instance, Dictionary<int, IdleMaidInfo> maidList)
        {
            if (maidList == null)
                return;
            if (StateManager.Instance.ModEventProgress != Constant.EventProgress.YotogiPlay)
                return;

            foreach(var kvp in maidList)
            {
                if (kvp.Value != null)
                    ProcessIdleMaid(kvp.Value);
            }
        }

        /*
         * Concept: At the moment of the review time
         * 1. Mind of the idle maid is recovered
         * 2. if the main group is having sex, Sensual of the idle maid increase. The rate depends on the sensual rate of the main group
         *    if the main group is at rest, no change or minor decrease in sensual.
         * 3. Always change the motion based on the current sensual value
         */ 
        private static void ProcessIdleMaid(IdleMaidInfo idleMaid)
        {
            if (DateTime.Now > idleMaid.NextActionReviewTime)
            {
                //Add Mind
                AddMindToMaid(idleMaid.Maid);

                //Add Sensual
                HandleMaidSensual(idleMaid.Maid);

                //Update the motion
                UpdateIdleMaidMotion(idleMaid.Maid);

                //Generate the next review time
                idleMaid.GenerateNextReviewTime();
            }
        }

        private static void AddMindToMaid(Maid maid)
        {
            //TODO: put that magic number to config
            maid.status.currentMind = Math.Min(maid.status.maxMind, maid.status.currentMind + RNG.Random.Next(6));
        }

        private static void HandleMaidSensual(Maid maid)
        {
            //Check the main group motion
            switch (PartyGroup.CurrentMainGroupMotionType)
            {
                case ForceSexPosInfo.Type.NormalPlay:
                    AddSensualToMaid(maid);
                    break;
                case ForceSexPosInfo.Type.Waiting:
                case ForceSexPosInfo.Type.Orgasm:
                    DeductSensualFromMaid(maid);
                    break;
            }
        }

        private static void AddSensualToMaid(Maid maid)
        {
            int mainGroupFactor = Math.Max(0, StateManager.Instance.PartyGroupList[0].Maid1.status.currentSensual / 100) + 1;
            maid.status.currentSensual = Math.Min(Constant.SensualRateMaxCap, maid.status.currentSensual + RNG.Random.Next(8 + mainGroupFactor));
        }

        private static void DeductSensualFromMaid(Maid maid)
        {
            maid.status.currentSensual = Math.Max(Constant.SensualRateMinCap, maid.status.currentSensual + RNG.Random.Next(3) - 2);
        }

        private static void UpdateIdleMaidMotion(Maid maid)
        {
            YotogiHandling.SetIdleMaidMotion(maid);
            PartyGroup.SetIdleMaidsPosition();
        }
    }
}
