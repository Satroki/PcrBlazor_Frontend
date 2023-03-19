using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcrBlazor.Shared
{
    public class BoxCopySetting
    {
        public string BoxShareId { get; set; }
        public string BoxServer { get; set; }
        public string Server { get; set; }
        public bool CopyAll { get; set; }
        public List<string> Fields { get; set; }
        public bool AddIfNotExist { get; set; }
    }
}
