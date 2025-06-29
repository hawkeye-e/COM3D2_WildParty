using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.WildParty.Plugin.Helper
{
    /* For trimming the voice to be played
     * Usage: we cant find a suitable tone of "Welcome" audio file to fit the situation of welcoming a guest
     * so could make use of a happy tone of "Welcome master" audio file and trim it to "Welcome" so that it sounds like the maid is welcoming a guest
     */

    class AudioChoppingManager
    {
        //Key: ADVStepID, Value: clip
        public static Dictionary<string, AudioClip> SubClipLibrary = new Dictionary<string, AudioClip>();


        public static void PlaySubClip(Maid maid, string advStepID, string fileName, float start, float end, bool forceNoLibrary = false)
        {
            AudioClip subClip = null;
            bool isExist = false;
            if (!string.IsNullOrEmpty(advStepID) && !forceNoLibrary)
            {
                //Check if we have already make this subclip before
                if (SubClipLibrary.ContainsKey(advStepID))
                {
                    subClip = SubClipLibrary[advStepID];
                    isExist = true;
                }
            }

            if (!isExist)
            {
                //we want to use the existing logic to load the audio file, but we dont want it to play
                StateManager.Instance.SpoofAudioLoadPlay = true;
                maid.AudioMan.LoadPlay(fileName, 0f, false);
                StateManager.Instance.SpoofAudioLoadPlay = false;
                //make a sub clip with the time we want, and play it instead
                if (start <= 0)
                    start = 0;
                
                //In case the system could not find the audio file due to lack of dlc etc, stop this process and let it be voiceless to avoid crash
                if (maid.AudioMan.audiosource.clip == null)
                    return;

                if (end > maid.AudioMan.audiosource.clip.length || end <= 0)
                    end = maid.AudioMan.audiosource.clip.length;
                subClip = MakeSubclip(maid.AudioMan.audiosource.clip, start, end);
                //Put it in dictionary if the step id is provided
                if (!string.IsNullOrEmpty(advStepID) && !forceNoLibrary)
                {
                    SubClipLibrary.Add(advStepID, subClip);
                }
            }

            if (subClip != null)
            {
                maid.AudioMan.audiosource.clip = subClip;
                maid.AudioMan.audiosource.Play();
            }


        }

        /* reference from https://discussions.unity.com/t/how-to-play-specific-part-of-the-audio/142016/2 */
        private static AudioClip MakeSubclip(AudioClip clip, float start, float stop)
        {
            try
            {
                /* Create a new audio clip */
                int frequency = clip.frequency;
                float timeLength = stop - start;
                int samplesLength = (int)(frequency * timeLength * clip.channels);
                AudioClip newClip = AudioClip.Create(clip.name + "-sub", samplesLength, 1, frequency, false);

                /* Create a temporary buffer for the samples */
                float[] data = new float[samplesLength];
                /* Get the data from the original clip */
                clip.GetData(data, (int)(frequency * start));
                /* Transfer the data to the new clip */
                newClip.SetData(data, 0);

                /* Return the sub clip */
                return newClip;
            }catch 
            { 
                //There are report that the user can reach this MakeSubclip with an empty clip... use try catch to prevent game freeze
                return null;
            }
        }
    }
}
