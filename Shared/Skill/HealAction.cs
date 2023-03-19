namespace PcrBlazor.Shared
{
    internal class HealAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var healClass = a.ActionDetail_1.ParseEnum(ClassModifier.Unknown);
            var key = healClass switch
            {
                ClassModifier.Unknown => null,
                ClassModifier.Magical => nameof(IUnitStatus.MagicStr),
                _ => nameof(IUnitStatus.Atk),
            };
            actionValues.Add(new ActionValue(a.ActionValue_4, a.ActionValue_5, key));
            actionValues.Add(new ActionValue(a.ActionValue_2, a.ActionValue_3, null));

            var p = GetPercent(a.ActionValue_1);
            return $"恢复{targetParameter.BuildTargetClause()}的HP【{BuildExpression()}{p}】";
        }
    }
}