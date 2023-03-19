using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace PcrBlazor.Shared
{
    public class UnitNickNames
    {
        [Key]
        [DatabaseGenerat‌ed(DatabaseGeneratedOp‌​tion.None)]
        public int UnitId { get; set; }
        public List<string> NickNames { get; set; } = new List<string>();
        public List<string> Requests { get; set; } = new List<string>();

        [NotMapped]
        [JsonIgnore]
        public string TempNames { get; set; }

        public bool Contains(string name)
        {
            return NickNames.Any(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase));
        }

        public void AddNewNickName(string s)
        {
            if (!NickNames.Contains(s))
                NickNames.Add(s);
        }

        public void AddNewNickNames(IEnumerable<string> s)
        {
            NickNames = NickNames.Union(s).ToList();
        }

        public void AddTempName(string s)
        {
            if (!Requests.Contains(s) && !NickNames.Contains(s))
                Requests.Add(s);
        }

        public void AddTempNames(IEnumerable<string> s)
        {
            Requests = Requests.Union(s).ToList();
        }

        public override string ToString()
        {
            return string.Join(",", NickNames);
        }

        public string ToString(bool req)
        {
            return string.Join(",", Requests);
        }
    }

    public class UnitNameCn
    {
        [Key]
        [DatabaseGenerat‌ed(DatabaseGeneratedOp‌​tion.None)]
        public int UnitId { get; set; }
        public string Name { get; set; }
    }
}
