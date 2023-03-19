namespace PcrBlazor.Shared
{
    internal class EffectAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            return $"对{ targetParameter.BuildTargetClause()}添加一些额外的视觉特效或音效";
        }
    }
}
