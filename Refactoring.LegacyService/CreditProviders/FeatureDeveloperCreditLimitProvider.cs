using Refactoring.LegacyService.Models;
using Refactoring.LegacyService.Services;

namespace Refactoring.LegacyService.CreditProviders
{
    public class FeatureDeveloperCreditLimitProvider : ICreditLimitProvider
    {
        private readonly ICandidateCreditService _candidateCreditService;

        public FeatureDeveloperCreditLimitProvider(ICandidateCreditService candidateCreditService)
        {
            _candidateCreditService = candidateCreditService;
        }

        public (bool HasCreditLimit, int CreditLimit) GetCreditLimits(Candidate candidate)
        {
            var creditLimit = _candidateCreditService.GetCredit(candidate.Firstname, candidate.Surname, candidate.DateOfBirth);
            return (true, creditLimit);

        }

        public string NameRequirement { get; } = "FeatureDeveloper";
    }
}
