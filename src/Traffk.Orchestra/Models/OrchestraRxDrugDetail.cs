namespace Traffk.Orchestra.Models
{

    public class DrugDetailResponse
    {
        public int DrugID { get; set; }
        public string DrugName { get; set; }
        public string DrugType { get; set; }
        public int DrugTypeID { get; set; }
        public string ChemicalName { get; set; }
        public Dosage[] Dosages { get; set; }
    }

    public class Dosage
    {
        public string DosageID { get; set; }
        public string ReferenceNDC { get; set; }
        public string LabelName { get; set; }
        public double CommonUserQuantity { get; set; }
        public double CommonMetricQuantity { get; set; }
        public double CommonDaysOfSupply { get; set; }
        public bool IsCommonDosage { get; set; }
        public Package[] Packages { get; set; }
    }

    public class Package
    {
        public int CommonUserQuantity { get; set; }
        public int CommonMetricQuantity { get; set; }
        public string PackageDescription { get; set; }
        public bool IsCommonPackage { get; set; }
        public int PackageSize { get; set; }
        public string PackageSizeUnitOfMeasure { get; set; }
        public int PackageQuantity { get; set; }
        public int TotalPackageQuantity { get; set; }
        public string ReferenceNDC { get; set; }
    }

}
