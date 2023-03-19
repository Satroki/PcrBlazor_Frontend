using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcrBlazor.Shared
{
    public class JdUnionAd
    {
        [Key]
        [DatabaseGenerat‌ed(DatabaseGeneratedOp‌​tion.None)]
        public long SkuId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Url { get; set; }
    }
}
