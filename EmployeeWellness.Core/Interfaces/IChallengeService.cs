using EmployeeWellness.Core.Entities;
using EmployeeWellness.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeWellness.Core.Interfaces
{
    public interface IChallengeService
    {
        Task<Guid> CreateChallengeAsync(CreateChallengeRequest request);

        Task SubmitProgressAsync(Guid challengeId, SubmitProgressRequest request);

        Task<IReadOnlyList<LeaderboardEntryResponse>> GetLeaderboardAsync(Guid challengeId);

        Task<IReadOnlyList<ChallengeResponse>> GetActiveChallengesForUserAsync(string userId);
    }
}
