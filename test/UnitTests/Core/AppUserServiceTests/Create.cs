using Moq;
using Noteapp.Core.Entities;
using Noteapp.Core.Exceptions;
using Noteapp.Core.Interfaces;
using Noteapp.Core.Services;
using System;
using Xunit;

namespace Noteapp.UnitTests.Core.AppUserServiceTests
{
    public class Create
    {
        private readonly Mock<IAppUserRepository> _mock = new();
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly DateTime _registrationDate = new DateTime(2021, 1, 1);

        public Create()
        {
            _dateTimeProvider = Mock.Of<IDateTimeProvider>(dateTimeProvider => 
                dateTimeProvider.Now == _registrationDate);
        }

        [Fact]
        public void CreatesUserGivenValidEmail()
        {
            // Arrange
            var service = new AppUserService(_mock.Object, _dateTimeProvider);

            // Act
            var user = service.Create(email: "some@email.com");

            // Assert
            Assert.Equal("some@email.com", user.Email);
            Assert.True(!string.IsNullOrWhiteSpace(user.EncryptionSalt));
            Assert.Equal(_registrationDate, user.RegistrationDate);
            _mock.Verify(repo => repo.Add(user), Times.Once);
        }

        [Fact]
        public void ThrowGivenInvalidEmail()
        {
            // Arrange
            var service = new AppUserService(_mock.Object, _dateTimeProvider);

            // Act
            Action act = () => service.Create(email: "not-a-valid-email");

            // Assert
            Assert.Throws<UserRegistrationException>(act);
            _mock.Verify(repo => repo.Add(It.IsAny<AppUser>()), Times.Never);
        }

        [Fact]
        public void ThrowGivenTakenEmail()
        {
            // Arrange
            _mock.Setup(repo => repo.FindByEmail("taken@email.com")).Returns(new AppUser());
            var service = new AppUserService(_mock.Object, _dateTimeProvider);

            // Act
            Action act = () => service.Create(email: "taken@email.com");

            // Assert
            Assert.Throws<UserRegistrationException>(act);
            _mock.Verify(repo => repo.Add(It.IsAny<AppUser>()), Times.Never);
        }
    }
}
