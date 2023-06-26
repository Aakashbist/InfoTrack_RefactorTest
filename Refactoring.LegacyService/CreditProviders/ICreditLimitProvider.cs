using Refactoring.LegacyService.Models;

namespace Refactoring.LegacyService.CreditProviders
{
    public interface ICreditLimitProvider
    {
        (bool HasCreditLimit, int CreditLimit) GetCreditLimits(Candidate candidate);
        public string NameRequirement { get; }
    }
}
