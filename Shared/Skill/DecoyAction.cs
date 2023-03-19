namespace PcrBlazor.Shared
{
    internal class DecoyAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            actionValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            return $"使{targetParameter.BuildTargetClause()}进入挑衅状态，持续时间【{BuildExpression()}】秒";
        }
    }
}