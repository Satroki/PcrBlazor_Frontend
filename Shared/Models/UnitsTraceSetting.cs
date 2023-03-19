using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PcrBlazor.Shared
{
    public class UnitsTraceSetting : IUserId
    {
        [Key]
        [DatabaseGenerat‌ed(DatabaseGeneratedOp‌​tion.None)]
        public int UserId { get; set; }

        public List<UnitTrace> UnitTraces { get; set; }
    }

    public class UnitTrace
    {
        public int UnitId { get; set; }
        public int Rank { get; set; }
        [NotMapped]
        [JsonIgnore]
        public UnitData Unit { get; set; }
        public UnitTraceType TraceType { get; set; }

        public UnitTrace()
        {

        }

        public UnitTrace(UnitData ud)
        {
            Unit = ud;
            UnitId = ud.UnitId;
            TraceType = UnitTraceType.Max;
        }
    }

    public enum UnitTraceType
    {
        Ignore = 0,
        Max = 1,
        Rank = 2,
    }
}
