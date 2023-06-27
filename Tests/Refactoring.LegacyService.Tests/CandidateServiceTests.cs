using AutoFixture;
using FluentAssertions;
using NSubstitute;
using Refactoring.LegacyService.DataAccess;
using Refactoring.LegacyService.Models;
using Refactoring.LegacyService.Repositories;
using Refactoring.LegacyService.Services;
using System;
using Xunit;

namespace Refactoring.LegacyService.Tests
{
    public class CandidateServiceTests
    {

        private readonly CandidateService _sut;
        private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        private readonly IPositionRepository _positionRepository = Substitute.For<IPositionRepository>();
        private readonly ICandidateDataAccess _candidateDataAccess = Substitute.For<ICandidateDataAccess>();
        private readonly ICandidateCreditService _candidateCreditService = Substitute.For<ICandidateCreditService>();
        private readonly IFixture _fixture = new Fixture();

        public CandidateServiceTests()
        {
            _sut = new CandidateService(_positionRepository, _candidateDataAccess, new Validator.CandidateValidator(_dateTimeProvider), new CreditProviders.CreditLimitProviderFactory(_candidateCreditService));
        }

        [Theory]
        [InlineData("FeatureDeveloper", 600)]
        [InlineData("SecuritySpecialist", 1100)]
        public void AddCandidate_ShouldCreateCandidate_WhenAllParametersAreValid(string positionName, int initialCreditLimit)
        {
            const int positionId = 1;
            const string firstName = "Aakash";
            const string lastName = "Bista";
            var dateOfBirth = new DateTime(1991, 1, 1);
            var position = _fixture.Build<Position>()
                .With(c => c.Id, positionId)
                .With(c => c.Name, positionName)
                .Create();

            _dateTimeProvider.DateTimeNow.Returns(new DateTime(2021, 2, 16));
            _positionRepository.GetById(positionId).Returns(position);
            _candidateCreditService.GetCredit(firstName, lastName, dateOfBirth)
                .Returns(initialCreditLimit);

            // Act
            var result = _sut.AddCandidate(firstName, lastName, "aakash.bistey@gmail.com", dateOfBirth, positionId);

            // Assert
            result.Should().BeTrue();
            _candidateDataAccess.Received(1).AddCandidate(Arg.Any<Candidate>());

        }

        [Theory]
        [InlineData("", "aakash", "aakash.bistey@gmail.com", 1991)]
        [InlineData("Aakash", "", "aakash.bistey@gmail.com", 1991)]
        [InlineData("Aakash", "Bista", "aakcom", 1991)]
        [InlineData("Aakash", "Bista", "aakash.bistey@gmail.com", 2008)]
        public void AddCandidate_ShouldNotCreateCandidate_WhenInputDetailsAreInvalid(
          string firstName, string lastName, string email, int yearOfBirth)
        {
            // Arrange
            const int positionId = 1;
            var dateOfBirth = new DateTime(yearOfBirth, 1, 1);
            var position = _fixture.Build<Position>()
                .With(c => c.Id, positionId)
                .Create();

            _dateTimeProvider.DateTimeNow.Returns(new DateTime(2021, 2, 16));
            _positionRepository.GetById(Arg.Is(positionId)).Returns(position);
            _candidateCreditService.GetCredit(Arg.Is(firstName), Arg.Is(lastName), Arg.Is(dateOfBirth)).Returns(600);

            // Act
            var result = _sut.AddCandidate(firstName, lastName, email, dateOfBirth, 1);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("SecuritySpecialist", true, 1200, 600)]
        [InlineData("FeatureDeveloper", true, 600, 600)]
        [InlineData("RandomClientName", false, 0, 0)]
        public void AddCandidate_ShouldCreateCandidateWithCorrectCreditLimit_WhenNameIndicatesDifferentClassification(
            string positionName, bool hasRequireCreditCheck, int initialCreditLimit, int finalCreditLimit)
        {
            // Arrange
            const int positionId = 1;
            const string firstName = "Aakash";
            const string lastName = "Bista";
            var dateOfBirth = new DateTime(1991, 1, 10);
            var position = _fixture.Build<Position>()
                .With(c => c.Id, positionId)
                .With(c => c.Name, positionName)
                .Create();

            _dateTimeProvider.DateTimeNow.Returns(new DateTime(2021, 2, 16));
            _positionRepository.GetById(Arg.Is(positionId)).Returns(position);
            _candidateCreditService.GetCredit(Arg.Is(firstName), Arg.Is(lastName), Arg.Is(dateOfBirth)).Returns(initialCreditLimit);

            // Act
            var result = _sut.AddCandidate(firstName, lastName, "aakash.bistey@gmail.com", dateOfBirth, 1);

            // Assert
            result.Should().BeTrue();
            _candidateDataAccess.Received()
               .AddCandidate(Arg.Is<Candidate>(candidate => candidate.RequireCreditCheck == hasRequireCreditCheck && candidate.Credit == finalCreditLimit));
        }

        [Theory]
        [InlineData("FeatureDeveloper")]
        [InlineData("SecuritySpecialist")]
        public void AddCandidate_ShouldNotCreateCandidate_WhenCandidateHasCreditLimitAndCreditLimitIsLessThan500(string positionName)
        {
            // Arrange
            const int positionId = 1;
            const string firstName = "Aakash";
            const string lastName = "Bista";
            var dateOfBirth = new DateTime(1993, 10, 10);
            var position = _fixture.Build<Position>()
                .With(c => c.Id, positionId)
                .With(c => c.Name, positionName)
                .Create();

            _dateTimeProvider.DateTimeNow.Returns(new DateTime(2021, 2, 16));
            _positionRepository.GetById(Arg.Is(positionId)).Returns(position);
            _candidateCreditService.GetCredit(Arg.Is(firstName), Arg.Is(lastName), Arg.Is(dateOfBirth)).Returns(400);

            // Act
            var result = _sut.AddCandidate(firstName, lastName, "aakash.bistey@gmail.com", dateOfBirth, 1);

            // Assert
            result.Should().BeFalse();
        }
    }
}
