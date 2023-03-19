using System;
using System.Collections.Generic;
using System.Text;

namespace PcrBlazor.Shared
{
   public class UpdateLog
    {
        public int Id { get; set; }
        public DateTimeOffset DateTime { get; set; }
        public string Log { get; set; }
        public string Version { get; set; }
    }
}
