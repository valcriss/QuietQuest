using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuietQuestAdmin.Models
{
    public class ClientStatus
    {
        public bool Active { get; set; }
        public int Threshold { get; set; }
        public bool IsPenaltyRunning { get; set; }
        public string LastPenaltyName { get; set; } = string.Empty;
        public DateTime? LastTriggered { get; set; }
    }
}
