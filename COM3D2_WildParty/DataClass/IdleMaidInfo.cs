using System;


namespace COM3D2.WildParty.Plugin
{
    class IdleMaidInfo
    {
        public Maid Maid;
        public DateTime NextActionReviewTime;


        public void GenerateNextReviewTime()
        {
            NextActionReviewTime = DateTime.Now.AddSeconds(
                        //RNG.Random.Next(ConfigurableValue.YotogiSimulation.MinBackgroundGroupReviewTimeInSeconds, ConfigurableValue.YotogiSimulation.MaxBackgroundGroupReviewTimeInSeconds)
                        RNG.Random.Next(Config.MinBackgroundGroupReviewTimeInSeconds, Config.MaxBackgroundGroupReviewTimeInSeconds)
                        );
        }

        public void GenerateNextReviewTime(int second)
        {
            NextActionReviewTime = DateTime.Now.AddSeconds(RNG.Random.Next(second));
        }

        public void StopNextReviewTime()
        {
            NextActionReviewTime = DateTime.MaxValue;
        }

        public void RequestNextReviewTimeAfter(float second)
        {
            NextActionReviewTime = DateTime.Now.AddMilliseconds(second * 1000);
        }
    }
}
