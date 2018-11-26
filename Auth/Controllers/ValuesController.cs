using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Auth.DataAccess.Repositories;
using Auth.Models.Notes;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly INoteRepository _noteRepository;

        public ValuesController(INoteRepository noteRepository)
        {
            _noteRepository = noteRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Note>> Get()
        {
            return await _noteRepository.GetAllNotes();
        }

        [HttpGet("{id}")]
        public async Task<Note> Get(string id)
        {
            return await _noteRepository.GetNote(id);
        }

        [HttpPost]
        public void Post([FromBody] NoteParam newNote)
        {
            _noteRepository.AddNote(new Note
            {
                Id = newNote.Id,
                    Body = newNote.Body,
                    UserId = 55
            });
        }
    }

    public class NoteParam
    {
        public string Id { get; set; }
        public string Body { get; set; }
    }
}
