using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeWellness.Core.Models
{
    public class JoinChallengeRequest
    {
        public Guid ChallengeId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
