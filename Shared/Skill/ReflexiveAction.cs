namespace PcrBlazor.Shared
{
    internal class ReflexiveAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var type = a.ActionDetail_1.ParseEnum(ReflexiveType.Unknown);
            var t = targetParameter.BuildTargetClause();
            if (targetParameter.targetType == TargetType.Absolute)
                return $"将自身的视点切换到{t}，距离【{a.ActionValue_1.ToString(withNo)}】";
            if (type == ReflexiveType.Search)
                return $"搜寻并将自身的视点切换到{t}";
            return $"将自身的视点切换到{t}";
        }

        enum ReflexiveType
        {
            Unknown = 0,
            Normal = 1,
            Search = 2,
            Position = 3
        }
    }
}