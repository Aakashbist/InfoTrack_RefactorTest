using Refactoring.LegacyService.Models;
using Refactoring.LegacyService.Services;
using System;

namespace Refactoring.LegacyService.Validator
{
    public class CandidateValidator
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public CandidateValidator(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public bool HasCreditLimitAndLimitIsLessThan500(Candidate candidate)
        {
            return candidate.RequireCreditCheck && candidate.Credit < 500;
        }

        public bool IsAtLeast18YearsOld(DateTime dateOfBirth)
        {
            var now = _dateTimeProvider.DateTimeNow;
            var age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;

            return age >= 18;
        }

        public bool HasValidEmail(string email)
        {
            return email.Contains("@") || email.Contains(".");
        }

        public bool HasValidFullName(string firname, string surname)
        {
            return !string.IsNullOrEmpty(firname) && !string.IsNullOrEmpty(surname);
        }
    }
}
