namespace PcrBlazor.Shared
{
    public interface IUnitStatus
    {
        double Accuracy { get; set; }
        double Atk { get; set; }
        double Def { get; set; }
        double Dodge { get; set; }
        double EnergyRecoveryRate { get; set; }
        double EnergyReduceRate { get; set; }
        double Hp { get; set; }
        double HpRecoveryRate { get; set; }
        double LifeSteal { get; set; }
        double MagicCritical { get; set; }
        double MagicDef { get; set; }
        double MagicPenetrate { get; set; }
        double MagicStr { get; set; }
        double PhysicalCritical { get; set; }
        double PhysicalPenetrate { get; set; }
        double WaveEnergyRecovery { get; set; }
        double WaveHpRecovery { get; set; }
    }
}