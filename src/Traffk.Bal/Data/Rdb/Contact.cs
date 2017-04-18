using System;
using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Traffk.Bal.Data.Rdb
{
    public enum ContactTypes
    {
        User,
        Provider,
        Person,
        Carrier,
        Organization,
    }

    public partial class Contact
    {
        public static Contact Create(ContactTypes contactType)
        {
            switch (contactType)
            {
                case ContactTypes.Organization:
                    return new OrganizationContact();
                case ContactTypes.Person:
                    return new PersonContact();
                case ContactTypes.User:
                    return new UserContact();
                case ContactTypes.Carrier:
                    return new CarrierContact();
                case ContactTypes.Provider:
                    return new ProviderContact();
                default:
                    throw new UnexpectedSwitchValueException(contactType);
            }
        }

        [NotMapped]
        [JsonIgnore]
        public bool IsPerson => this is PersonContact;

        [NotMapped]
        [JsonIgnore]
        public bool IsOrganization => this is OrganizationContact;

        [NotMapped]
        [JsonIgnore]
        public PersonContact AsPerson => this as PersonContact;

        [NotMapped]
        [JsonIgnore]
        public OrganizationContact AsOrganization => this as OrganizationContact;

        [InverseProperty("Contact")]
        [JsonIgnore]
        [IgnoreDataMember]
        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();


        public class ContactDetails_
        {
            public static ContactDetails_ CreateFromJson(string json) => TraffkHelpers.JsonConvertDeserializeObjectOrFallback<ContactDetails_>(json);

            public string ToJson() => JsonConvert.SerializeObject(this);

            public ContactDetails_()
            {
                PhoneNumbers = PhoneNumbers ?? new List<ContactPhone>();
            }

            [JsonProperty("tags")]
            [ConstrainedData]
            public List<string> Tags { get; set; }

            [JsonProperty("phoneNumbers")]
            [DisplayName("Phone Numbers")]
            public List<ContactPhone> PhoneNumbers { get; set; }
        }
    }

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

        public ContactPhone(string phoneNumber, string phoneType = null, string notes = null)
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
}
