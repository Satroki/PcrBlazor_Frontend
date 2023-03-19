namespace PcrBlazor.Shared
{
    internal class MultipleAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            actionValues.Add(new ActionValue(a.ActionValue_2, a.ActionValue_3, null));
            var av1 = a.ActionValue_1;
            var pre = $"使【动作{a.ActionDetail_1 % 10}】的系数{a.ActionDetail_2}乘以【{BuildExpression()} * ";
            if (av1 == 0)
                return $"{pre}HP / MaxHP】";
            else if (av1 == 1)
                return $"{pre}损失的HP / MaxHP】";
            else if (av1 == 2)
                return $"{pre}击杀的敌方数量】";
            else if (av1 >= 200 && av1 < 300)
                return $"{pre}标记【ID：{av1 % 200}】的层数】";
            else
                return UnknownExpression();
        }
    }
}