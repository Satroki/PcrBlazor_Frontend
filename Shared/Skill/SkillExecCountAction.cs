namespace PcrBlazor.Shared
{
    internal class SkillExecCountAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            return $"增加【{a.ActionValue_1.ToString(withNo)}】枚计数器";
        }
    }
}