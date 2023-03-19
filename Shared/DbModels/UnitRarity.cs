namespace PcrBlazor.Shared
{
    public class UnitRarity : IUnitStatus
    {
        public int UnitId { get; set; }
        public int Rarity { get; set; }
        public double Hp { get; set; }
        public double HpGrowth { get; set; }
        public double Atk { get; set; }
        public double AtkGrowth { get; set; }
        public double MagicStr { get; set; }
        public double MagicStrGrowth { get; set; }
        public double Def { get; set; }
        public double DefGrowth { get; set; }
        public double MagicDef { get; set; }
        public double MagicDefGrowth { get; set; }
        public double PhysicalCritical { get; set; }
        public double PhysicalCriticalGrowth { get; set; }
        public double MagicCritical { get; set; }
        public double MagicCriticalGrowth { get; set; }
        public double WaveHpRecovery { get; set; }
        public double WaveHpRecoveryGrowth { get; set; }
        public double WaveEnergyRecovery { get; set; }
        public double WaveEnergyRecoveryGrowth { get; set; }
        public double Dodge { get; set; }
        public double DodgeGrowth { get; set; }
        public double PhysicalPenetrate { get; set; }
        public double PhysicalPenetrateGrowth { get; set; }
        public double MagicPenetrate { get; set; }
        public double MagicPenetrateGrowth { get; set; }
        public double LifeSteal { get; set; }
        public double LifeStealGrowth { get; set; }
        public double HpRecoveryRate { get; set; }
        public double HpRecoveryRateGrowth { get; set; }
        public double EnergyRecoveryRate { get; set; }
        public double EnergyRecoveryRateGrowth { get; set; }
        public double EnergyReduceRate { get; set; }
        public double EnergyReduceRateGrowth { get; set; }
        public int UnitMaterialId { get; set; }
        public int ConsumeNum { get; set; }
        public int ConsumeGold { get; set; }
        public double Accuracy { get; set; }
        public double AccuracyGrowth { get; set; }
    }
}
