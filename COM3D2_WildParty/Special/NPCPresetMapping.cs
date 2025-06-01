using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.WildParty.Plugin
{
    internal class NPCPresetMapping
    {
        private static Dictionary<string, byte[]> Mapping = new Dictionary<string, byte[]> 
        {
            { "Kamigawara_Reika", ModResources.PresetResources.Kamigawara_Reika },
            { "Kamigawara_Reika_v2", ModResources.PresetResources.Kamigawara_Reika_v2 },
            { "Seikou_Haruna", ModResources.PresetResources.Seikou_Haruna },
            { "Seikou_Haruna_v2", ModResources.PresetResources.Seikou_Haruna_v2 },
            { "Shirohama_Natsumi", ModResources.PresetResources.Shirohama_Natsumi },
            { "Shirohama_Natsumi_v2", ModResources.PresetResources.Shirohama_Natsumi_v2 },

            { "ImmoralVillage_Sayaka", ModResources.PresetResources.Preset_ImmoralVillage_Sayaka },
            { "ImmoralVillage_Sayaka_v2", ModResources.PresetResources.Preset_ImmoralVillage_Sayaka_v2 },

            { "ImmoralVillage_Kaede", ModResources.PresetResources.Preset_ImmoralVillage_Kaede },
            { "ImmoralVillage_Kaede_v2", ModResources.PresetResources.Preset_ImmoralVillage_Kaede_v2 },

            { "ImmoralVillage_Minori", ModResources.PresetResources.Preset_ImmoralVillage_Minori },
            { "ImmoralVillage_Minori_v2", ModResources.PresetResources.Preset_ImmoralVillage_Minori_v2 },
        };

        internal static byte[] GetPresetResources(string presetFile)
        {
            if(Mapping.ContainsKey(presetFile))
                return Mapping[presetFile];
            return null;
        }
    }
}
