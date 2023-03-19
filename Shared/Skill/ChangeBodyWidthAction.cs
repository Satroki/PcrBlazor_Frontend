namespace PcrBlazor.Shared
{
    internal class ChangeBodyWidthAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            return $"将模型的宽度变为{a.ActionValue_1.ToString(withNo)}";
        }
    }
}