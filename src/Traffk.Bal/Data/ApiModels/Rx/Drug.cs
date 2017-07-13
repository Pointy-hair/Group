using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using Traffk.Orchestra.Models;

namespace Traffk.Bal.Data.ApiModels.Rx
{
    public class DrugResponse
    {
        public DrugResponse()
        {
        }

        public DrugResponse(Orchestra.Models.DrugResponse orchestraDrugResponse)
        {
            var data = new List<Drug>();
            foreach (var orchestraDrug in orchestraDrugResponse.Drugs)
            {
                var drug = new Drug(orchestraDrug);
                data.Add(drug);
            }
            Data = data.ToArray();
        }

        [JsonProperty("object")]
        public string Object { get; set; } = ApiObjectTypes.ObjectNames.List.ToString().ToLower();
        [JsonProperty("data")]
        public Drug[] Data { get; set; }
    }

    public class Drug
    {
        private string IdPrefix = "OD";

        public Drug()
        {
        }

        public Drug(Orchestra.Models.OrchestraDrug orchestraDrug)
        {
            Id = IdPrefix + orchestraDrug.DrugID.ToString();
            Name = orchestraDrug.DrugName;
            Type = orchestraDrug.DrugType;
            ChemicalName = orchestraDrug.ChemicalName;
            GenericDrugId = orchestraDrug.GenericDrugID;
            GenericDrugName = orchestraDrug.GenericDrugName;
            Ndc = orchestraDrug.ReferenceNDC;
        }

        public Drug(Orchestra.Models.DrugDetailResponse o)
        {
            Id = IdPrefix + o.DrugID.ToString();
            Name = o.DrugName;
            Type = o.DrugType;
            ChemicalName = o.ChemicalName;
            GenericDrugId = o.GenericDrugID;
            GenericDrugName = o.GenericDrugName;
            Ndc = o.Dosages.FirstOrDefault().ReferenceNDC;
            Dosages = new Dosages(o.Dosages);
        }

        public Drug(Orchestra.Models.Alternative o)
        {
            Name = o.DrugName;
            Type = o.DrugTypeShortDescription;
            ChemicalName = o.ChemicalName;
            Ndc = o.ReferenceNDC;
            var dosageList = new List<Dosage>();
            var dosage = new Dosage(o);
            var packageList = new List<Package>();
            if (o.Package != null)
            {
                var package = new Package(o.Package);
                packageList.Add(package);

                dosage.Packages = packageList.ToArray();
            }
            
            dosageList.Add(dosage);

            this.Dosages = new Dosages { Data = dosageList.ToArray() };
        }

        public Drug(DrugToReplace o)
        {
            Name = o.DrugName;
            Type = o.DrugTypeShortDescription;
            ChemicalName = o.ChemicalName;
            Ndc = o.ReferenceNDC;
            RouteOfAdministration = o.RouteOfAdministration;
            DEASchedule = o.DEASchedule.ToString();
            var dosageList = new List<Dosage>();
            var dosage = new Dosage(o);
            var packageList = new List<Package>();
            if (o.Package != null)
            {
                var package = new Package(o.Package);
                packageList.Add(package);

                dosage.Packages = packageList.ToArray();
            }

            dosageList.Add(dosage);

            this.Dosages = new Dosages {Data = dosageList.ToArray()};
        }

