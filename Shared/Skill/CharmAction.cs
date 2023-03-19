using System.Collections.Generic;

namespace PcrBlazor.Shared
{
    internal class CharmAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var charmType = a.ActionDetail_1.ParseEnum(CharmType.Unknown);
            durationValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            switch (charmType)
            {
                case CharmType.Charm:
                    chanceValues.Add(new ActionValue(a.ActionValue_3, a.ActionValue_4.Multiply(100), null));
                    break;
                default:
                    chanceValues.Add(new ActionValue(a.ActionValue_3.Multiply(100), a.ActionValue_4.Multiply(100), null));
                    break;
            }
            var t = targetParameter.BuildTargetClause();
            return charmType switch
            {
                CharmType.Charm => $"以【{BuildExpression(chanceValues)}%】的概率使{t}进入魅惑状态，持续【{BuildExpression(durationValues)}】秒",
                CharmType.Confusion => $"以【{BuildExpression(chanceValues)}%】的概率使{t}进入混乱状态，持续【{BuildExpression(durationValues)}】秒",
                _ => UnknownExpression()
            };
        }

        enum CharmType
        {
            Unknown = -1,
            Charm = 0,
            Confusion = 1
        }
    }
}