using System;
using System.ComponentModel.DataAnnotations;

namespace PcrBlazor.Shared
{
    public class CampaignFreegacha
    {
        [Key]
        public int Id { get; set; }
        public int CampaignId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
    }
}
