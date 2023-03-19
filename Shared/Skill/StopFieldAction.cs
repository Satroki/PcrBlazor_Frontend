namespace PcrBlazor.Shared
{
    internal class StopFieldAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            return $"解除【技能{a.ActionDetail_1 / 100 % 10}】的【动作{a.ActionDetail_1 % 10}】展开的领域";
        }
    }
}