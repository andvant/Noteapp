using Noteapp.Api.Data;
using Noteapp.Api.Entities;
using Noteapp.Api.Exceptions;
using Noteapp.Api.Infrastructure;
using Noteapp.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Noteapp.UnitTests.Api.NoteServiceTests
{
    public class GetPublishedNoteText
    {
        [Fact]
        public void ReturnsNoteTextGivenRightUrl()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note = new Note()
            {
                Id = 1,
                Text = "note 1",
                PublicUrl = "testtest"
            };
            noteRepository.Notes.Add(note);

            // Act
            var text = service.GetPublishedNoteText("testtest");

            // Assert
            Assert.Equal("note 1", text);
        }

        [Fact]
        public void ThrowsGivenWrongUrl()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            // Act
            Action act = () => service.GetPublishedNoteText("shouldfail");

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
        }

    }
}
