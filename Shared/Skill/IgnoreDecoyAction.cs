namespace PcrBlazor.Shared
{
    internal class IgnoreDecoyAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            return $"攻击{targetParameter.BuildTargetClause()}时，无视其他目标的挑衅效果";
        }
    }
}
