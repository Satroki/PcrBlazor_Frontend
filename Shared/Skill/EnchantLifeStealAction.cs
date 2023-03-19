using System.Collections.Generic;

namespace PcrBlazor.Shared
{
    internal class EnchantLifeStealAction : SkillActionParameter
    {
        private List<ActionValue> stackValues = new List<ActionValue>();
        public override string GetDescription()
        {
            actionValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            stackValues.Add(new ActionValue(a.ActionValue_3, a.ActionValue_4, null));
            return $"为{targetParameter.BuildTargetClause()}的下【{BuildExpression(stackValues)}】次攻击附加效果【{BuildExpression()}】{UnitStatus.Names[nameof(UnitStatus.LifeSteal)]}";
        }
    }
}