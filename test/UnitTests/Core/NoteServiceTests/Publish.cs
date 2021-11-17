﻿using Moq;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Noteapp.UnitTests.Core.NoteServiceTests
{
    public class Publish
    {
        private readonly Mock<INoteRepository> _mock = new Mock<INoteRepository>();
        private readonly INoteRepository _noteRepository;
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        public Publish()
        {
            _mock.Setup(repo => repo.Notes).Returns(new List<Note>());
            _noteRepository = _mock.Object;
        }

        [Fact]
        public void ReturnsUrlGivenValidUserIdAndNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            _noteRepository.Notes.Add(note);
            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            var url = noteService.Publish(userId: 1, noteId: 1);

            // Assert
            Assert.True(!string.IsNullOrWhiteSpace(url));
            Assert.Equal(url, _noteRepository.Notes.Single().PublicUrl);
        }


        [Fact]
        public void ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            _noteRepository.Notes.Add(note);
            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            Action act = () => noteService.Publish(userId: 1, noteId: 2);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.Null(_noteRepository.Notes.Single().PublicUrl);
        }

        [Fact]
        public void ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            _noteRepository.Notes.Add(note);
            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            Action act = () => noteService.Publish(userId: 2, noteId: 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.Null(_noteRepository.Notes.Single().PublicUrl);
        }


    }
}