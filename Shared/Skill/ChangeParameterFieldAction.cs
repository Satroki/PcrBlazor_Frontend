namespace PcrBlazor.Shared
{
    internal class ChangeParameterFieldAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var percent = GetPercent(a.ActionDetail_2);
            actionValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            durationValues.Add(new ActionValue(a.ActionValue_3, a.ActionValue_4, null));
            var auraActionType = (a.ActionDetail_1 % 10).ParseEnum(AuraActionType.Raise).GetDescription();
            var auraType = (a.ActionDetail_1 / 10).ParseEnum(AuraType.None);
            var t = targetParameter.BuildTargetClause();
            var d = BuildExpression(durationValues);
            var expr = BuildExpression();
            return targetParameter.targetType switch
            {
                TargetType.Absolute => $"展开半径为【{a.ActionValue_5.ToString(withNo)}】的领域，{auraActionType}{t}【{expr}{percent}】{GetAuraTypeDesp(auraType)}，持续【{d}】秒",
                _ => $"在{t}的位置展开半径为【{a.ActionValue_5.ToString(withNo)}】的领域，{auraActionType}【{expr}{percent}】{GetAuraTypeDesp(auraType)}，持续【{d}】秒"
            };
        }

        private string GetAuraTypeDesp(AuraType at)
        {
            return at.GetDescription() ?? UnitStatus.Names.GetValue(at.ToString(), string.Empty);
        }
    }
}