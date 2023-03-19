using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PcrBlazor.Shared
{
    public class EnemyParameter
    {
        [Key]
        public int EnemyId { get; set; }
        public int UnitId { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Rarity { get; set; }
        public int PromotionLevel { get; set; }
        public int Hp { get; set; }
        public int Atk { get; set; }
        public int MagicStr { get; set; }
        public int Def { get; set; }
        public int MagicDef { get; set; }
        public int PhysicalCritical { get; set; }
        public int MagicCritical { get; set; }
        public int WaveHpRecovery { get; set; }
        public int WaveEnergyRecovery { get; set; }
        public int Dodge { get; set; }
        public int PhysicalPenetrate { get; set; }
        public int MagicPenetrate { get; set; }
        public int LifeSteal { get; set; }
        public int HpRecoveryRate { get; set; }
        public int EnergyRecoveryRate { get; set; }
        public int EnergyReduceRate { get; set; }
        public int Accuracy { get; set; }
        public int ResistStatusId { get; set; }
        [ForeignKey(nameof(UnitId))]
        public UnitEnemyData EnemyData { get; set; }
        [ForeignKey(nameof(EnemyId))]
        public EnemyMParts EnemyMParts { get; set; }
        [ForeignKey(nameof(ResistStatusId))]
        public ResistData ResistData { get; set; }

        public int UnionBurstLevel { get; set; }
        public int MainSkillLv_1 { get; set; }
        public int MainSkillLv_2 { get; set; }
        public int MainSkillLv_3 { get; set; }
        public int MainSkillLv_4 { get; set; }
        public int MainSkillLv_5 { get; set; }
        public int MainSkillLv_6 { get; set; }
        public int MainSkillLv_7 { get; set; }
        public int MainSkillLv_8 { get; set; }
        public int MainSkillLv_9 { get; set; }
        public int MainSkillLv_10 { get; set; }
    }
}
