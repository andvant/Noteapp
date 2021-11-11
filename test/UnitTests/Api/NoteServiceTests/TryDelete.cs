﻿using Noteapp.Api.Data;
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
    public class TryDelete
    {
        [Fact]
        public void DeletesNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var note1 = new Note()
            {
                Id = 1,
                AuthorId = 1,
            };
            var note2 = new Note()
            {
                Id = 2,
                AuthorId = 2,
            };
            noteRepository.Notes.Add(note1);
            noteRepository.Notes.Add(note2);
            var noteService = new NoteService(noteRepository, new DateTimeProvider());

            // Act
            var success = noteService.TryDelete(userId: 1, noteId: 1);

            // Assert
            Assert.True(success);
            Assert.Equal(note2, noteRepository.Notes.Single());
        }

        [Fact]
        public void DoesNotDeleteGivenNonExistentNoteId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
            };
            noteRepository.Notes.Add(note);
            var noteService = new NoteService(noteRepository, new DateTimeProvider());

            // Act
            var success = noteService.TryDelete(userId: 1, noteId: 2);

            // Assert
            Assert.False(success);
            Assert.Equal(note, noteRepository.Notes.Single());
        }

        [Fact]
        public void DoesNotDeleteGivenWrongUserId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
            };
            noteRepository.Notes.Add(note);
            var noteService = new NoteService(noteRepository, new DateTimeProvider());

            // Act
            var success = noteService.TryDelete(userId: 2, noteId: 1);

            // Assert
            Assert.False(success);
            Assert.Equal(note, noteRepository.Notes.Single());
        }
    }
}
