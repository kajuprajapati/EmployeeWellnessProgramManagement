using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeWellness.Core.Entities
{
    public class Challenge
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // change from int to Guid
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Goal { get; set; } = string.Empty;

        public ICollection<Participant> Participants { get; set; } = new List<Participant>();
        public ICollection<ProgressEntry> ProgressEntries { get; set; } = new List<ProgressEntry>();
    }

}
