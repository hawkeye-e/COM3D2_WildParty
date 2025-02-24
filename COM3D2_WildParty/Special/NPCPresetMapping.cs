using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COM3D2.WildParty.Plugin
{
    internal class NPCPresetMapping
    {
        private static Dictionary<string, byte[]> Mapping = new Dictionary<string, byte[]> 
        {
            { "Kamigawara_Reika", ModResources.PresetResources.Kamigawara_Reika },
            { "Seikou_Haruna", ModResources.PresetResources.Seikou_Haruna },
            { "Shirohama_Natsumi", ModResources.PresetResources.Shirohama_Natsumi },
        };

        internal static byte[] GetPresetResources(string presetFile)
        {
            if(Mapping.ContainsKey(presetFile))
                return Mapping[presetFile];
            return null;
        }
    }
}