        [JsonProperty("object")]
        public string Object { get; set; } = typeof(Drug).Name.ToLower();
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("chemicalName")]
        public string ChemicalName { get; set; }
        //[JsonProperty("drugType")]
        //public string DrugType { get; set; }
        [JsonProperty("genericDrugId")]
        public object GenericDrugId { get; set; }
        [JsonProperty("genericDrugName")]
        public string GenericDrugName { get; set; }
        [JsonProperty("ndc")]
        public string Ndc { get; set; }
        [JsonProperty("routeOfAdministration")]
        public string RouteOfAdministration { get; set; }
        [JsonProperty("deaSchedule")]
        public string DEASchedule { get; set; }
        [JsonProperty("dosages")]
        public Dosages Dosages { get; set; }
    }

    public class Dosages
    {
        public Dosages()
        {
        }

        public Dosages(Orchestra.Models.OrchestraDosage[] orchestraDosages)
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
        public string Object { get; set; } = ApiObjectTypes.ObjectNames.List.ToString().ToLower();
        [JsonProperty("data")]
        public Dosage[] Data { get; set; }
    }

    public class Dosage
    {
        private string IdPrefix = "ODO";

        public Dosage()
        {

        }

        public Dosage(Orchestra.Models.OrchestraDosage orchestraDosage)
        {
            Id = IdPrefix + orchestraDosage.DosageID;
            ReferenceNdc = orchestraDosage.ReferenceNDC;
            LabelName = orchestraDosage.LabelName;
            CommonUserQuantity = orchestraDosage.CommonUserQuantity;
            CommonMetricQuantity = orchestraDosage.CommonMetricQuantity;
            CommonDaysOfSupply = orchestraDosage.CommonDaysOfSupply;
            IsCommonDosage = orchestraDosage.IsCommonDosage;
            GenericDosageId = orchestraDosage.GenericDosageID;
        }

        public Dosage(Orchestra.Models.Alternative o)
        {
            Id = IdPrefix + o.DoseID;
            CommonDaysOfSupply = o.DaysOfSupply;
            DosageForm = o.DosageForm;
            DosageComments = o.DoseComments;
            DosageSignature = o.DoseSignature;
            HasPackages = o.HasPackages;
            LabelName = o.LabelName;
            CommonMetricQuantity = o.MetricQuantity;
        }

        public Dosage(DrugToReplace o)
        {
            Id = IdPrefix + o.DoseID;
            CommonDaysOfSupply = o.DaysOfSupply;
            DosageForm = o.DosageForm;
            DosageComments = o.DoseComments;
            DosageSignature = o.DoseSignature;
            HasPackages = o.HasPackages;
            LabelName = o.LabelName;
            CommonMetricQuantity = o.MetricQuantity;
        }

        [JsonProperty("object")]
        public string Object { get; set; } = typeof(Dosage).Name.ToLower();
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("referenceNdc")]
        public string ReferenceNdc { get; set; }
        [JsonProperty("labelName")]
        public string LabelName { get; set; }
        [JsonProperty("commonUserQuantity")]
        public double CommonUserQuantity { get; set; }
        [JsonProperty("commonMetricQuantity")]
        public double CommonMetricQuantity { get; set; }
        [JsonProperty("commonDaysOfSupply")]
        public double CommonDaysOfSupply { get; set; }
        [JsonProperty("isCommonDosage")]
        public bool IsCommonDosage { get; set; }
        [JsonProperty("genericDosageId")]
        public string GenericDosageId { get; set; }
        [JsonProperty("hasGenericDosage")]
        public bool HasGenericDosage => !String.IsNullOrEmpty(GenericDosageId);
        [JsonProperty("dosageForm")]
        public string DosageForm { get; set; }
        [JsonProperty("dosageComments")]
        public string DosageComments { get; set; }
        [JsonProperty("dosageSignature")]
        public string DosageSignature { get; set; }
        [JsonProperty("hasPackages")]
        public bool HasPackages { get; set; }
        public Package[] Packages { get; set; }

    }

    public class Package
    {
        public Package()
        {
            
        }

        public Package(OrchestraPackage o)
        {
            CommonMetricQuantity = o.CommonMetricQuantity;
            CommonUserQuantity = o.CommonUserQuantity;
            PackageDescription = o.PackageDescription;
            IsCommonPackage = o.IsCommonPackage;
            PackageSize = o.PackageSize;
            PackageSizeUnitOfMeasure = String.IsNullOrEmpty(o.PackageSizeUnitOfMeasure) ? o.PackageSizeUOM : o.PackageSizeUnitOfMeasure;
            PackageQuantity = o.PackageQuantity;
        }

        [JsonProperty("object")]
        public string Object { get; set; } = typeof(Package).Name.ToLower();
        [JsonProperty("commonUserQuantity")]
        public double CommonUserQuantity { get; set; }
        [JsonProperty("commonMetricQuantity")]
        public double CommonMetricQuantity { get; set; }
        [JsonProperty("packageDescription")]
        public string PackageDescription { get; set; }
        [JsonProperty("isCommonPackage")]
        public bool IsCommonPackage { get; set; }
        [JsonProperty("packageSize")]
        public double PackageSize { get; set; }
        [JsonProperty("packageUnitOfMeasure")]
        public string PackageSizeUnitOfMeasure { get; set; }
        [JsonProperty("packageQuantity")]
        public int PackageQuantity { get; set; }
    }

}
