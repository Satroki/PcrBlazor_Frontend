using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PcrBlazor.Shared
{
    public class UniqueEquipmentData : IUnitStatus
    {
        [Key]
        public int EquipmentId { get; set; }
        [NotMapped]
        public int UnitId { get; set; }
        public string EquipmentName { get; set; }
        public string Description { get; set; }
        public int PromotionLevel { get; set; }
        public int CraftFlg { get; set; }
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
        public string DescriptionCn { get; set; }
        [NotMapped]
        public string EquipmentNameCn { get; set; }
    }
}
