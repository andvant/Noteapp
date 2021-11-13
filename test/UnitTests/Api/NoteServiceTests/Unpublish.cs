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
    public class Unpublish
    {

        [Fact]
        public void UnpublishesNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                PublicUrl = "testtest"
            };
            noteRepository.Notes.Add(note);

            // Act
            var success = service.Unpublish(userId: 1, noteId: 1);

            // Assert
            Assert.True(success);
            Assert.Null(noteRepository.Notes.Single().PublicUrl);
        }


        [Fact]
        public void DoesNotUnpublishGivenNonExistentNoteId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                PublicUrl = "testtest"
            };
            noteRepository.Notes.Add(note);

            // Act
            var success = service.Unpublish(userId: 1, noteId: 2);

            // Assert
            Assert.False(success);
            Assert.Equal("testtest", noteRepository.Notes.Single().PublicUrl);
        }

        [Fact]
        public void DoesNotUnpublishGivenWrongUserId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                PublicUrl = "testtest"
            };
            noteRepository.Notes.Add(note);

            // Act
            var success = service.Unpublish(userId: 2, noteId: 1);

            // Assert
            Assert.False(success);
            Assert.Equal("testtest", noteRepository.Notes.Single().PublicUrl);
        }


    }
}
