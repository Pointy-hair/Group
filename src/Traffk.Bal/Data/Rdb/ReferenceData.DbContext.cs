/*
This was autogenerated
DO NOT MODIFY BY HAND!!!
TT File: Rdb.Schema.tt
XML File: C:\src\traffk\HealthInformationPortal\src\Traffk.Bal\Data\Rdb\ReferenceData.SchemaMeta.xml
Generation Time: 09/10/2017 18:24:22
*/
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.ApplicationParts;
using RevolutionaryStuff.Core.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Traffk.Bal.Data.Rdb.ReferenceDataModel
{
    public partial class ReferenceDataDbContext
    {
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
            base.OnModelCreating(modelBuilder);
			AddFluentRelationships(modelBuilder);
		}

		#region Collections

		
		public DbSet<Country> Countries { get; set; } //ISO3166.Countries

		
		public DbSet<ICD10> ICD10 { get; set; } //InternationalClassificationDiseases.ICD10

		
		public DbSet<Labeler> Labelers { get; set; } //NationalDrugCode.Labelers

		
		public DbSet<MedicareSpecialtyCode> MedicareSpecialtyCodes { get; set; } //CmsGov.MedicareSpecialtyCodes

		
		public DbSet<Package> Packages { get; set; } //NationalDrugCode.Packages

		
		public DbSet<Product> Products { get; set; } //NationalDrugCode.Products

		#endregion

		#region Sprocs

		#endregion

	}

	[Table("MedicareSpecialtyCodes", Schema = "CmsGov")]
	public partial class MedicareSpecialtyCode : IRdbDataEntity, IValidate, IPreSave, IDontCreate, IPrimaryKey<int>
	{
        public static readonly MedicareSpecialtyCode[] None = new MedicareSpecialtyCode[0];

		//skipped: CmsGov.HealthCareProviderTaxonomyCodeCrosswalk

		object IPrimaryKey.Key { get { return MedicareSpecialtyCodeId; }}
	
		int IPrimaryKey<int>.Key { get { return MedicareSpecialtyCodeId; }}

		[DisplayName("Medicare Specialty Code Id")]
		[Display(Name = "Medicare Specialty Code Id")]
		[Key]
		[Column("MedicareSpecialtyCodeId")]
		public int MedicareSpecialtyCodeId { get; set; }

		[DisplayName("Medicare Specialty Code Property")]
		[Display(Name = "Medicare Specialty Code Property")]
		[NotNull]
		[Required]
		[Column("MedicareSpecialtyCode")]
		public string MedicareSpecialtyCodeProperty { get; set; }

		[DisplayName("Medicare Specialty Supplier Type Description")]
		[Display(Name = "Medicare Specialty Supplier Type Description")]
		[MaxLength(100)]
		[Column("MedicareSpecialtySupplierTypeDescription")]
		public string MedicareSpecialtySupplierTypeDescription { get; set; }

		partial void OnToString(ref string extras);

        public override string ToString()
		{
			string extras = null;
			OnToString(ref extras);
			return $"{GetType().Name} MedicareSpecialtyCodeId={MedicareSpecialtyCodeId} {extras}";
		}

		partial void OnConstructed();
	
		public MedicareSpecialtyCode()
			: this(null)
		{}

		public MedicareSpecialtyCode(MedicareSpecialtyCode other, bool copyKey=false)
		{
			if (other!=null)
			{
				if (copyKey)
				{
					MedicareSpecialtyCodeId = other.MedicareSpecialtyCodeId;
				}
				MedicareSpecialtyCodeProperty = other.MedicareSpecialtyCodeProperty;
				MedicareSpecialtySupplierTypeDescription = other.MedicareSpecialtySupplierTypeDescription;
			}
			OnConstructed();
		}

		partial void PartialPreSave();

		void IPreSave.PreSave()
        {
			OnPreSave();
		}

		protected virtual void OnPreSave()
		{
			PartialPreSave();
        }

		partial void PartialValidate();

		public virtual void Validate()
        {
			Requires.NonNull(MedicareSpecialtyCodeProperty, nameof(MedicareSpecialtyCodeProperty));
			Requires.Text(MedicareSpecialtySupplierTypeDescription, nameof(MedicareSpecialtySupplierTypeDescription), true, 0, 100);
			PartialValidate();
        }
	}//end of entity class MedicareSpecialtyCode

	[Table("ICD10", Schema = "InternationalClassificationDiseases")]
	public partial class ICD10 : IRdbDataEntity, IValidate, IPreSave, IPrimaryKey<int>
	{
        public static readonly ICD10[] None = new ICD10[0];


		//modelBuilder.Entity<ICD10>(e=>e.HasOne(r=>r.ParentIcd10).WithMany(u=>u.ParentIcd10ICD10s).HasForeignKey(r=>r.ParentIcd10Id));
		[InverseProperty("ParentIcd10")]
		[JsonIgnore]
        [IgnoreDataMember]
        public List<ICD10> ParentIcd10ICD10s { get; set; } = new List<ICD10>();

		object IPrimaryKey.Key { get { return Icd10Id; }}
	
		int IPrimaryKey<int>.Key { get { return Icd10Id; }}

		[DisplayName("Icd10 Id")]
		[Display(Name = "Icd10 Id")]
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("Icd10Id")]
		public int Icd10Id { get; set; }

		[DisplayName("Parent Icd10 Id")]
		[Display(Name = "Parent Icd10 Id")]
		[Column("ParentIcd10Id")]
		public int? ParentIcd10Id { get; set; }

		//LinksTo:InternationalClassificationDiseases.ICD10
		[ForeignKey("ParentIcd10Id")]
		[JsonIgnore]
        [IgnoreDataMember]
		public ICD10 ParentIcd10 { get; set; }

		[DisplayName("Diagnosis Code")]
		[Display(Name = "Diagnosis Code")]
		[NotNull]
		[Required]
		[MaxLength(16)]
		[Column("DiagnosisCode")]
		public string DiagnosisCode { get; set; }

		[DisplayName("Diagnosis Description")]
		[Display(Name = "Diagnosis Description")]
		[NotNull]
		[Required]
		[MaxLength(255)]
		[Column("DiagnosisDescription")]
		public string DiagnosisDescription { get; set; }

		partial void OnToString(ref string extras);

        public override string ToString()
		{
			string extras = null;
			OnToString(ref extras);
			return $"{GetType().Name} Icd10Id={Icd10Id} {extras}";
		}

		partial void OnConstructed();
	
		public ICD10()
			: this(null)
		{}

		public ICD10(ICD10 other, bool copyKey=false)
		{
			if (other!=null)
			{
				if (copyKey)
				{
					Icd10Id = other.Icd10Id;
				}
				ParentIcd10ICD10s = other.ParentIcd10ICD10s;
				ParentIcd10Id = other.ParentIcd10Id;
				DiagnosisCode = other.DiagnosisCode;
				DiagnosisDescription = other.DiagnosisDescription;
			}
			OnConstructed();
		}

		partial void PartialPreSave();

		void IPreSave.PreSave()
        {
			OnPreSave();
		}

		protected virtual void OnPreSave()
		{
			PartialPreSave();
        }

		partial void PartialValidate();

		public virtual void Validate()
        {
			Requires.NonNull(DiagnosisCode, nameof(DiagnosisCode));
			Requires.Text(DiagnosisCode, nameof(DiagnosisCode), true, 0, 16);
			Requires.NonNull(DiagnosisDescription, nameof(DiagnosisDescription));
			Requires.Text(DiagnosisDescription, nameof(DiagnosisDescription), true, 0, 255);
			PartialValidate();
        }
	}//end of entity class ICD10

	[Table("Countries", Schema = "ISO3166")]
	public partial class Country : IRdbDataEntity, IValidate, IPreSave, IDontCreate, IPrimaryKey<int>
	{
        public static readonly Country[] None = new Country[0];

		object IPrimaryKey.Key { get { return CountryId; }}
	
		int IPrimaryKey<int>.Key { get { return CountryId; }}

		[DisplayName("Country Id")]
		[Display(Name = "Country Id")]
		[Key]
		[Column("CountryId")]
		public int CountryId { get; set; }

		[DisplayName("Country Name")]
		[Display(Name = "Country Name")]
		[NotNull]
		[Required]
		[MaxLength(100)]
		[Column("CountryName")]
		public string CountryName { get; set; }

		[DisplayName("Alpha2")]
		[Display(Name = "Alpha2")]
		[NotNull]
		[Required]
		[Column("Alpha2")]
		public string Alpha2 { get; set; }

		[DisplayName("Alpha3")]
		[Display(Name = "Alpha3")]
		[NotNull]
		[Required]
		[Column("Alpha3")]
		public string Alpha3 { get; set; }

		[DisplayName("Numeric Code")]
		[Display(Name = "Numeric Code")]
		[NotNull]
		[Required]
		[Column("NumericCode")]
		public string NumericCode { get; set; }

		partial void OnToString(ref string extras);

        public override string ToString()
		{
			string extras = null;
			OnToString(ref extras);
			return $"{GetType().Name} CountryId={CountryId} {extras}";
		}

		partial void OnConstructed();
	
		public Country()
			: this(null)
		{}

		public Country(Country other, bool copyKey=false)
		{
			if (other!=null)
			{
				if (copyKey)
				{
					CountryId = other.CountryId;
				}
				CountryName = other.CountryName;
				Alpha2 = other.Alpha2;
				Alpha3 = other.Alpha3;
				NumericCode = other.NumericCode;
			}
			OnConstructed();
		}

		partial void PartialPreSave();

		void IPreSave.PreSave()
        {
			OnPreSave();
		}

		protected virtual void OnPreSave()
		{
			PartialPreSave();
        }

		partial void PartialValidate();

		public virtual void Validate()
        {
			Requires.NonNull(CountryName, nameof(CountryName));
			Requires.Text(CountryName, nameof(CountryName), true, 0, 100);
			Requires.NonNull(Alpha2, nameof(Alpha2));
			Requires.NonNull(Alpha3, nameof(Alpha3));
			Requires.NonNull(NumericCode, nameof(NumericCode));
			PartialValidate();
        }
	}//end of entity class Country

	[Table("Labelers", Schema = "NationalDrugCode")]
	public partial class Labeler : IRdbDataEntity, IValidate, IPreSave, IDontCreate, IPrimaryKey<int>
	{
        public static readonly Labeler[] None = new Labeler[0];


		//modelBuilder.Entity<Product>(e=>e.HasOne(r=>r.Labeler).WithMany(u=>u.LabelerProducts).HasForeignKey(r=>r.LabelerId));
		[InverseProperty("Labeler")]
		[JsonIgnore]
        [IgnoreDataMember]
        public List<Product> LabelerProducts { get; set; } = new List<Product>();

		object IPrimaryKey.Key { get { return LabelerId; }}
	
		int IPrimaryKey<int>.Key { get { return LabelerId; }}

		[DisplayName("Labeler Id")]
		[Display(Name = "Labeler Id")]
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("LabelerId")]
		public int LabelerId { get; set; }

		[DisplayName("Labeler Code")]
		[Display(Name = "Labeler Code")]
		[NotNull]
		[Required]
		[MaxLength(10)]
		[Column("LabelerCode")]
		public string LabelerCode { get; set; }

		[DisplayName("Labeler Name")]
		[Display(Name = "Labeler Name")]
		[MaxLength(255)]
		[Column("LabelerName")]
		public string LabelerName { get; set; }

		partial void OnToString(ref string extras);

        public override string ToString()
		{
			string extras = null;
			OnToString(ref extras);
			return $"{GetType().Name} LabelerId={LabelerId} {extras}";
		}

		partial void OnConstructed();
	
		public Labeler()
			: this(null)
		{}

		public Labeler(Labeler other, bool copyKey=false)
		{
			if (other!=null)
			{
				if (copyKey)
				{
					LabelerId = other.LabelerId;
				}
				LabelerProducts = other.LabelerProducts;
				LabelerCode = other.LabelerCode;
				LabelerName = other.LabelerName;
			}
			OnConstructed();
		}

		partial void PartialPreSave();

		void IPreSave.PreSave()
        {
			OnPreSave();
		}

		protected virtual void OnPreSave()
		{
			PartialPreSave();
        }

		partial void PartialValidate();

		public virtual void Validate()
        {
			Requires.NonNull(LabelerCode, nameof(LabelerCode));
			Requires.Text(LabelerCode, nameof(LabelerCode), true, 0, 10);
			Requires.Text(LabelerName, nameof(LabelerName), true, 0, 255);
			PartialValidate();
        }
	}//end of entity class Labeler

	[Table("Packages", Schema = "NationalDrugCode")]
	public partial class Package : IRdbDataEntity, IValidate, IPreSave, IDontCreate, IPrimaryKey<int>
	{
        public static readonly Package[] None = new Package[0];

		object IPrimaryKey.Key { get { return PackageId; }}
	
		int IPrimaryKey<int>.Key { get { return PackageId; }}

		[DisplayName("Package Id")]
		[Display(Name = "Package Id")]
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("PackageId")]
		public int PackageId { get; set; }

		[DisplayName("Productid")]
		[Display(Name = "Productid")]
		[Column("Productid")]
		public int Productid { get; set; }

		//LinksTo:NationalDrugCode.Products
		[ForeignKey("Productid")]
		[JsonIgnore]
        [IgnoreDataMember]
		public Product Product { get; set; }

		/// <summary>
		/// The labeler code and product code segments of the National Drug Code number, separated by a hyphen. Asterisks are no longer used or included within the product code segment to indicate certain configurations of the NDC.
		/// </summary>
		[Description("The labeler code and product code segments of the National Drug Code number, separated by a hyphen. Asterisks are no longer used or included within the product code segment to indicate certain configurations of the NDC.")]
		[DisplayName("Product NDC")]
		[Display(Name = "Product NDC", Description = "The labeler code and product code segments of the National Drug Code number, separated by a hyphen. Asterisks are no longer used or included within the product code segment to indicate certain configurations of the NDC.")]
		[NotNull]
		[Required]
		[MaxLength(16)]
		[Column("ProductNDC")]
		public string ProductNDC { get; set; }

		/// <summary>
		/// The labeler code, product code, and package code segments of the National Drug Code number, separated by hyphens. Asterisks are no longer used or included within the product and package code segments to indicate certain configurations of the NDC.
		/// </summary>
		[Description("The labeler code, product code, and package code segments of the National Drug Code number, separated by hyphens. Asterisks are no longer used or included within the product and package code segments to indicate certain configurations of the NDC.")]
		[DisplayName("NDCPackage Code")]
		[Display(Name = "NDCPackage Code", Description = "The labeler code, product code, and package code segments of the National Drug Code number, separated by hyphens. Asterisks are no longer used or included within the product and package code segments to indicate certain configurations of the NDC.")]
		[NotNull]
		[Required]
		[MaxLength(16)]
		[Column("NDCPackageCode")]
		public string NDCPackageCode { get; set; }

		/// <summary>
		/// A description of the size and type of packaging in sentence form. Multilevel packages will have the descriptions concatenated together.  For example: 4 BOTTLES in 1 CARTON/100 TABLETS in 1 BOTTLE.
		/// </summary>
		[Description("A description of the size and type of packaging in sentence form. Multilevel packages will have the descriptions concatenated together.  For example: 4 BOTTLES in 1 CARTON/100 TABLETS in 1 BOTTLE.")]
		[DisplayName("Package Description")]
		[Display(Name = "Package Description", Description = "A description of the size and type of packaging in sentence form. Multilevel packages will have the descriptions concatenated together.  For example: 4 BOTTLES in 1 CARTON/100 TABLETS in 1 BOTTLE.")]
		[NotNull]
		[Required]
		[Column("PackageDescription")]
		public string PackageDescription { get; set; }

		[DisplayName("Cms Code")]
		[Display(Name = "Cms Code")]
		[MaxLength(8000)]
		[Column("CmsCode")]
		public string CmsCode { get; set; }

		partial void OnToString(ref string extras);

        public override string ToString()
		{
			string extras = null;
			OnToString(ref extras);
			return $"{GetType().Name} PackageId={PackageId} {extras}";
		}

		partial void OnConstructed();
	
		public Package()
			: this(null)
		{}

		public Package(Package other, bool copyKey=false)
		{
			if (other!=null)
			{
				if (copyKey)
				{
					PackageId = other.PackageId;
				}
				Productid = other.Productid;
				ProductNDC = other.ProductNDC;
				NDCPackageCode = other.NDCPackageCode;
				PackageDescription = other.PackageDescription;
				CmsCode = other.CmsCode;
			}
			OnConstructed();
		}

		partial void PartialPreSave();

		void IPreSave.PreSave()
        {
			OnPreSave();
		}

		protected virtual void OnPreSave()
		{
			PartialPreSave();
        }

		partial void PartialValidate();

		public virtual void Validate()
        {
			Requires.NonNull(ProductNDC, nameof(ProductNDC));
			Requires.Text(ProductNDC, nameof(ProductNDC), true, 0, 16);
			Requires.NonNull(NDCPackageCode, nameof(NDCPackageCode));
			Requires.Text(NDCPackageCode, nameof(NDCPackageCode), true, 0, 16);
			Requires.NonNull(PackageDescription, nameof(PackageDescription));
			Requires.Text(CmsCode, nameof(CmsCode), true, 0, 8000);
			PartialValidate();
        }
	}//end of entity class Package

	[Table("Products", Schema = "NationalDrugCode")]
	public partial class Product : IRdbDataEntity, IValidate, IPreSave, IDontCreate, IPrimaryKey<int>
	{
        public static readonly Product[] None = new Product[0];


		//modelBuilder.Entity<Package>(e=>e.HasOne(r=>r.Product).WithMany(u=>u.ProductPackages).HasForeignKey(r=>r.Productid));
		[InverseProperty("Product")]
		[JsonIgnore]
        [IgnoreDataMember]
        public List<Package> ProductPackages { get; set; } = new List<Package>();

		object IPrimaryKey.Key { get { return ProductId; }}
	
		int IPrimaryKey<int>.Key { get { return ProductId; }}

		[DisplayName("Product Id")]
		[Display(Name = "Product Id")]
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Column("ProductId")]
		public int ProductId { get; set; }

		[DisplayName("Labeler Id")]
		[Display(Name = "Labeler Id")]
		[Column("LabelerId")]
		public int LabelerId { get; set; }

		//LinksTo:NationalDrugCode.Labelers
		[ForeignKey("LabelerId")]
		[JsonIgnore]
        [IgnoreDataMember]
		public Labeler Labeler { get; set; }

		/// <summary>
		/// ProductUID is a concatenation of the NDCproduct code and SPL documentID. It is included to help prevent duplicate rows from appearing when joining the product and package files together.  It has no regulatory value or significance.
		/// </summary>
		[Description("ProductUID is a concatenation of the NDCproduct code and SPL documentID. It is included to help prevent duplicate rows from appearing when joining the product and package files together.  It has no regulatory value or significance.")]
		[DisplayName("Product Uid")]
		[Display(Name = "Product Uid", Description = "ProductUID is a concatenation of the NDCproduct code and SPL documentID. It is included to help prevent duplicate rows from appearing when joining the product and package files together.  It has no regulatory value or significance.")]
		[NotNull]
		[Required]
		[MaxLength(50)]
		[Column("ProductUid")]
		public string ProductUid { get; set; }

		/// <summary>
		/// The labeler code and product code segments of the National Drug Code number, separated by a hyphen. Asterisks are no longer used or included within the product code segment to indicate certain configurations of the NDC.
		/// </summary>
		[Description("The labeler code and product code segments of the National Drug Code number, separated by a hyphen. Asterisks are no longer used or included within the product code segment to indicate certain configurations of the NDC.")]
		[DisplayName("Product NDC")]
		[Display(Name = "Product NDC", Description = "The labeler code and product code segments of the National Drug Code number, separated by a hyphen. Asterisks are no longer used or included within the product code segment to indicate certain configurations of the NDC.")]
		[NotNull]
		[Required]
		[MaxLength(16)]
		[Column("ProductNDC")]
		public string ProductNDC { get; set; }

		/// <summary>
		/// Indicates the type of product, such as Human Prescription Drug or Human OTC Drug. This data element corresponds to the "Document Type" of the SPL submission for the listing. The complete list of codes and translations can be found at
		/// </summary>
		[Description("Indicates the type of product, such as Human Prescription Drug or Human OTC Drug. This data element corresponds to the \"Document Type\" of the SPL submission for the listing. The complete list of codes and translations can be found at")]
		[DisplayName("Product Type Name")]
		[Display(Name = "Product Type Name", Description = "Indicates the type of product, such as Human Prescription Drug or Human OTC Drug. This data element corresponds to the \"Document Type\" of the SPL submission for the listing. The complete list of codes and translations can be found at")]
		[MaxLength(64)]
		[Column("ProductTypeName")]
		public string ProductTypeName { get; set; }

		/// <summary>
		/// Also known as the trade name. It is the name of the product chosen by the labeler.
		/// </summary>
		[Description("Also known as the trade name. It is the name of the product chosen by the labeler.")]
		[DisplayName("Proprietary Name")]
		[Display(Name = "Proprietary Name", Description = "Also known as the trade name. It is the name of the product chosen by the labeler.")]
		[MaxLength(1024)]
		[Column("ProprietaryName")]
		public string ProprietaryName { get; set; }

		/// <summary>
		/// A suffix to the proprietary name, a value here should be appended to the ProprietaryName field to obtain the complete name of the product. This suffix is often used to distinguish characteristics of a product such as extended release (“XR”) or sleep aid (“PM”). Although many companies follow certain naming conventions for suffices, there is no recognized standard.
		/// </summary>
		[Description("A suffix to the proprietary name, a value here should be appended to the ProprietaryName field to obtain the complete name of the product. This suffix is often used to distinguish characteristics of a product such as extended release (“XR”) or sleep aid (“PM”). Although many companies follow certain naming conventions for suffices, there is no recognized standard.")]
		[DisplayName("Proprietary Name Suffix")]
		[Display(Name = "Proprietary Name Suffix", Description = "A suffix to the proprietary name, a value here should be appended to the ProprietaryName field to obtain the complete name of the product. This suffix is often used to distinguish characteristics of a product such as extended release (“XR”) or sleep aid (“PM”). Although many companies follow certain naming conventions for suffices, there is no recognized standard.")]
		[MaxLength(255)]
		[Column("ProprietaryNameSuffix")]
		public string ProprietaryNameSuffix { get; set; }

		/// <summary>
		/// Sometimes called the generic name, this is usually the active ingredient(s) of the product.
		/// </summary>
		[Description("Sometimes called the generic name, this is usually the active ingredient(s) of the product.")]
		[DisplayName("Non Proprietary Name")]
		[Display(Name = "Non Proprietary Name", Description = "Sometimes called the generic name, this is usually the active ingredient(s) of the product.")]
		[MaxLength(1024)]
		[Column("NonProprietaryName")]
		public string NonProprietaryName { get; set; }

		/// <summary>
		/// The translation of the DosageForm Code submitted by the firm. The complete list of codes and translations can be found www.fda.gov/edrls under Structured Product Labeling Resources.
		/// </summary>
		[Description("The translation of the DosageForm Code submitted by the firm. The complete list of codes and translations can be found www.fda.gov/edrls under Structured Product Labeling Resources.")]
		[DisplayName("Dosage Form Name")]
		[Display(Name = "Dosage Form Name", Description = "The translation of the DosageForm Code submitted by the firm. The complete list of codes and translations can be found www.fda.gov/edrls under Structured Product Labeling Resources.")]
		[MaxLength(64)]
		[Column("DosageFormName")]
		public string DosageFormName { get; set; }

		/// <summary>
		/// The translation of the Route Code submitted by the firm, indicating route of administration. The complete list of codes and translations can be found at www.fda.gov/edrls under Structured Product Labeling Resources.
		/// </summary>
		[Description("The translation of the Route Code submitted by the firm, indicating route of administration. The complete list of codes and translations can be found at www.fda.gov/edrls under Structured Product Labeling Resources.")]
		[DisplayName("Route Name")]
		[Display(Name = "Route Name", Description = "The translation of the Route Code submitted by the firm, indicating route of administration. The complete list of codes and translations can be found at www.fda.gov/edrls under Structured Product Labeling Resources.")]
		[MaxLength(255)]
		[Column("RouteName")]
		public string RouteName { get; set; }

		/// <summary>
		/// This is the date that the labeler indicates was the start of its marketing of the drug product.
		/// </summary>
		[Description("This is the date that the labeler indicates was the start of its marketing of the drug product.")]
		[DisplayName("Start Marketing Date")]
		[Display(Name = "Start Marketing Date", Description = "This is the date that the labeler indicates was the start of its marketing of the drug product.")]
		[Column("StartMarketingDate")]
		public DateTime? StartMarketingDate { get; set; }

		/// <summary>
		/// This is the date the product will no longer be available on the market. If a product is no longer being manufactured, in most cases, the FDA recommends firms use the expiration date of the last lot produced as the EndMarketingDate, to reflect the potential for drug product to remain available after manufacturing has ceased. Products that are the subject of ongoing manufacturing will not ordinarily have any EndMarketingDate. Products with a value in the EndMarketingDate will be removed from the NDC Directory when the EndMarketingDate is reached.
		/// </summary>
		[Description("This is the date the product will no longer be available on the market. If a product is no longer being manufactured, in most cases, the FDA recommends firms use the expiration date of the last lot produced as the EndMarketingDate, to reflect the potential for drug product to remain available after manufacturing has ceased. Products that are the subject of ongoing manufacturing will not ordinarily have any EndMarketingDate. Products with a value in the EndMarketingDate will be removed from the NDC Directory when the EndMarketingDate is reached.")]
		[DisplayName("End Marketing Date")]
		[Display(Name = "End Marketing Date", Description = "This is the date the product will no longer be available on the market. If a product is no longer being manufactured, in most cases, the FDA recommends firms use the expiration date of the last lot produced as the EndMarketingDate, to reflect the potential for drug product to remain available after manufacturing has ceased. Products that are the subject of ongoing manufacturing will not ordinarily have any EndMarketingDate. Products with a value in the EndMarketingDate will be removed from the NDC Directory when the EndMarketingDate is reached.")]
		[Column("EndMarketingDate")]
		public DateTime? EndMarketingDate { get; set; }

		/// <summary>
		/// Product types are broken down into several potential Marketing Categories, such as NDA/ANDA/BLA, OTC Monograph, or Unapproved Drug. One and only one Marketing Category may be chosen for a product, not all marketing categories are available to all product types. Currently, only final marketed product categories are included.  The complete list of codes and translations can be found at www.fda.gov/edrls under Structured Product Labeling Resources.
		/// </summary>
		[Description("Product types are broken down into several potential Marketing Categories, such as NDA/ANDA/BLA, OTC Monograph, or Unapproved Drug. One and only one Marketing Category may be chosen for a product, not all marketing categories are available to all product types. Currently, only final marketed product categories are included.  The complete list of codes and translations can be found at www.fda.gov/edrls under Structured Product Labeling Resources.")]
		[DisplayName("Marketing Category Name")]
		[Display(Name = "Marketing Category Name", Description = "Product types are broken down into several potential Marketing Categories, such as NDA/ANDA/BLA, OTC Monograph, or Unapproved Drug. One and only one Marketing Category may be chosen for a product, not all marketing categories are available to all product types. Currently, only final marketed product categories are included.  The complete list of codes and translations can be found at www.fda.gov/edrls under Structured Product Labeling Resources.")]
		[MaxLength(64)]
		[Column("MarketingCategoryName")]
		public string MarketingCategoryName { get; set; }

		/// <summary>
		/// This corresponds to the NDA, ANDA, or BLA number reported by the labeler for products which have the corresponding Marketing Category designated. If the designated Marketing Category is OTC Monograph Final or OTC Monograph Not Final, then the Application number will be the CFR citation corresponding to the appropriate Monograph (e.g. “part 341”). For unapproved drugs, this field will be null.
		/// </summary>
		[Description("This corresponds to the NDA, ANDA, or BLA number reported by the labeler for products which have the corresponding Marketing Category designated. If the designated Marketing Category is OTC Monograph Final or OTC Monograph Not Final, then the Application number will be the CFR citation corresponding to the appropriate Monograph (e.g. “part 341”). For unapproved drugs, this field will be null.")]
		[DisplayName("Application Number")]
		[Display(Name = "Application Number", Description = "This corresponds to the NDA, ANDA, or BLA number reported by the labeler for products which have the corresponding Marketing Category designated. If the designated Marketing Category is OTC Monograph Final or OTC Monograph Not Final, then the Application number will be the CFR citation corresponding to the appropriate Monograph (e.g. “part 341”). For unapproved drugs, this field will be null.")]
		[MaxLength(11)]
		[Column("ApplicationNumber")]
		public string ApplicationNumber { get; set; }

		/// <summary>
		/// This is the active ingredient list. Each ingredient name is the preferred term of the UNII code submitted.
		/// </summary>
		[Description("This is the active ingredient list. Each ingredient name is the preferred term of the UNII code submitted.")]
		[DisplayName("Substance Name")]
		[Display(Name = "Substance Name", Description = "This is the active ingredient list. Each ingredient name is the preferred term of the UNII code submitted.")]
		[Column("SubstanceName")]
		public string SubstanceName { get; set; }

		/// <summary>
		/// These are the strength values (to be used with units below) of each active ingredient, listed in the same order as the SubstanceName field above.
		/// </summary>
		[Description("These are the strength values (to be used with units below) of each active ingredient, listed in the same order as the SubstanceName field above.")]
		[DisplayName("Strength Number")]
		[Display(Name = "Strength Number", Description = "These are the strength values (to be used with units below) of each active ingredient, listed in the same order as the SubstanceName field above.")]
		[MaxLength(4000)]
		[Column("StrengthNumber")]
		public string StrengthNumber { get; set; }

		/// <summary>
		/// These are the units to be used with the strength values above, listed in the same order as the SubstanceName and SubstanceNumber.
		/// </summary>
		[Description("These are the units to be used with the strength values above, listed in the same order as the SubstanceName and SubstanceNumber.")]
		[DisplayName("Strength Unit")]
		[Display(Name = "Strength Unit", Description = "These are the units to be used with the strength values above, listed in the same order as the SubstanceName and SubstanceNumber.")]
		[MaxLength(4000)]
		[Column("StrengthUnit")]
		public string StrengthUnit { get; set; }

		/// <summary>
		/// These are the reported pharmacological class categories corresponding to the SubstanceNames listed above.
		/// </summary>
		[Description("These are the reported pharmacological class categories corresponding to the SubstanceNames listed above.")]
		[DisplayName("Pharm Classes")]
		[Display(Name = "Pharm Classes", Description = "These are the reported pharmacological class categories corresponding to the SubstanceNames listed above.")]
		[Column("PharmClasses")]
		public string PharmClasses { get; set; }

		/// <summary>
		/// This is the assigned DEA Schedule number as reported by the labeler. Values are CI, CII, CIII, CIV, and CV.
		/// </summary>
		[Description("This is the assigned DEA Schedule number as reported by the labeler. Values are CI, CII, CIII, CIV, and CV.")]
		[DisplayName("DEASchedule")]
		[Display(Name = "DEASchedule", Description = "This is the assigned DEA Schedule number as reported by the labeler. Values are CI, CII, CIII, CIV, and CV.")]
		[MaxLength(4)]
		[Column("DEASchedule")]
		public string DEASchedule { get; set; }

		partial void OnToString(ref string extras);

        public override string ToString()
		{
			string extras = null;
			OnToString(ref extras);
			return $"{GetType().Name} ProductId={ProductId} {extras}";
		}

		partial void OnConstructed();
	
		public Product()
			: this(null)
		{}

		public Product(Product other, bool copyKey=false)
		{
			if (other!=null)
			{
				if (copyKey)
				{
					ProductId = other.ProductId;
				}
				ProductPackages = other.ProductPackages;
				LabelerId = other.LabelerId;
				ProductUid = other.ProductUid;
				ProductNDC = other.ProductNDC;
				ProductTypeName = other.ProductTypeName;
				ProprietaryName = other.ProprietaryName;
				ProprietaryNameSuffix = other.ProprietaryNameSuffix;
				NonProprietaryName = other.NonProprietaryName;
				DosageFormName = other.DosageFormName;
				RouteName = other.RouteName;
				StartMarketingDate = other.StartMarketingDate;
				EndMarketingDate = other.EndMarketingDate;
				MarketingCategoryName = other.MarketingCategoryName;
				ApplicationNumber = other.ApplicationNumber;
				SubstanceName = other.SubstanceName;
				StrengthNumber = other.StrengthNumber;
				StrengthUnit = other.StrengthUnit;
				PharmClasses = other.PharmClasses;
				DEASchedule = other.DEASchedule;
			}
			OnConstructed();
		}

		partial void PartialPreSave();

		void IPreSave.PreSave()
        {
			OnPreSave();
		}

		protected virtual void OnPreSave()
		{
			PartialPreSave();
        }

		partial void PartialValidate();

		public virtual void Validate()
        {
			Requires.NonNull(ProductUid, nameof(ProductUid));
			Requires.Text(ProductUid, nameof(ProductUid), true, 0, 50);
			Requires.NonNull(ProductNDC, nameof(ProductNDC));
			Requires.Text(ProductNDC, nameof(ProductNDC), true, 0, 16);
			Requires.Text(ProductTypeName, nameof(ProductTypeName), true, 0, 64);
			Requires.Text(ProprietaryName, nameof(ProprietaryName), true, 0, 1024);
			Requires.Text(ProprietaryNameSuffix, nameof(ProprietaryNameSuffix), true, 0, 255);
			Requires.Text(NonProprietaryName, nameof(NonProprietaryName), true, 0, 1024);
			Requires.Text(DosageFormName, nameof(DosageFormName), true, 0, 64);
			Requires.Text(RouteName, nameof(RouteName), true, 0, 255);
			Requires.Text(MarketingCategoryName, nameof(MarketingCategoryName), true, 0, 64);
			Requires.Text(ApplicationNumber, nameof(ApplicationNumber), true, 0, 11);
			Requires.Text(StrengthNumber, nameof(StrengthNumber), true, 0, 4000);
			Requires.Text(StrengthUnit, nameof(StrengthUnit), true, 0, 4000);
			Requires.Text(DEASchedule, nameof(DEASchedule), true, 0, 4);
			PartialValidate();
        }
	}//end of entity class Product


    public partial class ReferenceDataDbContext
    {
		private void AddFluentRelationships(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ICD10>(e=>e.HasOne(r=>r.ParentIcd10).WithMany(u=>u.ParentIcd10ICD10s).HasForeignKey(r=>r.ParentIcd10Id));
			modelBuilder.Entity<Product>(e=>e.HasOne(r=>r.Labeler).WithMany(u=>u.LabelerProducts).HasForeignKey(r=>r.LabelerId));
			modelBuilder.Entity<Package>(e=>e.HasOne(r=>r.Product).WithMany(u=>u.ProductPackages).HasForeignKey(r=>r.Productid));
		}
    }


}//end of file
