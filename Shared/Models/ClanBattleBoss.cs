using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace PcrBlazor.Shared
{
    public class ClanBattleBoss
    {
        public int UnitId { get; set; }
        public int EnemyId { get; set; }
        public double ScoreCoefficient { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public int Level { get; set; }
        public int Hp { get; set; }
        public int Atk { get; set; }
        public int MagicStr { get; set; }
        public int Def { get; set; }
        public int MagicDef { get; set; }
        public int PhysicalCritical { get; set; }
        public int MagicCritical { get; set; }
        public int Dodge { get; set; }
        public int Accuracy { get; set; }
        public int EnergyRecoveryRate { get; set; }
        public List<ClanBattleBoss> Parts { get; set; }

        public AttackPattern[] AttackPatterns { get; set; }
        public UnitSkillModel UnitSkillModel { get; set; }
        public List<AilmentData> AilmentsResist { get; set; }
        [JsonIgnore]
        public bool ShowInfo { get; set; }

        public ClanBattleBoss()
        {
            
        }

        public ClanBattleBoss(EnemyParameter enemy)
        {
            Name = enemy.Name;
            UnitId = enemy.UnitId;
            EnemyId = enemy.EnemyId;
            Level = enemy.Level;
            Hp = enemy.Hp;
            Atk = enemy.Atk;
            MagicStr = enemy.MagicStr;
            Def = enemy.Def;
            MagicDef = enemy.MagicDef;
            PhysicalCritical = enemy.PhysicalCritical;
            MagicCritical = enemy.MagicCritical;
            Dodge = enemy.Dodge;
            Accuracy = enemy.Accuracy;
            EnergyRecoveryRate = enemy.EnergyRecoveryRate;
            var unit = enemy.EnemyData;
            if (unit != null)
            {
                Name = unit.UnitName;
                Comment = unit.Comment.Replace("\\n", "").Replace("\n", "").Replace("　", "").Replace("·", "・").Replace("・", "\n・");
            }
            var parts = enemy.EnemyMParts;
            if (parts != null && parts.ChildEnemyParameters.Any())
            {
                Parts = parts.ChildEnemyParameters.OrderBy(p => p.EnemyId).Select(p => new ClanBattleBoss(p)
                {
                    Level = 0,
                    Accuracy = 0,
                    Dodge = 0
                }).ToList();
            }
        }

        public ClanBattleBoss(ClanBattleBossGroup bossGroup) : this(bossGroup.WaveGroupData.Enemy1)
        {
            ScoreCoefficient = bossGroup.ScoreCoefficient;
        }

        public ClanBattleBoss(WaveGroupData wave, double v) : this(wave.Enemy1)
        {
            ScoreCoefficient = v;
        }

        public IEnumerable<(string, int)> GetStatus()
        {
            var names = UnitStatus.Names;
            yield return ("等级", Level);
            yield return (names[nameof(Hp)], Hp);
            yield return (names[nameof(Atk)], Atk);
            yield return (names[nameof(MagicStr)], MagicStr);
            yield return (names[nameof(Def)], Def);
            yield return (names[nameof(MagicDef)], MagicDef);
            yield return (names[nameof(PhysicalCritical)], PhysicalCritical);
            yield return (names[nameof(MagicCritical)], MagicCritical);
            yield return (names[nameof(Accuracy)], Accuracy);
            yield return (names[nameof(Dodge)], Dodge);
            yield return (names[nameof(EnergyRecoveryRate)], EnergyRecoveryRate);
        }

        public Dictionary<string, int> GetSkillStatus()
        {
            var names = UnitStatus.Names;
            var dict = new Dictionary<string, int>();
            dict.Add("技能等级", 0);
            dict.Add(names[nameof(Atk)], GetChildValue(this, b => b.Atk));
            dict.Add(names[nameof(MagicStr)], GetChildValue(this, b => b.MagicStr));
            if (Def > 0)
                dict.Add(names[nameof(Def)], Def);
            if (MagicDef > 0)
                dict.Add(names[nameof(MagicDef)], MagicDef);
            return dict;
        }

        private static int GetChildValue(ClanBattleBoss b, Func<ClanBattleBoss, int> val)
        {
            var v = val(b);
            if (v > 0)
                return v;
            else if (!b.Parts.IsNullOrEmpty())
            {
                foreach (var p in b.Parts)
                {
                    v = val(p);
                    if (v > 0)
                        return v;
                }
            }
            return 0;
        }
    }
}
