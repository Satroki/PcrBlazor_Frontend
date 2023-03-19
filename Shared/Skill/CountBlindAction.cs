namespace PcrBlazor.Shared
{
    internal class CountBlindAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            actionValues.Add(new ActionValue(a.ActionValue_2, a.ActionValue_3, null));
            var t = targetParameter.BuildTargetClause();
            var expr = BuildExpression();
            return ((int)a.ActionValue_1.Value) switch
            {
                1 => $"使%{t}的物理攻击必定miss，持续【{expr}】秒",
                2 => $"使{t}的下【{expr}】次物理攻击必定miss",
                _ => UnknownExpression(),
            };
        }
    }
}