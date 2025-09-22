using EmployeeWellness.Core.Data;
using EmployeeWellness.Core.Entities;
using EmployeeWellness.Core.Interfaces;
using EmployeeWellness.Core.Models;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace EmployeeWellness.Core
{
    public class ChallengeService : IChallengeService
    {
        private readonly EmployeeWellnessContext _context;
        private readonly IDatabase _redis;
        private const string LeaderboardKeyPrefix = "leaderboard:";

        public ChallengeService(EmployeeWellnessContext context, IConnectionMultiplexer redis)
        {
            _context = context;
            _redis = redis.GetDatabase();
        }

        public async Task<Guid> CreateChallengeAsync(CreateChallengeRequest request)
        {
            var challenge = new Challenge
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Goal = request.Goal
            };

            _context.Challenges.Add(challenge);
            await _context.SaveChangesAsync();

            return challenge.Id;
        }

        public async Task SubmitProgressAsync(Guid challengeId, SubmitProgressRequest request)
        {

            // Check if participant exists
            var participant = await _context.Participants
                .FirstOrDefaultAsync(p => p.ChallengeId == challengeId && p.UserId == request.UserId);

            if (participant == null)
            {
                // Create participant if not exists
                participant = new Participant
                {
                    Id = Guid.NewGuid(),
                    ChallengeId = challengeId,
                    UserId = request.UserId,
                    TotalProgress = 0
                };
                _context.Participants.Add(participant);
            }

            var progress = new ProgressEntry
            {
                Id = Guid.NewGuid(),
                ChallengeId = challengeId,
                UserId = request.UserId,
                Value = request.Value,
                Timestamp = request.Timestamp
            };

            _context.ProgressEntries.Add(progress);

            // Save to DB (later replaced with queue + background worker)
            await _context.SaveChangesAsync();

            // Update Redis leaderboard
            string key = LeaderboardKeyPrefix + challengeId;
            await _redis.SortedSetIncrementAsync(key, request.UserId, request.Value);
        }

        public async Task<IReadOnlyList<LeaderboardEntryResponse>> GetLeaderboardAsync(Guid challengeId)
        {
            string key = LeaderboardKeyPrefix + challengeId;

            // Try Redis first
            var redisLeaderboard = await _redis.SortedSetRangeByRankWithScoresAsync(key, 0, 9, Order.Descending);

            if (redisLeaderboard.Length > 0)
            {
                return redisLeaderboard.Select(x => new LeaderboardEntryResponse
                {
                    UserId = x.Element!,
                    TotalProgress = (long)x.Score
                }).ToList();
            }

            // Fallback: query DB and repopulate Redis
            var dbLeaderboard = await _context.Participants
                .Where(p => p.ChallengeId == challengeId)
                .OrderByDescending(p => p.TotalProgress)
                .Take(10)
                .Select(p => new LeaderboardEntryResponse
                {
                    UserId = p.UserId,
                    TotalProgress = p.TotalProgress
                })
                .ToListAsync();

            foreach (var entry in dbLeaderboard)
            {
                await _redis.SortedSetAddAsync(key, entry.UserId, entry.TotalProgress);
            }

            return dbLeaderboard;
        }

        public async Task<IReadOnlyList<ChallengeResponse>> GetActiveChallengesForUserAsync(string userId)
        {
            var now = DateTime.UtcNow;

            return await _context.Challenges
                .Where(c => c.StartDate <= now && c.EndDate >= now
                            && c.Participants.Any(p => p.UserId == userId))
                .Select(c => new ChallengeResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    Goal = c.Goal
                })
                .ToListAsync();
        }
    }
}