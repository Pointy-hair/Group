using Newtonsoft.Json;
using RevolutionaryStuff.Core;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Traffk.Bal.Data.Rdb
{
    public partial class Contact
    {
        public static class ContactTypes
        {
            public const string Person = "Person";
            public const string Organization = "Organization";

            public static readonly ICollection<string> All = new List<string> { Person, Organization }.AsReadOnly();

            public static bool IsPerson(string contactType)
            {
                return contactType == Person || contactType.StartsWith(Person + "/");
            }
            public static bool IsOrganization(string contactType)
            {
                return contactType == Organization || contactType.StartsWith(Organization + "/");
            }

            public static void RequiresPredefined(string contactType, string argName)
            {
                Requires.SetMembership(All, nameof(ContactTypes) + "." + nameof(All), contactType, argName);
            }
        }

        public static Contact Create(string contactType)
        {
            switch (contactType)
            {
                case ContactTypes.Organization:
                    return new Organization();
                case ContactTypes.Person:
                    return new PersonZ();
                default:
                    throw new UnexpectedSwitchValueException(contactType);
            }
        }

        [NotMapped]
        [JsonIgnore]
        public bool IsPerson => this is PersonZ;

        [NotMapped]
        [JsonIgnore]
        public bool IsOrganization => this is Organization;

        [NotMapped]
        [JsonIgnore]
        public PersonZ AsPerson => this as PersonZ;

        [NotMapped]
        [JsonIgnore]
        public Organization AsOrganization => this as Organization;

        public Ddb.Note FindNoteById(string noteId)
        {
            return FindNoteById(noteId, ContactDetails.Notes);
        }

        private static Ddb.Note FindNoteById(string noteId, IEnumerable<Ddb.Note> notes)
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
                Notes = Notes ?? new List<Ddb.Note>();
                Addresses = Addresses ?? new List<Ddb.Crm.Zontact.ContactAddress>();
                PhoneNumbers = PhoneNumbers ?? new List<Ddb.Crm.Zontact.ContactPhone>();
            }

  //          [JsonProperty("relations")]
//            public List<Related> Relations { get; set; }

            [JsonProperty("tags")]
            [ConstrainedData]
            public List<string> Tags { get; set; }

            [JsonProperty("notes")]
            [FreeFormData]
            public List<Ddb.Note> Notes { get; set; }

            [JsonProperty("addresses")]
            public List<Ddb.Crm.Zontact.ContactAddress> Addresses { get; set; }

            [JsonProperty("phoneNumbers")]
            public List<Ddb.Crm.Zontact.ContactPhone> PhoneNumbers { get; set; }
        }
    }
}
