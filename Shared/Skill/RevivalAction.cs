namespace PcrBlazor.Shared
{
    internal class RevivalAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            return a.ActionDetail_1 switch
            {
                1 => $"使{targetParameter.BuildTargetClause()}复活，并恢复其【{a.ActionValue_2 * 100}%】HP",
                _ => UnknownExpression(),
            };
        }
    }
}