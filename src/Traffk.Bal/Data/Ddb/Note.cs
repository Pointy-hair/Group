using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Traffk.Bal.Data.Ddb
{
    public class Note
    {
        [JsonProperty("noteId")]
        public string NoteId { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("createdAtUtc")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAtUtc { get; set; }

        [JsonIgnore]
        [DisplayName("Created At")]
        public DateTime CreatedAt => CreatedAtUtc.ToLocalTime();

        [JsonProperty("createdByContactId")]
        public string CreatedByUserId { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("children")]
        public List<Note> Children { get; set; }

    }
}
