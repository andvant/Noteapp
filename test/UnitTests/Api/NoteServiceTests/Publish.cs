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
    public class Publish
    {

        [Fact]
        public void ReturnsNewUrlGivenValidUserIdAndNoteId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            noteRepository.Notes.Add(note);

            // Act
            var url = service.Publish(userId: 1, noteId: 1);

            // Assert
            Assert.True(!string.IsNullOrWhiteSpace(url));
            Assert.Equal(url, noteRepository.Notes.Single().PublicUrl);
        }


        [Fact]
        public void ReturnsNullGivenNonExistentNoteId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            noteRepository.Notes.Add(note);

            // Act
            Action act = () => service.Publish(userId: 1, noteId: 2);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.Null(noteRepository.Notes.Single().PublicUrl);
        }

        [Fact]
        public void ReturnsNullGivenWrongUserId()
        {
            // Arrange
            var noteRepository = new NoteRepository(false);
            var service = new NoteService(noteRepository, new DateTimeProvider());

            var note = new Note()
            {
                Id = 1,
                AuthorId = 1
            };
            noteRepository.Notes.Add(note);

            // Act
            Action act = () => service.Publish(userId: 2, noteId: 1);

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.Null(noteRepository.Notes.Single().PublicUrl);
        }


    }
}
