using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuietQuestShared.Models
{
    public class Config
    {
        public bool Active { get; set; } = true;
        public int Threshold { get; set; } = 10;
        public int AlertCount { get; set; } = 3;
    }
}
