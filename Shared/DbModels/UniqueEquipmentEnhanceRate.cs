using System.ComponentModel.DataAnnotations;

namespace PcrBlazor.Shared
{
    public class UniqueEquipEnhanceRate : IUnitStatus
    {
        [Key]
        public int Id { get; set; }
        public int EquipmentId { get; set; }
        public int MinLv { get; set; }
        public int MaxLv { get; set; }
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
        public double Accuracy { get; set; }
    }

    public class UniqueEquipmentEnhanceRate : IUnitStatus
    {
        [Key]
        public int EquipmentId { get; set; }
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
        public double Accuracy { get; set; }

        public UniqueEquipEnhanceRate ToUniqueEquipEnhanceRate()
        {
            var uer = new UniqueEquipEnhanceRate()
            {
                Id = EquipmentId,
                EquipmentId = EquipmentId,
                MaxLv = -1,
                MinLv = 2,
            };
            this.StatusCopy(uer);
            return uer;
        }
    }
}
