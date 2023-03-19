using System;
using System.Collections.Generic;
using System.Text;

namespace PcrBlazor.Shared
{
    public class ServerNames
    {
        public int Key { get; set; }
        public string CnName { get; set; }
        public string JpName { get; set; }

        public string GetName(string s)
        {
            return s switch
            {
                "cn" => CnName ?? JpName,
                _ => JpName
            };
        }
    }
}
