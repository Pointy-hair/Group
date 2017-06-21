using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Traffk.Bal.Data.ApiModels.Rx
{
    public class DrugResponse
    {
        public DrugResponse()
        {
            Object = "list";
        }

        public DrugResponse(Orchestra.Models.DrugResponse orchestraDrugResponse)
        {
            Object = "list";
            var data = new List<Drug>();
            foreach (var orchestraDrug in orchestraDrugResponse.Drugs)
            {
                var drug = new Drug(orchestraDrug);
                data.Add(drug);
            }
            Data = data.ToArray();
        }

        [JsonProperty("object")]
        public string Object { get; set; }
        [JsonProperty("data")]
        public Drug[] Data { get; set; }
    }

    public class Drug
    {
        public Drug()
        {
            Object = typeof(Drug).Name;
        }

        public Drug(Orchestra.Models.Drug orchestraDrug)
        {
            Object = typeof(Drug).Name;
            Id = orchestraDrug.DrugID;
            Name = orchestraDrug.DrugName;
            Type = orchestraDrug.DrugType;
            ChemicalName = orchestraDrug.ChemicalName;
            GenericDrugId = orchestraDrug.GenericDrugID;
            GenericDrugName = orchestraDrug.GenericDrugName;
            Ndc = orchestraDrug.ReferenceNDC;
        }

        [JsonProperty("object")]
        public string Object { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("chemical_name")]
        public string ChemicalName { get; set; }
        [JsonProperty("generic_drug_id")]
        public object GenericDrugId { get; set; }
        [JsonProperty("generic_drug_name")]
        public string GenericDrugName { get; set; }
        [JsonProperty("ndc")]
        public string Ndc { get; set; }
        [JsonProperty("dosages")]
        public Dosages Dosages { get; set; }
    }

    public class Dosages
    {
        public Dosages()
        {
            Object = "list";
        }

        public Dosages(Orchestra.Models.Dosage[] orchestraDosages)
        {
            var tDosages = new List<Dosage>();
            foreach (var dosage in orchestraDosages)
            {
                var tDosage = new Dosage(dosage);
                tDosages.Add(tDosage);
            }
            Data = tDosages.ToArray();
        }

        [JsonProperty("object")]
        public string Object { get; set; }
        [JsonProperty("data")]
        public Dosage[] Data { get; set; }
    }

    public class Dosage
    {
        public Dosage()
        {
            Object = typeof(Dosage).Name;
        }

        public Dosage(Orchestra.Models.Dosage orchestraDosage)
        {
            Object = typeof(Dosage).Name;
            Id = orchestraDosage.DosageID;
            ReferenceNdc = orchestraDosage.ReferenceNDC;
            LabelName = orchestraDosage.LabelName;
            CommonUserQuantity = orchestraDosage.CommonUserQuantity;
            CommonMetricQuantity = orchestraDosage.CommonMetricQuantity;
            CommonDaysOfSupply = orchestraDosage.CommonDaysOfSupply;
            IsCommonDosage = orchestraDosage.IsCommonDosage;
            //GenericDosageId = ?
        }

        [JsonProperty("object")]
        public string Object { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("reference_ndc")]
        public string ReferenceNdc { get; set; }
        [JsonProperty("label_name")]
        public string LabelName { get; set; }
        [JsonProperty("common_user_quantity")]
        public double CommonUserQuantity { get; set; }
        [JsonProperty("common_metric_quantity")]
        public double CommonMetricQuantity { get; set; }
        [JsonProperty("common_days_of_supply")]
        public double CommonDaysOfSupply { get; set; }
        [JsonProperty("is_common_dosage")]
        public bool IsCommonDosage { get; set; }
        [JsonProperty("generic_dosage_id")]
        public string GenericDosageId { get; set; }
    }

}
