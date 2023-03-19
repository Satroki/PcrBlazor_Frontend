namespace PcrBlazor.Shared
{
    internal class RatioDamageAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            actionValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            var hptype = a.ActionDetail_1.ParseEnum(HPtype.Unknown);
            var t = targetParameter.BuildTargetClause();
            var expr = BuildExpression();
            return hptype switch
            {
                HPtype.Max => $"对{t}造成最大HP【{expr}%】的伤害",
                HPtype.Current => $"对{t}造成剩余HP【{expr}%】的伤害",
                HPtype.OriginalMax => $"对{t}造成初始HP【{expr}%】的伤害。",
                _ => UnknownExpression(),
            };
        }
    }
}