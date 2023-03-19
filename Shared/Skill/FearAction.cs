namespace PcrBlazor.Shared
{
    internal class FearAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            durationValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            chanceValues.Add(new ActionValue(a.ActionValue_3, a.ActionValue_4, null));
            return $"以【{BuildExpression(chanceValues)}%】的概率使{targetParameter.BuildTargetClause()}进入恐慌状态，持续【{BuildExpression(durationValues)}】秒";
        }
    }
}