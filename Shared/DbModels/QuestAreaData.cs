using System.ComponentModel.DataAnnotations;

namespace PcrBlazor.Shared
{
    public class QuestAreaData
    {
        [Key]
        public int AreaId { get; set; }
        public string AreaName { get; set; }
    }
}
