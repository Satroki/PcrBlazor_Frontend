namespace PcrBlazor.Shared
{
    internal class TriggerAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var triggerType = a.ActionDetail_1.ParseEnum(TriggerType.Unknown);
            return triggerType switch
            {
                TriggerType.Hp => $"触发条件：HP为【{a.ActionValue_3.ToString(withNo)}%】以下时触发",
                TriggerType.LimitTime => $"触发条件：战斗剩余时间为【{a.ActionValue_3.ToString(withNo)}】秒以下时触发",
                TriggerType.Damage => $"触发条件：受到伤害时【{a.ActionValue_1.ToString(withNo)}%】的概率触发",
                TriggerType.Dead => $"触发条件：死亡时【{a.ActionValue_1.ToString(withNo)}%】的概率触发",
                TriggerType.Critical => $"触发条件：受到暴击时【{a.ActionValue_1.ToString(withNo)}%】的概率触发",
                TriggerType.StealthFree => $"触发条件：潜伏时【{a.ActionValue_1.ToString(withNo)}%】的概率触发",
                TriggerType.Break => $"触发条件：Break时【{a.ActionValue_1.ToString(withNo)}%】的概率触发，持续【{a.ActionValue_3.ToString(withNo)}】秒",
                TriggerType.Dot => $"触发条件：受到持续伤害时【{a.ActionValue_1.ToString(withNo)}%】的概率触发",
                TriggerType.AllBreak => $"触发条件：所有部位被Break时触发",
                _ => UnknownExpression(),
            };
        }

        enum TriggerType
        {
            Unknown = 0,
            Dodge = 1,
            Damage = 2,
            Hp = 3,
            Dead = 4,
            Critical = 5,
            CriticalWithSummon = 6,
            LimitTime = 7,
            StealthFree = 8,
            Break = 9,
            Dot = 10,
            AllBreak = 11
        }
    }
}