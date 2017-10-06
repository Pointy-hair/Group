using Traffk.Bal.Data.Rdb;

namespace Traffk.Portal.Models.NoteModels
{
    public class NoteViewModel
    {
        public NoteViewModel(string entityType, string entityId, string parentNoteId = null)
        {
            EntityType = entityType;
            EntityId = entityId;
            ParentNoteId = parentNoteId;
        }
        public string EntityType { get; set; }
        public string EntityId { get; set; }
        public string ParentNoteId { get; set; }
    }
}
