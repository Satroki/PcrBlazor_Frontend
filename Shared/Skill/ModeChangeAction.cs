namespace PcrBlazor.Shared
{
    internal class ModeChangeAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var modeChangeType = a.ActionDetail_1.ParseEnum(ModeChangeType.Unknown);
            return modeChangeType switch
            {
                ModeChangeType.Time => $"切换至技能循环{a.ActionDetail_2 % 10}，持续【{a.ActionValue_1.ToString(withNo)}】秒",
                ModeChangeType.Energy => $"TP耗光前切换至技能循环{a.ActionDetail_2 % 10}，每秒消费TP【{a.ActionValue_1.ToString(withNo)}】",
                ModeChangeType.Release => $"效果结束后，切换回技能循环{a.ActionDetail_2 % 10}",
                _ => UnknownExpression(),
            };
        }

        enum ModeChangeType
        {
            Unknown = 0,
            Time = 1,
            Energy = 2,
            Release = 3
        }
    }
}