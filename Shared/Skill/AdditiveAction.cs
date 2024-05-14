using System.Collections.Generic;

namespace PcrBlazor.Shared
{
    internal class AdditiveAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            actionValues.Add(new ActionValue(a.ActionValue_2, a.ActionValue_3, null));
            var limitValues = new List<ActionValue> { new ActionValue(a.ActionValue_4, a.ActionValue_5, null) };
            var av1 = a.ActionValue_1.Value;
            var propKey = (int)av1 switch
            {
                7 => nameof(UnitStatus.Atk),
                8 => nameof(UnitStatus.MagicStr),
                9 => nameof(UnitStatus.Def),
                10 => nameof(UnitStatus.MagicDef),
                _ => null
            };
            var pre = $"使【动作{a.ActionDetail_1 % 10}】的系数{a.ActionDetail_2}提高【{BuildExpression()} * ";
            var desp = (int)av1 switch
            {
                0 => $"{pre}HP】",
                1 => $"{pre}损失的HP】",
                2 => $"使【动作{a.ActionDetail_1 % 10}】的系数{a.ActionDetail_2}提高【{BuildExpression([new ActionValue(a.ActionValue_2 * 2, a.ActionValue_3, null)])} * 击杀的敌方数量】",// 乘2
                4 => $"{pre}目标的数量】",
                5 => $"{pre}受到伤害的目标数量】",
                6 => $"{pre}伤害量】",
                >= 7 and <= 10 => $"{pre}{targetParameter.BuildTargetClause()}的{UnitStatus.Names[propKey]}】",
                12 => $"{pre}后方的{targetParameter.BuildTargetClause()}数量】",
                13 => $"{pre}{targetParameter.BuildTargetClause()}损失的 HP / 最大 HP】",
                102 => $"{pre}omeme酱的数量】",
                >= 20 and < 30 => $"{pre}计数器的数量】",
                >= 200 and < 300 => $"{pre}标记【ID：{av1 % 2000}】的层数】",
                >= 2000 and < 3000 => $"{pre}标记【ID：{av1 % 200}】的层数】",
                _ => string.Empty
            };
            if (a.ActionValue_4 != 0 && a.ActionValue_5 != 0)
            {
                desp += $"，提高值的上限为【{BuildExpression(limitValues)}】";
            }
            return desp.IsNullOrEmpty() ? UnknownExpression() : desp;
        }
    }
}