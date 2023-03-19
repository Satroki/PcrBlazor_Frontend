namespace PcrBlazor.Shared
{
    internal class ChangePatternAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            return a.ActionDetail_1 switch
            {
                1 => $"切换至技能循环{a.ActionDetail_2 % 10}，持续时间【{a.ActionValue_1.ToString(withNo)}】秒",
                2 => $"改变技能的动画，持续时间【{a.ActionValue_1.ToString(withNo)}】秒",
                _ => UnknownExpression(),
            };
        }
    }
}