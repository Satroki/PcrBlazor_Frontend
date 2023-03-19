namespace PcrBlazor.Shared
{
    internal class ChannelAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var percent = GetPercent(a.ActionValue_1);
            actionValues.Add(new ActionValue(a.ActionValue_2, a.ActionValue_3, null));
            durationValues.Add(new ActionValue(a.ActionValue_4, a.ActionValue_5, null));
            var auraActionType = (a.ActionDetail_1 % 10).ParseEnum(AuraActionType.Raise).GetDescription();
            var auraType = (a.ActionDetail_1 / 10).ParseEnum(AuraType.None).GetDescription();
            var t = targetParameter.BuildTargetClause();

            return a.ActionDetail_2 switch
            {
                1 => $"{auraActionType}{t}【{BuildExpression()}{percent}】{auraType}，本动作最长持续【{BuildExpression(durationValues)}】秒，受到【{a.ActionDetail_3}】次伤害时被中断",
                _ => UnknownExpression(),
            };
        }
    }
}