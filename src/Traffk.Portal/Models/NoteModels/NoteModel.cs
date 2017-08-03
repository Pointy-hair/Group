using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Traffk.Portal.Models.NoteModels
{
    public enum NoteObjectType
    {
        Contact,
        Report
    }

    public class NoteModel
    {
        public NoteModel(NoteObjectType noteObjectType, string entityId)
        {
            NoteObjectType = noteObjectType;
            EntityId = entityId;
        }

        public NoteObjectType NoteObjectType {get; set;}
        public string EntityId { get; set; }
    }
}
