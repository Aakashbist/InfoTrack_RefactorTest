using Refactoring.LegacyService.Models;

namespace Refactoring.LegacyService.DataAccess
{
    public class CandidateDataAccessProxy : ICandidateDataAccess
    {
        public void AddCandidate(Candidate candidate)
        {
            CandidateDataAccess.AddCandidate(candidate);
        }
    }
}
