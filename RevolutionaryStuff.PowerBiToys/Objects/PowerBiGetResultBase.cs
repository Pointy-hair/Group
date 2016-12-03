using System.Runtime.Serialization;

namespace RevolutionaryStuff.PowerBiToys.Objects
{
    [DataContract]
    public abstract class PowerBiGetResultBase
    {
        [DataMember(Name = "@odata.context")]
        public string ODataContext { get; set; }

        public override string ToString() => $"{GetType().Name} context=[{ODataContext}]";
    }

    [DataContract]
    public abstract class PowerBiGetResultBase<TValue> : PowerBiGetResultBase
    {
        [DataMember(Name = "value")]
        public TValue[] Values { get; set; }

        public override string ToString() => $"{base.ToString()} valuesCnt={(Values == null ? 0 : Values.Length)}";
    }
}
