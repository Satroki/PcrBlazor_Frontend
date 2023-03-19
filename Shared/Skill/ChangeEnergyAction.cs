namespace PcrBlazor.Shared
{
    internal class ChangeEnergyAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            actionValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            var t = targetParameter.BuildTargetClause();
            return a.ActionDetail_1 switch
            {
                1 => $"恢复{t}【{BuildExpression()}】TP（此值会受到被恢复角色的TP上升值影响）",
                _ => $"使{t}失去【{BuildExpression()}】TP",
            };
        }
    }
}