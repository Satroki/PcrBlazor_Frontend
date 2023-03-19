namespace PcrBlazor.Shared
{
    internal class PassiveAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var key = a.ActionDetail_1 switch
            {
                1 => nameof(UnitStatus.Hp),
                2 => nameof(UnitStatus.Atk),
                3 => nameof(UnitStatus.Def),
                4 => nameof(UnitStatus.MagicStr),
                5 => nameof(UnitStatus.MagicDef),
                _ => null,
            };
            var keyDesp = UnitStatus.Names.GetValue(key);
            actionValues.Add(new ActionValue(a.ActionValue_2, a.ActionValue_3, null));
            return $"提高{keyDesp}【{BuildExpression()}】";
        }
    }
}
