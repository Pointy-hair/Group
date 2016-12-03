using System.Runtime.Serialization;

namespace Traffk.Bal.Settings
{
    [DataContract]
    public class HostSettings
    {
        [DataContract]
        public class HostInfo
        {
            [DataMember(Name = "Hostname")]
            public string Hostname { get; set; }
        }

        [DataMember(Name = "HostInfos")]
        public HostInfo[] HostInfos { get; set; }
    }
}
