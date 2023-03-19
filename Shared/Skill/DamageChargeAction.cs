namespace PcrBlazor.Shared
{
    internal class DamageChargeAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            actionValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            return $"蓄力【{a.ActionValue_3.ToString(withNo)}】秒，使下一个动作的效果提高【{BuildExpression()} * 蓄力中受到的伤害】";
        }
    }
}