namespace Refactoring.LegacyService
{
    using Refactoring.LegacyService.CreditProviders;
    using Refactoring.LegacyService.DataAccess;
    using Refactoring.LegacyService.Models;
    using Refactoring.LegacyService.Repositories;
    using Refactoring.LegacyService.Validator;
    using System;

    public class CandidateService
    {
        private readonly IPositionRepository _positionRepository;
        private readonly ICandidateDataAccess _candidateDataAccess;
        private readonly CandidateValidator _candidateValidator;
        private readonly CreditLimitProviderFactory _creditLimitProviderFactory;

        public CandidateService(IPositionRepository positionRepository,
                                ICandidateDataAccess candidateDataAccess,
                                CandidateValidator candidateValidator,
                                CreditLimitProviderFactory creditLimitProviderFactory)
        {
            _positionRepository = positionRepository;
            _candidateDataAccess = candidateDataAccess;
            _candidateValidator = candidateValidator;
            _creditLimitProviderFactory = creditLimitProviderFactory;
        }

        public bool AddCandidate(string firname, string surname, string email, DateTime dateOfBirth, int positionid)
        {

            if (!CandidateProvidedDataIsValid(firname, surname, email, dateOfBirth))
            {
                return false;
            }



            var position = _positionRepository.GetById(positionid);

            var candidate = new Candidate
            {
                Position = position,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                Firstname = firname,
                Surname = surname
            };

            ApplyCreditLimits(candidate);


            if (_candidateValidator.HasCreditLimitAndLimitIsLessThan500(candidate))
            {
                return false;
            }

            _candidateDataAccess.AddCandidate(candidate);

            return true;
        }

        private void ApplyCreditLimits(Candidate candidate)
        {
            var provider = _creditLimitProviderFactory.GetProviderByClientName(candidate.Position.Name);
            var (hasCreditLimit, creditLimit) = provider.GetCreditLimits(candidate);
            candidate.RequireCreditCheck = hasCreditLimit;
            candidate.Credit = creditLimit;
        }

        private bool CandidateProvidedDataIsValid(string firname, string surname, string email, DateTime dateOfBirth)
        {
            if (!_candidateValidator.HasValidFullName(firname, surname))
            {
                return false;
            }

            if (!_candidateValidator.HasValidEmail(email))
            {
                return false;
            }

            if (!_candidateValidator.IsAtLeast18YearsOld(dateOfBirth))
            {
                return false;
            }

            return true;
        }
    }
}
