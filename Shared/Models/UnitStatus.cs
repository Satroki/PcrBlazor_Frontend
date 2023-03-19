using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PcrBlazor.Shared
{
    public class UnitStatus : IUnitStatus
    {
        public static IReadOnlyDictionary<string, string> Names = new Dictionary<string, string>
        {
            [nameof(Hp)] = "HP",
            [nameof(Atk)] = "物理攻击力",
            [nameof(Def)] = "物理防御力",
            [nameof(MagicStr)] = "魔法攻击力",
            [nameof(MagicDef)] = "魔法防御力",
            [nameof(PhysicalCritical)] = "物理暴击",
            [nameof(MagicCritical)] = "魔法暴击",
            [nameof(WaveHpRecovery)] = "HP自动回复",
            [nameof(WaveEnergyRecovery)] = "TP自动回复",
            [nameof(Dodge)] = "回避",
            [nameof(Accuracy)] = "命中",
            [nameof(PhysicalPenetrate)] = "物理穿透",
            [nameof(MagicPenetrate)] = "魔法穿透",
            [nameof(HpRecoveryRate)] = "回复量上升",
            [nameof(EnergyRecoveryRate)] = "TP上升",
            [nameof(LifeSteal)] = "HP吸收",
            [nameof(EnergyReduceRate)] = "TP消耗降低",
        };

        public static string[] StatusKeys = new[]
        {
            nameof(Hp),
            nameof(Atk),
            nameof(MagicStr),
            nameof(Def),
            nameof(MagicDef),
            nameof(PhysicalCritical),
            nameof(MagicCritical),
            nameof(WaveHpRecovery),
            nameof(WaveEnergyRecovery),
            nameof(Dodge),
            nameof(PhysicalPenetrate),
            nameof(MagicPenetrate),
            nameof(LifeSteal),
            nameof(HpRecoveryRate),
            nameof(EnergyRecoveryRate),
            nameof(EnergyReduceRate),
            nameof(Accuracy)
        };

        public Dictionary<string, double> StatusDict { get; set; }

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

        public Dictionary<string, double> GetStatusDiff(UnitStatus other)
        {
            return StatusKeys.ToDictionary(k => k, k => StatusDict[k] - other.StatusDict[k]);
        }
    }

    public class UnitComparedStatus
    {
        public int UnitId { get; set; }
        public UnitData Unit { get; set; }
        public UnitSourceData SourceData { get; set; }
        public UnitStatus OldStatus { get; set; }
        public UnitStatus NewStatus { get; set; }
        public Dictionary<string, double> StatusDiff { get; set; }

        public UnitComparedStatus()
        {

        }

        public UnitComparedStatus(UnitData u)
        {
            Unit = u;
            UnitId = u.UnitId;
        }
    }
}
