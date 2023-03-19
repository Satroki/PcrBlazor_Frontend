using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PcrBlazor.Shared
{
    public class SkillTargetParameter
    {
        public TargetAssignment targetAssignment;
        public TargetNumber targetNumber;
        public int rawTargetType;
        public TargetType targetType;
        public TargetRange targetRange;
        public DirectionType direction;
        public TargetCount targetCount;
        private SkillAction dependAction;

        private bool hasRelationPhrase;
        private bool hasCountPhrase;
        private bool hasRangePhrase;
        private bool hasNthModifier;
        private bool hasDirectionPhrase;
        private bool hasTargetType;
        private bool hasDependAction;


        public SkillTargetParameter(SkillAction a, SkillAction da)
        {
            targetAssignment = a.TargetAssignment.ParseEnum(TargetAssignment.None);
            targetNumber = a.TargetNumber.ParseEnum(TargetNumber.Other);
            rawTargetType = a.TargetType;
            targetType = a.TargetType.ParseEnum(TargetType.Unknown);
            targetRange = new TargetRange(a.TargetRange);
            direction = a.TargetArea.ParseEnum(DirectionType.All);
            targetCount = a.TargetCount.ParseEnum(TargetCount.All);
            dependAction = da;
            SetBooleans();
        }

        private void SetBooleans()
        {
            hasRelationPhrase = targetType != TargetType.Self && targetType != TargetType.Absolute;
            hasCountPhrase = targetType != TargetType.Self && !(targetType == TargetType.None && targetCount == TargetCount.Zero);
            hasRangePhrase = targetRange.Type == RangeType.Finite;
            hasNthModifier = new[] { TargetNumber.Second, TargetNumber.Third, TargetNumber.Fourth, TargetNumber.Fifth }.Contains(targetNumber);
            hasDirectionPhrase = direction == DirectionType.Front && (hasRangePhrase || targetCount == TargetCount.All);
            hasTargetType = !(targetType.ExclusiveWithAll() == ExclusiveAllType.Exclusive && targetCount == TargetCount.All);
            hasDependAction = dependAction?.ActionId > 0
                && targetType != TargetType.Absolute
                && dependAction.ActionType != (int)ActionType.ChooseArea;
            //&& new[] { ActionType.IfForChildren, ActionType.IfForAll, ActionType.Damage, ActionType.Knock }.Contains((ActionType)dependAction.ActionType);

        }

        public string BuildTargetClause(bool anyOfModifier)
        {
            if (targetCount.GetPluralModifier() == PluralModifier.Many && anyOfModifier)
                return string.Format("（仅对符合条件的目标使用）{0}中任意的目标", BuildTargetClause());
            else
                return BuildTargetClause();
        }


        public string BuildTargetClause()
        {
            if (hasDependAction)
            {
                if (dependAction.ActionType == (int)ActionType.Damage)
                    return $"受到【动作{dependAction.ActionId % 100}】伤害的目标";
                else
                    return $"【动作{dependAction.ActionId % 100}】的目标";
            }
            if (!hasRelationPhrase)
            {
                return targetType.GetDescription();
            }
            if (!hasCountPhrase && !hasNthModifier && !hasRangePhrase && hasRelationPhrase)
            {
                return "上一个动作的目标";
            }
            if (!hasCountPhrase && hasNthModifier && hasRangePhrase && hasRelationPhrase)
            {
                return "上一个动作的目标";
            }
            var directionPhrase = hasDirectionPhrase ? "前方" : string.Empty;
            var rangePhrase = hasRangePhrase ? $"范围【{targetRange.Range}】内的" : string.Empty;

            if (hasCountPhrase && !hasNthModifier && !hasRangePhrase && hasRelationPhrase && !hasDirectionPhrase)
            {
                if (targetCount == TargetCount.All)
                {
                    switch (targetType.ExclusiveWithAll())
                    {
                        case ExclusiveAllType.Not:
                            return $"{targetType.GetDescription()}{targetAssignment.GetDescription()}全体";
                        case ExclusiveAllType.Exclusive:
                            return $"{targetAssignment.GetDescription()}全体";
                        case ExclusiveAllType.HalfExclusive:
                            return $"{targetAssignment.GetDescription()}全体" + "（除自身以外）";
                    }
                }
                else if (targetCount == TargetCount.One && targetType.IgnoresOne())
                {
                    return $"{targetType.GetDescription()}{targetAssignment.GetDescription()}";
                }
                else
                {
                    return $"{targetType.GetDescription()}{targetAssignment.GetDescription()}{targetCount.GetDescription()}";
                }
            }
            else if (hasCountPhrase && !hasNthModifier && !hasRangePhrase && hasRelationPhrase && hasDirectionPhrase)
            {
                var (ttype, half) = targetType.ExclusiveWithAll() switch
                {
                    ExclusiveAllType.Not => (targetType.GetDescription(), string.Empty),
                    ExclusiveAllType.HalfExclusive => (string.Empty, "（除自身以外）"),
                    _ => (string.Empty, string.Empty),
                };
                return targetAssignment switch
                {
                    TargetAssignment.Enemy => $"前方{ttype}所有的敌方{half}",
                    TargetAssignment.Friendly => $"前方{ttype}所有的友方（包括自身）",
                    _ => $"前方{ttype}所有的目标{half}",
                };
            }
            else if (!hasCountPhrase && !hasNthModifier && hasRangePhrase && hasRelationPhrase)
            {
                return $"{directionPhrase}范围【{targetRange.Range}】内的{targetAssignment.GetDescription()}";
            }

            else if (hasCountPhrase && !hasNthModifier && hasRangePhrase && hasRelationPhrase)
            {
                if (targetCount == TargetCount.All)
                {
                    switch (targetType.ExclusiveWithAll())
                    {
                        case ExclusiveAllType.Not:
                            return $"{directionPhrase}范围【{targetRange.Range}】内的{targetAssignment.GetDescription()}{targetType.GetDescription()}";
                        case ExclusiveAllType.Exclusive:
                            return $"{directionPhrase}范围【{targetRange.Range}】内的{targetAssignment.GetDescription()}";
                        case ExclusiveAllType.HalfExclusive:
                            return $"{directionPhrase}范围【{targetRange.Range}】内的{targetAssignment.GetDescription()}（除自身以外）";
                    }
                }
                else if (targetCount == TargetCount.One && targetType.IgnoresOne())
                {
                    return $"{directionPhrase}范围【{targetRange.Range}】内的{targetType.GetDescription()}{targetAssignment.GetDescription()}";
                }
                else
                {
                    return $"{directionPhrase}范围【{targetRange.Range}】内的{targetType.GetDescription()}{targetAssignment.GetDescription()}{targetCount.GetDescription()}";
                }
            }
            else if (hasCountPhrase && hasNthModifier && !hasRangePhrase && hasRelationPhrase && !hasDirectionPhrase)
            {
                if (targetCount == TargetCount.One && targetType.IgnoresOne())
                {
                    return $"{targetType.GetDescription(targetNumber, null)}{targetAssignment.GetDescription()}";
                }
                else
                {
                    var modifier = $"从{targetNumber.GetDescription()}至{GetUntilNumber().GetDescription()}";
                    return $"{targetType.GetDescription(targetNumber, modifier)}{targetAssignment.GetDescription()}{targetCount.GetPluralModifier().GetDescription()}";
                }
            }
            else if (hasCountPhrase && hasNthModifier && !hasRangePhrase && hasRelationPhrase && hasDirectionPhrase)
            {
                var modifier = $"从{targetNumber.GetDescription()}至{GetUntilNumber().GetDescription()}";
                return $"前方{targetType.GetDescription(targetNumber, modifier)}{targetAssignment.GetDescription()}{targetCount.GetPluralModifier().GetDescription()}";
            }
            else if (hasCountPhrase && hasNthModifier && hasRangePhrase && hasRelationPhrase)
            {
                if (targetCount == TargetCount.One && targetType.IgnoresOne())
                {
                    return $"{directionPhrase}范围【{targetRange.Range}】内的{targetType.GetDescription(targetNumber, null)}{targetAssignment.GetDescription()}";
                }
                else
                {
                    var modifier = $"从{targetNumber.GetDescription()}至{GetUntilNumber().GetDescription()}";
                    return $"{directionPhrase}范围【{targetRange.Range}】内的{targetType.GetDescription(targetNumber, modifier)}{targetAssignment.GetDescription()}{targetCount.GetPluralModifier().GetDescription()}";
                }
            }
            return "";
        }

        private TargetNumber GetUntilNumber()
        {
            var tUntil = ((int)targetNumber + (int)targetCount).ParseEnum(TargetNumber.Other);
            if (tUntil == TargetNumber.Other)
                return TargetNumber.Fifth;
            else
                return tUntil;
        }
    }

    public static class TargetEnumHelper
    {
        public static ExclusiveAllType ExclusiveWithAll(this TargetType t)
        {
            switch (t)
            {
                case TargetType.Unknown:
                case TargetType.Magic:
                case TargetType.Physics:
                case TargetType.Summon:
                case TargetType.Boss:
                    return ExclusiveAllType.Not;
                case TargetType.NearWithoutSelf:
                    return ExclusiveAllType.HalfExclusive;
                default:
                    return ExclusiveAllType.Exclusive;
            }
        }

        public static bool IgnoresOne(this TargetType t)
        {
            switch (t)
            {
                case TargetType.Unknown:
                case TargetType.Random:
                case TargetType.RandomOnce:
                case TargetType.Absolute:
                case TargetType.Summon:
                case TargetType.SelfSummonRandom:
                case TargetType.AllSummonRandom:
                case TargetType.Magic:
                case TargetType.Physics:
                    return false;
                default:
                    return true;
            }
        }

        public static PluralModifier GetPluralModifier(this TargetCount t)
        {
            return t switch
            {
                TargetCount.One => PluralModifier.One,
                _ => PluralModifier.Many
            };
        }

        public static string GetDescription(this TargetType t, TargetNumber targetNumber, string localizedNumber)
        {
            switch (targetNumber)
            {
                case TargetNumber.Second:
                case TargetNumber.Third:
                case TargetNumber.Fourth:
                case TargetNumber.Fifth:
                    {
                        var localizedModifier = localizedNumber ?? targetNumber.GetDescription();
                        var fd = t.GetFDescriptionAttr();
                        return string.Format(fd.Description2 ?? fd.Description, localizedModifier);
                    }
                default:
                    return t.GetDescription();
            }
        }
    }

    public enum TargetAssignment
    {
        [Description("")]
        None = 0,
        [Description("敌方")]
        Enemy = 1,
        [Description("我方")]
        Friendly = 2,
        [Description("敌我双方")]
        All = 3,
    }

    public enum TargetNumber
    {
        [Description("第1位")]
        First = 0,
        [Description("第2位")]
        Second = 1,
        [Description("第3位")]
        Third = 2,
        [Description("第4位")]
        Fourth = 3,
        [Description("第5位")]
        Fifth = 4,
        [Description("")]
        Other = 5
    }

    public enum TargetCount
    {
        Zero = 0,
        [Description("1体")]
        One = 1,
        [Description("2体")]
        Two = 2,
        [Description("3体")]
        Three = 3,
        [Description("4体")]
        Four = 4,
        [Description("全体")]
        All = 99
    }

    public enum PluralModifier
    {
        [Description("目标")]
        One,
        [Description("目标")]
        Many
    }

    public enum TargetType
    {
        [FDescription("不明", "{0}不明种类")]
        Unknown = -1,
        [FDescription("最近的", "{0}最近的")]
        Zero = 0,
        [FDescription("最近的", "{0}最近的")]
        None = 1,
        [FDescription("随机的", "随机的{0}")]
        Random = 2,
        [FDescription("最近的", "{0}最近的")]
        Near = 3,
        [FDescription("最远的", "{0}最远的")]
        Far = 4,
        [FDescription("HP比例最低的", "HP比例{0}最低的")]
        HpAscending = 5,
        [FDescription("HP比例最高的", "HP比例{0}最高的")]
        HpDescending = 6,
        [Description("自身")]
        Self = 7,
        [FDescription("随机的", "随机的{0}")]
        RandomOnce = 8,
        [FDescription("最后方的", "{0}最后方的")]
        Forward = 9,
        [FDescription("最前方的", "{0}最前方的")]
        Backward = 10,
        [FDescription("范围内的目标")]
        Absolute = 11,
        [FDescription("TP最高的", "TP{0}最高的")]
        TpDescending = 12,
        [FDescription("TP最低的", "TP{0}最低的")]
        TpAscending = 13,
        [FDescription("物理攻击力最高的", "物理攻击力{0}最高的")]
        AtkDescending = 14,
        [FDescription("物理攻击力最低的", "物理攻击力{0}最低的")]
        AtkAscending = 15,
        [FDescription("魔法攻击力最高的", "魔法攻击力{0}最高的")]
        MagicSTRDescending = 16,
        [FDescription("魔法攻击力最低的", "魔法攻击力{0}最低的")]
        MagicSTRAscending = 17,
        [FDescription("召唤物", "{0}召唤物")]
        Summon = 18,
        [FDescription("TP最低的", "TP{0}最低的")]
        TpReducing = 19,
        [FDescription("使用物理攻击的", "使用物理攻击的{0}")]
        Physics = 20,
        [FDescription("使用魔法攻击的", "使用魔法攻击的{0}")]
        Magic = 21,
        [Description("随机的召唤物")]
        AllSummonRandom = 22,
        [Description("自身的随机召唤物")]
        SelfSummonRandom = 23,
        [FDescription("Boss级", "Boss级{0}")]
        Boss = 24,
        [FDescription("HP比例最低的", "HP比例{0}最低的")]
        HpAscendingOrNear = 25,
        [FDescription("HP比例最高的", "HP比例{0}最高的")]
        HpDescendingOrNear = 26,
        [FDescription("TP最高的", "TP{0}最高的")]
        TpDescendingOrNear = 27,
        [FDescription("TP最低的", "TP{0}最低的")]
        TpAscendingOrNear = 28,
        [FDescription("物理攻击力最高的", "物理攻击力{0}最高的")]
        AtkDescendingOrNear = 29,
        [FDescription("物理攻击力最低的", "物理攻击力{0}最低的")]
        AtkAscendingOrNear = 30,
        [FDescription("魔法攻击力最高的", "魔法攻击力{0}最高的")]
        MagicSTRDescendingOrNear = 31,
        [FDescription("魔法攻击力最低的", "魔法攻击力{0}最低的")]
        MagicSTRAscendingOrNear = 32,
        [FDescription("暗影", "{0}暗影")]
        Shadow = 33,
        [FDescription("除自身以外最近的", "除自身以外{0}近的")]
        NearWithoutSelf = 34,
        [FDescription("HP最高的", "HP{0}最高的")]
        HpDescendingOrNearForward,
        [FDescription("HP最低的", "HP{0}最低的")]
        HpAscendingOrNearForward,
        [FDescription("TP最高的", "TP{0}最高的")]
        TpDescendingOrMaxForward,
        [FDescription("物理或魔法攻击力最高的", "物理或魔法攻击力{0}最高的")]
        AtkOrMagicSTRDescending = 38
    }

    public enum ExclusiveAllType
    {
        Not,
        Exclusive,
        HalfExclusive
    }

    public enum RangeType
    {
        Zero = 0,
        All = 1,
        Finite = 2,
        Infinite = 3,
        Unknown = 4
    }

    public class TargetRange
    {
        public int Range { get; set; }
        public RangeType Type { get; set; }

        public TargetRange(int range)
        {
            Range = range;
            if (range == -1)
                Type = RangeType.Infinite;
            else if (range == 0)
                Type = RangeType.Zero;
            else if (range > 0 && range < 2160)
                Type = RangeType.Finite;
            else if (range >= 2160)
            {
                Type = RangeType.All;
                Range = 2160;
                return;
            }
            else
                Type = RangeType.Unknown;
        }
    }

    public enum DirectionType
    {
        [Description("前方（包括自身）")]
        Front = 1,
        [Description("前方和后方")]
        FrontAndBack = 2,
        All = 3
    }
}
