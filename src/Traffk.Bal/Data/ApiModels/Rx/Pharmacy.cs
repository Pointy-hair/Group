using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Traffk.Bal.Data.ApiModels.Rx
{
    public class PharmacyResponse
    {
        public PharmacyResponse()
        { }

        public PharmacyResponse(Orchestra.Models.PharmacyResponse response)
        {
            var pharmacyList = new List<Pharmacy>();
            foreach (var orchestraPharmacy in response.PharmacyList)
            {
                var pharmacy = new Pharmacy(orchestraPharmacy);
                pharmacyList.Add(pharmacy);
            }
            Data = pharmacyList.ToArray();
        }

        [JsonProperty("object")]
        public string Object { get; set; } = ApiObjectTypes.ObjectNames.List.ToString().ToLower();
        [JsonProperty("data")]
        public Pharmacy[] Data { get; set; }
    }

    public class Pharmacy
    {
        private string IdPrefix = "OP";

        public Pharmacy()
        { }

        public Pharmacy(Orchestra.Models.OrchestraPharmacy pharmacy)
        {
            Id = IdPrefix + pharmacy.PharmacyID;
            Name = pharmacy.Name;
            Nabp = pharmacy.PharmacyNABP;
            Address = new ApiAddress(pharmacy);
            Phone = pharmacy.PharmacyPhone;
            ChainId = pharmacy.Chain;
            ChainName = pharmacy.ChainName;
            Services = new PharmacyServices(pharmacy);
        }
        
        [JsonProperty("object")]
        public string Object { get; set; } = typeof(Pharmacy).Name.ToLower();
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("nabp")]
        public string Nabp { get; set; }
        [JsonProperty("address")]
        public ApiAddress Address { get; set; }
        [JsonProperty("phone")]
        public string Phone { get; set; }
        [JsonProperty("chainId")]
        public string ChainId { get; set; }
        [JsonProperty("chainName")]
        public string ChainName { get; set; }
        [JsonProperty("services")]
        public PharmacyServices Services { get; set; }
    }

    public class ApiAddress
    {
        public ApiAddress()
        { }

        public ApiAddress(Orchestra.Models.OrchestraPharmacy pharmacy)
        {
            Line1 = pharmacy.Address1;
            Line2 = pharmacy.Address2;
            City = pharmacy.City;
            State = pharmacy.State;
            Zip = pharmacy.Zip;
            Latitude = pharmacy.Latitude;
            Longitude = pharmacy.Longitude;
        }
        [JsonProperty("line1")]
        public string Line1 { get; set; }
        [JsonProperty("line2")]
        public string Line2 { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("zip")]
        public string Zip { get; set; }
        [JsonProperty("latitude")]
        public float Latitude { get; set; }
        [JsonProperty("longitude")]
        public float Longitude { get; set; }
    }

    public class PharmacyServices
    {
        public PharmacyServices()
        { }

        public PharmacyServices(Orchestra.Models.OrchestraPharmacy pharmacy)
        {
            var pharmacyService = pharmacy.PharmacyServices;
            Has24HrService = pharmacyService.Has24hrService;
            HasCompounding = pharmacyService.HasCompounding;
            HasDelivery = pharmacyService.HasDelivery;
            HasDriveup = pharmacyService.HasDriveup;
            HasDurableEquipment = pharmacyService.HasDurableEquipment;
            HasEPrescriptions = pharmacyService.HasEPrescriptions;
            HasHandicapAccess = pharmacyService.HasHandicapAccess;
        }
        [JsonProperty("has24hrService")]
        public bool Has24HrService { get; set; }
        [JsonProperty("hasCompounding")]
        public bool HasCompounding { get; set; }
        [JsonProperty("hasDelivery")]
        public bool HasDelivery { get; set; }
        [JsonProperty("hasDriveup")]
        public bool HasDriveup { get; set; }
        [JsonProperty("hasDurableEquipment")]
        public bool HasDurableEquipment { get; set; }
        [JsonProperty("hasEPrescriptions")]
        public bool HasEPrescriptions { get; set; }
        [JsonProperty("hasHandicapAccess")]
        public bool HasHandicapAccess { get; set; }
    }

}
