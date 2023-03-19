namespace PcrBlazor.Shared
{
    internal class UnknownAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            return UnknownExpression();
        }
    }
}
