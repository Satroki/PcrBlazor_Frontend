using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PcrBlazor.Shared
{
    public class QuestData
    {
        [Key]
        public int QuestId { get; set; }
        public int AreaId { get; set; }
        public string QuestName { get; set; }
        public int Stamina { get; set; }
        public int WaveGroupId_1 { get; set; }
        public int WaveGroupId_2 { get; set; }
        public int WaveGroupId_3 { get; set; }

        [NotMapped]
        public List<WaveGroupData> Waves { get; set; }
    }
}
