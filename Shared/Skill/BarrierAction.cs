namespace PcrBlazor.Shared
{
    internal class BarrierAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var type = a.ActionDetail_1.ParseEnum(BarrierType.Unknown);
            actionValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            var target = targetParameter.BuildTargetClause();
            var expr = BuildExpression();
            return type switch
            {
                BarrierType.PhysicalGuard => $"对{target}展开可承受【{expr}】物理伤害的护盾，持续【{a.ActionValue_3.ToString(withNo)}】秒",
                BarrierType.MagicalGuard => $"对{target}展开可承受【{expr}】魔法伤害的护盾，持续【{a.ActionValue_3.ToString(withNo)}】秒",
                BarrierType.PhysicalDrain => $"对{target}展开可吸收【{expr}】物理伤害的护盾，持续【{a.ActionValue_3.ToString(withNo)}】秒",
                BarrierType.MagicalDrain => $"对{target}展开可吸收【{expr}】魔法伤害的护盾，持续【{a.ActionValue_3.ToString(withNo)}】秒",
                BarrierType.BothGuard => $"对{target}展开可承受【{expr}】物理&魔法伤害的护盾，持续【{a.ActionValue_3.ToString(withNo)}】秒",
                BarrierType.BothDrain => $"对{target}展开可吸收【{expr}】物理&魔法伤害的护盾，持续【{a.ActionValue_3.ToString(withNo)}】秒",
                _ => UnknownExpression(),
            };
        }
    }
}