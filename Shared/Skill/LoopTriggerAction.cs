namespace PcrBlazor.Shared
{
    internal class LoopTriggerAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            actionValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            var triggerType = a.ActionDetail_1.ParseEnum(TriggerType.Unknown);
            return triggerType switch
            {
                TriggerType.Damaged => $"条件分歧：【{a.ActionValue_4.ToString(withNo)}】秒内受到伤害的场合，以【{BuildExpression()}%】的概率使用【动作{a.ActionDetail_2 % 10}】",
                _ => UnknownExpression(),
            };
        }
    }
}