using System;
using System.Collections.Generic;

namespace PcrBlazor.Shared
{
    public class UnitSourceData
    {
        public int UnitId { get; set; }
        public List<UnitRarity> UnitRarities { get; set; }
        public List<UnitPromotion> UnitPromotions { get; set; }
        public List<UnitPromotionStatus> UnitPromotionStatuses { get; set; }
        public List<CharaStoryStatus> CharaStoryStatuses { get; set; }
        public UniqueEquipmentData UniqueEquipmentData { get; set; }
        //public UniqueEquipEnhanceRate UniqueEquipmentEnhanceRate { get; set; }
        public List<UniqueEquipEnhanceRate> UniqueEquipmentEnhanceRates { get; set; }
        public List<PromotionBonus> PromotionBonus { get; set; }
        public List<PromotionBonus> PromotionBonusCn { get; set; }
        public DateTimeOffset? LastModified { get; set; }
    }
}
