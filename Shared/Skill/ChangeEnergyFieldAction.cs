namespace PcrBlazor.Shared
{
    public class ChangeEnergyFieldAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            actionValues.Add(new(a.ActionValue_1, a.ActionValue_2, null));
            durationValues.Add(new(a.ActionValue_3, a.ActionValue_4, null));
            var t = targetParameter.BuildTargetClause();
            var exp = BuildExpression(actionValues);
            var dur = BuildExpression(durationValues);
            return a.ActionDetail_1 switch
            {
                1 => $"在{t}的位置展开半径【{a.ActionValue_5.ToString(withNo)}】的领域，每秒恢复【{exp}】TP。持续【{dur}】秒",
                2 => $"在{t}的位置展开半径【{a.ActionValue_5.ToString(withNo)}】的领域，每秒失去【{exp}】TP。持续【{dur}】秒",
                _ => UnknownExpression()
            };
        }
    }
}
