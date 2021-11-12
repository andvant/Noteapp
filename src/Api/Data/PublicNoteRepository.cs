using Noteapp.Api.Entities;
using System;
using System.Collections.Generic;

namespace Noteapp.Api.Data
{
    public class PublicNoteRepository
    {
        public List<PublicNote> PublicNotes { get; set; }

        public PublicNoteRepository(bool seed = true)
        {
            PublicNotes = seed ? GetInMemoryPublicNotes() : new List<PublicNote>();
        }

        private List<PublicNote> GetInMemoryPublicNotes()
        {
            // const int userId = 1;

            var notes = new List<PublicNote>()
            {
                //new()
                //{
                //    Id = 1,
                //    Text = "note 1",
                //    Created = DateTime.Now,
                //    LastModified = DateTime.Now,
                //    AuthorId = userId
                //},
                //new()
                //{
                //    Id = 2,
                //    Text = "note 2",
                //    Created = DateTime.Now,
                //    LastModified = DateTime.Now,
                //    AuthorId = userId
                //},
                //new()
                //{
                //    Id = 3,
                //    Text = "note 3",
                //    Created = DateTime.Now,
                //    LastModified = DateTime.Now,
                //    AuthorId = userId
                //}
            };

            return notes;
        }
    }
}
