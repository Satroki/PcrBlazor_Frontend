namespace PcrBlazor.Shared
{
    internal class RegenerationAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var healClass = a.ActionDetail_1.ParseEnum(ClassModifier.Unknown);
            var regenerationType = a.ActionDetail_2.ParseEnum(RegenerationType.Unknown);
            durationValues.Add(new ActionValue(a.ActionValue_5, a.ActionValue_6, null));
            var key = healClass switch
            {
                ClassModifier.Magical => nameof(UnitStatus.MagicStr),
                _ => nameof(UnitStatus.Atk)
            };
            actionValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            actionValues.Add(new ActionValue(a.ActionValue_3, a.ActionValue_4, key));

            return $"每秒恢复{targetParameter.BuildTargetClause()}【{BuildExpression()}】{regenerationType.GetDescription()}，持续{BuildExpression(durationValues)}秒";
        }
    }
}