namespace PcrBlazor.Shared
{
    internal class NoDamageAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var noDamageType = a.ActionDetail_1.ParseEnum(NoDamageType.Unknown);
            actionValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            var t = targetParameter.BuildTargetClause();
            return noDamageType switch
            {
                NoDamageType.NoDamage => $"使{t}进入无敌状态，持续【{BuildExpression()}】秒",
                NoDamageType.DodgePhysics => $"使{t}进入物理攻击无效状态，持续【{BuildExpression()}】秒",
                NoDamageType.Break => $"对{t}附加Break免疫效果，持续时间【{BuildExpression()}】秒",
                _ => UnknownExpression(),
            };
        }

        enum NoDamageType
        {
            Unknown = 0,
            NoDamage = 1,
            DodgePhysics = 2,
            DodgeAll = 3,
            Abnormal = 4,
            Debuff = 5,
            Break = 6
        }
    }
}