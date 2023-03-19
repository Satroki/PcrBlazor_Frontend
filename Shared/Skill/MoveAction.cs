using System;

namespace PcrBlazor.Shared
{
    internal class MoveAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var type = a.ActionDetail_1.ParseEnum(MoveType.Unknown);
            var direction = a.ActionValue_1 > 0 ? "前" : "后";
            var d = Math.Abs(a.ActionValue_1);
            var target = targetParameter.BuildTargetClause();
            return type switch
            {
                MoveType.TargetReturn => $"移动至{target}的位置，技能施放完成后回到原来的位置",
                MoveType.AbsoluteReturn => $"向{direction}移动【{d}】，技能施放完成后回到原来的位置",
                MoveType.Target => $"移动至{target}的位置",
                MoveType.Absolute => $"向{direction}移动【{d}】",
                MoveType.TargetByVelocity => $"以【{a.ActionValue_2.ToString(withNo)}】的速度移动到{target}的{direction}方【{d}】的位置",
                MoveType.AbsoluteByVelocity => $"以【{a.ActionValue_2.ToString(withNo)}】的速度向前移动【{d}】",
                MoveType.AbsoluteWithoutDirection => $"向{direction}移动【{d}】",
                _ => UnknownExpression()
            };
        }

        enum MoveType
        {
            Unknown = 0,
            TargetReturn = 1,
            AbsoluteReturn = 2,
            Target = 3,
            Absolute = 4,
            TargetByVelocity = 5,
            AbsoluteByVelocity = 6,
            AbsoluteWithoutDirection = 7
        }
    }
}