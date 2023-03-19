using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PcrBlazor.Shared
{
    public class ClanBattle2MapData
    {
        [Key]
        public int Id { get; set; }
        public int ClanBattleId { get; set; }
        public int LapNumFrom { get; set; }
        public int LapNumTo { get; set; }
        public int WaveGroupId_1 { get; set; }
        public int WaveGroupId_2 { get; set; }
        public int WaveGroupId_3 { get; set; }
        public int WaveGroupId_4 { get; set; }
        public int WaveGroupId_5 { get; set; }
        public double ScoreCoefficient_1 { get; set; }
        public double ScoreCoefficient_2 { get; set; }
        public double ScoreCoefficient_3 { get; set; }
        public double ScoreCoefficient_4 { get; set; }
        public double ScoreCoefficient_5 { get; set; }
        [NotMapped]
        public List<WaveGroupData> WaveGroupDatas { get; set; }
        [NotMapped]
        public int[] WaveGroupIds => new[] { WaveGroupId_1, WaveGroupId_2, WaveGroupId_3, WaveGroupId_4, WaveGroupId_5 };
        [NotMapped]
        public double[] ScoreCoefficients => new[] { ScoreCoefficient_1, ScoreCoefficient_2, ScoreCoefficient_3, ScoreCoefficient_4, ScoreCoefficient_5, };
    }
}
