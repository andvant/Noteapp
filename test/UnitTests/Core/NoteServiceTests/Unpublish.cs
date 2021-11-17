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
    public class Unpublish
    {
        private readonly Mock<INoteRepository> _mock = new Mock<INoteRepository>();
        private readonly INoteRepository _noteRepository;
        private readonly IDateTimeProvider _dateTimeProvider = Mock.Of<IDateTimeProvider>();

        public Unpublish()
        {
            _mock.Setup(repo => repo.Notes).Returns(new List<Note>());
            _noteRepository = _mock.Object;
        }

        [Fact]
        public void UnpublishesNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                PublicUrl = "testtest"
            };
            _noteRepository.Notes.Add(note);
            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            noteService.Unpublish(userId: 1, noteId: 1);

            // Assert
            Assert.Null(_noteRepository.Notes.Single().PublicUrl);
        }


        [Fact]
        public void ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                PublicUrl = "testtest"
            };
            _noteRepository.Notes.Add(note);
            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            Action act = () => noteService.Unpublish(userId: 1, noteId: 2);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.Equal("testtest", _noteRepository.Notes.Single().PublicUrl);
        }

        [Fact]
        public void ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                PublicUrl = "testtest"
            };
            _noteRepository.Notes.Add(note);
            var noteService = new NoteService(_noteRepository, _dateTimeProvider);

            // Act
            Action act = () => noteService.Unpublish(userId: 2, noteId: 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.Equal("testtest", _noteRepository.Notes.Single().PublicUrl);
        }


    }
}