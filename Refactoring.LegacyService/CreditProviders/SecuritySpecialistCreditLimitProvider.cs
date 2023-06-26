using Refactoring.LegacyService.Models;
using Refactoring.LegacyService.Services;

namespace Refactoring.LegacyService.CreditProviders
{
    public class SecuritySpecialistCreditLimitProvider : ICreditLimitProvider
    {
        private readonly ICandidateCreditService _candidateCreditService;

        public SecuritySpecialistCreditLimitProvider(ICandidateCreditService candidateCreditService)
        {
            _candidateCreditService = candidateCreditService;
        }

        public (bool HasCreditLimit, int CreditLimit) GetCreditLimits(Candidate candidate)
        {
            var creditLimit = _candidateCreditService.GetCredit(candidate.Firstname, candidate.Surname, candidate.DateOfBirth);
            return (true, creditLimit / 2);

        }

        public string NameRequirement { get; } = "SecuritySpecialist";
    }
}
