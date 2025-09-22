using EmployeeWellness.Core.Data;
using EmployeeWellness.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using System;
using System.Text.Json;

namespace EmployeeWellness.Core.Services
{
    public class ProgressWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IDatabase _redis;

        private const string QueueKey = "progress-queue";
        private const string LeaderboardKeyPrefix = "leaderboard:";

        public ProgressWorker(IServiceScopeFactory scopeFactory, IConnectionMultiplexer redis)
        {
            _scopeFactory = scopeFactory;
            _redis = redis.GetDatabase();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var batch = new List<ProgressEntry>();

                for (int i = 0; i < 50; i++) // batch size = 50
                {
                    var data = await _redis.ListLeftPopAsync(QueueKey);
                    if (data.IsNullOrEmpty) break;

                    var msg = JsonSerializer.Deserialize<ProgressEntry>(data!);
                    if (msg != null)
                        batch.Add(msg);
                }

                if (batch.Count > 0)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<EmployeeWellnessContext>();

                    foreach (var msg in batch)
                    {
                        db.ProgressEntries.Add(new ProgressEntry
                        {
                            ChallengeId = msg.ChallengeId,
                            UserId = msg.UserId,
                            Value = msg.Value,
                            Timestamp = msg.Timestamp
                        });

                        // update leaderboard in Redis
                        await _redis.SortedSetIncrementAsync(
                            LeaderboardKeyPrefix + msg.ChallengeId,
                            msg.UserId,
                            msg.Value);
                    }

                    await db.SaveChangesAsync();
                }

                await Task.Delay(500, stoppingToken); // poll interval
            }
        }
    }
}