using System;
using System.Collections.Generic;
using System.Text;

namespace PcrBlazor.Shared
{
    public class UnitFilter
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string LevelFilter { get; set; }
        public string RarityFilter { get; set; }
        public string PromotionFilter { get; set; }
    }

    public class UnitFilterResult
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public List<UserBoxLine> Lines { get; set; }
    }
}
