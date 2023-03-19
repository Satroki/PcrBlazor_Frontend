using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace PcrBlazor.Shared
{
    public class EventCalendar
    {
        public int Id { get; set; }
        public EventType Type { get; set; }
        public string Title { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        [NotMapped]
        [JsonIgnore]
        public DateTime LocalStartTime
        {
            get => StartTime.LocalDateTime;
            set => StartTime = value;
        }

        [NotMapped]
        [JsonIgnore]
        public DateTime LocalEndTime
        {
            get => EndTime.LocalDateTime;
            set => EndTime = value;
        }

        [NotMapped]
        [JsonIgnore]
        public string Display { get; set; }
        [NotMapped]
        [JsonIgnore]
        public string Icon { get; set; }
        [NotMapped]
        [JsonIgnore]
        public string ProgressType { get; set; }
    }

    public enum EventType
    {
        [Description("其他")]
        Other = 0,
        [Description("庆典活动")]
        Festival = 1,
        [Description("剧情活动")]
        Story = 2,
        [Description("团队战")]
        ClanBattle = 3,
        [Description("特别活动")]
        Special = 4,
        [Description("扭蛋")]
        Gacha = 5,
    }
}
