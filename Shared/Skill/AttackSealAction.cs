namespace PcrBlazor.Shared
{
    internal class AttackSealAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var condition = a.ActionDetail_1.ParseEnum(Condition.Unknown);
            var target = a.ActionDetail_3;
            durationValues.Add(new ActionValue(a.ActionValue_3, a.ActionValue_4, null));
            var t = targetParameter.BuildTargetClause();
            var d = BuildExpression(durationValues);
            return condition switch
            {
                Condition.Hit => $"自身每Hit{t}时，对其增加1层标记【ID：{a.ActionValue_2.ToString(withNo)}】，持续【{d}】秒，本效果最多叠加【{a.ActionValue_1.ToString(withNo)}】层",
                Condition.Damage when target == 1 => $"{t}每次造成伤害时，增加1层标记【ID：{a.ActionValue_2.ToString(withNo)}】，持续【{d}】秒，本效果最多叠加【{a.ActionValue_1.ToString(withNo)}】层",
                Condition.CriticalHit when target == 1 => $"{t}每次造成暴击时，增加1层标记【ID：{a.ActionValue_2.ToString(withNo)}】，持续【{d}】秒，本效果最多叠加【{a.ActionValue_1.ToString(withNo)}】层",
                _ => UnknownExpression()
            };
        }
    }
}