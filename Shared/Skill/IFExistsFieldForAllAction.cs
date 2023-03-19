namespace PcrBlazor.Shared
{
    internal class IFExistsFieldForAllAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            if (a.ActionDetail_2 != 0 && a.ActionDetail_3 != 0)
                return $"条件分歧：特定的领域效果存在时使用【动作{a.ActionDetail_2 % 10}】，否则使用【动作{a.ActionDetail_3 % 10}】";
            else if (a.ActionDetail_2 != 0)
                return $"条件分歧：特定的领域效果存在时使用【动作{a.ActionDetail_2 % 10}】";
            else
                return UnknownExpression();
        }
    }
}