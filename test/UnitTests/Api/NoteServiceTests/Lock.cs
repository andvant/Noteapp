﻿using Moq;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace Noteapp.UnitTests.Api.NoteServiceTests
{
    public class Lock
    {
        private readonly Mock<INoteRepository> _mock = new Mock<INoteRepository>();
        private readonly INoteRepository _noteRepository;
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        public Lock()
        {
            _mock.Setup(repo => repo.Notes).Returns(new List<Note>());
            _noteRepository = _mock.Object;
        }

        [Fact]
        public void LocksNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Locked = false
            };
            _noteRepository.Notes.Add(note);
            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            noteService.Lock(userId: 1, noteId: 1);

            // Assert
            Assert.True(note.Locked);
        }

        [Fact]
        public void ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Locked = false
            };
            _noteRepository.Notes.Add(note);
            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            Action act = () => noteService.Lock(userId: 1, noteId: 2);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.False(note.Locked);
        }

        [Fact]
        public void ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Locked = false
            };
            _noteRepository.Notes.Add(note);
            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            Action act = () => noteService.Lock(userId: 2, noteId: 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.False(note.Locked);
        }
    }
}
