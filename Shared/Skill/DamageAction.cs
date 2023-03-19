using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PcrBlazor.Shared
{
    internal class DamageAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var damageClass = a.ActionDetail_1.ParseEnum(ClassModifier.Unknown);
            var critical = ((int)a.ActionValue_5.Value).ParseEnum(CriticalModifier.Normal);

            actionValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            var key = damageClass switch
            {
                ClassModifier.Unknown => null,
                ClassModifier.Magical => nameof(IUnitStatus.MagicStr),
                _ => nameof(IUnitStatus.Atk),
            };
            actionValues.Add(new ActionValue(a.ActionValue_3, a.ActionValue_4, key));

            var sb = new StringBuilder($"对{targetParameter.BuildTargetClause()}造成【{BuildExpression()}】{damageClass.GetDescription()}伤害");
            if (critical == CriticalModifier.Critical)
                sb.Append($"，且第{a.ActionValue_5.ToString(withNo)}Hit必定暴击");
            if (a.ActionValue_6 > 0)
            {
                sb.Append($"\n触发暴击时，爆伤倍率将变为【{2 * a.ActionValue_6}】倍");
            }
            if (a.ActionDetail_2 == 1)
            {
                sb.Append("，伤害类型会转换为目标防御较低的类型");
            }
            return sb.ToString();
        }
    }
}
