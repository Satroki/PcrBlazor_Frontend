using System;
using System.Collections.Generic;
using System.Text;

namespace PcrBlazor.Shared
{
    public class UnitSkillModel
    {
        public SkillModel UnionBurst { get; set; }
        public SkillModel UnionBurstEvo { get; set; }
        public SkillModel Main1 { get; set; }
        public SkillModel Main1Evo { get; set; }
        public SkillModel Main2 { get; set; }
        public SkillModel Main2Evo { get; set; }
        public SkillModel Ex { get; set; }
        public SkillModel ExEvo { get; set; }
        public SkillModel Sp1 { get; set; }
        public SkillModel Sp1Evo { get; set; }
        public SkillModel Sp2 { get; set; }
        public SkillModel Sp2Evo { get; set; }
        public SkillModel Sp3 { get; set; }
        public SkillModel Sp4 { get; set; }
        public SkillModel Sp5 { get; set; }
        public SkillModel Main3 { get; set; }
        public SkillModel Main4 { get; set; }
        public SkillModel Main5 { get; set; }
        public SkillModel Main6 { get; set; }
        public SkillModel Main7 { get; set; }
        public SkillModel Main8 { get; set; }
        public SkillModel Main9 { get; set; }
        public SkillModel Main10 { get; set; }


        public IEnumerable<SkillModel> GetSkills()
        {
            var props = typeof(UnitSkillModel).GetProperties();
            foreach (var p in props)
            {
                if (p.GetValue(this) is SkillModel m)
                    yield return m;
            }
        }
    }
}
