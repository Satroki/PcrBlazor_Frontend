using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PcrBlazor.Shared
{
    public class EquipmentData : IUnitStatus, IEquipmentId
    {
        [Key]
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public string Description { get; set; }
        /// <summary>
        /// 3银 4金 5紫
        /// </summary>
        public int PromotionLevel { get; set; }
        public bool CraftFlg { get; set; }
        public int EquipmentEnhancePoint { get; set; }
        public int SalePrice { get; set; }
        public int RequireLevel { get; set; }
        public double Hp { get; set; }
        public double Atk { get; set; }
        public double MagicStr { get; set; }
        public double Def { get; set; }
        public double MagicDef { get; set; }
        public double PhysicalCritical { get; set; }
        public double MagicCritical { get; set; }
        public double WaveHpRecovery { get; set; }
        public double WaveEnergyRecovery { get; set; }
        public double Dodge { get; set; }
        public double PhysicalPenetrate { get; set; }
        public double MagicPenetrate { get; set; }
        public double LifeSteal { get; set; }
        public double HpRecoveryRate { get; set; }
        public double EnergyRecoveryRate { get; set; }
        public double EnergyReduceRate { get; set; }
        public int EnableDonation { get; set; }
        public double Accuracy { get; set; }

        [NotMapped]
        [JsonIgnore]
        public List<SimpleEquipmentData> ConsumedEquips { get; set; }
        [NotMapped]
        [JsonIgnore]
        public List<SimpleEquipmentData> ConsumeByEquips { get; set; }
        [NotMapped]
        public bool IsVisible { get; set; }

        [NotMapped]
        [JsonIgnore]
        public EquipmentData EquipmentDataCn { get; set; }
    }
}
