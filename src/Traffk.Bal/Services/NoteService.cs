using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Traffk.Bal.Data.Rdb;
using Traffk.Bal.Data.Rdb.TraffkTenantModel;
using Traffk.Utility;

namespace Traffk.Bal.Services
{
    public interface INoteService
    {
        SerializableTreeNode<Note> GetNoteNodes(IRdbDataEntity entityWithAttachedNotes);
    }

    public class NoteService : INoteService
    {
        private TraffkTenantModelDbContext Rdb;

        public NoteService(TraffkTenantModelDbContext rdb)
        {
            Rdb = rdb;
        }

        SerializableTreeNode<Note> INoteService.GetNoteNodes(IRdbDataEntity entityWithAttachedNotes)
        {
            var notes = Rdb.GetAttachedNotes(entityWithAttachedNotes).Include(z => z.CreatedByContact);
            var root = new SerializableTreeNode<Note>(null);

            foreach (var note in notes)
            {
                var currentNoteNode = new SerializableTreeNode<Note>(note);
                if (note.ParentNoteId != null)
                {
                    SerializableTreeNode<Note> parentNoteNode = null;
                    
                    //find parent in root and add child to it
                    root.Walk((node, depth) =>
                    {
                        if (node.Data?.NoteId == note.ParentNoteId)
                        {
                            node.Add(currentNoteNode);
                            parentNoteNode = node;
                            return;
                        }
                    });

                    //parent isn't in root
                    if (parentNoteNode == null)
                    {
                        var parentNote = notes.SingleOrDefault(x => x.NoteId == note.ParentNoteId);
                        parentNoteNode = new SerializableTreeNode<Note>(parentNote);
                        parentNoteNode?.Add(currentNoteNode);
                    }
                }
                else
                {
                    root.Add(currentNoteNode);
                }
            }

            root.Children = root.Children.OrderBy(x => x.Data.CreatedAtUtc).ToList();
            return root;
        }
    }
}
