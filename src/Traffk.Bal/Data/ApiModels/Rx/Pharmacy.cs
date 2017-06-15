using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Traffk.Bal.Data.ApiModels.Rx
{
    public class PharmacyResponse
    {
        public PharmacyResponse(Orchestra.Models.PharmacyResponse response)
        {
            Object = "list";
            var pharmacyList = new List<Pharmacy>();
            foreach (var orchestraPharmacy in response.PharmacyList)
            {
                var pharmacy = new Pharmacy(orchestraPharmacy);
                pharmacyList.Add(pharmacy);
            }
            data = pharmacyList.ToArray();
        }

        [DataMember(Name = "object")]
        public string Object { get; set; }
        public Pharmacy[] data { get; set; }
    }

    public class Pharmacy
    {
        public Pharmacy(Orchestra.Models.Pharmacy pharmacy)
        {
            Object = typeof(Pharmacy).Name.ToString();
            id = pharmacy.PharmacyID;
            name = pharmacy.Name;
            nabp = "";
            address = new ApiAddress(pharmacy);
            phone = pharmacy.PharmacyPhone;
            chain_id = pharmacy.Chain;
            chain_name = pharmacy.ChainName;
            services = new PharmacyServices(pharmacy);
        }

        [DataMember(Name = "object")]
        public string Object { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string nabp { get; set; }
        public ApiAddress address { get; set; }
        public string phone { get; set; }
        public string chain_id { get; set; }
        public string chain_name { get; set; }
        public PharmacyServices services { get; set; }
    }

    public class ApiAddress
    {
        public ApiAddress(Orchestra.Models.Pharmacy pharmacy)
        {
            line_1 = pharmacy.Address1;
            line_2 = pharmacy.Address2;
            city = pharmacy.City;
            state = pharmacy.State;
            zip = pharmacy.Zip;
            latitude = pharmacy.Latitude;
            longitude = pharmacy.Longitude;
        }

        public string line_1 { get; set; }
        public string line_2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
    }

    public class PharmacyServices
    {
        public PharmacyServices(Orchestra.Models.Pharmacy pharmacy)
        {
            var pharmacyService = pharmacy.PharmacyServices;
            has_24hr_service = pharmacyService.Has24hrService;
            has_compounding = pharmacyService.HasCompounding;
            has_delivery = pharmacyService.HasDelivery;
            has_driveup = pharmacyService.HasDriveup;
            has_durable_equipment = pharmacyService.HasDurableEquipment;
            has_e_prescriptions = pharmacyService.HasEPrescriptions;
            has_handicapAccess = pharmacyService.HasHandicapAccess;
        }

        public bool has_24hr_service { get; set; }
        public bool has_compounding { get; set; }
        public bool has_delivery { get; set; }
        public bool has_driveup { get; set; }
        public bool has_durable_equipment { get; set; }
        public bool has_e_prescriptions { get; set; }
        public bool has_handicapAccess { get; set; }
    }

}
