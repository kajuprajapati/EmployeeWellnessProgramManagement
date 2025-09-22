using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeWellness.Core.Models
{
    public class LeaderboardEntryResponse
    {
        public string UserId { get; set; } = string.Empty;
        public long TotalProgress { get; set; }
    }
}
