namespace Traffk.Orchestra.Models
{
    public class DrugResponse
    {
        public Drug[] Drugs { get; set; }
    }

    public class Drug
    {
        public int DrugID { get; set; }
        public string DrugName { get; set; }
        public string DrugType { get; set; }
        public int DrugTypeID { get; set; }
        public string ChemicalName { get; set; }
        public int GenericDrugID { get; set; }
        public string GenericDrugName { get; set; }
        public int SearchMatchType { get; set; }
        public string ReferenceNDC { get; set; }
    }

}
