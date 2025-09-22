using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeWellness.Core.Entities
{
    public class Participant
    {
        public Guid Id { get; set; }
        public Guid ChallengeId { get; set; } // changed to int
        public string UserId { get; set; } = null!;
        public long TotalProgress { get; set; }

        public Challenge? Challenge { get; set; }
    }
}
