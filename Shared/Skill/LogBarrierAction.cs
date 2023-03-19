namespace PcrBlazor.Shared
{
    internal class LogBarrierAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            //var barrierType = (int)a.ActionValue_1;
            actionValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            durationValues.Add(new ActionValue(a.ActionValue_3, a.ActionValue_4, null));
            return $"为{targetParameter.BuildTargetClause()}展开护盾，在一次行动中受到的伤害超过【{a.ActionValue_5.ToString(withNo)}】时，" +
                $"伤害值将会被衰减，衰减系数【{BuildExpression()}】（值越小衰减量越大），持续【{BuildExpression(durationValues)}】秒";
        }
    }
}
