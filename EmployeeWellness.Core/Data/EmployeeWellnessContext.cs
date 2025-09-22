using EmployeeWellness.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeWellness.Core.Data
{
    public class EmployeeWellnessContext : DbContext
    {
        public EmployeeWellnessContext(DbContextOptions<EmployeeWellnessContext> options) : base(options) { }
        public DbSet<Challenge> Challenges => Set<Challenge>();
        public DbSet<Participant> Participants => Set<Participant>();
        public DbSet<ProgressEntry> ProgressEntries => Set<ProgressEntry>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Challenge
            modelBuilder.Entity<Challenge>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasMany(x => x.Participants)
                 .WithOne(p => p.Challenge)
                 .HasForeignKey(p => p.ChallengeId);

                b.HasMany(x => x.ProgressEntries)
                 .WithOne()
                 .HasForeignKey(pe => pe.ChallengeId);
            });

            // Participant
            modelBuilder.Entity<Participant>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasIndex(x => new { x.ChallengeId, x.UserId }).IsUnique();
                b.HasIndex(x => new { x.ChallengeId, x.TotalProgress });
            });

            // ProgressEntry
            modelBuilder.Entity<ProgressEntry>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasIndex(x => new { x.ChallengeId, x.UserId });
            });
        }

    }
}
