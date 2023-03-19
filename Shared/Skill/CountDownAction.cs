namespace PcrBlazor.Shared
{
    internal class CountDownAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            return $"对{targetParameter.BuildTargetClause()}设置倒计时，【{a.ActionValue_1.ToString(withNo)}】秒后触发【动作{a.ActionDetail_1 % 10}】";
        }
    }
}