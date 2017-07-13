namespace Traffk.Orchestra.Models
{
    public class OrchestraDrugAlternativeResponse
    {
        public DrugToReplace[] DrugOptions { get; set; }
    }

    public class DrugToReplace
    {
        public string ConditionDescription { get; set; }
        public string ConditionName { get; set; }
        public string Warning { get; set; }
        public Option[] Options { get; set; }
        public OrchestraPharmacy[] Pharmacies { get; set; }
        public float AveragePrice { get; set; }
        public string ChemicalName { get; set; }
        public int DEASchedule { get; set; }
        public float DaysOfSupply { get; set; }
        public string DosageForm { get; set; }
        public string DoseComments { get; set; }
        public string DoseSignature { get; set; }
        public string DrugName { get; set; }
        public string DrugTypeShortDescription { get; set; }
        public int DrugTypeId { get; set; }
        public ExternalReference[] ExternalReferences { get; set; }
        public bool HasGenericDosage { get; set; }
        public bool HasPackages { get; set; }
        public bool HasAlternatives { get; set; }
        public string LabelName { get; set; }
        public float MetricQuantity { get; set; }
        public OrchestraPackage Package { get; set; }
        public string ReferenceNDC { get; set; }
        public string RouteOfAdministration { get; set; }
        public string RouteOfAdministrationCode { get; set; }
        public string DoseID { get; set; }
        public PriceData[] PriceDataList { get; set; }
    }

    public partial class OrchestraPackage
    {
        public string PackageDisplayDescription { get; set; }
        public string PackageSizeUOM { get; set; }
    }

    public class Option
    {
        public float AWPTotalCost { get; set; }
        public string AlternativeName { get; set; }
        public Alternative[] Alternatives { get; set; }
        public string SwitchDescription { get; set; }
        public int SwitchScore { get; set; }
    }

    public class Alternative
    {
        public bool IsCrossCategory { get; set; }
        public bool IsDirectGeneric { get; set; }
        public bool IsPillSplit { get; set; }
        public FormularyStatus FormularyStatus { get; set; }
        public float AveragePrice { get; set; }
        public string ChemicalName { get; set; }
        public int DEASchedule { get; set; }
        public float DaysOfSupply { get; set; }
        public string DosageForm { get; set; }
        public string DoseComments { get; set; }
        public string DoseSignature { get; set; }
        public string DrugName { get; set; }
        public string DrugTypeShortDescription { get; set; }
        public int DrugTypeId { get; set; }
        public ExternalReference[] ExternalReferences { get; set; }
        public bool HasGenericDosage { get; set; }
        public bool HasPackages { get; set; }
        public bool HasAlternatives { get; set; }
        public string LabelName { get; set; }
        public float MetricQuantity { get; set; }
        public OrchestraPackage Package { get; set; }
        public string ReferenceNDC { get; set; }
        public string RouteOfAdministration { get; set; }
        public string RouteOfAdministrationCode { get; set; }
        public string DoseID { get; set; }
        public PriceData[] PriceDataList { get; set; }
    }

    public class FormularyStatus
    {
        public bool HasPriorAuthorization { get; set; }
        public bool HasQuantityLimit { get; set; }
        public bool HasStepTherapy { get; set; }
        public int QuantityLimitDays { get; set; }
        public string QuantityLimitDescription { get; set; }
        public string TierDescription { get; set; }
        public int TierNumber { get; set; }
    }

    public class ExternalReference
    {
        public string ExternalReferenceDescription { get; set; }
        public bool ExternalReferenceFlag { get; set; }
        public string ExternalReferenceNotes { get; set; }
        public string ExternalReferenceType { get; set; }
    }

    public class PriceData
    {
        public string PharmacyID { get; set; }
        public string PharmacyNABP { get; set; }
        public string DoseID { get; set; }
        public float DaysOfSupply { get; set; }
        public string[] DeductibleMessages { get; set; }
        public bool DisplayPrice { get; set; }
        public string[] ErrorMessages { get; set; }
        public float MemberCost { get; set; }
        public float TotalCost { get; set; }
        public bool HasReferencePrice { get; set; }
        public string DoseIDReference { get; set; }
        public bool IsAWPSourced { get; set; }
    }

    public class HasAlternativesResponse
    {
        public bool HasAlternatives { get; set; }
    }

    public class DrugAlternativeSearchQuery
    {
        public string NDC { get; set; }
        public double MetricQuantity { get; set; }
        public double DaysOfSupply { get; set; }
    }

    public enum PharmacyIDType
    {
        DRXPharmacyID = 0,
        NABP = 1,
        NPI = 2
    }

    public class DrugAlternativePharmacyFilterQuery
    {
        public string PharmacyID { get; set; }
        public PharmacyIDType PharmacyIDType { get; set; }
        public bool isMailOrder { get; set; }
    }
}
