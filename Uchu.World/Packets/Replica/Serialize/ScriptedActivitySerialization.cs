using Uchu.Core;

namespace Uchu.World
{
    public struct ScriptedActivitySerialization
    {
        [Default]
        public ActivityUserInfo[] ActivityUserInfos { get; set; }
    }

    [Struct]
    public struct ActivityUserInfo
    {
        public GameObject User { get; set; }
        public float ActivityValue0 { get; set; }
        public float ActivityValue1 { get; set; }
        public float ActivityValue2 { get; set; }
        public float ActivityValue3 { get; set; }
        public float ActivityValue4 { get; set; }
        public float ActivityValue5 { get; set; }
        public float ActivityValue6 { get; set; }
        public float ActivityValue7 { get; set; }
        public float ActivityValue8 { get; set; }
        public float ActivityValue9 { get; set; }
    }
}
