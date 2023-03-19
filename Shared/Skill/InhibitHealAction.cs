namespace PcrBlazor.Shared
{
    internal class InhibitHealAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            durationValues.Add(new ActionValue(a.ActionValue_2, a.ActionValue_3, null));
            return $"{targetParameter.BuildTargetClause()}受到恢复效果时，使恢复无效并给予其【{a.ActionValue_1.ToString(withNo)} * 回复量】伤害，持续【{BuildExpression(durationValues)}】秒。本效果是领域效果的场合，持续时间无限大";
        }
    }
}