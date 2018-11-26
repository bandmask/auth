using System.Collections.Generic;
using System.Threading.Tasks;
using Auth.DataAccess.Contexts;
using Auth.Models.Notes;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Auth.DataAccess.Repositories
{
    public class NoteRepository : INoteRepository
    {
        private NoteContext _noteContext = new NoteContext();

        public async Task<IEnumerable<Note>> GetAllNotes()
        {
            return await _noteContext.Notes.Find(_ => true).ToListAsync();
        }

        public async Task<Note> GetNote(string id)
        {
            ObjectId internalId;
            if (!ObjectId.TryParse(id, out internalId))
            {
                internalId = ObjectId.Empty;
            }

            return await _noteContext.Notes.Find(note => note.Id == id || note.InternalId == internalId).FirstOrDefaultAsync();
        }

        public async Task AddNote(Note note)
        {
            await _noteContext.Notes.InsertOneAsync(note);
        }

    }

    public interface INoteRepository
    {
        Task<IEnumerable<Note>> GetAllNotes();
        Task<Note> GetNote(string id);
        Task AddNote(Note note);
    }
}
