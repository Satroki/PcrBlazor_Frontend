namespace PcrBlazor.Shared
{
    internal class SealAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            return a.ActionValue_4 >= 0
                ? $"对{targetParameter.BuildTargetClause()}追加【{a.ActionValue_4.ToString(withNo)}】层标记【ID：{a.ActionValue_2.ToString(withNo)}】，持续时间【{a.ActionValue_3.ToString(withNo)}】秒，最大叠【{a.ActionValue_1.ToString(withNo)}】层"
                : $"使{targetParameter.BuildTargetClause()}的标记【ID：{a.ActionValue_2.ToString(withNo)}】层数减少【{a.ActionValue_4.Multiply(-1).ToString(withNo)}】层";
        }
    }
}