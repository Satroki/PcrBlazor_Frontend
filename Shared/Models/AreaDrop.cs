using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcrBlazor.Shared
{
    public class AreaDrop
    {
        [Key]
        [DatabaseGenerat‌ed(DatabaseGeneratedOp‌​tion.None)]
        public int Area { get; set; }
        public List<int> ItemIds { get; set; }
        [NotMapped]
        public string Url { get; set; }
    }
}
