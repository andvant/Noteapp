using Moq;
using Noteapp.Core.Dtos;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Noteapp.UnitTests.Core.NoteServiceTests
{
    public class Create
    {
        private readonly Mock<INoteRepository> _mock = new();

        [Fact]
        public async Task CreatesNewNote()
        {
            // Arrange
            var dateTime = new DateTime(2021, 1, 1);
            var dateTimeProvider = Mock.Of<IDateTimeProvider>(dateTimeProvider =>
                dateTimeProvider.Now == dateTime);
            var noteService = new NoteService(_mock.Object, dateTimeProvider);

            var noteRequest = new NoteRequest()
            {
                Text = "new note"
            };

            // Act
            var createdNote = await noteService.Create(userId: 1, noteRequest);

            // Assert
            Assert.Equal(1, createdNote.AuthorId);
            Assert.Equal("new note", createdNote.Text);
            Assert.Equal(dateTime, createdNote.Created);
            Assert.Equal(dateTime, createdNote.Updated);
            _mock.Verify(repo => repo.Add(createdNote), Times.Once);
        }
    }
}
