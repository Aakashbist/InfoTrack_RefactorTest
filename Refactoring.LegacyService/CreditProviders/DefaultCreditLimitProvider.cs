using Refactoring.LegacyService.Models;

namespace Refactoring.LegacyService.CreditProviders
{
    public class DefaultCreditLimitProvider : ICreditLimitProvider
    {

        public (bool HasCreditLimit, int CreditLimit) GetCreditLimits(Candidate candidate)
        {

            return (false, 0);
        }

        public string NameRequirement { get; } = string.Empty;
    }
}
