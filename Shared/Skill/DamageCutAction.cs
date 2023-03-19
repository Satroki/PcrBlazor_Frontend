using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcrBlazor.Shared
{
    internal class DamageCutAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var dt = a.ActionDetail_1.ParseEnum(DamageType.Phisical);
            actionValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            durationValues.Add(new ActionValue(a.ActionValue_3, a.ActionValue_4, null));
            return $"降低{targetParameter.BuildTargetClause()}受到的{dt.GetDescription()}伤害【{BuildExpression(actionValues)}%】，持续【{BuildExpression(durationValues)}】秒";
        }
    }

    internal enum DamageType
    {
        [Description("物理")]
        Phisical = 1,
        [Description("魔法")]
        Magical,
        [Description("")]
        All
    }
}
