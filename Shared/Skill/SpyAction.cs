namespace PcrBlazor.Shared
{
    internal class SpyAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            durationValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            return a.ActionDetail_2 switch
            {
                1 => $"使{targetParameter.BuildTargetClause()}隐身【{BuildExpression(durationValues)}】秒，受到伤害时取消",
                _ => UnknownExpression()
            };
        }
    }
}
