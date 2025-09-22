using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeWellness.Core.Entities
{
    public class ProgressEntry
    {
        public Guid Id { get; set; }
        public Guid ChallengeId { get; set; }
        public string UserId { get; set; } = null!;
        public long Value { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
