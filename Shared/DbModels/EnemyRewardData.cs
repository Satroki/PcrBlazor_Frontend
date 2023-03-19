using System.ComponentModel.DataAnnotations;

namespace PcrBlazor.Shared
{
    public class EnemyRewardData
    {
        [Key]
        public int DropRewardId { get; set; }
        public int DropCount { get; set; }
        public int RewardType_1 { get; set; }
        public int RewardId_1 { get; set; }
        public int RewardNum_1 { get; set; }
        public int Odds_1 { get; set; }
        public int RewardType_2 { get; set; }
        public int RewardId_2 { get; set; }
        public int RewardNum_2 { get; set; }
        public int Odds_2 { get; set; }
        public int RewardType_3 { get; set; }
        public int RewardId_3 { get; set; }
        public int RewardNum_3 { get; set; }
        public int Odds_3 { get; set; }
        public int RewardType_4 { get; set; }
        public int RewardId_4 { get; set; }
        public int RewardNum_4 { get; set; }
        public int Odds_4 { get; set; }
        public int RewardType_5 { get; set; }
        public int RewardId_5 { get; set; }
        public int RewardNum_5 { get; set; }
        public int Odds_5 { get; set; }
    }
}
