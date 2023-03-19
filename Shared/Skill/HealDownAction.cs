namespace PcrBlazor.Shared
{
    internal class HealDownAction : SkillActionParameter
    {
        public HealDownAction()
        {
        }

        public override string GetDescription()
        {
            var percentModifier = GetPercent(a.ActionValue_1);
            actionValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            durationValues.Add(new ActionValue(a.ActionValue_3, a.ActionValue_4, null));

            return $"使{targetParameter.BuildTargetClause()}的治疗效果变为原本的【{BuildExpression()}】倍，持续【{BuildExpression(durationValues)}】秒。";
        }
    }
}