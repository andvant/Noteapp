using Moq;
using Noteapp.Api.Data;
using Noteapp.Api.Entities;
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
    public class Create
    {
        private readonly Mock<INoteRepository> _mock = new Mock<INoteRepository>();
        private readonly INoteRepository _noteRepository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly DateTime _dateTime = new DateTime(2021, 1, 1);

        public Create()
        {
            _mock.Setup(repo => repo.Notes).Returns(new List<Note>());
            _noteRepository = _mock.Object;
            _dateTimeProvider = Mock.Of<IDateTimeProvider>(dateTimeProvider => 
                dateTimeProvider.Now == _dateTime);
        }

        [Fact]
        public void CreatesNewNote()
        {
            // Arrange
            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            var createdNote = noteService.Create(userId: 1, text: "new note");

            // Assert
            Assert.Same(createdNote, _noteRepository.Notes.Single());
            Assert.Equal(_dateTime, createdNote.Created);
            Assert.Equal(_dateTime, createdNote.Updated);
            Assert.Equal(1, createdNote.Id);
            Assert.Equal(1, createdNote.AuthorId);
            Assert.Equal("new note", createdNote.Text);
        }
    }
}
