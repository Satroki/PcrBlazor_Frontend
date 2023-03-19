using System;
using System.ComponentModel.DataAnnotations;

namespace PcrBlazor.Shared
{
    public class ClanBattlePeriod
    {
        [Key]
        public int ClanBattleId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}
