﻿using Moq;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using Xunit;

namespace Noteapp.UnitTests.Core.NoteServiceTests
{
    public class Update
    {
        private readonly Mock<IRepository<Note>> _mock = new Mock<IRepository<Note>>();
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly DateTime _created = new DateTime(2021, 1, 1);
        private readonly DateTime _updated = new DateTime(2021, 2, 2);

        public Update()
        {
            _dateTimeProvider = Mock.Of<IDateTimeProvider>(dateTimeProvider =>
                dateTimeProvider.Now == _updated);
        }

        [Fact]
        public void UpdatesNoteGivenValidUserIdAndNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = _created,
                Updated = _created,
                Text = "original text"
            };
            _mock.Setup(repo => repo.Find(1)).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            noteService.Update(userId: 1, noteId: 1, text: "updated text");

            // Assert
            Assert.Equal("updated text", note.Text);
            Assert.Equal(_updated, note.Updated, TimeSpan.Zero);
            Assert.Equal(_created, note.Created, TimeSpan.Zero);
        }

        [Fact]
        public void ThrowsGivenNonExistentNoteId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = _created,
                Updated = _created,
                Text = "original text"
            };
            _mock.Setup(repo => repo.Find(1)).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Action act = () => noteService.Update(userId: 1, noteId: 2, text: "updated text");

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.Equal("original text", note.Text);
            Assert.Equal(_created, note.Updated, TimeSpan.Zero);
            Assert.Equal(_created, note.Created, TimeSpan.Zero);
        }

        [Fact]
        public void ThrowsGivenWrongUserId()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = _created,
                Updated = _created,
                Text = "original text"
            };
            _mock.Setup(repo => repo.Find(1)).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Action act = () => noteService.Update(userId: 2, noteId: 1, text: "updated text");

            // Assert
            Assert.Throws<NoteNotFoundException>(act);
            Assert.Equal("original text", note.Text);
            Assert.Equal(_created, note.Updated, TimeSpan.Zero);
            Assert.Equal(_created, note.Created, TimeSpan.Zero);
        }

        [Fact]
        public void ThrowsGivenNoteIsLocked()
        {
            // Arrange
            var note = new Note()
            {
                Id = 1,
                AuthorId = 1,
                Created = _created,
                Updated = _created,
                Text = "original text",
                Locked = true
            };
            _mock.Setup(repo => repo.Find(1)).Returns(note);
            var noteService = new NoteService(_mock.Object, _dateTimeProvider);

            // Act
            Action act = () => noteService.Update(userId: 1, noteId: 1, text: "updated text");

            // Assert
            Assert.Throws<NoteLockedException>(act);
            Assert.Equal("original text", note.Text);
            Assert.Equal(_created, note.Updated, TimeSpan.Zero);
            Assert.Equal(_created, note.Created, TimeSpan.Zero);
        }
    }
}
