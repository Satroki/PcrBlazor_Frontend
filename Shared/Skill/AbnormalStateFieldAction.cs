namespace PcrBlazor.Shared
{
    internal class AbnormalStateFieldAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            durationValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            return $"在{targetParameter.BuildTargetClause()}的位置展开半径为【{a.ActionValue_3.ToString(withNo)}】的领域，持续施放【动作{a.ActionDetail_1 % 10}】，持续【{BuildExpression(durationValues)}】秒";
        }
    }
}