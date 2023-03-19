namespace PcrBlazor.Shared
{
    internal class PassiveDamageUpAction : SkillActionParameter
    {
        public PassiveDamageUpAction()
        {
        }

        public override string GetDescription()
        {
            var debuffDamageUpLimitValue = a.ActionValue_2;
            var debuffDamageUpValue = a.ActionValue_1;
            var debuffDamageUpTimer = a.ActionValue_3;
            var countType = a.ActionDetail_1.ParseEnum(CountType.Unknown);
            var effectType = a.ActionDetail_2.ParseEnum(EffectType.Unknown);
            actionValues.Add(new ActionValue(a.ActionValue_3, a.ActionValue_4, null));
            var t = targetParameter.BuildTargetClause();
            return $"使{t}的伤害变为【1 {effectType.GetDescription()} {debuffDamageUpValue} * {t}的{countType.GetDescription()}数】倍（最大不超过{debuffDamageUpLimitValue}倍），持续【{BuildExpression()}】 秒";
        }
    }
}