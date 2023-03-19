namespace PcrBlazor.Shared
{
    internal class DestroyAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            return $"使{targetParameter.BuildTargetClause()}即死";
        }
    }
}