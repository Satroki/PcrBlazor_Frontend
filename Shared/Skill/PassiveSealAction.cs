using System.Collections.Generic;

namespace PcrBlazor.Shared
{
    internal class PassiveSealAction : SkillActionParameter
    {
        public PassiveSealAction()
        {
        }

        public override string GetDescription()
        {
            var sealNumLimit = (int)a.ActionValue_1;
            var sealDuration = new List<ActionValue> { new ActionValue(a.ActionValue_3, a.ActionValue_4, null) };
            var lifeTime = new List<ActionValue> { new ActionValue(a.ActionValue_5, a.ActionValue_6, null) };
            var passiveTiming = a.ActionDetail_1.ParseEnum(PassiveTiming.Unknown);
            var sealTarget = a.ActionDetail_3.ParseEnum(SealTarget.Unknown);

            return $"被动效果：每当{targetParameter.BuildTargetClause()}受到{passiveTiming.GetDescription()}时，" +
                $"为{sealTarget.GetDescription()}增加【{a.ActionDetail_2}】层标记【ID: {a.ActionValue_2:0}】，" +
                $"标记持续时间【{BuildExpression(sealDuration)}】秒，最大叠【{sealNumLimit}】层。被动效果持续【{BuildExpression(lifeTime)}】秒";
        }
    }
}