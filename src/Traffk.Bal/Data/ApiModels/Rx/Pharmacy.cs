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
            Object = "list";
            var pharmacyList = new List<Pharmacy>();
            foreach (var orchestraPharmacy in response.PharmacyList)
            {
                var pharmacy = new Pharmacy(orchestraPharmacy);
                pharmacyList.Add(pharmacy);
            }
            Data = pharmacyList.ToArray();
        }

        [JsonProperty("object")]
        public string Object { get; set; }
        [JsonProperty("data")]
        public Pharmacy[] Data { get; set; }
    }

    public class Pharmacy
    {
        public Pharmacy()
        { }

        public Pharmacy(Orchestra.Models.OrchestraPharmacy pharmacy)
        {
            Object = typeof(Pharmacy).Name.ToString();
            Id = pharmacy.PharmacyID;
            Name = pharmacy.Name;
            Nabp = "";
            Address = new ApiAddress(pharmacy);
            Phone = pharmacy.PharmacyPhone;
            ChainId = pharmacy.Chain;
            ChainName = pharmacy.ChainName;
            Services = new PharmacyServices(pharmacy);
        }
        
        [JsonProperty("object")]
        public string Object { get; set; }
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
        [JsonProperty("chain_id")]
        public string ChainId { get; set; }
        [JsonProperty("chain_name")]
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
        [JsonProperty("line_1")]
        public string Line1 { get; set; }
        [JsonProperty("line_2")]
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
        [JsonProperty("has_24hr_service")]
        public bool Has24HrService { get; set; }
        [JsonProperty("has_compounding")]
        public bool HasCompounding { get; set; }
        [JsonProperty("has_delivery")]
        public bool HasDelivery { get; set; }
        [JsonProperty("has_driveup")]
        public bool HasDriveup { get; set; }
        [JsonProperty("has_durable_equipment")]
        public bool HasDurableEquipment { get; set; }
        [JsonProperty("has_e_prescriptions")]
        public bool HasEPrescriptions { get; set; }
        [JsonProperty("has_handicapAccess")]
        public bool HasHandicapAccess { get; set; }
    }

}
