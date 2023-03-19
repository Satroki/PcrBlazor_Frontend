using System.ComponentModel;

namespace PcrBlazor.Shared
{
    internal class AuraAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var percent = GetPercent(a.ActionValue_1);
            actionValues.Add(new ActionValue(a.ActionValue_2, a.ActionValue_3, null));
            durationValues.Add(new ActionValue(a.ActionValue_4, a.ActionValue_5, null));
            var auraActionType = (a.ActionDetail_1 % 10).ParseEnum(AuraActionType.Raise);
            var auraType = a.ActionDetail_1 == 1 ? AuraType.MaxHP : (a.ActionDetail_1 / 10).ParseEnum(AuraType.None);
            var breakType = a.ActionDetail_2.ParseEnum(BreakType.Unknown);
            var t = targetParameter.BuildTargetClause();
            //141 クリティカルを受けた際のダメージを12％アップ
            if (a.ActionDetail_1 == 141)
                return $"增加{t}受到的暴击伤害【{BuildExpression()}%】，持续【{BuildExpression(durationValues)}】秒";
            return breakType switch
            {
                BreakType.Break => $"Break期间，{auraActionType.GetDescription()}{t}【{BuildExpression()}{percent}】{GetAuraTypeDesp(auraType)}",
                _ => $"{auraActionType.GetDescription()}{t}【{BuildExpression()}{percent}】的{GetAuraTypeDesp(auraType)}，持续【{BuildExpression(durationValues)}】秒",
            };
        }
        private string GetAuraTypeDesp(AuraType at)
        {
            return at.GetDescription() ?? UnitStatus.Names.GetValue(at.ToString(), string.Empty);
        }
    }
}