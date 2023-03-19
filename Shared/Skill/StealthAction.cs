namespace PcrBlazor.Shared
{
    internal class StealthAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            return $"进入潜伏状态，持续时间【{a.ActionValue_1.ToString(withNo)}】秒";
        }
    }
}