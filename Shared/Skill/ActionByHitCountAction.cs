namespace PcrBlazor.Shared
{
    internal class ActionByHitCountAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var conditionType = a.ActionDetail_1.ParseEnum(ConditionType.Unknown);
            durationValues.Add(new ActionValue(a.ActionValue_3, a.ActionValue_4, null));
            var limitation = a.ActionValue_5 > 0 ? $"（上限为{a.ActionValue_5.ToString(withNo)}次）" : string.Empty;
            return conditionType switch
            {
                ConditionType.Hit => $"【{BuildExpression(durationValues)}】秒内，每【{a.ActionValue_1.ToString(withNo)}】Hit即使用【动作{a.ActionDetail_2 % 10}】{limitation}",
                _ => UnknownExpression(),
            };
        }

        enum ConditionType
        {
            Unknown = 0,
            Damage = 1,
            Target = 2,
            Hit = 3,
            Critical = 4
        }
    }
}