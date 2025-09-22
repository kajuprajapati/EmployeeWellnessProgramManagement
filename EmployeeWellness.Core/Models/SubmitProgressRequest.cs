using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeWellness.Core.Models
{
    public class SubmitProgressRequest
    {
        public string UserId { get; set; } = string.Empty;
        public long Value { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
