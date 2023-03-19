namespace PcrBlazor.Shared
{
    internal class UpperLimitAttackAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            return $"{UnknownExpression()} 对低等级的玩家造成的伤害将被减轻";
        }
    }
}