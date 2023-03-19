using System;

namespace PcrBlazor.Shared
{
    internal class KnockAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var type = a.ActionDetail_1.ParseEnum(KnockType.Unknown);
            var target = targetParameter.BuildTargetClause();
            var d = Math.Abs(a.ActionValue_1);
            var direction = a.ActionValue_1 > 0 ? "击退" : "吸引";
            return type switch
            {
                KnockType.UpDown => $"击飞{target}，高度【{d}】",
                KnockType.Back => $"{direction}{target}，距离【{d}】",
                KnockType.BackLimited => $"{direction}{target}，距离【{d}】",
                KnockType.PullOwner => a.ActionValue_2 == -1 ? $"将{target}拉至自身前方【{d}】" : "",
                KnockType.MoveTarget => throw new System.NotImplementedException(),
                KnockType.MoveTargetParaboric => throw new System.NotImplementedException(),
                _ => UnknownExpression()
            };
        }

        enum KnockType
        {
            Unknown = 0,
            UpDown = 1,
            Up = 2,
            Back = 3,
            MoveTarget = 4,
            MoveTargetParaboric = 5,
            BackLimited = 6,
            PullOwner = 8
        }
    }
}