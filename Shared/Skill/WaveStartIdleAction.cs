namespace PcrBlazor.Shared
{
    internal class WaveStartIdleAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            return $"战斗开始【{a.ActionValue_1.ToString(withNo)}】秒后入场";
        }
    }
}