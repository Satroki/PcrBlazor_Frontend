using System;
using System.ComponentModel.DataAnnotations;

namespace PcrBlazor.Shared
{
    public class TowerSchedule
    {
        [Key]
        public int TowerScheduleId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}
