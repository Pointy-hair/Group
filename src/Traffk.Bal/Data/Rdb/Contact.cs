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
                    return new Organization();
                case ContactTypes.Person:
                    return new Person();
                case ContactTypes.User:
                    return new UserContact();
                case ContactTypes.Carrier:
                    return new Carrier();
                case ContactTypes.Provider:
                    return new Provider();
                default:
                    throw new UnexpectedSwitchValueException(contactType);
            }
        }

        [NotMapped]
        [JsonIgnore]
        public bool IsPerson => this is Person;

        [NotMapped]
        [JsonIgnore]
        public bool IsOrganization => this is Organization;

        [NotMapped]
        [JsonIgnore]
        public Person AsPerson => this as Person;

        [NotMapped]
        [JsonIgnore]
        public Organization AsOrganization => this as Organization;

        [InverseProperty("Contact")]
        [JsonIgnore]
        [IgnoreDataMember]
        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();


        public Note FindNoteById(string noteId)
        {
            return FindNoteById(noteId, ContactDetails.Notes);
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

        public class ContactDetails_
        {
            public static ContactDetails_ CreateFromJson(string json) => TraffkHelpers.JsonConvertDeserializeObjectOrFallback<ContactDetails_>(json);

            public string ToJson() => JsonConvert.SerializeObject(this);

            public ContactDetails_()
            {
                Notes = Notes ?? new List<Note>();
                Addresses = Addresses ?? new List<ContactAddress>();
                PhoneNumbers = PhoneNumbers ?? new List<ContactPhone>();
            }

            [JsonProperty("tags")]
            [ConstrainedData]
            public List<string> Tags { get; set; }

            [JsonProperty("notes")]
            [FreeFormData]
            public List<Note> Notes { get; set; }

            [JsonProperty("addresses")]
            public List<ContactAddress> Addresses { get; set; }

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

}
