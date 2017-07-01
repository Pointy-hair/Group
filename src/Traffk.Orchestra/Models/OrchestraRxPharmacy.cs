namespace Traffk.Orchestra.Models
{
    public class PharmacyResponse
    {
        public double Radius { get; set; }
        public OrchestraPharmacy[] PharmacyList { get; set; }
    }

    public class OrchestraPharmacy
    {
        public string PharmacyID { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public float Distance { get; set; }
        public string PharmacyPhone { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string Chain { get; set; }
        public OrchestraPharmacyServices PharmacyServices { get; set; }
        public string ChainName { get; set; }
    }

    public class OrchestraPharmacyServices
    {
        public bool Has24hrService { get; set; }
        public bool HasCompounding { get; set; }
        public bool HasDelivery { get; set; }
        public bool HasDriveup { get; set; }
        public bool HasDurableEquipment { get; set; }
        public bool HasEPrescriptions { get; set; }
        public bool HasHandicapAccess { get; set; }
        public bool IsHomeInfusion { get; set; }
        public bool IsLongTermCare { get; set; }
    }

}
