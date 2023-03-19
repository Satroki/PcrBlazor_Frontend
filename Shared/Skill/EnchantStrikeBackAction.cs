namespace PcrBlazor.Shared
{
    internal class EnchantStrikeBackAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var type = a.ActionDetail_1.ParseEnum(BarrierType.Unknown);
            actionValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            var t = targetParameter.BuildTargetClause();
            var expr = BuildExpression();
            return type switch
            {
                BarrierType.PhysicalGuard => $"为{t}展开护盾，受到物理伤害时反射【{expr}】伤害",
                BarrierType.MagicalGuard => $"为{t}展开护盾，受到魔法伤害时反射【{expr}】伤害",
                BarrierType.PhysicalDrain => $"为{t}展开护盾，受到物理伤害时反射【{expr}】伤害，并使自身HP恢复同等的值",
                BarrierType.MagicalDrain => $"为{t}展开护盾，受到魔法伤害时反射【{expr}】伤害，并使自身HP恢复同等的值",
                BarrierType.BothGuard => $"为{t}展开护盾，受到伤害时反射【{expr}】伤害",
                BarrierType.BothDrain => $"为{t}展开护盾，受到伤害时反射【{expr}】伤害，并使自身HP恢复同等的值",
                _ => UnknownExpression(),
            };
        }
    }
}