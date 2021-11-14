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
    public class Pin
    {
        [Fact]
        public void PinsNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Pinned = false
            };
            noteRepository.Notes.Add(note);

            // Act
            service.Pin(userId: 1, noteId: 1);

            // Assert
            Assert.True(note.Pinned);
        }

        [Fact]
        public void ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Pinned = false
            };
            noteRepository.Notes.Add(note);

            // Act
            Action act = () => service.Pin(userId: 1, noteId: 2);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.False(note.Pinned);
        }

        [Fact]
        public void ThrowsGivenWrongUserId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Pinned = false
            };
            noteRepository.Notes.Add(note);

            // Act
            Action act = () => service.Pin(userId: 2, noteId: 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.False(note.Pinned);
        }
    }
}