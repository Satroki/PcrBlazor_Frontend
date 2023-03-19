namespace PcrBlazor.Shared
{
    internal class HealFieldAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var healClass = a.ActionDetail_1 % 2 == 0 ? ClassModifier.Magical : ClassModifier.Physical;
            var fieldType = a.ActionDetail_1 <= 2 ? FieldType.Normal : FieldType.Repeat;
            var p = GetPercent(a.ActionDetail_2);
            var key = healClass switch
            {
                ClassModifier.Magical => nameof(UnitStatus.MagicStr),
                _ => nameof(UnitStatus.Atk)
            };
            actionValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            actionValues.Add(new ActionValue(a.ActionValue_3, a.ActionValue_4, key));
            durationValues.Add(new ActionValue(a.ActionValue_5, a.ActionValue_6, null));

            return fieldType switch
            {
                FieldType.Repeat => targetParameter.targetType == TargetType.Absolute
                                       ? $"展开半径为【{a.ActionValue_7.ToString(withNo)}】的领域，每秒恢复{targetParameter.BuildTargetClause()}【{BuildExpression()}{p}】HP，持续【{BuildExpression(durationValues)}】秒"
                                       : $"在{targetParameter.BuildTargetClause()}的位置展开半径为【{a.ActionValue_7.ToString(withNo)}】的领域，每秒恢复【{BuildExpression()}{p}】HP，持续【{BuildExpression(durationValues)}】秒",
                _ => UnknownExpression(),
            };
        }
    }
}