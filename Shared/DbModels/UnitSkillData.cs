using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PcrBlazor.Shared
{
    public class UnitSkillData
    {
        [Key]
        public int UnitId { get; set; }
        public int UnionBurst { get; set; }
        public int MainSkill_1 { get; set; }
        public int MainSkill_2 { get; set; }
        public int ExSkill_1 { get; set; }
        public int ExSkillEvolution_1 { get; set; }
        public int SpSkill_1 { get; set; }
        public int SpSkill_2 { get; set; }
        public int SpSkill_3 { get; set; }
        public int SpSkill_4 { get; set; }
        public int SpSkill_5 { get; set; }
        public int UnionBurstEvolution { get; set; }
        public int MainSkillEvolution_1 { get; set; }
        public int MainSkillEvolution_2 { get; set; }
        public int SpSkillEvolution_1 { get; set; }
        public int SpSkillEvolution_2 { get; set; }
        public int MainSkill_3 { get; set; }
        public int MainSkill_4 { get; set; }
        public int MainSkill_5 { get; set; }
        public int MainSkill_6 { get; set; }
        public int MainSkill_7 { get; set; }
        public int MainSkill_8 { get; set; }
        public int MainSkill_9 { get; set; }
        public int MainSkill_10 { get; set; }


        public IEnumerable<int> GetSkillIds()
        {
            var props = typeof(UnitSkillData).GetProperties().Where(p => p.Name != nameof(UnitId));
            foreach (var p in props)
            {
                var id = (int)p.GetValue(this);
                if (id > 0)
                    yield return id;
            }
        }
    }
}
