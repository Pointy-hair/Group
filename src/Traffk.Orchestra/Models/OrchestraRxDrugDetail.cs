namespace Traffk.Orchestra.Models
{

    public class DrugDetailResponse
    {
        public int DrugID { get; set; }
        public string DrugName { get; set; }
        public string DrugType { get; set; }
        public int DrugTypeID { get; set; }
        public string ChemicalName { get; set; }
        public OrchestraDosage[] Dosages { get; set; }
        public int GenericDrugID { get; set; }
        public string GenericDrugName { get; set; }
    }

    public class OrchestraDosage
    {
        public string DosageID { get; set; }
        public string ReferenceNDC { get; set; }
        public string LabelName { get; set; }
        public double CommonUserQuantity { get; set; }
        public double CommonMetricQuantity { get; set; }
        public double CommonDaysOfSupply { get; set; }
        public string GenericDosageID { get; set; }
        public bool IsCommonDosage { get; set; }
        public OrchestraPackage[] Packages { get; set; }
    }

    public partial class OrchestraPackage
    {
        public double CommonUserQuantity { get; set; }
        public double CommonMetricQuantity { get; set; }
        public string PackageDescription { get; set; }
        public bool IsCommonPackage { get; set; }
        public double PackageSize { get; set; }
        public string PackageSizeUnitOfMeasure { get; set; }
        public int PackageQuantity { get; set; }
        public double TotalPackageQuantity { get; set; }
        public string ReferenceNDC { get; set; }
    }

}
