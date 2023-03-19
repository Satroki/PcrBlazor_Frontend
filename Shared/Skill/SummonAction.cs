using System.ComponentModel;

namespace PcrBlazor.Shared
{
    internal class SummonAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            //var side = a.ActionDetail_3.ParseEnum(Side.Other);
            //var unitType = ((int)a.ActionValue_5).ParseEnum(UnitType.Unknown);
            RelatedUnit = a.ActionDetail_2;
            var t = targetParameter.BuildTargetClause();
            if (a.ActionValue_7 > 0)
                return $"在{t}的前方【{a.ActionValue_7.ToString(withNo)}】的位置，召唤【ID：{a.ActionDetail_2}】的召唤物";
            else if (a.ActionValue_7 < 0)
                return $"在{t}的后方【{a.ActionValue_7.Multiply(-1).ToString(withNo)}】的位置，召唤【ID：{a.ActionDetail_2}】的召唤物";
            else
                return $"在{t}的位置，召唤【ID：{a.ActionDetail_2}】的召唤物";
        }

        enum Side
        {
            Unknown = -1,
            [Description("我方")]
            Ours = 1,
            [Description("敌方")]
            Other = 2
        }

        enum UnitType
        {
            Unknown = -1,
            [Description("普通类型")]
            Normal = 1,
            [Description("幽灵类型")]
            Phantom = 2
        }
    }
}