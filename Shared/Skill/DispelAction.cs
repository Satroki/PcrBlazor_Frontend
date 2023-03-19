using System.Collections.Generic;

namespace PcrBlazor.Shared
{
    internal class DispelAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var dispelType = a.ActionDetail_1.ParseEnum(DispelType.Unknown);
            chanceValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));

            return $"【{BuildExpression(chanceValues)}%】的概率使{targetParameter.BuildTargetClause()}的{dispelType.GetDescription()}全部解除";
        }
    }
}