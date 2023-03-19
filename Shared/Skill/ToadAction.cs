namespace PcrBlazor.Shared
{
    internal class ToadAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            durationValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            return $"使{targetParameter.BuildTargetClause()}变貌{BuildExpression(durationValues)}秒";
        }
    }
}
