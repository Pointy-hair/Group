#if false
using Newtonsoft.Json;
using RevolutionaryStuff.Azure.DocumentDb;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Traffk.Bal.Data.Ddb.Crm
{
    [DocumentCollection(CrmDdbContext.DatabaseName, "Eligibility")]
    public partial class Eligibility : DeerwalkDdbEntity
    {
        public static readonly Eligibility[] None = new Eligibility[0];

        /// <summary>
        /// If this element has been linked to a Contact, this is said Id
        /// </summary>
        [JsonProperty("contactId")]
        public string ContactId { get; set; }

        /// <summary>Member ID to display on the application, as sent by client</summary>
        [DisplayName("Member ID to display on the application, as sent by client")]
        [Required]
        [JsonProperty("mbr_id")]
        [SampleData("9916897")]
        [MaxLength(20)]
        public string mbr_id { get; set; }

        /// <summary>Policy Number for Member</summary>
        [DisplayName("Policy Number for Member")]
        [JsonProperty("ins_policy_id")]
        [MaxLength(20)]
        public string ins_policy_id { get; set; }

        /// <summary>Member SSN</summary>
        [DisplayName("Member SSN")]
        [JsonProperty("mbr_ssn")]
        [SampleData("811619")]
        [MaxLength(20)]
        [ProtectedHealthInformationIdentifier]
        public string mbr_ssn { get; set; }

        /// <summary>Member first name</summary>
        [DisplayName("Member first name")]
        [JsonProperty("mbr_first_name")]
        [SampleData("BEVERLY")]
        [MaxLength(30)]
        [ProtectedHealthInformationIdentifier]
        [HideInPortal]
        public string mbr_first_name { get; set; }

        /// <summary>Member middle name</summary>
        [DisplayName("Member middle name")]
        [JsonProperty("mbr_middle_name")]
        [SampleData("George")]
        [MaxLength(30)]
        [ProtectedHealthInformationIdentifier]
        [HideInPortal]
        public string mbr_middle_name { get; set; }

        /// <summary>Member last name</summary>
        [DisplayName("Member last name")]
        [JsonProperty("mbr_last_name")]
        [SampleData("BARRETT")]
        [MaxLength(30)]
        [ProtectedHealthInformationIdentifier]
        [HideInPortal]
        public string mbr_last_name { get; set; }

        /// <summary> Current status of member</summary>
        [DisplayName(" Current status of member")]
        [JsonProperty("mbr_current_status")]
        [SampleData("active")]
        [MaxLength(20)]
        public string mbr_current_status { get; set; }

        /// <summary>Member gender</summary>
        [DisplayName("Member gender")]
        [Required]
        [JsonProperty("mbr_gender")]
        [SampleData("M")]
        [MaxLength(2)]
        [ContrainedData]
        public string mbr_gender { get; set; }

        /// <summary>Member date of Birth</summary>
        [DisplayName("Member date of Birth")]
        [DataType(DataType.Date)]
        [Required]
        [JsonProperty("mbr_dob")]
        [SampleData("31597")]
        [ProtectedHealthInformationIdentifier]
        public DateTime mbr_dob { get; set; }

        /// <summary>Member Street Address 1</summary>
        [DisplayName("Member Street Address 1")]
        [JsonProperty("mbr_street_1")]
        [SampleData("5621 TEAKWOOD ROAD")]
        [MaxLength(50)]
        [ProtectedHealthInformationIdentifier]
        [HideInPortal]
        public string mbr_street_1 { get; set; }

        /// <summary>Member Street Address 2</summary>
        [DisplayName("Member Street Address 2")]
        [JsonProperty("mbr_street_2")]
        [MaxLength(50)]
        [ProtectedHealthInformationIdentifier]
        [HideInPortal]
        public string mbr_street_2 { get; set; }

        /// <summary>Member City</summary>
        [DisplayName("Member City")]
        [JsonProperty("mbr_city")]
        [SampleData("Lakeworth")]
        [MaxLength(100)]
        [ProtectedHealthInformationIdentifier]
        [HideInPortal]
        public string mbr_city { get; set; }

        /// <summary>Member County</summary>
        [DisplayName("Member County")]
        [JsonProperty("mbr_county")]
        [SampleData("Lexington")]
        [MaxLength(20)]
        [HideInPortal]
        public string mbr_county { get; set; }

        /// <summary>Abbreviation of State</summary>
        [DisplayName("Abbreviation of State")]
        [JsonProperty("mbr_state")]
        [SampleData("FL")]
        [MaxLength(2)]
        public string mbr_state { get; set; }

        /// <summary>Zip code</summary>
        [DisplayName("Zip code")]
        [JsonProperty("mbr_zip")]
        [SampleData("34746")]
        [MaxLength(12)]
        [ProtectedHealthInformationIdentifier]
        public string mbr_zip { get; set; }

        /// <summary>Member Phone</summary>
        [DisplayName("Member Phone")]
        [JsonProperty("mbr_phone")]
        [SampleData("7802966511")]
        [MaxLength(15)]
        [ProtectedHealthInformationIdentifier]
        [HideInPortal]
        public string mbr_phone { get; set; }

        /// <summary>Member Region code</summary>
        [DisplayName("Member Region code")]
        [JsonProperty("mbr_region_code")]
        [MaxLength(32)]
        [ContrainedData]
        public string mbr_region_code { get; set; }

        /// <summary>Member Region</summary>
        [DisplayName("Member Region")]
        [JsonProperty("mbr_region_name")]
        [MaxLength(50)]
        [ContrainedData]
        public string mbr_region_name { get; set; }

        /// <summary>Relationship Code to the Subscriber; subscriber(01), spouse (02),child (03), other (04)</summary>
        [DisplayName("Relationship Code to the Subscriber; subscriber(01), spouse(02), child(03), other(04)")]
        [JsonProperty("mbr_relationship_code")]
        [MaxLength(5)]
        [ContrainedData]
        public string mbr_relationship_code { get; set; }

        /// <summary>Relationship Description to the Subscriber, Dependent, Spouse</summary>
        [DisplayName("Relationship Description to the Subscriber, Dependent, Spouse")]
        [JsonProperty("mbr_relationship_desc")]
        [SampleData("Dependent")]
        [MaxLength(20)]
        [ContrainedData]
        public string mbr_relationship_desc { get; set; }

        /// <summary>Plan type code</summary>
        [DisplayName("Plan type code")]
        [JsonProperty("ins_plan_type_code")]
        [SampleData("com")]
        [MaxLength(20)]
        [ContrainedData]
        public string ins_plan_type_code { get; set; }

        /// <summary>Plan type name</summary>
        [DisplayName("Plan type name")]
        [JsonProperty("ins_plan_type_desc")]
        [SampleData("Commercial")]
        [MaxLength(255)]
        [ContrainedData]
        public string ins_plan_type_desc { get; set; }

        /// <summary>TPA/ASO/HMO</summary>
        [DisplayName("TPA / ASO / HMO")]
        [JsonProperty("ins_carrier_id")]
        [SampleData("1")]
        [MaxLength(20)]
        [ContrainedData]
        public string ins_carrier_id { get; set; }

        /// <summary>TPA/ASO/HMO name</summary>
        [DisplayName("TPA / ASO / HMO name")]
        [JsonProperty("ins_carrier_name")]
        [SampleData("Harry TPA")]
        [MaxLength(50)]
        [ContrainedData]
        public string ins_carrier_name { get; set; }

        /// <summary>Coverage type</summary>
        [DisplayName("Coverage type")]
        [JsonProperty("ins_coverage_type_code")]
        [SampleData("1")]
        [MaxLength(20)]
        [ContrainedData]
        public string ins_coverage_type_code { get; set; }

        /// <summary>Coverage type name; infer from code</summary>
        [DisplayName("Coverage type name; infer from code")]
        [JsonProperty("ins_coverage_type_desc")]
        [SampleData("Family")]
        [MaxLength(50)]
        [ContrainedData]
        public string ins_coverage_type_desc { get; set; }

        /// <summary>Plan id of insurance</summary>
        [DisplayName("Plan id of insurance")]
        [JsonProperty("ins_plan_id")]
        [SampleData("M720000 - M")]
        [MaxLength(20)]
        public string ins_plan_id { get; set; }

        /// <summary>Plan name of insurance</summary>
        [DisplayName("Plan name of insurance")]
        [JsonProperty("ins_plan_desc")]
        [SampleData("Family")]
        [MaxLength(100)]
        [ContrainedData]
        public string ins_plan_desc { get; set; }

        /// <summary>Identification of the group the subscriber is employed with</summary>
        [DisplayName("Identification of the group the subscriber is employed with")]
        [JsonProperty("ins_emp_group_id")]
        [SampleData("3198508")]
        [MaxLength(20)]
        [ContrainedData]
        public string ins_emp_group_id { get; set; }

        /// <summary>Name of the group the subscriber is employed with</summary>
        [DisplayName("Name of the group the subscriber is employed with")]
        [JsonProperty("ins_emp_group_name")]
        [SampleData("Deerwalk")]
        [MaxLength(50)]
        [ContrainedData]
        public string ins_emp_group_name { get; set; }

        /// <summary>Identification of the division the subscriber is employed with</summary>
        [DisplayName("Identification of the division the subscriber is employed with")]
        [JsonProperty("ins_division_id")]
        [MaxLength(20)]
        public string ins_division_id { get; set; }

        /// <summary>Name of the group the division  subscriber is employed with</summary>
        [DisplayName("Name of the group the division  subscriber is employed with")]
        [JsonProperty("ins_division_name")]
        [MaxLength(100)]
        public string ins_division_name { get; set; }

        /// <summary>Status Code of the Employee - Not Specified : 00, Working : 01, Terminated : 02</summary>
        [DisplayName("Status Code of the Employee - Not Specified: 00, Working: 01, Terminated: 02")]
        [JsonProperty("ins_cobra_code")]
        [SampleData("1")]
        [MaxLength(2)]
        [ContrainedData]
        public string ins_cobra_code { get; set; }

        /// <summary>Status of the Employee - Working, Terminated, etc</summary>
        [DisplayName("Status of the Employee - Working, Terminated, etc")]
        [JsonProperty("ins_cobra_desc")]
        [SampleData("Working")]
        [MaxLength(30)]
        [ContrainedData]
        public string ins_cobra_desc { get; set; }

        /// <summary>Effective date for medical plan</summary>
        [DisplayName("Effective date for medical plan")]
        [DataType(DataType.Date)]
        [Required]
        [JsonProperty("ins_med_eff_date")]
        [SampleData("39814")]
        public DateTime ins_med_eff_date { get; set; }

        /// <summary>Termination date for medical plan</summary>
        [DisplayName("Termination date for medical plan")]
        [DataType(DataType.Date)]
        [Required]
        [JsonProperty("ins_med_term_date")]
        [SampleData("30 / 09 / 2011")]
        public DateTime ins_med_term_date { get; set; }

        /// <summary>Effective date for drug plan</summary>
        [DisplayName("Effective date for drug plan")]
        [DataType(DataType.Date)]
        [JsonProperty("ins_rx_eff_date")]
        [SampleData("39819")]
        public DateTime ins_rx_eff_date { get; set; }

        /// <summary>Termination date for drug plan</summary>
        [DisplayName("Termination date for drug plan")]
        [DataType(DataType.Date)]
        [JsonProperty("ins_rx_term_date")]
        [SampleData("30 / 06 / 2011")]
        public DateTime ins_rx_term_date { get; set; }

        /// <summary>Effective date for dental plan</summary>
        [DisplayName("Effective date for dental plan")]
        [DataType(DataType.Date)]
        [JsonProperty("ins_den_eff_date")]
        [SampleData("39821")]
        public DateTime ins_den_eff_date { get; set; }

        /// <summary>Termination date for dental plan</summary>
        [DisplayName("Termination date for dental plan")]
        [DataType(DataType.Date)]
        [JsonProperty("ins_den_term_date")]
        [SampleData("40550")]
        public DateTime ins_den_term_date { get; set; }

        /// <summary>Effective date for vision plan</summary>
        [DisplayName("Effective date for vision plan")]
        [DataType(DataType.Date)]
        [JsonProperty("ins_vis_eff_date")]
        [SampleData("39821")]
        public DateTime ins_vis_eff_date { get; set; }

        /// <summary>Termination date for vision plan</summary>
        [DisplayName("Termination date for vision plan")]
        [DataType(DataType.Date)]
        [JsonProperty("ins_vis_term_date")]
        [SampleData("40550")]
        public DateTime ins_vis_term_date { get; set; }

        /// <summary>Effective date for long term disability plan plan</summary>
        [DisplayName("Effective date for long term disability plan plan")]
        [DataType(DataType.Date)]
        [JsonProperty("ins_ltd_eff_date")]
        [SampleData("39821")]
        public DateTime ins_ltd_eff_date { get; set; }

        /// <summary>Termination date for long term disability plan</summary>
        [DisplayName("Termination date for long term disability plan")]
        [DataType(DataType.Date)]
        [JsonProperty("ins_ltd_term_date")]
        [SampleData("40550")]
        public DateTime ins_ltd_term_date { get; set; }

        /// <summary>Effective date for short term disability plan</summary>
        [DisplayName("Effective date for short term disability plan")]
        [DataType(DataType.Date)]
        [JsonProperty("ins_std_eff_date")]
        [SampleData("39821")]
        public DateTime ins_std_eff_date { get; set; }

        /// <summary>Termination date for short term disability plan</summary>
        [DisplayName("Termination date for short term disability plan")]
        [DataType(DataType.Date)]
        [JsonProperty("ins_std_term_date")]
        [SampleData("40550")]
        public DateTime ins_std_term_date { get; set; }

        /// <summary>Primary Care Physician identification number</summary>
        [DisplayName("Primary Care Physician identification number")]
        [JsonProperty("prv_pcp_id")]
        [SampleData("5687456598")]
        [MaxLength(50)]
        public string prv_pcp_id { get; set; }

        /// <summary>Primary Care Physician First Name</summary>
        [DisplayName("Primary Care Physician First Name")]
        [JsonProperty("prv_pcp_first_name")]
        [SampleData("Ashay")]
        [MaxLength(100)]
        public string prv_pcp_first_name { get; set; }

        /// <summary>Primary Care Physician Middle Name</summary>
        [DisplayName("Primary Care Physician Middle Name")]
        [JsonProperty("prv_pcp_middle_name")]
        [SampleData("Kumar")]
        [MaxLength(30)]
        public string prv_pcp_middle_name { get; set; }

        /// <summary>Primary Care Physician Last Name</summary>
        [DisplayName("Primary Care Physician Last Name")]
        [JsonProperty("prv_pcp_last_name")]
        [SampleData("Thakur")]
        [MaxLength(30)]
        public string prv_pcp_last_name { get; set; }

        /// <summary>PCP Location ID</summary>
        [DisplayName("PCP Location ID")]
        [JsonProperty("prv_pcp_site_id")]
        [SampleData("120")]
        [MaxLength(15)]
        public string prv_pcp_site_id { get; set; }

        /// <summary>User Defined Field 1</summary>
        [DisplayName("User Defined Field 1")]
        [JsonProperty("udf1")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf1 { get; set; }

        /// <summary>User Defined Field 2</summary>
        [DisplayName("User Defined Field 2")]
        [JsonProperty("udf2")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf2 { get; set; }

        /// <summary>User Defined Field 3</summary>
        [DisplayName("User Defined Field 3")]
        [JsonProperty("udf3")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf3 { get; set; }

        /// <summary>User Defined Field 4</summary>
        [DisplayName("User Defined Field 4")]
        [JsonProperty("udf4")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf4 { get; set; }

        /// <summary>User Defined Field 5</summary>
        [DisplayName("User Defined Field 5")]
        [JsonProperty("udf5")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf5 { get; set; }

        /// <summary>User Defined Field 6</summary>
        [DisplayName("User Defined Field 6")]
        [JsonProperty("udf6")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf6 { get; set; }

        /// <summary>User Defined Field 7</summary>
        [DisplayName("User Defined Field 7")]
        [JsonProperty("udf7")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf7 { get; set; }

        /// <summary>User Defined Field 8</summary>
        [DisplayName("User Defined Field 8")]
        [JsonProperty("udf8")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf8 { get; set; }

        /// <summary>User Defined Field 9</summary>
        [DisplayName("User Defined Field 9")]
        [JsonProperty("udf9")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf9 { get; set; }

        /// <summary>User Defined Field 10</summary>
        [DisplayName("User Defined Field 10")]
        [JsonProperty("udf10")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf10 { get; set; }

        /// <summary>User Defined Field 11</summary>
        [DisplayName("User Defined Field 11")]
        [JsonProperty("udf11")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf11 { get; set; }

        /// <summary>User Defined Field 12</summary>
        [DisplayName("User Defined Field 12")]
        [JsonProperty("udf12")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf12 { get; set; }

        /// <summary>User Defined Field 13</summary>
        [DisplayName("User Defined Field 13")]
        [JsonProperty("udf13")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf13 { get; set; }

        /// <summary>User Defined Field 14</summary>
        [DisplayName("User Defined Field 14")]
        [JsonProperty("udf14")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf14 { get; set; }

        /// <summary>User Defined Field 15</summary>
        [DisplayName("User Defined Field 15")]
        [JsonProperty("udf15")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf15 { get; set; }

        /// <summary>User Defined Field 16</summary>
        [DisplayName("User Defined Field 16")]
        [JsonProperty("udf16")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf16 { get; set; }

        /// <summary>User Defined Field 17</summary>
        [DisplayName("User Defined Field 17")]
        [JsonProperty("udf17")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf17 { get; set; }

        /// <summary>User Defined Field 18</summary>
        [DisplayName("User Defined Field 18")]
        [JsonProperty("udf18")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf18 { get; set; }

        /// <summary>User Defined Field 19</summary>
        [DisplayName("User Defined Field 19")]
        [JsonProperty("udf19")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf19 { get; set; }

        /// <summary>User Defined Field 20</summary>
        [DisplayName("User Defined Field 20")]
        [JsonProperty("udf20")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf20 { get; set; }

        /// <summary>Member ID</summary>
        [DisplayName("Member ID")]
        [JsonProperty("dw_member_id")]
        [SampleData("Hash Encrypted")]
        [MaxLength(50)]
        public string dw_member_id { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("dw_rawfilename")]
        [MaxLength(500)]
        [HideInPortal]
        public string dw_rawfilename { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf21")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf21 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf22")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf22 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf23")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf23 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf24")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf24 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf25")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf25 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf26")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf26 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf27")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf27 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf28")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf28 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf29")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf29 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf30")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf30 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf31")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf31 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf32")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf32 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf33")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf33 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf34")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf34 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf35")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf35 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf36")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf36 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf37")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf37 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf38")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf38 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf39")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf39 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf40")]
        [MaxLength(100)]
        [HideInPortal]
        public string udf40 { get; set; }

    }

    [DocumentCollection(CrmDdbContext.DatabaseName, "Pharmacy")]
    public partial class Pharmacy : DeerwalkDdbEntity
    {
        public static readonly Pharmacy[] None = new Pharmacy[0];

        /// <summary>Number generated by claim syst1)em</summary>
        [DisplayName("Number generated by claim syst1)em")]
        [Required]
        [JsonProperty("rev_transaction_num")]
        [SampleData("90272068301")]
        [MaxLength(80)]
        public string rev_transaction_num { get; set; }

        /// <summary>Plan type code</summary>
        [DisplayName("Plan type code")]
        [JsonProperty("ins_plan_type_code")]
        [SampleData("Commercial")]
        [MaxLength(20)]
        public string ins_plan_type_code { get; set; }

        /// <summary>Plan type desciption</summary>
        [DisplayName("Plan type desciption")]
        [JsonProperty("ins_plan_type_desc")]
        [MaxLength(255)]
        public string ins_plan_type_desc { get; set; }

        /// <summary>Member identification number</summary>
        [DisplayName("Member identification number")]
        [Required]
        [JsonProperty("mbr_id")]
        [SampleData("345677")]
        [MaxLength(30)]
        public string mbr_id { get; set; }

        /// <summary>Member first name</summary>
        [DisplayName("Member first name")]
        [JsonProperty("mbr_first_name")]
        [SampleData("BEVERLY")]
        [MaxLength(30)]
        public string mbr_first_name { get; set; }

        /// <summary>Member middle name</summary>
        [DisplayName("Member middle name")]
        [JsonProperty("mbr_middle_name")]
        [SampleData("George")]
        [MaxLength(30)]
        public string mbr_middle_name { get; set; }

        /// <summary>Member last name</summary>
        [DisplayName("Member last name")]
        [JsonProperty("mbr_last_name")]
        [SampleData("BARRETT")]
        [MaxLength(30)]
        public string mbr_last_name { get; set; }

        /// <summary>Member gender</summary>
        [DisplayName("Member gender")]
        [Required]
        [JsonProperty("mbr_gender")]
        [SampleData("M")]
        [MaxLength(2)]
        public string mbr_gender { get; set; }

        /// <summary>Member date of Birth</summary>
        [DisplayName("Member date of Birth")]
        [DataType(DataType.Date)]
        [Required]
        [JsonProperty("mbr_dob")]
        [SampleData("31597")]
        public DateTime mbr_dob { get; set; }

        /// <summary>Member City</summary>
        [DisplayName("Member City")]
        [JsonProperty("mbr_city")]
        [SampleData("Lakeworth")]
        [MaxLength(20)]
        public string mbr_city { get; set; }

        /// <summary>Member County</summary>
        [DisplayName("Member County")]
        [JsonProperty("mbr_county")]
        [SampleData("Lexington")]
        [MaxLength(20)]
        public string mbr_county { get; set; }

        /// <summary>Zip code</summary>
        [DisplayName("Zip code")]
        [JsonProperty("mbr_zip")]
        [SampleData("34746")]
        [MaxLength(10)]
        public string mbr_zip { get; set; }

        /// <summary>Relationship Code to the Subscriber; subscriber(01), spouse (02),child (03), other (04)</summary>
        [DisplayName("Relationship Code to the Subscriber; subscriber(01), spouse(02), child(03), other(04)")]
        [JsonProperty("mbr_relationship_code")]
        [MaxLength(5)]
        public string mbr_relationship_code { get; set; }

        /// <summary>Relationship Description to the Subscriber, Dependent, Spouse</summary>
        [DisplayName("Relationship Description to the Subscriber, Dependent, Spouse")]
        [JsonProperty("mbr_relationship_desc")]
        [SampleData("Dependent")]
        [MaxLength(20)]
        public string mbr_relationship_desc { get; set; }

        /// <summary>PBM</summary>
        [DisplayName("PBM")]
        [JsonProperty("ins_carrier_id")]
        [MaxLength(20)]
        public string ins_carrier_id { get; set; }

        /// <summary>PBM name</summary>
        [DisplayName("PBM name")]
        [JsonProperty("ins_carrier_name")]
        [SampleData("Walgreens")]
        [MaxLength(50)]
        public string ins_carrier_name { get; set; }

        /// <summary>Coverage type</summary>
        [DisplayName("Coverage type")]
        [JsonProperty("ins_coverage_type_code")]
        [SampleData("1")]
        [MaxLength(20)]
        public string ins_coverage_type_code { get; set; }

        /// <summary>Coverage type name; infer from code</summary>
        [DisplayName("Coverage type name; infer from code")]
        [JsonProperty("ins_coverage_type_desc")]
        [SampleData("Family")]
        [MaxLength(50)]
        public string ins_coverage_type_desc { get; set; }

        /// <summary>Identification of the group the subscriber is employed with</summary>
        [DisplayName("Identification of the group the subscriber is employed with")]
        [JsonProperty("ins_emp_group_id")]
        [SampleData("3198508")]
        [MaxLength(20)]
        public string ins_emp_group_id { get; set; }

        /// <summary>Name of the group the subscriber is employed with</summary>
        [DisplayName("Name of the group the subscriber is employed with")]
        [JsonProperty("ins_emp_group_name")]
        [SampleData("Deerwalk")]
        [MaxLength(100)]
        public string ins_emp_group_name { get; set; }

        /// <summary>Identification of the division the subscriber is employed with</summary>
        [DisplayName("Identification of the division the subscriber is employed with")]
        [JsonProperty("ins_division_id")]
        [MaxLength(20)]
        public string ins_division_id { get; set; }

        /// <summary>Name of the group the division  subscriber is employed with</summary>
        [DisplayName("Name of the group the division  subscriber is employed with")]
        [JsonProperty("ins_division_name")]
        [MaxLength(100)]
        public string ins_division_name { get; set; }

        /// <summary>Status Code of Employee - Not Specified : 00, Working : 01, Terminated : 02</summary>
        [DisplayName("Status Code of Employee - Not Specified: 00, Working: 01, Terminated: 02")]
        [JsonProperty("ins_cobra_code")]
        [SampleData("1")]
        [MaxLength(2)]
        public string ins_cobra_code { get; set; }

        /// <summary>Status of the Employee - Working, Terminated, etc</summary>
        [DisplayName("Status of the Employee - Working, Terminated, etc")]
        [JsonProperty("ins_cobra_desc")]
        [SampleData("Working")]
        [MaxLength(30)]
        public string ins_cobra_desc { get; set; }

        /// <summary>Member prescriber</summary>
        [DisplayName("Member prescriber")]
        [JsonProperty("prv_prescriber_id")]
        [SampleData("381882404 - 014")]
        [MaxLength(20)]
        public string prv_prescriber_id { get; set; }

        /// <summary>Prescriber First Name</summary>
        [DisplayName("Prescriber First Name")]
        [JsonProperty("prv_prescriber_first_name")]
        [SampleData("Sanket")]
        [MaxLength(70)]
        public string prv_prescriber_first_name { get; set; }

        /// <summary>Prescriber Middle Name</summary>
        [DisplayName("Prescriber Middle Name")]
        [JsonProperty("prv_prescriber_middle_name")]
        [SampleData("Lal")]
        [MaxLength(30)]
        public string prv_prescriber_middle_name { get; set; }

        /// <summary>Prescriber Last Name</summary>
        [DisplayName("Prescriber Last Name")]
        [JsonProperty("prv_prescriber_last_name")]
        [SampleData("Shrestha")]
        [MaxLength(40)]
        public string prv_prescriber_last_name { get; set; }

        /// <summary>Prescriber Provider Type if present.</summary>
        [DisplayName("Prescriber Provider Type if present.")]
        [JsonProperty("prv_prescriber_type_Code")]
        [MaxLength(10)]
        public string prv_prescriber_type_Code { get; set; }

        /// <summary>Prescriber Provider Type Description.</summary>
        [DisplayName("Prescriber Provider Type Description.")]
        [JsonProperty("prv_prescriber_type_desc")]
        [MaxLength(50)]
        public string prv_prescriber_type_desc { get; set; }

        /// <summary>Provider dea number</summary>
        [DisplayName("Provider dea number")]
        [JsonProperty("prv_dea")]
        [SampleData("CC15772")]
        [MaxLength(30)]
        public string prv_dea { get; set; }

        /// <summary>National Provider ID</summary>
        [DisplayName("National Provider ID")]
        [JsonProperty("prv_npi")]
        [SampleData("5687456598")]
        [MaxLength(30)]
        public string prv_npi { get; set; }

        /// <summary>Provider nabp number</summary>
        [DisplayName("Provider nabp number")]
        [JsonProperty("prv_nabp")]
        [MaxLength(30)]
        public string prv_nabp { get; set; }

        /// <summary>Provider Type Code; I/P/A</summary>
        [DisplayName("Provider Type Code; I / P / A")]
        [JsonProperty("prv_type_code")]
        [SampleData("I")]
        [MaxLength(10)]
        public string prv_type_code { get; set; }

        /// <summary>date prescription was written</summary>
        [DisplayName("date prescription was written")]
        [DataType(DataType.Date)]
        [JsonProperty("svc_written_date")]
        [SampleData("20 / 01 / 2011")]
        public DateTime svc_written_date { get; set; }

        /// <summary>date prescription was filled</summary>
        [DisplayName("date prescription was filled")]
        [DataType(DataType.Date)]
        [Required]
        [JsonProperty("svc_filled_date")]
        [SampleData("40696")]
        public DateTime svc_filled_date { get; set; }

        /// <summary>date of service</summary>
        [DisplayName("date of service")]
        [DataType(DataType.Date)]
        [JsonProperty("svc_service_date")]
        [SampleData("40699")]
        public DateTime svc_service_date { get; set; }

        /// <summary>date of payment</summary>
        [DisplayName("date of payment")]
        [DataType(DataType.Date)]
        [JsonProperty("rev_paid_date")]
        [SampleData("40700")]
        public DateTime rev_paid_date { get; set; }

        /// <summary>National Drug Code</summary>
        [DisplayName("National Drug Code")]
        [JsonProperty("svc_ndc_code")]
        [SampleData("2416502")]
        [MaxLength(11)]
        public string svc_ndc_code { get; set; }

        /// <summary>National Drug Code description</summary>
        [DisplayName("National Drug Code description")]
        [JsonProperty("svc_ndc_desc")]
        [SampleData("Evista 60 Mg Tablet")]
        [MaxLength(256)]
        public string svc_ndc_desc { get; set; }

        /// <summary>Pharmacy class code</summary>
        [DisplayName("Pharmacy class code")]
        [JsonProperty("svc_rx_class_code")]
        [SampleData("77")]
        [MaxLength(12)]
        public string svc_rx_class_code { get; set; }

        /// <summary>Pharmacy class code descirption</summary>
        [DisplayName("Pharmacy class code descirption")]
        [JsonProperty("svc_rx_class_desc")]
        [SampleData("Anticoagulants")]
        [MaxLength(200)]
        public string svc_rx_class_desc { get; set; }

        /// <summary>Drug name</summary>
        [DisplayName("Drug name")]
        [JsonProperty("svc_drug_name")]
        [SampleData("Warfarin Sodium")]
        [MaxLength(200)]
        public string svc_drug_name { get; set; }

        /// <summary>Drug dose</summary>
        [DisplayName("Drug dose")]
        [JsonProperty("svc_dosage")]
        [MaxLength(10)]
        public string svc_dosage { get; set; }

        /// <summary>Drug Strength</summary>
        [DisplayName("Drug Strength")]
        [JsonProperty("svc_drug_strength")]
        [SampleData("60 mg")]
        [MaxLength(100)]
        public string svc_drug_strength { get; set; }

        /// <summary>Quanitiy of physical unit</summary>
        [DisplayName("Quanitiy of physical unit")]
        [JsonProperty("svc_unit_qty")]
        [SampleData("5")]
        public int svc_unit_qty { get; set; }

        /// <summary>Prescription supply based in Days</summary>
        [DisplayName("Prescription supply based in Days")]
        [JsonProperty("svc_days_of_supply")]
        [SampleData("10")]
        public int svc_days_of_supply { get; set; }

        /// <summary>Label Name of Prescription</summary>
        [DisplayName("Label Name of Prescription")]
        [JsonProperty("svc_label_name")]
        [MaxLength(200)]
        public string svc_label_name { get; set; }

        /// <summary>Formulary Plan Code</summary>
        [DisplayName("Formulary Plan Code")]
        [JsonProperty("svc_formulary_plan_code")]
        [MaxLength(10)]
        public string svc_formulary_plan_code { get; set; }

        /// <summary>Formulary flag</summary>
        [DisplayName("Formulary flag")]
        [JsonProperty("svc_formulary_flag")]
        [SampleData("Y")]
        [MaxLength(1)]
        public string svc_formulary_flag { get; set; }

        /// <summary>Brand / Generic Indicator</summary>
        [DisplayName("Brand / Generic Indicator")]
        [JsonProperty("svc_generic_flag")]
        [SampleData("N")]
        [MaxLength(1)]
        public string svc_generic_flag { get; set; }

        /// <summary>Mail Order Flag</summary>
        [DisplayName("Mail Order Flag")]
        [JsonProperty("svc_mail_order_flag")]
        [SampleData("Y")]
        [MaxLength(1)]
        public string svc_mail_order_flag { get; set; }

        /// <summary>Per refill quantity</summary>
        [DisplayName("Per refill quantity")]
        [JsonProperty("svc_refill_qty")]
        public int svc_refill_qty { get; set; }

        /// <summary>Number of Refills allowed</summary>
        [DisplayName("Number of Refills allowed")]
        [JsonProperty("svc_refill_allowed")]
        public int svc_refill_allowed { get; set; }

        /// <summary>Allowance Provided at the Pharmacy Sales Counter</summary>
        [DisplayName("Allowance Provided at the Pharmacy Sales Counter")]
        [JsonProperty("svc_counter_allow")]
        public int svc_counter_allow { get; set; }

        /// <summary>Dispensed as Written Instructions</summary>
        [DisplayName("Dispensed as Written Instructions")]
        [JsonProperty("svc_daw_code")]
        [MaxLength(10)]
        public string svc_daw_code { get; set; }

        /// <summary>Dispensed as Written Instructions Description</summary>
        [DisplayName("Dispensed as Written Instructions Description")]
        [JsonProperty("svc_daw_desc")]
        [MaxLength(50)]
        public string svc_daw_desc { get; set; }

        /// <summary>Amount allowed under contract</summary>
        [DisplayName("Amount allowed under contract")]
        [JsonProperty("rev_allowed_amt")]
        [SampleData("800")]
        //[MaxLength(19, 2)]
        public double rev_allowed_amt { get; set; }

        /// <summary>Gross charges</summary>
        [DisplayName("Gross charges")]
        [JsonProperty("rev_billed_amt")]
        [SampleData("500")]
        //[MaxLength(19, 2)]
        public double rev_billed_amt { get; set; }

        /// <summary>Coinsurance due from patient</summary>
        [DisplayName("Coinsurance due from patient")]
        [JsonProperty("rev_coinsurance_amt")]
        [SampleData("10")]
        //[MaxLength(19, 2)]
        public double rev_coinsurance_amt { get; set; }

        /// <summary>Amount collected from the patient as a co-payment.</summary>
        [DisplayName("Amount collected from the patient as a co - payment.")]
        [JsonProperty("rev_copay_amt")]
        [SampleData("5")]
        //[MaxLength(19, 2)]
        public double rev_copay_amt { get; set; }

        /// <summary>Deductible Portion of the Allowed Amount </summary>
        [DisplayName("Deductible Portion of the Allowed Amount ")]
        [JsonProperty("rev_deductible_amt")]
        [SampleData("5")]
        //[MaxLength(19, 2)]
        public double rev_deductible_amt { get; set; }

        /// <summary>Dispensing Fee textged by the Pharmacy to the PBM</summary>
        [DisplayName("Dispensing Fee textged by the Pharmacy to the PBM")]
        [JsonProperty("rev_disp_fee_amt")]
        [SampleData("6")]
        //[MaxLength(19, 2)]
        public double rev_disp_fee_amt { get; set; }

        /// <summary>Cost of ingredients</summary>
        [DisplayName("Cost of ingredients")]
        [JsonProperty("rev_ingred_cost_amt")]
        [SampleData("10")]
        //[MaxLength(19, 2)]
        public double rev_ingred_cost_amt { get; set; }

        /// <summary>State Tax Paid</summary>
        [DisplayName("State Tax Paid")]
        [JsonProperty("rev_stax_amt")]
        [SampleData("20")]
        //[MaxLength(19, 2)]
        public double rev_stax_amt { get; set; }

        /// <summary>Usual and Customary Fee</summary>
        [DisplayName("Usual and Customary Fee")]
        [JsonProperty("rev_usual_cust_amt")]
        [SampleData("30")]
        //[MaxLength(19, 2)]
        public double rev_usual_cust_amt { get; set; }

        /// <summary>Amount paid</summary>
        [DisplayName("Amount paid")]
        [Required]
        [JsonProperty("rev_paid_amt")]
        [SampleData("400")]
        //[MaxLength(19, 2)]
        public double rev_paid_amt { get; set; }

        /// <summary>Adjudication code</summary>
        [DisplayName("Adjudication code")]
        [JsonProperty("rev_adjudication_code")]
        [SampleData("P")]
        [MaxLength(8)]
        public string rev_adjudication_code { get; set; }

        /// <summary>Adjudication description</summary>
        [DisplayName("Adjudication description")]
        [JsonProperty("rev_adjudication_desc")]
        [SampleData("Paid")]
        [MaxLength(50)]
        public string rev_adjudication_desc { get; set; }

        /// <summary>User Defined Field 1</summary>
        [DisplayName("User Defined Field 1")]
        [JsonProperty("udf1")]
        [MaxLength(100)]
        public string udf1 { get; set; }

        /// <summary>User Defined Field 2</summary>
        [DisplayName("User Defined Field 2")]
        [JsonProperty("udf2")]
        [MaxLength(100)]
        public string udf2 { get; set; }

        /// <summary>User Defined Field 3</summary>
        [DisplayName("User Defined Field 3")]
        [JsonProperty("udf3")]
        [MaxLength(100)]
        public string udf3 { get; set; }

        /// <summary>User Defined Field 4</summary>
        [DisplayName("User Defined Field 4")]
        [JsonProperty("udf4")]
        [MaxLength(100)]
        public string udf4 { get; set; }

        /// <summary>User Defined Field 5</summary>
        [DisplayName("User Defined Field 5")]
        [JsonProperty("udf5")]
        [MaxLength(100)]
        public string udf5 { get; set; }

        /// <summary>User Defined Field 6</summary>
        [DisplayName("User Defined Field 6")]
        [JsonProperty("udf6")]
        [MaxLength(100)]
        public string udf6 { get; set; }

        /// <summary>User Defined Field 7</summary>
        [DisplayName("User Defined Field 7")]
        [JsonProperty("udf7")]
        [MaxLength(100)]
        public string udf7 { get; set; }

        /// <summary>User Defined Field 8</summary>
        [DisplayName("User Defined Field 8")]
        [JsonProperty("udf8")]
        [MaxLength(100)]
        public string udf8 { get; set; }

        /// <summary>User Defined Field 9</summary>
        [DisplayName("User Defined Field 9")]
        [JsonProperty("udf9")]
        [MaxLength(100)]
        public string udf9 { get; set; }

        /// <summary>User Defined Field 10</summary>
        [DisplayName("User Defined Field 10")]
        [JsonProperty("udf10")]
        [MaxLength(100)]
        public string udf10 { get; set; }

        /// <summary>User Defined Field 11</summary>
        [DisplayName("User Defined Field 11")]
        [JsonProperty("udf11")]
        [MaxLength(100)]
        public string udf11 { get; set; }

        /// <summary>User Defined Field 12</summary>
        [DisplayName("User Defined Field 12")]
        [JsonProperty("udf12")]
        [MaxLength(100)]
        public string udf12 { get; set; }

        /// <summary>User Defined Field 13</summary>
        [DisplayName("User Defined Field 13")]
        [JsonProperty("udf13")]
        [MaxLength(100)]
        public string udf13 { get; set; }

        /// <summary>User Defined Field 14</summary>
        [DisplayName("User Defined Field 14")]
        [JsonProperty("udf14")]
        [MaxLength(100)]
        public string udf14 { get; set; }

        /// <summary>User Defined Field 15</summary>
        [DisplayName("User Defined Field 15")]
        [JsonProperty("udf15")]
        [MaxLength(100)]
        public string udf15 { get; set; }

        /// <summary>User Defined Field 16</summary>
        [DisplayName("User Defined Field 16")]
        [JsonProperty("udf16")]
        [MaxLength(100)]
        public string udf16 { get; set; }

        /// <summary>User Defined Field 17</summary>
        [DisplayName("User Defined Field 17")]
        [JsonProperty("udf17")]
        [MaxLength(100)]
        public string udf17 { get; set; }

        /// <summary>User Defined Field 18</summary>
        [DisplayName("User Defined Field 18")]
        [JsonProperty("udf18")]
        [MaxLength(100)]
        public string udf18 { get; set; }

        /// <summary>User Defined Field 19</summary>
        [DisplayName("User Defined Field 19")]
        [JsonProperty("udf19")]
        [MaxLength(100)]
        public string udf19 { get; set; }

        /// <summary>User Defined Field 20</summary>
        [DisplayName("User Defined Field 20")]
        [JsonProperty("udf20")]
        [MaxLength(100)]
        public string udf20 { get; set; }

        /// <summary>Member ID</summary>
        [DisplayName("Member ID")]
        [JsonProperty("dw_member_id")]
        [SampleData("Hash Encrypted")]
        [MaxLength(50)]
        public string dw_member_id { get; set; }

        /// <summary>Boolean Field</summary>
        [DisplayName("Boolean Field")]
        [JsonProperty("is_makalu_used")]
        [SampleData("True for Non - EM members and False for EM members")]
        [MaxLength(20)]
        public string is_makalu_used { get; set; }

        /// <summary>Source Filename</summary>
        [DisplayName("Source Filename")]
        [JsonProperty("dw_rawfilename")]
        [MaxLength(500)]
        public string dw_rawfilename { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf21")]
        [MaxLength(100)]
        public string udf21 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf22")]
        [MaxLength(100)]
        public string udf22 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf23")]
        [MaxLength(100)]
        public string udf23 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf24")]
        [MaxLength(100)]
        public string udf24 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf25")]
        [MaxLength(100)]
        public string udf25 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf26")]
        [MaxLength(100)]
        public string udf26 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf27")]
        [MaxLength(100)]
        public string udf27 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf28")]
        [MaxLength(100)]
        public string udf28 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf29")]
        [MaxLength(100)]
        public string udf29 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf30")]
        [MaxLength(100)]
        public string udf30 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf31")]
        [MaxLength(100)]
        public string udf31 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf32")]
        [MaxLength(100)]
        public string udf32 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf33")]
        [MaxLength(100)]
        public string udf33 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf34")]
        [MaxLength(100)]
        public string udf34 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf35")]
        [MaxLength(100)]
        public string udf35 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf36")]
        [MaxLength(100)]
        public string udf36 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf37")]
        [MaxLength(100)]
        public string udf37 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf38")]
        [MaxLength(100)]
        public string udf38 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf39")]
        [MaxLength(100)]
        public string udf39 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf40")]
        [MaxLength(100)]
        public string udf40 { get; set; }

    }

    [DocumentCollection(CrmDdbContext.DatabaseName, "MedicalClaims")]
    public partial class MedicalClaim : DeerwalkDdbEntity
    {
        public static readonly MedicalClaim[] None = new MedicalClaim[0];

        /// <summary>Number generated by claim system</summary>
        [DisplayName("Number generated by claim system")]
        [Required]
        [JsonProperty("rev_claim_id")]
        [SampleData("AAA6819")]
        [MaxLength(20)]
        public string rev_claim_id { get; set; }

        /// <summary>Number of line numbers for this claim</summary>
        [DisplayName("Number of line numbers for this claim")]
        [JsonProperty("rev_claim_line_id")]
        [SampleData("1")]
        [MaxLength(25)]
        public string rev_claim_line_id { get; set; }

        /// <summary>Should be HCFA 1500 or UB04, Dental, Vision, STD</summary>
        [DisplayName("Should be HCFA 1500 or UB04, Dental, Vision, STD")]
        [JsonProperty("rev_claim_type")]
        [SampleData("Prof")]
        [MaxLength(15)]
        public string rev_claim_type { get; set; }

        /// <summary>Claim type description; 0: professional or 1: institutional</summary>
        [DisplayName("Claim type description; 0: professional or 1: institutional")]
        [JsonProperty("rev_claim_type_flag")]
        [SampleData("1")]
        [MaxLength(1)]
        public char rev_claim_type_flag { get; set; }

        /// <summary>Plan type code</summary>
        [DisplayName("Plan type code")]
        [JsonProperty("ins_plan_type_code")]
        [SampleData("com")]
        [MaxLength(20)]
        public string ins_plan_type_code { get; set; }

        /// <summary>Plan type description</summary>
        [DisplayName("Plan type description")]
        [JsonProperty("ins_plan_type_desc")]
        [SampleData("Commercial")]
        [MaxLength(255)]
        public string ins_plan_type_desc { get; set; }

        /// <summary>TPA/ASO/HMO</summary>
        [DisplayName("TPA / ASO / HMO")]
        [JsonProperty("ins_carrier_id")]
        [SampleData("1")]
        [MaxLength(20)]
        public string ins_carrier_id { get; set; }

        /// <summary>TPA/ASO/HMO name</summary>
        [DisplayName("TPA / ASO / HMO name")]
        [JsonProperty("ins_carrier_name")]
        [SampleData("Harry TPA")]
        [MaxLength(50)]
        public string ins_carrier_name { get; set; }

        /// <summary>Coverage type</summary>
        [DisplayName("Coverage type")]
        [JsonProperty("ins_coverage_type_code")]
        [SampleData("1")]
        [MaxLength(10)]
        public string ins_coverage_type_code { get; set; }

        /// <summary>Coverage type name; infer from code</summary>
        [DisplayName("Coverage type name; infer from code")]
        [JsonProperty("ins_coverage_type_desc")]
        [SampleData("Family")]
        [MaxLength(50)]
        public string ins_coverage_type_desc { get; set; }

        /// <summary>Plan id of insurance</summary>
        [DisplayName("Plan id of insurance")]
        [JsonProperty("ins_plan_id")]
        [SampleData("M720000 - M")]
        [MaxLength(20)]
        public string ins_plan_id { get; set; }

        /// <summary>Identification of the group the subscriber is employed with</summary>
        [DisplayName("Identification of the group the subscriber is employed with")]
        [JsonProperty("ins_emp_group_id")]
        [SampleData("3198508")]
        [MaxLength(20)]
        public string ins_emp_group_id { get; set; }

        /// <summary>Name of the group the subscriber is employed with</summary>
        [DisplayName("Name of the group the subscriber is employed with")]
        [JsonProperty("ins_emp_group_name")]
        [SampleData("Deerwalk")]
        [MaxLength(50)]
        public string ins_emp_group_name { get; set; }

        /// <summary>Identification of the division the subscriber is employed with</summary>
        [DisplayName("Identification of the division the subscriber is employed with")]
        [JsonProperty("ins_division_id")]
        [MaxLength(20)]
        public string ins_division_id { get; set; }

        /// <summary>Name of the group the division  subscriber is employed with</summary>
        [DisplayName("Name of the group the division  subscriber is employed with")]
        [JsonProperty("ins_division_name")]
        [MaxLength(100)]
        public string ins_division_name { get; set; }

        /// <summary>Status Code of the Employee - Not Specified : 00, Working : 01, Terminated : 02</summary>
        [DisplayName("Status Code of the Employee - Not Specified: 00, Working: 01, Terminated: 02")]
        [JsonProperty("ins_cobra_code")]
        [SampleData("1")]
        [MaxLength(2)]
        public string ins_cobra_code { get; set; }

        /// <summary>Status of the Employee - Working, Terminated, etc</summary>
        [DisplayName("Status of the Employee - Working, Terminated, etc")]
        [JsonProperty("ins_cobra_desc")]
        [SampleData("Working")]
        [MaxLength(30)]
        public string ins_cobra_desc { get; set; }

        /// <summary>Member identification number</summary>
        [DisplayName("Member identification number")]
        [Required]
        [JsonProperty("mbr_id")]
        [SampleData("345677")]
        [MaxLength(20)]
        public string mbr_id { get; set; }

        /// <summary>Member SSN</summary>
        [DisplayName("Member SSN")]
        [JsonProperty("mbr_ssn")]
        [SampleData("811619")]
        [MaxLength(20)]
        public string mbr_ssn { get; set; }

        /// <summary>Member first name</summary>
        [DisplayName("Member first name")]
        [JsonProperty("mbr_first_name")]
        [SampleData("BEVERLY")]
        [MaxLength(30)]
        public string mbr_first_name { get; set; }

        /// <summary>Member middle name</summary>
        [DisplayName("Member middle name")]
        [JsonProperty("mbr_middle_name")]
        [SampleData("George")]
        [MaxLength(30)]
        public string mbr_middle_name { get; set; }

        /// <summary>Member last name</summary>
        [DisplayName("Member last name")]
        [JsonProperty("mbr_last_name")]
        [SampleData("BARRETT")]
        [MaxLength(30)]
        public string mbr_last_name { get; set; }

        /// <summary>Member gender</summary>
        [DisplayName("Member gender")]
        [Required]
        [JsonProperty("mbr_gender")]
        [SampleData("M")]
        [MaxLength(2)]
        public string mbr_gender { get; set; }

        /// <summary>Member date of Birth</summary>
        [DisplayName("Member date of Birth")]
        [DataType(DataType.Date)]
        [Required]
        [JsonProperty("mbr_dob")]
        [SampleData("31597")]
        public DateTime mbr_dob { get; set; }

        /// <summary>Member Street Address 1</summary>
        [DisplayName("Member Street Address 1")]
        [JsonProperty("mbr_street_1")]
        [SampleData("5621 TEAKWOOD ROAD")]
        [MaxLength(50)]
        public string mbr_street_1 { get; set; }

        /// <summary>Member Street Address 2</summary>
        [DisplayName("Member Street Address 2")]
        [JsonProperty("mbr_street_2")]
        [MaxLength(50)]
        public string mbr_street_2 { get; set; }

        /// <summary>Member City</summary>
        [DisplayName("Member City")]
        [JsonProperty("mbr_city")]
        [SampleData("Lakeworth")]
        [MaxLength(50)]
        public string mbr_city { get; set; }

        /// <summary>Member County</summary>
        [DisplayName("Member County")]
        [JsonProperty("mbr_county")]
        [SampleData("Lexington")]
        [MaxLength(20)]
        public string mbr_county { get; set; }

        /// <summary>Abbreviation of State</summary>
        [DisplayName("Abbreviation of State")]
        [JsonProperty("mbr_state")]
        [SampleData("FL")]
        [MaxLength(2)]
        public string mbr_state { get; set; }

        /// <summary>Zip code</summary>
        [DisplayName("Zip code")]
        [JsonProperty("mbr_zip")]
        [SampleData("34746")]
        [MaxLength(10)]
        public string mbr_zip { get; set; }

        /// <summary>Member Phone</summary>
        [DisplayName("Member Phone")]
        [JsonProperty("mbr_phone")]
        [SampleData("7802966511")]
        [MaxLength(15)]
        public string mbr_phone { get; set; }

        /// <summary>Member Region code</summary>
        [DisplayName("Member Region code")]
        [JsonProperty("mbr_region_code")]
        [MaxLength(32)]
        public string mbr_region_code { get; set; }

        /// <summary>Member Region</summary>
        [DisplayName("Member Region")]
        [JsonProperty("mbr_region_name")]
        [MaxLength(50)]
        public string mbr_region_name { get; set; }

        /// <summary>Relationship Code to the Subscriber; subscriber(01), spouse (02),child (03), other (04)</summary>
        [DisplayName("Relationship Code to the Subscriber; subscriber(01), spouse(02), child(03), other(04)")]
        [JsonProperty("mbr_relationship_code")]
        [MaxLength(10)]
        public string mbr_relationship_code { get; set; }

        /// <summary>Relationship description</summary>
        [DisplayName("Relationship description")]
        [JsonProperty("mbr_relationship_desc")]
        [MaxLength(20)]
        public string mbr_relationship_desc { get; set; }

        /// <summary>Provider of services for ClaimType=HIC/PHYSICIANS or DENTAL</summary>
        [DisplayName("Provider of services for ClaimType = HIC / PHYSICIANS or DENTAL")]
        [JsonProperty("prv_service_provider_id")]
        [SampleData("772698")]
        [MaxLength(30)]
        public string prv_service_provider_id { get; set; }

        /// <summary>National Provider ID</summary>
        [DisplayName("National Provider ID")]
        [JsonProperty("prv_npi")]
        [SampleData("5687456598")]
        [MaxLength(30)]
        public string prv_npi { get; set; }

        /// <summary>Provider Tax ID</summary>
        [DisplayName("Provider Tax ID")]
        [JsonProperty("prv_tin")]
        [SampleData("381882404")]
        [MaxLength(30)]
        public string prv_tin { get; set; }

        /// <summary>Provider Type Name; Institutional / Professional / Ancillary</summary>
        [DisplayName("Provider Type Name; Institutional / Professional / Ancillary")]
        [JsonProperty("prv_type_desc")]
        [SampleData("Institutional")]
        [MaxLength(70)]
        public string prv_type_desc { get; set; }

        /// <summary>First Name of provider</summary>
        [DisplayName("First Name of provider")]
        [JsonProperty("prv_first_name")]
        [SampleData("Dilli")]
        [MaxLength(100)]
        public string prv_first_name { get; set; }

        /// <summary>Middle name of provider</summary>
        [DisplayName("Middle name of provider")]
        [JsonProperty("prv_middle_name")]
        [SampleData("Raj")]
        [MaxLength(30)]
        public string prv_middle_name { get; set; }

        /// <summary>Last Name of provider</summary>
        [DisplayName("Last Name of provider")]
        [JsonProperty("prv_last_name")]
        [SampleData("Ghimire")]
        [MaxLength(40)]
        public string prv_last_name { get; set; }

        /// <summary>Gender of provider</summary>
        [DisplayName("Gender of provider")]
        [JsonProperty("prv_gender")]
        [SampleData("M")]
        [MaxLength(2)]
        public string prv_gender { get; set; }

        /// <summary>Provider  Native Language</summary>
        [DisplayName("Provider  Native Language")]
        [JsonProperty("prv_native_language")]
        [MaxLength(30)]
        public string prv_native_language { get; set; }

        /// <summary>Network Code Provider Paid Through</summary>
        [DisplayName("Network Code Provider Paid Through")]
        [JsonProperty("prv_network_code")]
        [SampleData("PPOM")]
        [MaxLength(10)]
        public string prv_network_code { get; set; }

        /// <summary>Network Name Provider Paid through</summary>
        [DisplayName("Network Name Provider Paid through")]
        [JsonProperty("prv_network_name")]
        [MaxLength(50)]
        public string prv_network_name { get; set; }

        /// <summary>Phone of Provider</summary>
        [DisplayName("Phone of Provider")]
        [JsonProperty("prv_phone")]
        [SampleData("7802222334")]
        [MaxLength(20)]
        public string prv_phone { get; set; }

        /// <summary>First Specialty of provider</summary>
        [DisplayName("First Specialty of provider")]
        [JsonProperty("prv_speciality_1_code")]
        [SampleData("1054")]
        [MaxLength(10)]
        public string prv_speciality_1_code { get; set; }

        /// <summary>First Specialty of provider</summary>
        [DisplayName("First Specialty of provider")]
        [JsonProperty("prv_Specialty_1_desc")]
        [SampleData("Radiology")]
        [MaxLength(100)]
        public string prv_Specialty_1_desc { get; set; }

        /// <summary>Second Specialty of provider</summary>
        [DisplayName("Second Specialty of provider")]
        [JsonProperty("prv_speciality_2_code")]
        [MaxLength(10)]
        public string prv_speciality_2_code { get; set; }

        /// <summary>Second Specialty of provider</summary>
        [DisplayName("Second Specialty of provider")]
        [JsonProperty("prv_Specialty_2_desc")]
        [MaxLength(100)]
        public string prv_Specialty_2_desc { get; set; }

        /// <summary>Third Specialty of provider</summary>
        [DisplayName("Third Specialty of provider")]
        [JsonProperty("prv_speciality_3_code")]
        [MaxLength(10)]
        public string prv_speciality_3_code { get; set; }

        /// <summary>Third Specialty of provider</summary>
        [DisplayName("Third Specialty of provider")]
        [JsonProperty("prv_Specialty_3_desc")]
        [MaxLength(100)]
        public string prv_Specialty_3_desc { get; set; }

        /// <summary>Provider first address line</summary>
        [DisplayName("Provider first address line")]
        [JsonProperty("prv_street_1")]
        [MaxLength(128)]
        public string prv_street_1 { get; set; }

        /// <summary>Provider second address line</summary>
        [DisplayName("Provider second address line")]
        [JsonProperty("prv_street_2")]
        [MaxLength(128)]
        public string prv_street_2 { get; set; }

        /// <summary>City of provider</summary>
        [DisplayName("City of provider")]
        [JsonProperty("prv_city")]
        [SampleData("Saginaw")]
        [MaxLength(32)]
        public string prv_city { get; set; }

        /// <summary>County of provider</summary>
        [DisplayName("County of provider")]
        [JsonProperty("prv_county")]
        [SampleData("Lexington")]
        [MaxLength(32)]
        public string prv_county { get; set; }

        /// <summary>Provider State</summary>
        [DisplayName("Provider State")]
        [JsonProperty("prv_state")]
        [SampleData("MA")]
        [MaxLength(2)]
        public string prv_state { get; set; }

        /// <summary>Zip code of provider</summary>
        [DisplayName("Zip code of provider")]
        [JsonProperty("prv_zip")]
        [SampleData("2420")]
        [MaxLength(10)]
        public string prv_zip { get; set; }

        /// <summary>Identifies if Provider is - 0: in Network or 1: out of network</summary>
        [DisplayName("Identifies if Provider is - 0: in Network or 1: out of network")]
        [JsonProperty("prv_in_network_flag")]
        [SampleData("0")]
        [MaxLength(1)]
        public char prv_in_network_flag { get; set; }

        /// <summary>Primary Care Physician identification number</summary>
        [DisplayName("Primary Care Physician identification number")]
        [JsonProperty("prv_pcp_id")]
        [MaxLength(30)]
        public string prv_pcp_id { get; set; }

        /// <summary>Primary Care Physician First Name</summary>
        [DisplayName("Primary Care Physician First Name")]
        [JsonProperty("prv_pcp_first_name")]
        [SampleData("Meredith")]
        [MaxLength(100)]
        public string prv_pcp_first_name { get; set; }

        /// <summary>Primary Care Physician Middle Name</summary>
        [DisplayName("Primary Care Physician Middle Name")]
        [JsonProperty("prv_pcp_middle_name")]
        [MaxLength(30)]
        public string prv_pcp_middle_name { get; set; }

        /// <summary>Primary Care Physician Last Name</summary>
        [DisplayName("Primary Care Physician Last Name")]
        [JsonProperty("prv_pcp_last_name")]
        [SampleData("Gray")]
        [MaxLength(50)]
        public string prv_pcp_last_name { get; set; }

        /// <summary>Place of Service code</summary>
        [DisplayName("Place of Service code")]
        [Required]
        [JsonProperty("svc_pos_code")]
        [SampleData("21")]
        [MaxLength(2)]
        public string svc_pos_code { get; set; }

        /// <summary>Place of Service description; from Master POS table. </summary>
        [DisplayName("Place of Service description; from Master POS table. ")]
        [JsonProperty("svc_pos_desc")]
        [SampleData("Inpatient")]
        [MaxLength(50)]
        public string svc_pos_desc { get; set; }

        /// <summary>Primary ICD</summary>
        [DisplayName("Primary ICD")]
        [Required]
        [JsonProperty("svc_diag_1_code")]
        [SampleData("272")]
        [MaxLength(8)]
        public string svc_diag_1_code { get; set; }

        /// <summary>Diagnosis Description; From master ICD9 table. For home grown codes, use client description.</summary>
        [DisplayName("Diagnosis Description; From master ICD9 table.For home grown codes, use client description.")]
        [JsonProperty("svc_diag_1_desc")]
        [MaxLength(100)]
        public string svc_diag_1_desc { get; set; }

        /// <summary>Secondary ICD</summary>
        [DisplayName("Secondary ICD")]
        [JsonProperty("svc_diag_2_code")]
        [SampleData("401.1")]
        [MaxLength(30)]
        public string svc_diag_2_code { get; set; }

        /// <summary>Diagnosis Description; From master ICD9 table. For home grown codes, use client description.</summary>
        [DisplayName("Diagnosis Description; From master ICD9 table.For home grown codes, use client description.")]
        [JsonProperty("svc_diag_2_desc")]
        [MaxLength(100)]
        public string svc_diag_2_desc { get; set; }

        /// <summary>Tertiary ICD</summary>
        [DisplayName("Tertiary ICD")]
        [JsonProperty("svc_diag_3_code")]
        [MaxLength(30)]
        public string svc_diag_3_code { get; set; }

        /// <summary>Diagnosis Description; From master ICD9 table. For home grown codes, use client description.</summary>
        [DisplayName("Diagnosis Description; From master ICD9 table.For home grown codes, use client description.")]
        [JsonProperty("svc_diag_3_desc")]
        [MaxLength(100)]
        public string svc_diag_3_desc { get; set; }

        /// <summary>4th ICD</summary>
        [DisplayName("4th ICD")]
        [JsonProperty("svc_diag_4_code")]
        [MaxLength(30)]
        public string svc_diag_4_code { get; set; }

        /// <summary>Diagnosis Description; From master ICD9 table. For home grown codes, use client description.</summary>
        [DisplayName("Diagnosis Description; From master ICD9 table.For home grown codes, use client description.")]
        [JsonProperty("svc_diag_4_desc")]
        [MaxLength(100)]
        public string svc_diag_4_desc { get; set; }

        /// <summary>5th ICD</summary>
        [DisplayName("5th ICD")]
        [JsonProperty("svc_diag_5_code")]
        [MaxLength(30)]
        public string svc_diag_5_code { get; set; }

        /// <summary>Diagnosis Description; From master ICD9 table. For home grown codes, use client description.</summary>
        [DisplayName("Diagnosis Description; From master ICD9 table.For home grown codes, use client description.")]
        [JsonProperty("svc_diag_5_desc")]
        [MaxLength(100)]
        public string svc_diag_5_desc { get; set; }

        /// <summary>6th ICD</summary>
        [DisplayName("6th ICD")]
        [JsonProperty("svc_diag_6_code")]
        [MaxLength(30)]
        public string svc_diag_6_code { get; set; }

        /// <summary>Diagnosis Description; From master ICD9 table. For home grown codes, use client description.</summary>
        [DisplayName("Diagnosis Description; From master ICD9 table.For home grown codes, use client description.")]
        [JsonProperty("svc_diag_6_desc")]
        [MaxLength(100)]
        public string svc_diag_6_desc { get; set; }

        /// <summary>7th ICD</summary>
        [DisplayName("7th ICD")]
        [JsonProperty("svc_diag_7_code")]
        [MaxLength(30)]
        public string svc_diag_7_code { get; set; }

        /// <summary>Diagnosis Description; From master ICD9 table. For home grown codes, use client description.</summary>
        [DisplayName("Diagnosis Description; From master ICD9 table.For home grown codes, use client description.")]
        [JsonProperty("svc_diag_7_desc")]
        [MaxLength(100)]
        public string svc_diag_7_desc { get; set; }

        /// <summary>8th ICD</summary>
        [DisplayName("8th ICD")]
        [JsonProperty("svc_diag_8_code")]
        [MaxLength(30)]
        public string svc_diag_8_code { get; set; }

        /// <summary>Diagnosis Description; From master ICD9 table. For home grown codes, use client description.</summary>
        [DisplayName("Diagnosis Description; From master ICD9 table.For home grown codes, use client description.")]
        [JsonProperty("svc_diag_8_desc")]
        [MaxLength(100)]
        public string svc_diag_8_desc { get; set; }

        /// <summary>9th ICD</summary>
        [DisplayName("9th ICD")]
        [JsonProperty("svc_diag_9_code")]
        [MaxLength(30)]
        public string svc_diag_9_code { get; set; }

        /// <summary>Diagnosis Description; From master ICD9 table. For home grown codes, use client description.</summary>
        [DisplayName("Diagnosis Description; From master ICD9 table.For home grown codes, use client description.")]
        [JsonProperty("svc_diag_9_desc")]
        [MaxLength(100)]
        public string svc_diag_9_desc { get; set; }

        /// <summary>Procedure code type - CPT4, Revenue, HCPCS, DRG, RUG (Resource Utilization Group)</summary>
        [DisplayName("Procedure code type - CPT4, Revenue, HCPCS, DRG, RUG(Resource Utilization Group)")]
        [JsonProperty("svc_procedure_type")]
        [SampleData("HCPCS")]
        [MaxLength(10)]
        public string svc_procedure_type { get; set; }

        /// <summary>Procedure code; CPT, HCPCS, ICD, REV, DRG in order</summary>
        [DisplayName("Procedure code; CPT, HCPCS, ICD, REV, DRG in order")]
        [Required]
        [JsonProperty("svc_procedure_code")]
        [SampleData("G0107")]
        [MaxLength(10)]
        public string svc_procedure_code { get; set; }

        /// <summary>Procedure description; From master Procedure table</summary>
        [DisplayName("Procedure description; From master Procedure table")]
        [JsonProperty("svc_procedure_desc")]
        [SampleData("Fecal - Occult Blood Test")]
        [MaxLength(200)]
        public string svc_procedure_desc { get; set; }

        /// <summary>Revenue code </summary>
        [DisplayName("Revenue code ")]
        [JsonProperty("svc_rev_code")]
        [SampleData("R002")]
        [MaxLength(5)]
        public string svc_rev_code { get; set; }

        /// <summary>Revenue code description; From master procedure table</summary>
        [DisplayName("Revenue code description; From master procedure table")]
        [JsonProperty("svc_rev_desc")]
        [SampleData("Total Charge")]
        [MaxLength(100)]
        public string svc_rev_desc { get; set; }

        /// <summary>CPT code</summary>
        [DisplayName("CPT code")]
        [JsonProperty("svc_cpt_code")]
        [SampleData("100")]
        [MaxLength(5)]
        public string svc_cpt_code { get; set; }

        /// <summary>CPT code description; From master procedure table</summary>
        [DisplayName("CPT code description; From master procedure table")]
        [JsonProperty("svc_cpt_desc")]
        [SampleData("Anes - Salivary Glands InclBx")]
        [MaxLength(100)]
        public string svc_cpt_desc { get; set; }

        /// <summary>First ICD procedure code</summary>
        [DisplayName("First ICD procedure code")]
        [JsonProperty("svc_icd_proc_1_code")]
        [SampleData("9432")]
        [MaxLength(10)]
        public string svc_icd_proc_1_code { get; set; }

        /// <summary>First ICD procedure description</summary>
        [DisplayName("First ICD procedure description")]
        [JsonProperty("svc_icd_proc_1_desc")]
        [SampleData("Hypnotherapy")]
        [MaxLength(100)]
        public string svc_icd_proc_1_desc { get; set; }

        /// <summary>Second ICD procedure code</summary>
        [DisplayName("Second ICD procedure code")]
        [JsonProperty("svc_icd_proc_2_code")]
        [MaxLength(5)]
        public string svc_icd_proc_2_code { get; set; }

        /// <summary>Second ICD procedure description</summary>
        [DisplayName("Second ICD procedure description")]
        [JsonProperty("svc_icd_proc_2_desc")]
        [MaxLength(100)]
        public string svc_icd_proc_2_desc { get; set; }

        /// <summary>DRG Type Code</summary>
        [DisplayName("DRG Type Code")]
        [JsonProperty("svc_drg_type_code")]
        [SampleData("1")]
        [MaxLength(10)]
        public string svc_drg_type_code { get; set; }

        /// <summary>DRG Type Description</summary>
        [DisplayName("DRG Type Description")]
        [JsonProperty("svc_drg_type_Desc")]
        [SampleData("MS - DRG, DRG")]
        [MaxLength(100)]
        public string svc_drg_type_Desc { get; set; }

        /// <summary>Diagnosis related group code</summary>
        [DisplayName("Diagnosis related group code")]
        [JsonProperty("svc_drg_code")]
        [SampleData("1")]
        [MaxLength(7)]
        public string svc_drg_code { get; set; }

        /// <summary>Diagnosis related group description</summary>
        [DisplayName("Diagnosis related group description")]
        [JsonProperty("svc_drg_desc")]
        [SampleData("HEART TRANSPLANT OR IMPLANT OF HEART ASSIST SYSTEM W MCC")]
        [MaxLength(100)]
        public string svc_drg_desc { get; set; }

        /// <summary>HCPCS code</summary>
        [DisplayName("HCPCS code")]
        [JsonProperty("svc_hcpcs_code")]
        [SampleData("G0107")]
        [MaxLength(5)]
        public string svc_hcpcs_code { get; set; }

        /// <summary>HCPCS description</summary>
        [DisplayName("HCPCS description")]
        [JsonProperty("svc_hcpcs_desc")]
        [SampleData("Fecal - Occult Blood Test")]
        [MaxLength(100)]
        public string svc_hcpcs_desc { get; set; }

        /// <summary>CPT4 modifier code</summary>
        [DisplayName("CPT4 modifier code")]
        [JsonProperty("svc_modifier_code")]
        [SampleData("90")]
        [MaxLength(8)]
        public string svc_modifier_code { get; set; }

        /// <summary>CPT4 description</summary>
        [DisplayName("CPT4 description")]
        [JsonProperty("svc_modifier_desc")]
        [SampleData("Lab send out")]
        [MaxLength(100)]
        public string svc_modifier_desc { get; set; }

        /// <summary>modifier code</summary>
        [DisplayName("modifier code")]
        [JsonProperty("svc_modifier_2_code")]
        [MaxLength(8)]
        public string svc_modifier_2_code { get; set; }

        /// <summary>modifier description</summary>
        [DisplayName("modifier description")]
        [JsonProperty("svc_modifier_2_desc")]
        [MaxLength(100)]
        public string svc_modifier_2_desc { get; set; }

        /// <summary>modifier code</summary>
        [DisplayName("modifier code")]
        [JsonProperty("svc_modifier_3_code")]
        [MaxLength(8)]
        public string svc_modifier_3_code { get; set; }

        /// <summary>modifier description</summary>
        [DisplayName("modifier description")]
        [JsonProperty("svc_modifier_3_desc")]
        [MaxLength(100)]
        public string svc_modifier_3_desc { get; set; }

        /// <summary>Type of service code</summary>
        [DisplayName("Type of service code")]
        [JsonProperty("svc_tos_code")]
        [SampleData("85")]
        [MaxLength(5)]
        public string svc_tos_code { get; set; }

        /// <summary>Type of service description</summary>
        [DisplayName("Type of service description")]
        [JsonProperty("svc_tos_desc")]
        [MaxLength(100)]
        public string svc_tos_desc { get; set; }

        /// <summary>Type of discharge code</summary>
        [DisplayName("Type of discharge code")]
        [JsonProperty("svc_discharge_code")]
        [MaxLength(20)]
        public string svc_discharge_code { get; set; }

        /// <summary>Type of discharge description</summary>
        [DisplayName("Type of discharge description")]
        [JsonProperty("svc_discharge_desc")]
        [MaxLength(100)]
        public string svc_discharge_desc { get; set; }

        /// <summary>Service quantity</summary>
        [DisplayName("Service quantity")]
        [JsonProperty("svc_service_qty")]
        public int svc_service_qty { get; set; }

        /// <summary>Inpatient stay days</summary>
        [DisplayName("Inpatient stay days")]
        [JsonProperty("svc_ip_days")]
        [SampleData("12")]
        public int svc_ip_days { get; set; }

        /// <summary>IP days covered by the insurance</summary>
        [DisplayName("IP days covered by the insurance")]
        [JsonProperty("svc_covered_days")]
        [SampleData("3")]
        public int svc_covered_days { get; set; }

        /// <summary>Internal codes</summary>
        [DisplayName("Internal codes")]
        [JsonProperty("svc_admit_type")]
        [MaxLength(6)]
        public string svc_admit_type { get; set; }

        /// <summary>From date</summary>
        [DisplayName("From date")]
        [DataType(DataType.Date)]
        [Required]
        [JsonProperty("svc_service_frm_date")]
        [SampleData("39823")]
        public DateTime svc_service_frm_date { get; set; }

        /// <summary>To date / Thru date</summary>
        [DisplayName("To date / Thru date")]
        [DataType(DataType.Date)]
        [JsonProperty("svc_service_to_date")]
        [SampleData("40128")]
        public DateTime svc_service_to_date { get; set; }

        /// <summary>date the claim was adjudicated</summary>
        [DisplayName("date the claim was adjudicated")]
        [DataType(DataType.Date)]
        [JsonProperty("rev_adjudication_date")]
        [SampleData("40211")]
        public DateTime rev_adjudication_date { get; set; }

        /// <summary>date of payment</summary>
        [DisplayName("date of payment")]
        [DataType(DataType.Date)]
        [JsonProperty("rev_paid_date")]
        [SampleData("40239")]
        public DateTime rev_paid_date { get; set; }

        /// <summary>Benefit Code</summary>
        [DisplayName("Benefit Code")]
        [JsonProperty("svc_benefit_code")]
        [SampleData("105")]
        [MaxLength(10)]
        public string svc_benefit_code { get; set; }

        /// <summary>Benefit Code description</summary>
        [DisplayName("Benefit Code description")]
        [JsonProperty("svc_benefit_desc")]
        [SampleData("Emergency and Urgent Care Services")]
        [MaxLength(100)]
        public string svc_benefit_desc { get; set; }

        /// <summary>Amount allowed under contract</summary>
        [DisplayName("Amount allowed under contract")]
        [JsonProperty("rev_allowed_amt")]
        [SampleData("180")]
        //[MaxLength(19, 2)]
        public double rev_allowed_amt { get; set; }

        /// <summary>Gross charges</summary>
        [DisplayName("Gross charges")]
        [JsonProperty("rev_billed_amt")]
        [SampleData("100")]
        //[MaxLength(19, 2)]
        public double rev_billed_amt { get; set; }

        /// <summary>Coordination of benefits on the medical plan</summary>
        [DisplayName("Coordination of benefits on the medical plan")]
        [JsonProperty("rev_cob_paid_amt")]
        [SampleData("10")]
        //[MaxLength(19, 2)]
        public double rev_cob_paid_amt { get; set; }

        /// <summary>Coinsurance due from patient</summary>
        [DisplayName("Coinsurance due from patient")]
        [JsonProperty("rev_coinsurance_amt")]
        [SampleData("5")]
        //[MaxLength(19, 2)]
        public double rev_coinsurance_amt { get; set; }

        /// <summary>Amount collected from the patient as a co-payment.</summary>
        [DisplayName("Amount collected from the patient as a co - payment.")]
        [JsonProperty("rev_copay_amt")]
        [SampleData("5")]
        //[MaxLength(19, 2)]
        public double rev_copay_amt { get; set; }

        /// <summary>Network usage charge</summary>
        [DisplayName("Network usage charge")]
        [JsonProperty("rev_coverage_charge_amt")]
        [SampleData("30")]
        //[MaxLength(19, 2)]
        public double rev_coverage_charge_amt { get; set; }

        /// <summary>Deductible Portion of the Allowed Amount </summary>
        [DisplayName("Deductible Portion of the Allowed Amount ")]
        [JsonProperty("rev_deductible_amt")]
        [SampleData("5")]
        //[MaxLength(19, 2)]
        public double rev_deductible_amt { get; set; }

        /// <summary>Billed Charges not covered under the Member policy</summary>
        [DisplayName("Billed Charges not covered under the Member policy")]
        [JsonProperty("rev_not_covered_amt")]
        [SampleData("30")]
        //[MaxLength(19, 2)]
        public double rev_not_covered_amt { get; set; }

        /// <summary>Other Savings generated</summary>
        [DisplayName("Other Savings generated")]
        [JsonProperty("rev_other_savings")]
        [SampleData("10")]
        //[MaxLength(19, 2)]
        public double rev_other_savings { get; set; }

        /// <summary>PPO Savings</summary>
        [DisplayName("PPO Savings")]
        [JsonProperty("rev_ppo_savings")]
        [SampleData("10")]
        //[MaxLength(19, 2)]
        public double rev_ppo_savings { get; set; }

        /// <summary>Amount paid</summary>
        [DisplayName("Amount paid")]
        [Required]
        [JsonProperty("rev_paid_amt")]
        [SampleData("300")]
        //[MaxLength(19, 2)]
        public double rev_paid_amt { get; set; }

        /// <summary>Fee for service vs Capitated (FFS or CAP)</summary>
        [DisplayName("Fee for service vs Capitated(FFS or CAP)")]
        [JsonProperty("rev_pay_type")]
        [MaxLength(4)]
        public string rev_pay_type { get; set; }

        /// <summary>Insurance check number</summary>
        [DisplayName("Insurance check number")]
        [JsonProperty("rev_check_num")]
        [MaxLength(20)]
        public string rev_check_num { get; set; }

        /// <summary>Authorization Number from Insurance Company</summary>
        [DisplayName("Authorization Number from Insurance Company")]
        [JsonProperty("svc_pre_authorization")]
        [MaxLength(50)]
        public string svc_pre_authorization { get; set; }

        /// <summary>Adjudication code</summary>
        [DisplayName("Adjudication code")]
        [JsonProperty("rev_adjudication_code")]
        [SampleData("P")]
        [MaxLength(8)]
        public string rev_adjudication_code { get; set; }

        /// <summary>Adjudication description</summary>
        [DisplayName("Adjudication description")]
        [JsonProperty("rev_adjudication_desc")]
        [SampleData("Paid")]
        [MaxLength(50)]
        public string rev_adjudication_desc { get; set; }

        /// <summary>Patient Number issued by Provider</summary>
        [DisplayName("Patient Number issued by Provider")]
        [JsonProperty("mbr_mrn")]
        [MaxLength(30)]
        public string mbr_mrn { get; set; }

        /// <summary>Health Insurance Claim Number to identify Medicare Patients</summary>
        [DisplayName("Health Insurance Claim Number to identify Medicare Patients")]
        [JsonProperty("mbr_hicn")]
        [MaxLength(11)]
        public string mbr_hicn { get; set; }

        /// <summary>Type of Bill.</summary>
        [DisplayName("Type of Bill.")]
        [JsonProperty("rev_bill_type_code")]
        [SampleData("110")]
        [MaxLength(3)]
        public string rev_bill_type_code { get; set; }

        /// <summary>Description out of master table for Bill type</summary>
        [DisplayName("Description out of master table for Bill type")]
        [JsonProperty("rev_bill_type_desc")]
        [MaxLength(100)]
        public string rev_bill_type_desc { get; set; }

        /// <summary>User Defined Field 1</summary>
        [DisplayName("User Defined Field 1")]
        [JsonProperty("udf1")]
        [MaxLength(100)]
        public string udf1 { get; set; }

        /// <summary>User Defined Field 2</summary>
        [DisplayName("User Defined Field 2")]
        [JsonProperty("udf2")]
        [MaxLength(100)]
        public string udf2 { get; set; }

        /// <summary>User Defined Field 3</summary>
        [DisplayName("User Defined Field 3")]
        [JsonProperty("udf3")]
        [MaxLength(100)]
        public string udf3 { get; set; }

        /// <summary>User Defined Field 4</summary>
        [DisplayName("User Defined Field 4")]
        [JsonProperty("udf4")]
        [MaxLength(100)]
        public string udf4 { get; set; }

        /// <summary>User Defined Field 5</summary>
        [DisplayName("User Defined Field 5")]
        [JsonProperty("udf5")]
        [MaxLength(100)]
        public string udf5 { get; set; }

        /// <summary>User Defined Field 6</summary>
        [DisplayName("User Defined Field 6")]
        [JsonProperty("udf6")]
        [MaxLength(100)]
        public string udf6 { get; set; }

        /// <summary>User Defined Field 7</summary>
        [DisplayName("User Defined Field 7")]
        [JsonProperty("udf7")]
        [MaxLength(100)]
        public string udf7 { get; set; }

        /// <summary>User Defined Field 8</summary>
        [DisplayName("User Defined Field 8")]
        [JsonProperty("udf8")]
        [MaxLength(100)]
        public string udf8 { get; set; }

        /// <summary>User Defined Field 9</summary>
        [DisplayName("User Defined Field 9")]
        [JsonProperty("udf9")]
        [MaxLength(100)]
        public string udf9 { get; set; }

        /// <summary>User Defined Field 10</summary>
        [DisplayName("User Defined Field 10")]
        [JsonProperty("udf10")]
        [MaxLength(100)]
        public string udf10 { get; set; }

        /// <summary>User Defined Field 11</summary>
        [DisplayName("User Defined Field 11")]
        [JsonProperty("udf11")]
        [MaxLength(100)]
        public string udf11 { get; set; }

        /// <summary>User Defined Field 12</summary>
        [DisplayName("User Defined Field 12")]
        [JsonProperty("udf12")]
        [MaxLength(100)]
        public string udf12 { get; set; }

        /// <summary>User Defined Field 13</summary>
        [DisplayName("User Defined Field 13")]
        [JsonProperty("udf13")]
        [MaxLength(100)]
        public string udf13 { get; set; }

        /// <summary>User Defined Field 14</summary>
        [DisplayName("User Defined Field 14")]
        [JsonProperty("udf14")]
        [MaxLength(100)]
        public string udf14 { get; set; }

        /// <summary>User Defined Field 15</summary>
        [DisplayName("User Defined Field 15")]
        [JsonProperty("udf15")]
        [MaxLength(100)]
        public string udf15 { get; set; }

        /// <summary>User Defined Field 16</summary>
        [DisplayName("User Defined Field 16")]
        [JsonProperty("udf16")]
        [MaxLength(100)]
        public string udf16 { get; set; }

        /// <summary>User Defined Field 17</summary>
        [DisplayName("User Defined Field 17")]
        [JsonProperty("udf17")]
        [MaxLength(100)]
        public string udf17 { get; set; }

        /// <summary>User Defined Field 18</summary>
        [DisplayName("User Defined Field 18")]
        [JsonProperty("udf18")]
        [MaxLength(100)]
        public string udf18 { get; set; }

        /// <summary>User Defined Field 19</summary>
        [DisplayName("User Defined Field 19")]
        [JsonProperty("udf19")]
        [MaxLength(100)]
        public string udf19 { get; set; }

        /// <summary>User Defined Field 20</summary>
        [DisplayName("User Defined Field 20")]
        [JsonProperty("udf20")]
        [MaxLength(100)]
        public string udf20 { get; set; }

        [JsonProperty("dw_vendor_name")]
        [MaxLength(20)]
        public string dw_vendor_name { get; set; }

        [JsonProperty("dw_admrule")]
        [MaxLength(6)]
        public string dw_admrule { get; set; }

        [JsonProperty("proc1_grouper_id")]
        [MaxLength(100)]
        public string proc1_grouper_id { get; set; }

        [JsonProperty("proc1_grouper_desc")]
        [MaxLength(100)]
        public string proc1_grouper_desc { get; set; }

        [JsonProperty("proc1_Subgrouper_id")]
        [MaxLength(100)]
        public string proc1_Subgrouper_id { get; set; }

        [JsonProperty("proc1_Subgrouper_desc")]
        [MaxLength(100)]
        public string proc1_Subgrouper_desc { get; set; }

        [JsonProperty("rev_grouper_id")]
        [MaxLength(100)]
        public string rev_grouper_id { get; set; }

        [JsonProperty("rev_grouper_desc")]
        [MaxLength(100)]
        public string rev_grouper_desc { get; set; }

        [JsonProperty("rev_subgrouper_id")]
        [MaxLength(100)]
        public string rev_subgrouper_id { get; set; }

        [JsonProperty("rev_subgrouper_desc")]
        [MaxLength(100)]
        public string rev_subgrouper_desc { get; set; }

        [JsonProperty("cpt_grouper_id")]
        [MaxLength(100)]
        public string cpt_grouper_id { get; set; }

        [JsonProperty("cpt_grouper_desc")]
        [MaxLength(100)]
        public string cpt_grouper_desc { get; set; }

        [JsonProperty("cpt_subgrouper_id")]
        [MaxLength(100)]
        public string cpt_subgrouper_id { get; set; }

        [JsonProperty("cpt_subgrouper_desc")]
        [MaxLength(100)]
        public string cpt_subgrouper_desc { get; set; }

        [JsonProperty("icd1_grouper_id")]
        [MaxLength(100)]
        public string icd1_grouper_id { get; set; }

        [JsonProperty("icd1_grouper_desc")]
        [MaxLength(100)]
        public string icd1_grouper_desc { get; set; }

        [JsonProperty("icd1_subgrouper_id")]
        [MaxLength(100)]
        public string icd1_subgrouper_id { get; set; }

        [JsonProperty("icd1_subgrouper_desc")]
        [MaxLength(100)]
        public string icd1_subgrouper_desc { get; set; }

        [JsonProperty("icd2_grouper_id")]
        [MaxLength(100)]
        public string icd2_grouper_id { get; set; }

        [JsonProperty("icd2_grouper_desc")]
        [MaxLength(100)]
        public string icd2_grouper_desc { get; set; }

        [JsonProperty("icd2_subgrouper_id")]
        [MaxLength(100)]
        public string icd2_subgrouper_id { get; set; }

        [JsonProperty("icd2_subgrouper_desc")]
        [MaxLength(100)]
        public string icd2_subgrouper_desc { get; set; }

        [JsonProperty("drg_grouper_id")]
        [MaxLength(100)]
        public string drg_grouper_id { get; set; }

        [JsonProperty("drg_grouper_desc")]
        [MaxLength(100)]
        public string drg_grouper_desc { get; set; }

        [JsonProperty("drg_subgrouper_id")]
        [MaxLength(100)]
        public string drg_subgrouper_id { get; set; }

        [JsonProperty("drg_subgrouper_desc")]
        [MaxLength(100)]
        public string drg_subgrouper_desc { get; set; }

        [JsonProperty("hcpcs_grouper_id")]
        [MaxLength(100)]
        public string hcpcs_grouper_id { get; set; }

        [JsonProperty("hcpcs_grouper_desc")]
        [MaxLength(100)]
        public string hcpcs_grouper_desc { get; set; }

        [JsonProperty("hcpcs_subgrouper_id")]
        [MaxLength(100)]
        public string hcpcs_subgrouper_id { get; set; }

        [JsonProperty("hcpcs_subgrouper_desc")]
        [MaxLength(100)]
        public string hcpcs_subgrouper_desc { get; set; }

        [JsonProperty("diag1_grouper_id")]
        [MaxLength(100)]
        public string diag1_grouper_id { get; set; }

        [JsonProperty("diag1_grouper_desc")]
        [MaxLength(100)]
        public string diag1_grouper_desc { get; set; }

        [JsonProperty("diag1_supergrouper_id")]
        [MaxLength(100)]
        public string diag1_supergrouper_id { get; set; }

        [JsonProperty("diag1_supergrouper_desc")]
        [MaxLength(100)]
        public string diag1_supergrouper_desc { get; set; }

        [JsonProperty("diag2_grouper_id")]
        [MaxLength(100)]
        public string diag2_grouper_id { get; set; }

        [JsonProperty("diag2_grouper_desc")]
        [MaxLength(100)]
        public string diag2_grouper_desc { get; set; }

        [JsonProperty("diag2_supergrouper_id")]
        [MaxLength(100)]
        public string diag2_supergrouper_id { get; set; }

        [JsonProperty("diag2_supergrouper_desc")]
        [MaxLength(100)]
        public string diag2_supergrouper_desc { get; set; }

        [JsonProperty("diag3_grouper_id")]
        [MaxLength(100)]
        public string diag3_grouper_id { get; set; }

        [JsonProperty("diag3_grouper_desc")]
        [MaxLength(100)]
        public string diag3_grouper_desc { get; set; }

        [JsonProperty("diag3_supergrouper_id")]
        [MaxLength(100)]
        public string diag3_supergrouper_id { get; set; }

        [JsonProperty("diag3_supergrouper_desc")]
        [MaxLength(100)]
        public string diag3_supergrouper_desc { get; set; }

        [JsonProperty("diag4_grouper_id")]
        [MaxLength(100)]
        public string diag4_grouper_id { get; set; }

        [JsonProperty("diag4_grouper_desc")]
        [MaxLength(100)]
        public string diag4_grouper_desc { get; set; }

        [JsonProperty("diag4_supergrouper_id")]
        [MaxLength(100)]
        public string diag4_supergrouper_id { get; set; }

        [JsonProperty("diag4_supergrouper_desc")]
        [MaxLength(100)]
        public string diag4_supergrouper_desc { get; set; }

        [JsonProperty("diag5_grouper_id")]
        [MaxLength(100)]
        public string diag5_grouper_id { get; set; }

        [JsonProperty("diag5_grouper_desc")]
        [MaxLength(100)]
        public string diag5_grouper_desc { get; set; }

        [JsonProperty("diag5_supergrouper_id")]
        [MaxLength(100)]
        public string diag5_supergrouper_id { get; set; }

        [JsonProperty("diag5_supergrouper_desc")]
        [MaxLength(100)]
        public string diag5_supergrouper_desc { get; set; }

        [JsonProperty("diag6_grouper_id")]
        [MaxLength(100)]
        public string diag6_grouper_id { get; set; }

        [JsonProperty("diag6_grouper_desc")]
        [MaxLength(100)]
        public string diag6_grouper_desc { get; set; }

        [JsonProperty("diag6_supergrouper_id")]
        [MaxLength(100)]
        public string diag6_supergrouper_id { get; set; }

        [JsonProperty("diag6_supergrouper_desc")]
        [MaxLength(100)]
        public string diag6_supergrouper_desc { get; set; }

        [JsonProperty("diag7_grouper_id")]
        [MaxLength(100)]
        public string diag7_grouper_id { get; set; }

        [JsonProperty("diag7_grouper_desc")]
        [MaxLength(100)]
        public string diag7_grouper_desc { get; set; }

        [JsonProperty("diag7_supergrouper_id")]
        [MaxLength(100)]
        public string diag7_supergrouper_id { get; set; }

        [JsonProperty("diag7_supergrouper_desc")]
        [MaxLength(100)]
        public string diag7_supergrouper_desc { get; set; }

        [JsonProperty("diag8_grouper_id")]
        [MaxLength(100)]
        public string diag8_grouper_id { get; set; }

        [JsonProperty("diag8_grouper_desc")]
        [MaxLength(100)]
        public string diag8_grouper_desc { get; set; }

        [JsonProperty("diag8_supergrouper_id")]
        [MaxLength(100)]
        public string diag8_supergrouper_id { get; set; }

        [JsonProperty("diag8_supergrouper_desc")]
        [MaxLength(100)]
        public string diag8_supergrouper_desc { get; set; }

        [JsonProperty("diag9_grouper_id")]
        [MaxLength(100)]
        public string diag9_grouper_id { get; set; }

        [JsonProperty("diag9_grouper_desc")]
        [MaxLength(100)]
        public string diag9_grouper_desc { get; set; }

        [JsonProperty("diag9_supergrouper_id")]
        [MaxLength(100)]
        public string diag9_supergrouper_id { get; set; }

        [JsonProperty("diag9_supergrouper_desc")]
        [MaxLength(100)]
        public string diag9_supergrouper_desc { get; set; }

        [JsonProperty("cpt_betos")]
        [MaxLength(10)]
        public string cpt_betos { get; set; }

        [JsonProperty("cpt_betos_grouper")]
        [MaxLength(100)]
        public string cpt_betos_grouper { get; set; }

        [JsonProperty("cpt_betos_sub_grouper")]
        [MaxLength(100)]
        public string cpt_betos_sub_grouper { get; set; }

        [JsonProperty("hcpcs_betos")]
        [MaxLength(100)]
        public string hcpcs_betos { get; set; }

        [JsonProperty("hcpcs_betos_grouper")]
        [MaxLength(100)]
        public string hcpcs_betos_grouper { get; set; }

        [JsonProperty("hcpcs_betos_sub_grouper")]
        [MaxLength(100)]
        public string hcpcs_betos_sub_grouper { get; set; }

        [JsonProperty("rev_betos")]
        [MaxLength(100)]
        public string rev_betos { get; set; }

        [JsonProperty("rev_betos_grouper")]
        [MaxLength(100)]
        public string rev_betos_grouper { get; set; }

        [JsonProperty("rev_betos_sub_grouper")]
        [MaxLength(100)]
        public string rev_betos_sub_grouper { get; set; }

        [JsonProperty("icd1_betos")]
        [MaxLength(100)]
        public string icd1_betos { get; set; }

        [JsonProperty("icd1_betos_grouper")]
        [MaxLength(100)]
        public string icd1_betos_grouper { get; set; }

        [JsonProperty("icd1_betos_sub_grouper")]
        [MaxLength(100)]
        public string icd1_betos_sub_grouper { get; set; }

        [JsonProperty("icd2_betos")]
        [MaxLength(100)]
        public string icd2_betos { get; set; }

        [JsonProperty("icd2_betos_grouper")]
        [MaxLength(100)]
        public string icd2_betos_grouper { get; set; }

        [JsonProperty("icd2_betos_sub_grouper")]
        [MaxLength(100)]
        public string icd2_betos_sub_grouper { get; set; }

        [JsonProperty("drg_betos")]
        [MaxLength(100)]
        public string drg_betos { get; set; }

        [JsonProperty("drg_betos_grouper")]
        [MaxLength(100)]
        public string drg_betos_grouper { get; set; }

        [JsonProperty("drg_betos_sub_grouper")]
        [MaxLength(100)]
        public string drg_betos_sub_grouper { get; set; }

        [JsonProperty("proc7_betos")]
        [MaxLength(100)]
        public string proc7_betos { get; set; }

        [JsonProperty("proc7_betos_grouper")]
        [MaxLength(100)]
        public string proc7_betos_grouper { get; set; }

        [JsonProperty("proc7_betos_sub_grouper")]
        [MaxLength(100)]
        public string proc7_betos_sub_grouper { get; set; }

        [DataType(DataType.Date)]
        [JsonProperty("dw_creation_date")]
        public DateTime dw_creation_date { get; set; }

        [DataType(DataType.Date)]
        [JsonProperty("dw_update_date")]
        public DateTime dw_update_date { get; set; }

        [JsonProperty("dw_rawfilename")]
        [MaxLength(255)]
        public string dw_rawfilename { get; set; }

        [JsonProperty("dw_recievedmonth")]
        [MaxLength(10)]
        public string dw_recievedmonth { get; set; }

        /// <summary>To map with visit table (dw_record_id)</summary>
        [DisplayName("To map with visit table(dw_record_id)")]
        [JsonProperty("visit_id")]
        [SampleData("17")]
        public int visit_id { get; set; }

        /// <summary>Member ID</summary>
        [DisplayName("Member ID")]
        [JsonProperty("dw_member_id")]
        [SampleData("Hash Encrypted")]
        [MaxLength(50)]
        public string dw_member_id { get; set; }

        /// <summary>Boolean Field</summary>
        [DisplayName("Boolean Field")]
        [JsonProperty("is_makalu_used")]
        [SampleData("True for Non - EM members and False for EM members")]
        [MaxLength(20)]
        public string is_makalu_used { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf21")]
        [MaxLength(100)]
        public string udf21 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf22")]
        [MaxLength(100)]
        public string udf22 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf23")]
        [MaxLength(100)]
        public string udf23 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf24")]
        [MaxLength(100)]
        public string udf24 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf25")]
        [MaxLength(100)]
        public string udf25 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf26")]
        [MaxLength(100)]
        public string udf26 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf27")]
        [MaxLength(100)]
        public string udf27 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf28")]
        [MaxLength(100)]
        public string udf28 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf29")]
        [MaxLength(100)]
        public string udf29 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf30")]
        [MaxLength(100)]
        public string udf30 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf31")]
        [MaxLength(100)]
        public string udf31 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf32")]
        [MaxLength(100)]
        public string udf32 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf33")]
        [MaxLength(100)]
        public string udf33 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf34")]
        [MaxLength(100)]
        public string udf34 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf35")]
        [MaxLength(100)]
        public string udf35 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf36")]
        [MaxLength(100)]
        public string udf36 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf37")]
        [MaxLength(100)]
        public string udf37 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf38")]
        [MaxLength(100)]
        public string udf38 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf39")]
        [MaxLength(100)]
        public string udf39 { get; set; }

        /// <summary>User Defined Field</summary>
        [DisplayName("User Defined Field")]
        [JsonProperty("udf40")]
        [MaxLength(100)]
        public string udf40 { get; set; }


    }

    [DocumentCollection(CrmDdbContext.DatabaseName, "Demographics")]
    public partial class Demographic : DeerwalkDdbEntity
    {
        public static readonly Demographic[] None = new Demographic[0];

        /// <summary>Auto-increment number-a unique identifier for Makalu engine</summary>
        [DisplayName("Auto - increment number - a unique identifier for Makalu engine")]
        [JsonProperty("dw_record_id")]
        [SampleData("1")]
        public int dw_record_id { get; set; }

        /// <summary>Account id</summary>
        [DisplayName("Account id")]
        [JsonProperty("dw_account_id")]
        [SampleData("1027")]
        [MaxLength(10)]
        public string dw_account_id { get; set; }

        /// <summary>Clientid</summary>
        [DisplayName("Clientid")]
        [JsonProperty("dw_client_id")]
        [SampleData("1")]
        [MaxLength(10)]
        public string dw_client_id { get; set; }

        /// <summary>Member ID</summary>
        [DisplayName("Member ID")]
        [JsonProperty("dw_member_id")]
        [SampleData("Hash Encrypted")]
        [MaxLength(50)]
        public string dw_member_id { get; set; }

        /// <summary>Member ID to display on the application, as sent by client</summary>
        [DisplayName("Member ID to display on the application, as sent by client")]
        [Required]
        [JsonProperty("mbr_id")]
        [SampleData("9916897")]
        [MaxLength(20)]
        public string mbr_id { get; set; }

        /// <summary>Member SSN</summary>
        [DisplayName("Member SSN")]
        [JsonProperty("mbr_ssn")]
        [SampleData("811619")]
        [MaxLength(20)]
        public string mbr_ssn { get; set; }

        /// <summary>Member first name</summary>
        [DisplayName("Member first name")]
        [JsonProperty("mbr_first_name")]
        [SampleData("BEVERLY")]
        [MaxLength(100)]
        public string mbr_first_name { get; set; }

        /// <summary>Member middle name</summary>
        [DisplayName("Member middle name")]
        [JsonProperty("mbr_middle_name")]
        [SampleData("George")]
        [MaxLength(100)]
        public string mbr_middle_name { get; set; }

        /// <summary>Member last name</summary>
        [DisplayName("Member last name")]
        [JsonProperty("mbr_last_name")]
        [SampleData("BARRETT")]
        [MaxLength(100)]
        public string mbr_last_name { get; set; }

        /// <summary> Current status of member</summary>
        [DisplayName(" Current status of member")]
        [JsonProperty("mbr_current_status")]
        [SampleData("active")]
        [MaxLength(10)]
        public string mbr_current_status { get; set; }

        /// <summary>Member gender</summary>
        [DisplayName("Member gender")]
        [Required]
        [JsonProperty("mbr_gender")]
        [SampleData("M")]
        [MaxLength(10)]
        public string mbr_gender { get; set; }

        /// <summary>Member date of Birth</summary>
        [DisplayName("Member date of Birth")]
        [DataType(DataType.Date)]
        [Required]
        [JsonProperty("mbr_dob")]
        [SampleData("31597")]
        public DateTime mbr_dob { get; set; }

        /// <summary>Member Street Address 1</summary>
        [DisplayName("Member Street Address 1")]
        [JsonProperty("mbr_street_1")]
        [SampleData("5621 TEAKWOOD ROAD")]
        [MaxLength(100)]
        public string mbr_street_1 { get; set; }

        /// <summary>Member Street Address 2</summary>
        [DisplayName("Member Street Address 2")]
        [JsonProperty("mbr_street_2")]
        [MaxLength(100)]
        public string mbr_street_2 { get; set; }

        /// <summary>Member City</summary>
        [DisplayName("Member City")]
        [JsonProperty("mbr_city")]
        [SampleData("Lakeworth")]
        [MaxLength(100)]
        public string mbr_city { get; set; }

        /// <summary>Member County</summary>
        [DisplayName("Member County")]
        [JsonProperty("mbr_county")]
        [SampleData("Lexington")]
        [MaxLength(100)]
        public string mbr_county { get; set; }

        /// <summary>Abbreviation of State</summary>
        [DisplayName("Abbreviation of State")]
        [JsonProperty("mbr_state")]
        [SampleData("FL")]
        [MaxLength(100)]
        public string mbr_state { get; set; }

        /// <summary>Zip code</summary>
        [DisplayName("Zip code")]
        [JsonProperty("mbr_zip")]
        [SampleData("34746")]
        [MaxLength(100)]
        public string mbr_zip { get; set; }

        /// <summary>Member Phone</summary>
        [DisplayName("Member Phone")]
        [JsonProperty("mbr_phone")]
        [SampleData("7802966511")]
        [MaxLength(100)]
        public string mbr_phone { get; set; }

        /// <summary>Member Region code</summary>
        [DisplayName("Member Region code")]
        [JsonProperty("mbr_region_code")]
        [MaxLength(100)]
        public string mbr_region_code { get; set; }

        /// <summary>Member Region</summary>
        [DisplayName("Member Region")]
        [JsonProperty("mbr_region_name")]
        [MaxLength(100)]
        public string mbr_region_name { get; set; }

        /// <summary>Relationship Code to the Subscriber; subscriber(01), spouse (02),child (03), other (04)</summary>
        [DisplayName("Relationship Code to the Subscriber; subscriber(01), spouse(02), child(03), other(04)")]
        [JsonProperty("mbr_relationship_code")]
        [MaxLength(10)]
        public string mbr_relationship_code { get; set; }

        /// <summary>Relationship Description to the Subscriber, Dependent, Spouse</summary>
        [DisplayName("Relationship Description to the Subscriber, Dependent, Spouse")]
        [JsonProperty("mbr_relationship_desc")]
        [SampleData("Dependent")]
        [MaxLength(50)]
        public string mbr_relationship_desc { get; set; }

        /// <summary>Filename from vendor</summary>
        [DisplayName("Filename from vendor")]
        [JsonProperty("dw_rawfilename")]
        [MaxLength(100)]
        public string dw_rawfilename { get; set; }

        /// <summary>Month when data is recieved</summary>
        [DisplayName("Month when data is recieved")]
        [JsonProperty("dw_recievedmonth")]
        [SampleData("201106")]
        [MaxLength(100)]
        public string dw_recievedmonth { get; set; }

        /// <summary>Data Vendor Name</summary>
        [DisplayName("Data Vendor Name")]
        [JsonProperty("dw_vendor_name")]
        [MaxLength(100)]
        public string dw_vendor_name { get; set; }

    }

    [DocumentCollection(CrmDdbContext.DatabaseName, "Visit")]
    public partial class Visit : DeerwalkDdbEntity
    {
        public static readonly Visit[] None = new Visit[0];

        /// <summary>Auto-increment number-a unique identifier for Makalu engine</summary>
        [DisplayName("Auto - increment number - a unique identifier for Makalu engine")]
        [JsonProperty("dw_record_id")]
        [SampleData("1")]
        public int dw_record_id { get; set; }

        /// <summary>Account id</summary>
        [DisplayName("Account id")]
        [JsonProperty("dw_account_id")]
        [SampleData("1027")]
        [MaxLength(50)]
        public string dw_account_id { get; set; }

        /// <summary>Clientid</summary>
        [DisplayName("Clientid")]
        [JsonProperty("dw_client_id")]
        [SampleData("1")]
        [MaxLength(50)]
        public string dw_client_id { get; set; }

        /// <summary>Member ID</summary>
        [DisplayName("Member ID")]
        [JsonProperty("dw_member_id")]
        [SampleData("Hash Encrypted")]
        [MaxLength(50)]
        public string dw_member_id { get; set; }

        /// <summary>Member ID to display on the application, as sent by client</summary>
        [DisplayName("Member ID to display on the application, as sent by client")]
        [Required]
        [JsonProperty("mbr_id")]
        [SampleData("9916897")]
        [MaxLength(20)]
        public string mbr_id { get; set; }

        /// <summary>Where the visit was made</summary>
        [DisplayName("Where the visit was made")]
        [JsonProperty("mbr_visit_type")]
        [SampleData("ER, office etc.")]
        [MaxLength(50)]
        public string mbr_visit_type { get; set; }

        /// <summary>Date when the visit started</summary>
        [DisplayName("Date when the visit started")]
        [DataType(DataType.Date)]
        [JsonProperty("mbr_start_date")]
        public DateTime mbr_start_date { get; set; }

        /// <summary>Date when the visit ended</summary>
        [DisplayName("Date when the visit ended")]
        [DataType(DataType.Date)]
        [JsonProperty("mbr_end_date")]
        public DateTime mbr_end_date { get; set; }

        /// <summary>Units of the visit</summary>
        [DisplayName("Units of the visit")]
        [JsonProperty("value")]
        [SampleData("20")]
        [MaxLength(20)]
        public string value { get; set; }

        /// <summary>Admission type</summary>
        [DisplayName("Admission type")]
        [JsonProperty("admission_type")]
        [SampleData("Maternity, Medical")]
        [MaxLength(55)]
        public string admission_type { get; set; }

        /// <summary>Inpatient days</summary>
        [DisplayName("Inpatient days")]
        [JsonProperty("ip_days")]
        public double ip_days { get; set; }

        /// <summary>Yes or no on admissions from ER (Options : Y/N)</summary>
        [DisplayName("Yes or no on admissions from ER (Options : Y / N)")]
        [JsonProperty("admission_from_er")]
        [SampleData("Y")]
        [MaxLength(1)]
        public string admission_from_er { get; set; }

    }

    /// <summary>Beneficiary Provider --- Switching of a member from one provider to next and the next in different point of time. Also need to apply patient attribution logic.</summary>
    [DisplayName("Beneficiary Provider --- Switching of a member from one provider to next and the next in different point of time.Also need to apply patient attribution logic.")]
    [DocumentCollection(CrmDdbContext.DatabaseName, "MemberPCP")]
    public partial class MemberPCP : DeerwalkDdbEntity
    {
        public static readonly MemberPCP[] None = new MemberPCP[0];

        /// <summary>Auto-increment number-a unique identifier for Makalu engine</summary>
        [DisplayName("Auto - increment number - a unique identifier for Makalu engine")]
        [JsonProperty("dw_record_id")]
        [SampleData("1")]
        public int dw_record_id { get; set; }

        /// <summary>Account id</summary>
        [DisplayName("Account id")]
        [JsonProperty("dw_account_id")]
        [SampleData("1027")]
        [MaxLength(10)]
        public string dw_account_id { get; set; }

        /// <summary>Clientid</summary>
        [DisplayName("Clientid")]
        [JsonProperty("dw_client_id")]
        [SampleData("1")]
        [MaxLength(10)]
        public string dw_client_id { get; set; }

        /// <summary>Member ID</summary>
        [DisplayName("Member ID")]
        [JsonProperty("dw_member_id")]
        [SampleData("Hash Encrypted")]
        [MaxLength(50)]
        public string dw_member_id { get; set; }

        /// <summary>Member ID to display on the application, as sent by client</summary>
        [DisplayName("Member ID to display on the application, as sent by client")]
        [Required]
        [JsonProperty("mbr_id")]
        [SampleData("9916897")]
        [MaxLength(20)]
        public string mbr_id { get; set; }

        [JsonProperty("pcp_name")]
        [MaxLength(100)]
        public string pcp_name { get; set; }

        /// <summary>May be null</summary>
        [DisplayName("May be null")]
        [JsonProperty("pcp_npi")]
        [MaxLength(100)]
        public string pcp_npi { get; set; }

        /// <summary>May be null</summary>
        [DisplayName("May be null")]
        [DataType(DataType.Date)]
        [JsonProperty("start_date")]
        public DateTime start_date { get; set; }

        /// <summary>May be null</summary>
        [DisplayName("May be null")]
        [DataType(DataType.Date)]
        [JsonProperty("end_date")]
        public DateTime end_date { get; set; }

    }

    /// <summary>Milliman Advanced Risk Adjuster Scores</summary>
    [DisplayName("Milliman Advanced Risk Adjuster Scores")]
    [DocumentCollection(CrmDdbContext.DatabaseName, "Scores")]
    public partial class Score : DeerwalkDdbEntity
    {
        public static readonly Score[] None = new Score[0];

        /// <summary>Auto-increment number-a unique identifier for Makalu engine</summary>
        [DisplayName("Auto - increment number - a unique identifier for Makalu engine")]
        [JsonProperty("dw_record_id")]
        [SampleData("1")]
        public int dw_record_id { get; set; }

        /// <summary>Account id</summary>
        [DisplayName("Account id")]
        [JsonProperty("dw_account_id")]
        [SampleData("1027")]
        [MaxLength(50)]
        public string dw_account_id { get; set; }

        /// <summary>Clientid</summary>
        [DisplayName("Clientid")]
        [JsonProperty("dw_client_id")]
        [SampleData("1")]
        [MaxLength(50)]
        public string dw_client_id { get; set; }

        /// <summary>Member ID</summary>
        [DisplayName("Member ID")]
        [JsonProperty("dw_member_id")]
        [SampleData("Hash Encrypted")]
        [MaxLength(50)]
        public string dw_member_id { get; set; }

        /// <summary>Member ID to display on the application, as sent by client</summary>
        [DisplayName("Member ID to display on the application, as sent by client")]
        [Required]
        [JsonProperty("mbr_id")]
        [SampleData("9916897")]
        [MaxLength(20)]
        public string mbr_id { get; set; }

        /// <summary>Score scope </summary>
        [DisplayName("Score scope ")]
        [Required]
        [JsonProperty("score_type")]
        [SampleData("Group ID, ALL")]
        [MaxLength(50)]
        public string score_type { get; set; }

        /// <summary>Risk calculation start date</summary>
        [DisplayName("Risk calculation start date")]
        [DataType(DataType.Date)]
        [JsonProperty("score_start_date")]
        public DateTime score_start_date { get; set; }

        /// <summary>Risk calculation end  date</summary>
        [DisplayName("Risk calculation end  date")]
        [DataType(DataType.Date)]
        [JsonProperty("score_end_date")]
        public DateTime score_end_date { get; set; }

        [JsonProperty("ip_score")]
        [MaxLength(50)]
        public string ip_score { get; set; }

        [JsonProperty("op_score")]
        [MaxLength(50)]
        public string op_score { get; set; }

        [JsonProperty("phy_score")]
        [MaxLength(50)]
        public string phy_score { get; set; }

        [JsonProperty("rx_score")]
        [MaxLength(50)]
        public string rx_score { get; set; }

        /// <summary>IP+OP+PHY</summary>
        [DisplayName("IP + OP + PHY")]
        [JsonProperty("med_score")]
        [MaxLength(50)]
        public string med_score { get; set; }

        /// <summary>Med+Rx</summary>
        [DisplayName("Med + Rx")]
        [JsonProperty("total_score")]
        [MaxLength(50)]
        public string total_score { get; set; }

        [JsonProperty("concurrent_total")]
        [MaxLength(50)]
        public string concurrent_total { get; set; }

        [JsonProperty("erScore")]
        [MaxLength(50)]
        public string erScore { get; set; }

        [JsonProperty("otherScore")]
        [MaxLength(50)]
        public string otherScore { get; set; }

        [JsonProperty("concurrentInpatient")]
        [MaxLength(50)]
        public string concurrentInpatient { get; set; }

        [JsonProperty("concurrentMedical")]
        [MaxLength(50)]
        public string concurrentMedical { get; set; }

        [JsonProperty("concurrentOutpatient")]
        [MaxLength(50)]
        public string concurrentOutpatient { get; set; }

        [JsonProperty("concurrentPharmacy")]
        [MaxLength(50)]
        public string concurrentPharmacy { get; set; }

        [JsonProperty("concurrentPhysician")]
        [MaxLength(50)]
        public string concurrentPhysician { get; set; }

        [JsonProperty("concurrentIpNormalizedToGroup")]
        [MaxLength(50)]
        public string concurrentIpNormalizedToGroup { get; set; }

        [JsonProperty("concurrentOpNormalizedToGroup")]
        [MaxLength(50)]
        public string concurrentOpNormalizedToGroup { get; set; }

        [JsonProperty("concurrentPhyNormalizedToGroup")]
        [MaxLength(50)]
        public string concurrentPhyNormalizedToGroup { get; set; }


    }

    /// <summary>Milliman Advanced Risk Adjuster Scores</summary>
    [DisplayName("Milliman Advanced Risk Adjuster Scores")]
    [DocumentCollection(CrmDdbContext.DatabaseName, "HistoricalScores")]
    public partial class HistoricalScore : DeerwalkDdbEntity
    {
        public static readonly HistoricalScore[] None = new HistoricalScore[0];

        /// <summary>Auto-increment number-a unique identifier for Makalu engine</summary>
        [DisplayName("Auto - increment number - a unique identifier for Makalu engine")]
        [JsonProperty("dw_record_id")]
        [SampleData("1")]
        public int dw_record_id { get; set; }

        /// <summary>Account id</summary>
        [DisplayName("Account id")]
        [JsonProperty("dw_account_id")]
        [SampleData("1027")]
        [MaxLength(50)]
        public string dw_account_id { get; set; }

        /// <summary>Clientid</summary>
        [DisplayName("Clientid")]
        [JsonProperty("dw_client_id")]
        [SampleData("1")]
        [MaxLength(50)]
        public string dw_client_id { get; set; }

        /// <summary>Member ID</summary>
        [DisplayName("Member ID")]
        [JsonProperty("dw_member_id")]
        [SampleData("Hash Encrypted")]
        [MaxLength(50)]
        public string dw_member_id { get; set; }

        /// <summary>Member ID to display on the application, as sent by client</summary>
        [DisplayName("Member ID to display on the application, as sent by client")]
        [Required]
        [JsonProperty("mbr_id")]
        [SampleData("9916897")]
        [MaxLength(20)]
        public string mbr_id { get; set; }

        /// <summary>Score scope </summary>
        [DisplayName("Score scope ")]
        [Required]
        [JsonProperty("score_type")]
        [SampleData("Group ID, ALL")]
        [MaxLength(50)]
        public string score_type { get; set; }

        /// <summary>Risk calculation start date</summary>
        [DisplayName("Risk calculation start date")]
        [DataType(DataType.Date)]
        [JsonProperty("score_start_date")]
        public DateTime score_start_date { get; set; }

        /// <summary>Risk calculation end  date</summary>
        [DisplayName("Risk calculation end  date")]
        [DataType(DataType.Date)]
        [JsonProperty("score_end_date")]
        public DateTime score_end_date { get; set; }

        [JsonProperty("ip_score")]
        [MaxLength(50)]
        public string ip_score { get; set; }

        [JsonProperty("op_score")]
        [MaxLength(50)]
        public string op_score { get; set; }

        [JsonProperty("phy_score")]
        [MaxLength(50)]
        public string phy_score { get; set; }

        [JsonProperty("rx_score")]
        [MaxLength(50)]
        public string rx_score { get; set; }

        /// <summary>IP+OP+PHY</summary>
        [DisplayName("IP + OP + PHY")]
        [JsonProperty("med_score")]
        [MaxLength(50)]
        public string med_score { get; set; }

        /// <summary>Med+Rx</summary>
        [DisplayName("Med + Rx")]
        [JsonProperty("total_score")]
        [MaxLength(50)]
        public string total_score { get; set; }

        [JsonProperty("concurrent_total")]
        [MaxLength(50)]
        public string concurrent_total { get; set; }

        [JsonProperty("erScore")]
        [MaxLength(50)]
        public string erScore { get; set; }

        [JsonProperty("otherScore")]
        [MaxLength(50)]
        public string otherScore { get; set; }

        [JsonProperty("concurrentInpatient")]
        [MaxLength(50)]
        public string concurrentInpatient { get; set; }

        [JsonProperty("concurrentMedical")]
        [MaxLength(50)]
        public string concurrentMedical { get; set; }

        [JsonProperty("concurrentOutpatient")]
        [MaxLength(50)]
        public string concurrentOutpatient { get; set; }

        [JsonProperty("concurrentPharmacy")]
        [MaxLength(50)]
        public string concurrentPharmacy { get; set; }

        [JsonProperty("concurrentPhysician")]
        [MaxLength(50)]
        public string concurrentPhysician { get; set; }

        [JsonProperty("concurrentIpNormalizedToGroup")]
        [MaxLength(50)]
        public string concurrentIpNormalizedToGroup { get; set; }

        [JsonProperty("concurrentOpNormalizedToGroup")]
        [MaxLength(50)]
        public string concurrentOpNormalizedToGroup { get; set; }

        [JsonProperty("concurrentPhyNormalizedToGroup")]
        [MaxLength(50)]
        public string concurrentPhyNormalizedToGroup { get; set; }

    }

    [DocumentCollection(CrmDdbContext.DatabaseName, "Participation")]
    public partial class Participation : DeerwalkDdbEntity
    {
        public static readonly Participation[] None = new Participation[0];

        /// <summary>Auto-increment number-a unique identifier for Makalu engine</summary>
        [DisplayName("Auto - increment number - a unique identifier for Makalu engine")]
        [JsonProperty("dw_record_id")]
        [SampleData("1")]
        public int dw_record_id { get; set; }

        /// <summary>Account id</summary>
        [DisplayName("Account id")]
        [JsonProperty("dw_account_id")]
        [SampleData("1027")]
        [MaxLength(50)]
        public string dw_account_id { get; set; }

        /// <summary>Clientid</summary>
        [DisplayName("Clientid")]
        [JsonProperty("dw_client_id")]
        [SampleData("1")]
        [MaxLength(50)]
        public string dw_client_id { get; set; }

        /// <summary>Member ID</summary>
        [DisplayName("Member ID")]
        [JsonProperty("dw_member_id")]
        [SampleData("Hash Encrypted")]
        [MaxLength(50)]
        public string dw_member_id { get; set; }

        /// <summary>Member ID to display on the application, as sent by client</summary>
        [DisplayName("Member ID to display on the application, as sent by client")]
        [Required]
        [JsonProperty("mbr_id")]
        [SampleData("9916897")]
        [MaxLength(20)]
        public string mbr_id { get; set; }

        /// <summary>Type of Program for participation</summary>
        [DisplayName("Type of Program for participation")]
        [JsonProperty("program_type")]
        [SampleData("Disease Management, Wellness")]
        [MaxLength(50)]
        public string program_type { get; set; }

        /// <summary>Code to identify program</summary>
        [DisplayName("Code to identify program")]
        [Required]
        [JsonProperty("program_code")]
        [SampleData("C, A, PC")]
        [MaxLength(20)]
        public string program_code { get; set; }

        /// <summary>Name of the program</summary>
        [DisplayName("Name of the program")]
        [JsonProperty("program_name")]
        [SampleData("CAD, ASTHMA, Preventive Care")]
        [MaxLength(50)]
        public string program_name { get; set; }

        /// <summary>Current status of the program</summary>
        [DisplayName("Current status of the program")]
        [JsonProperty("program_status")]
        [SampleData("Open, Ongoing, Closed")]
        [MaxLength(20)]
        public string program_status { get; set; }

        /// <summary>Program start date</summary>
        [DisplayName("Program start date")]
        [DataType(DataType.Date)]
        [JsonProperty("program_start_date")]
        public DateTime program_start_date { get; set; }

        /// <summary>Program end  date</summary>
        [DisplayName("Program end  date")]
        [DataType(DataType.Date)]
        [JsonProperty("program_end_date")]
        public DateTime program_end_date { get; set; }


    }

    [DocumentCollection(CrmDdbContext.DatabaseName, "QualityMetrics")]
    public partial class QualityMetric : DeerwalkDdbEntity
    {
        public static readonly QualityMetric[] None = new QualityMetric[0];

        /// <summary>Auto-increment number-a unique identifier for Makalu engine</summary>
        [DisplayName("Auto - increment number - a unique identifier for Makalu engine")]
        [Required]
        [JsonProperty("dw_record_id")]
        [SampleData("1")]
        public int dw_record_id { get; set; }

        /// <summary>Member ID</summary>
        [DisplayName("Member ID")]
        [Required]
        [JsonProperty("dw_member_id")]
        [SampleData("Hash Encrypted")]
        [MaxLength(50)]
        public string dw_member_id { get; set; }

        [JsonProperty("memberFirstName")]
        [SampleData("Mark")]
        [MaxLength(30)]
        public string memberFirstName { get; set; }

        [JsonProperty("memberLastName")]
        [SampleData("Hinds")]
        [MaxLength(30)]
        public string memberLastName { get; set; }

        [JsonProperty("memberGender")]
        [SampleData("F")]
        [MaxLength(5)]
        public string memberGender { get; set; }

        [DataType(DataType.Date)]
        [JsonProperty("memberDOB")]
        [SampleData("YYYY - MM - DD")]
        public DateTime memberDOB { get; set; }

        [JsonProperty("measureId")]
        [SampleData("123")]
        [MaxLength(50)]
        public string measureId { get; set; }

        [JsonProperty("measureDesc")]
        [SampleData("3 or more ER Visits in the last 6 months")]
        [MaxLength(200)]
        public string measureDesc { get; set; }

        [JsonProperty("PositiveNegative")]
        [SampleData("0 = Negative metric; 1 = Positive metric")]
        [MaxLength(50)]
        public string PositiveNegative { get; set; }

        [JsonProperty("measureName")]
        [SampleData("Utilization")]
        [MaxLength(200)]
        public string measureName { get; set; }

        [DataType(DataType.Date)]
        [JsonProperty("startDate")]
        [SampleData("YYYY - MM - DD")]
        public DateTime startDate { get; set; }

        [DataType(DataType.Date)]
        [JsonProperty("EndDate")]
        [SampleData("YYYY - MM - DD")]
        public DateTime EndDate { get; set; }

        [JsonProperty("numerator")]
        [SampleData("0")]
        [MaxLength(1)]
        public string numerator { get; set; }

        [JsonProperty("denomenator")]
        [SampleData("1")]
        [MaxLength(1)]
        public string denomenator { get; set; }

    }

    [DocumentCollection(CrmDdbContext.DatabaseName, "HighCostDiagnosis")]
    public partial class HighCostDiagnosis : DeerwalkDdbEntity
    {
        public static readonly HighCostDiagnosis[] None = new HighCostDiagnosis[0];

        /// <summary>Auto-increment number-a unique identifier for Makalu engine</summary>
        [DisplayName("Auto - increment number - a unique identifier for Makalu engine")]
        [JsonProperty("dw_record_id")]
        [SampleData("1")]
        public int dw_record_id { get; set; }

        /// <summary>Account id</summary>
        [DisplayName("Account id")]
        [JsonProperty("dw_account_id")]
        [SampleData("1027")]
        [MaxLength(50)]
        public string dw_account_id { get; set; }

        /// <summary>Clientid</summary>
        [DisplayName("Clientid")]
        [JsonProperty("dw_client_id")]
        [SampleData("1")]
        [MaxLength(50)]
        public string dw_client_id { get; set; }

        /// <summary>Member ID</summary>
        [DisplayName("Member ID")]
        [JsonProperty("dw_member_id")]
        [SampleData("Hash Encrypted")]
        [MaxLength(50)]
        public string dw_member_id { get; set; }

        /// <summary>Member ID to display on the application, as sent by client</summary>
        [DisplayName("Member ID to display on the application, as sent by client")]
        [JsonProperty("mbr_id")]
        [SampleData("9916897")]
        [MaxLength(20)]
        public string mbr_id { get; set; }

        /// <summary>Diagnosis Code</summary>
        [DisplayName("Diagnosis Code")]
        [JsonProperty("Diagnosis_code")]
        [SampleData("123.12")]
        [MaxLength(10)]
        public string Diagnosis_code { get; set; }

        /// <summary>Paid Amount</summary>
        [DisplayName("Paid Amount")]
        [JsonProperty("Paid_Amount")]
        [SampleData("123456.99")]
        [MaxLength(50)]
        public string Paid_Amount { get; set; }

        /// <summary>Infections</summary>
        [DisplayName("Infections")]
        [JsonProperty("SuperGrouperDescription")]
        [SampleData("Infections")]
        [MaxLength(50)]
        public string SuperGrouperDescription { get; set; }

        /// <summary>Tuberculosis</summary>
        [DisplayName("Tuberculosis")]
        [JsonProperty("GrouperDescription")]
        [SampleData("Infectious Diseases")]
        [MaxLength(50)]
        public string GrouperDescription { get; set; }


    }

    [DocumentCollection(CrmDdbContext.DatabaseName, "CareAlerts")]
    public partial class CareAlert : DeerwalkDdbEntity
    {
        public static readonly CareAlert[] None = new CareAlert[0];

        /// <summary>Auto-increment number-a unique identifier for Makalu engine</summary>
        [DisplayName("Auto - increment number - a unique identifier for Makalu engine")]
        [JsonProperty("dw_record_id")]
        [SampleData("1")]
        public int dw_record_id { get; set; }

        /// <summary>Member ID</summary>
        [DisplayName("Member ID")]
        [JsonProperty("dw_member_id")]
        [SampleData("Hash Encrypted")]
        [MaxLength(50)]
        public string dw_member_id { get; set; }

        /// <summary>Member ID to display on the application, as sent by client</summary>
        [DisplayName("Member ID to display on the application, as sent by client")]
        [JsonProperty("mbr_id")]
        [SampleData("15435")]
        [MaxLength(50)]
        public string mbr_id { get; set; }

        /// <summary>Member first name</summary>
        [DisplayName("Member first name")]
        [JsonProperty("first_name")]
        [MaxLength(50)]
        public string first_name { get; set; }

        /// <summary>Member last name</summary>
        [DisplayName("Member last name")]
        [JsonProperty("last_name")]
        [MaxLength(20)]
        public string last_name { get; set; }

        /// <summary>Member middle name</summary>
        [DisplayName("Member middle name")]
        [JsonProperty("middle_name")]
        [MaxLength(10)]
        public string middle_name { get; set; }

        /// <summary>Member date of birth</summary>
        [DisplayName("Member date of birth")]
        [DataType(DataType.Date)]
        [JsonProperty("mbr_dob")]
        [SampleData("yyyy - mm - dd")]
        public DateTime mbr_dob { get; set; }

        [JsonProperty("mbr_gender")]
        [SampleData("Male, Female")]
        [MaxLength(10)]
        public string mbr_gender { get; set; }

        /// <summary>Active or Termed</summary>
        [DisplayName("Active or Termed")]
        [JsonProperty("mbr_status")]
        [MaxLength(10)]
        public string mbr_status { get; set; }

        /// <summary>Relationship</summary>
        [DisplayName("Relationship")]
        [JsonProperty("mbr_relationship")]
        [SampleData("Employee, Dependent")]
        [MaxLength(50)]
        public string mbr_relationship { get; set; }

        /// <summary>PCP name</summary>
        [DisplayName("PCP name")]
        [JsonProperty("pcp_full_name")]
        [MaxLength(50)]
        public string pcp_full_name { get; set; }

        /// <summary>Age of member</summary>
        [DisplayName("Age of member")]
        [JsonProperty("mbr_age")]
        [SampleData("30")]
        [MaxLength(2)]
        public string mbr_age { get; set; }

        /// <summary>Member Months</summary>
        [DisplayName("Member Months")]
        [JsonProperty("mbr_months")]
        [SampleData("11")]
        [MaxLength(3)]
        public string mbr_months { get; set; }

        /// <summary>Care Alert Date</summary>
        [DisplayName("Care Alert Date")]
        [DataType(DataType.Date)]
        [JsonProperty("care_alert_startDate")]
        [SampleData("yyyy - mm - dd")]
        public DateTime care_alert_startDate { get; set; }

        /// <summary>Care Alert Id</summary>
        [DisplayName("Care Alert Id")]
        [JsonProperty("care_alert_id")]
        [MaxLength(50)]
        public string care_alert_id { get; set; }

        /// <summary>Care Alert Description</summary>
        [DisplayName("Care Alert Description")]
        [JsonProperty("care_alert_desc")]
        [MaxLength(50)]
        public string care_alert_desc { get; set; }

        /// <summary>Metric type</summary>
        [DisplayName("Metric type")]
        [JsonProperty("metric_Type")]
        [SampleData("Positive Metric or Negative Metric")]
        [MaxLength(50)]
        public string metric_Type { get; set; }

        /// <summary>Metric Name</summary>
        [DisplayName("Metric Name")]
        [JsonProperty("metric_name")]
        [SampleData("Wellness, Hypertension")]
        [MaxLength(50)]
        public string metric_name { get; set; }


    }


}
#endif