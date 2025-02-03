using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace COM3D2.WildParty.Plugin
{
    public class EyeSightSetting
    {
        public EyeSightType Type = EyeSightType.NoChange;
        public EyeToCameraSettingDetail EyeToCameraSetting;
        public EyeToCharaSettingDetail EyeToCharaSetting;
        public EyeToObjectSettingDetail EyeToObjectSetting;
        public GroupMemberType SourceGroupMember;       //Used in Group only (Coordinate json)

        public class EyeToCameraSettingDetail
        {
            public EyeToCameraMoveType MoveType;
        }

        public class EyeToCharaSettingDetail
        {
            public TargetType Type;
            public int ArrayPosition;
            public GroupMemberType TargetGroupMember;
            

            public enum TargetType
            {
                Man,
                Maid,
                ClubOwner,
                GroupMember
            }

            
        }

        public class EyeToObjectSettingDetail
        {
            public GameObject Target;
        }

        public enum EyeSightType
        {
            ToChara,
            ToCamera,
            ToObject,
            Reset,
            NoChange
        }



        public enum EyeToCameraMoveType
        {
            None = 0,
            Ignore = 1,
            LookAt = 2,
            MoveFaceOnly = 3,
            AvoidFace = 4,
            FaceAndEye = 5,
            EyeOnly = 6,
            AvoidEye = 7
        }

        public enum GroupMemberType
        {
            Maid1,
            Maid2,
            Man1,
            Man2
        }
    }
}
