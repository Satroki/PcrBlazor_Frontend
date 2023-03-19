using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PcrBlazor.Shared
{
    public class WaveGroupData
    {
        [Key]
        public int Id { get; set; }
        public int WaveGroupId { get; set; }
        public int EnemyId_1 { get; set; }
        public int DropRewardId_1 { get; set; }
        public int DropRewardId_2 { get; set; }
        public int DropRewardId_3 { get; set; }
        public int DropRewardId_4 { get; set; }
        public int DropRewardId_5 { get; set; }

        [NotMapped]
        public List<EnemyRewardData> RewardDatas { get; set; }

        [ForeignKey(nameof(EnemyId_1))]
        public EnemyParameter Enemy1 { get; set; }
    }
}
