
namespace Traffk.Bal.Data.Rdb
{
    [ConstrainedData(
        nameof(ins_cobra_code),
        nameof(ins_cobra_desc),
        nameof(ins_coverage_type_code),
        nameof(ins_coverage_type_desc),
        nameof(ins_division_name),
        nameof(ins_division_id),
        nameof(ins_emp_group_id),
        nameof(ins_emp_group_name),
        nameof(ins_plan_desc),
        nameof(ins_plan_id),
        nameof(ins_plan_type_code),
        nameof(ins_plan_type_desc),
        nameof(mbr_region_code),
        nameof(mbr_region_name),
        nameof(mbr_relationship_code),
        nameof(mbr_relationship_desc),
        nameof(mbr_current_status),
        nameof(mbr_gender)
    )]

    public partial class Eligibility
    {
        [HideInPortal]
        public IAddress Address => new Address
        {
            Address1 = mbr_street_1,
            Address2 = mbr_street_2,
            State = mbr_state,
            //Country = mbr_county,
            PostalCode = mbr_zip,
            City = mbr_city
        };
    }
}
