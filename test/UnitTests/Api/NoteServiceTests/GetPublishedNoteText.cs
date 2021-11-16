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
        private readonly INoteRepository _noteRepository = new NoteRepository(false);

        [Fact]
        public void ReturnsNoteTextGivenRightUrl()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                Text = "note 1",
                PublicUrl = "testtest"
            };
            _noteRepository.Notes.Add(note);
            var service = new NoteService(_noteRepository, new DateTimeProvider());

            // Act
            var text = service.GetPublishedNoteText("testtest");

            // Assert
            Assert.Equal("note 1", text);
        }

        [Fact]
        public void ThrowsGivenWrongUrl()
        {
            // Arrange
            var service = new NoteService(_noteRepository, new DateTimeProvider());

            // Act
            Action act = () => service.GetPublishedNoteText("shouldfail");

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
        }

    }
}
