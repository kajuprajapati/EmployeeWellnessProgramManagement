using EmployeeWellness.Core.Entities;
using EmployeeWellness.Core.Interfaces;
using EmployeeWellness.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeWellness.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChallengesController : ControllerBase
    {
        private readonly IChallengeService _challengeService;

        public ChallengesController(IChallengeService challengeService)
        {
            _challengeService = challengeService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateChallenge([FromBody] CreateChallengeRequest request)
        {
            var challengeId = await _challengeService.CreateChallengeAsync(request);
            return CreatedAtAction(nameof(GetLeaderboard), new { challengeId }, request);
        }

        [HttpPost("{challengeId}/progress")]
        public async Task<IActionResult> SubmitProgress(Guid challengeId, [FromBody] SubmitProgressRequest request)
        {
            await _challengeService.SubmitProgressAsync(challengeId, request);
            return Ok();
        }

        [HttpGet("{challengeId}/leaderboard")]
        public async Task<IActionResult> GetLeaderboard(Guid challengeId)
        {
            var leaderboard = await _challengeService.GetLeaderboardAsync(challengeId);
            return Ok(leaderboard);
        }

        [HttpGet("/api/users/{userId}/challenges/active")]
        public async Task<IActionResult> GetActiveChallengesForUser(string userId)
        {
            var challenges = await _challengeService.GetActiveChallengesForUserAsync(userId);
            return Ok(challenges);
        }
    }

}
