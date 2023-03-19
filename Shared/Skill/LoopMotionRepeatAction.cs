namespace PcrBlazor.Shared
{
    internal class LoopMotionRepeatAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            string successClause = null, failureClause = null;
            if (a.ActionDetail_2 != 0)
                successClause = $"持续时间结束后，使用【动作{a.ActionDetail_2 % 10}】";
            if (a.ActionDetail_3 != 0)
                failureClause = $"效果被中断后，使用【动作{a.ActionDetail_3 % 10}】";

            var mainClause = $"每{a.ActionValue_2.ToString(withNo)}秒使用1次【动作{a.ActionDetail_1 % 10}】，最长持续【{a.ActionValue_1.ToString(withNo)}】秒。受到的伤害量超过【{a.ActionValue_3.ToString(withNo)}】时中断此效果";

            return mainClause.Join(successClause, "；").Join(failureClause, "；");
        }
    }
}
