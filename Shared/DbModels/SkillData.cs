using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PcrBlazor.Shared
{
    public class SkillData
    {
        [Key]
        public int SkillId { get; set; }
        public string Name { get; set; }
        public int SkillType { get; set; }
        public int SkillAreaWidth { get; set; }
        public double SkillCastTime { get; set; }
        public double BossUbCoolTime { get; set; }
        public int Action_1 { get; set; }
        public int Action_2 { get; set; }
        public int Action_3 { get; set; }
        public int Action_4 { get; set; }
        public int Action_5 { get; set; }
        public int Action_6 { get; set; }
        public int Action_7 { get; set; }
        public int DependAction_1 { get; set; }
        public int DependAction_2 { get; set; }
        public int DependAction_3 { get; set; }
        public int DependAction_4 { get; set; }
        public int DependAction_5 { get; set; }
        public int DependAction_6 { get; set; }
        public int DependAction_7 { get; set; }
        public string Description { get; set; }
        public int IconType { get; set; }

        public IEnumerable<(int actionId, int dependActionId)> GetActionIds()
        {
            var props = typeof(SkillData).GetProperties().ToDictionary(p => p.Name);
            for (int i = 1; i <= 7; i++)
            {
                var id = (int)props[$"Action_{i}"].GetValue(this);
                if (id == 0)
                    break;
                var did = (int)props[$"DependAction_{i}"].GetValue(this);
                yield return (id, did);
            }
        }
    }
}
