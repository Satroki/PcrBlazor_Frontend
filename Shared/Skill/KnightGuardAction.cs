namespace PcrBlazor.Shared
{
    internal class KnightGuardAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var guardType = (int)a.ActionValue_1 switch
            {
                1 => nameof(UnitStatus.Atk),
                _ => nameof(UnitStatus.MagicStr)
            };
            actionValues.Add(new ActionValue(a.ActionValue_4, a.ActionValue_5, guardType));
            actionValues.Add(new ActionValue(a.ActionValue_2, a.ActionValue_3, null));
            durationValues.Add(new ActionValue(a.ActionValue_6, a.ActionValue_7, null));

            return $"对{targetParameter.BuildTargetClause()}赋予「HP降至0的一瞬之前，恢复【{BuildExpression()}】HP，并解除此效果」的效果，持续【{BuildExpression(durationValues)}】秒";
        }
    }
}
