﻿using Noteapp.Api.Data;
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
    public class Archive
    {
        private readonly INoteRepository _noteRepository = new NoteRepository(false);

        [Fact]
        public void ArchivesNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = false
            };
            _noteRepository.Notes.Add(note);
            var service = new NoteService(_noteRepository, new DateTimeProvider());

            // Act
            service.Archive(userId: 1, noteId: 1);

            // Assert
            Assert.True(note.Archived);
        }

        [Fact]
        public void ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = false
            };
            _noteRepository.Notes.Add(note);
            var service = new NoteService(_noteRepository, new DateTimeProvider());

            // Act
            Action act = () => service.Archive(userId: 1, noteId: 2);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.False(note.Archived);
        }

        [Fact]
        public void ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Archived = false
            };
            _noteRepository.Notes.Add(note);
            var service = new NoteService(_noteRepository, new DateTimeProvider());

            // Act
            Action act = () => service.Archive(userId: 2, noteId: 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.False(note.Archived);
        }
    }
}
