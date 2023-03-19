namespace PcrBlazor.Shared
{
    internal class AweAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            durationValues.Add(new ActionValue(a.ActionValue_3, a.ActionValue_4, null));
            percentValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            var aweType = a.ActionDetail_1;
            var t = targetParameter.BuildTargetClause();
            var d = $"，持续时间【{BuildExpression(durationValues)}】秒";
            return aweType switch
            {
                1 => $"{t}的UB或技能对任意目标造成伤害或直接恢复时，使其效果值降低【{BuildExpression(percentValues)}%】{d}",
                0 => $"{t}的UB对任意目标造成伤害或直接恢复时，使其效果值降低【{BuildExpression(percentValues)}%】{d}",
                _ => UnknownExpression(),
            };
        }
    }
}
/* enum AweType{
unknown(-1),
ubOnly(0),
ubAndSkill(1);
*/
