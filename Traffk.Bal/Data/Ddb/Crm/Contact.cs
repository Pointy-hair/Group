using Newtonsoft.Json;
using RevolutionaryStuff.Azure.DocumentDb;
using RevolutionaryStuff.Core;
using RevolutionaryStuff.Core.ApplicationParts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Traffk.Bal.Data.Rdb;

namespace Traffk.Bal.Data.Ddb.Crm
{
    [DocumentPreTrigger(CrmDdbContext.CommonTriggerNames.ApplyCreatedAtUtc)]
    [DocumentPostTrigger(CrmDdbContext.CommonTriggerNames.ValidateTenandIdExists)]
    [DocumentCollection(CrmDdbContext.DatabaseName, "Contacts")]
    public class Zontact : TenantedDdbEntity, IValidate
    {
        public class ContactPhone
        {
            [JsonProperty("phoneType")]
            [ConstrainedData]
            public string PhoneType { get; set; }

            [JsonProperty("notes")]
            [FreeFormData]
            public string Notes { get; set; }

            [Phone]
            [FreeFormData]
            [JsonProperty("phoneNumber")]
            public string PhoneNumber { get; set; }

            public ContactPhone() { }

            public ContactPhone(string phoneNumber, string phoneType=null, string notes=null)
            {
                PhoneNumber = phoneNumber;
                PhoneType = phoneType;
                Notes = notes;
            }

            public ContactPhone(ContactPhone other)
            {
                PhoneType = other.PhoneType;
                Notes = other.Notes;
                PhoneNumber = other.PhoneNumber;
            }
            public override string ToString() => $"{this.GetType().Name} number={PhoneNumber} type={PhoneType} note={Notes}";
        }

        public class ContactAddress : Address
        {
            [JsonProperty("addressType")]
            [ConstrainedData]
            public string AddressType { get; set; }

            [JsonProperty("notes")]
            [FreeFormData]
            public string Notes { get; set; }

            public ContactAddress() { }

            public ContactAddress(ContactAddress other)
                : base(other)
            {
                AddressType = other.AddressType;
                Notes = other.Notes;
            }
            public ContactAddress(Address other)
                : base(other)
            { }

            public override string ToString() => $"{base.ToString()} type={AddressType} note={Notes}";
        }

        [JsonProperty("createdAtUtc")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAtUtc { get; set; }

        [JsonIgnore]
        public DateTime CreatedAt => CreatedAtUtc.ToLocalTime();

        [JsonProperty("contactType")]
        [ConstrainedData]
        public string ContactType { get; set; }

        [JsonProperty("primaryEmail")]
        [EmailAddress]
        public string PrimaryEmail { get; set; }

        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("person")]
        public Person Person { get; set; }

        [JsonProperty("company")]
        public Company Company { get; set; }

        [JsonProperty("relations")]
        public List<Related> Relations { get; set; }

        [JsonProperty("tags")]
        [ConstrainedData]
        public List<string> Tags { get; set; }

        [JsonProperty("notes")]
        [FreeFormData]
        public List<Note> Notes { get; set; }

        [JsonProperty("addresses")]
        public List<ContactAddress> Addresses { get; set; }

        [JsonProperty("phoneNumbers")]
        public List<ContactPhone> PhoneNumbers { get; set; }

        public Zontact Build()
        {
            Person = Person ?? new Person();
            Person.Name = Person.Name ?? new PersonName();
            Company = Company ?? new Company();
            Notes = Notes ?? new List<Note>();
            Addresses = Addresses ?? new List<ContactAddress>();
            PhoneNumbers = PhoneNumbers ?? new List<ContactPhone>();
            return this;
        }

        public Note FindNoteById(string noteId)
        {
            return FindNoteById(noteId, Notes);
        }
        private static Note FindNoteById(string noteId, IEnumerable<Note> notes)
        {
            if (notes == null) return null;
            foreach (var n in notes)
            {
                if (n.NoteId == noteId) return n;
                if (n.Children != null)
                {
                    var z = FindNoteById(noteId, n.Children);
                    if (z != null) return z;
                }
            }
            return null;
        }

        public void Validate()
        {
            Contact.ContactTypes.RequiresPredefined(ContactType, nameof(ContactType));
            //if (Company != null && Person != null) throw new ArgumentOutOfRangeException($"At most one of {nameof(Person)} and {nameof(Company)} can be specified.");
        }

        public Zontact() { }

        public Zontact(Eligibility el)
        {
            ContactType = Contact.ContactTypes.Person;
            Person = new Person
            {
                Name = new PersonName
                {
                    FirstName = el.mbr_first_name,
                    MiddleName = el.mbr_middle_name,
                    LastName = el.mbr_last_name
                },
                Gender = Person.CommonGenders.Normalize(el.mbr_gender),
                MemberId = el.mbr_id,
                DateOfBirth = el.mbr_dob
            };
            FullName = Person.Name.GetFullName();
            if (StringHelpers.TrimOrNull(el.mbr_street_1) != null)
            {
                Addresses = new List<Zontact.ContactAddress>(new[] { new Zontact.ContactAddress(el.Address) { } });
            }
            if (StringHelpers.TrimOrNull(el.mbr_phone) != null)
            {
                PhoneNumbers = new List<Zontact.ContactPhone>(new[] { new ContactPhone(el.mbr_phone) });
            }
        }

        public override string ToString() => $"{base.ToString()} name={this.FullName}";
    }

    public class Company
    {
        [JsonProperty("companyField1")]
        [FreeFormData]
        public string CompanyField1 { get; set; }
    }

    public class Person
    {
        public static class CommonGenders
        {
            public const string Male = "male";
            public const string Female = "female";

            public static string Normalize(string gender)
            {
                if (gender == null) return null;
                var g = gender.Trim().ToLower();
                switch (g)
                {
                    case "":
                        return null;
                    case "m":
                    case CommonGenders.Male:
                        return CommonGenders.Male;
                    case "f":
                    case CommonGenders.Female:
                        return CommonGenders.Female;
                    default:
                        return g;
                }
            }
        }

        [JsonProperty("name")]
        [FreeFormData]
        public PersonName Name { get; set; }

        [JsonProperty("gender")]
        [ConstrainedData]
        public string Gender { get; set; }

        /// <summary>Member identification number</summary>
        [Description("Member identification number")]
        [JsonProperty("mbr_id")]
        [SampleData("345677")]
        [MaxLength(30)]
        [FreeFormData]
        public string MemberId { get; set; }

        [DataType(DataType.Date)]
        [JsonProperty("dateOfBirth")]
        [ProtectedHealthInformationIdentifier]
        public DateTime? DateOfBirth { get; set; }
    }

    public class PersonName
    {
        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("firstName")]
        [FreeFormData]
        public string FirstName { get; set; }

        [JsonProperty("middleName")]
        [FreeFormData]
        public string MiddleName { get; set; }

        [JsonProperty("lastName")]
        [FreeFormData]
        public string LastName { get; set; }

        [JsonProperty("suffix")]
        public string Suffix { get; set; }

        public string GetFullName() => StringHelpers.TrimOrNull(RegexHelpers.Common.Whitespace.Replace($"{Prefix} {FirstName} {MiddleName} {LastName} {Suffix}", " "));
    }

    public class Related
    {
        [JsonProperty("id")]
        public string Id { get; set; } = TraffkHelpers.CreateRandomStringId();

        [JsonProperty("relatedContactId")]
        public string RelatedContactId { get; set; }

        [JsonProperty("relationType")]
        [ConstrainedData]
        public string RelationType { get; set; }
    }
}
