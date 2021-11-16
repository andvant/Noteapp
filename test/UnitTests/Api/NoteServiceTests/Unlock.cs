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
    public class Unlock
    {
        private readonly INoteRepository _noteRepository = new NoteRepository(false);

        [Fact]
        public void UnlocksNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Locked = true
            };
            _noteRepository.Notes.Add(note);
            var noteService = new NoteService(_noteRepository, new DateTimeProvider());

            // Act
            noteService.Unlock(userId: 1, noteId: 1);

            // Assert
            Assert.False(note.Locked);
        }

        [Fact]
        public void ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Locked = true
            };
            _noteRepository.Notes.Add(note);
            var noteService = new NoteService(_noteRepository, new DateTimeProvider());

            // Act
            Action act = () => noteService.Unlock(userId: 1, noteId: 2);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.True(note.Locked);
        }

        [Fact]
        public void ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Locked = true
            };
            _noteRepository.Notes.Add(note);
            var noteService = new NoteService(_noteRepository, new DateTimeProvider());

            // Act
            Action act = () => noteService.Unlock(userId: 2, noteId: 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.True(note.Locked);
        }
    }
}
