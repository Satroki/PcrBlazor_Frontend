namespace PcrBlazor.Shared
{
    internal class MovePartsAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            return $"使部位{a.ActionValue_4.ToString(withNo)}向前移动【{a.ActionValue_1.Multiply(-1).ToString(withNo)}】，随后使其返回原位置";
        }
    }
}