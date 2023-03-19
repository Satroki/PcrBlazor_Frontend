using System.Collections.Generic;

namespace PcrBlazor.Shared
{
    internal class AccumulativeDamageAction : SkillActionParameter
    {
        private List<ActionValue> stackValues = new List<ActionValue>();
        public override string GetDescription()
        {
            actionValues.Add(new ActionValue(a.ActionValue_2, a.ActionValue_3, null));
            stackValues.Add(new ActionValue(a.ActionValue_4, a.ActionValue_5, null));
            return $"每次攻击当前的目标，将会追加【{BuildExpression()}】伤害，本效果最多叠【{BuildExpression(stackValues)}】层";
        }
    }
}