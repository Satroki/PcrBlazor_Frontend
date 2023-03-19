using System;
using System.ComponentModel;

namespace PcrBlazor.Shared
{
    public class Ailment
    {
        public AilmentType AilmentType { get; }
        public Enum AilmentDetail { get; }
        public Ailment(int type, int detail)
        {
            AilmentType = type.ParseEnum(AilmentType.Unknown);
            AilmentDetail = AilmentType switch
            {
                AilmentType.Action => detail.ParseEnum(AilmentActionDetail.Unknown),
                AilmentType.Dot or AilmentType.DamageByBehaviour => detail.ParseEnum(DotDetail.Unknown),
                AilmentType.Charm => detail.ParseEnum(CharmDetail.Unknown),
                _ => null,
            };
        }

        public string GetDescription()
        {
            return AilmentDetail?.GetDescription() ?? AilmentType.GetDescription();
        }
    }

    public enum DotDetail
    {
        [Description("禁锢（伤害）")]
        Detain = 0,
        [Description("毒")]
        Poison = 1,
        [Description("火伤")]
        Burn = 2,
        [Description("诅咒")]
        Curse = 3,
        [Description("猛毒")]
        ViolentPoison = 4,
        [Description("恶咒")]
        Hex = 5,
        Compensation = 6,
        [Description("不明")]
        Unknown = -1
    }

    public enum CharmDetail
    {
        [Description("魅惑")]
        Charm = 0,
        [Description("混乱")]
        Confuse = 1,
        [Description("不明")]
        Unknown = -1
    }

    public enum AilmentActionDetail
    {
        [Description("减速")]
        Slow = 1,
        [Description("加速")]
        Haste = 2,
        [Description("麻痹")]
        Paralyse = 3,
        [Description("冻结")]
        Freeze = 4,
        [Description("束缚")]
        Bind = 5,
        [Description("睡眠")]
        Sleep = 6,
        [Description("眩晕")]
        Stun = 7,
        [Description("石化")]
        Petrify = 8,
        [Description("拘留")]
        Detain = 9,
        [Description("气绝")]
        Faint = 10,
        [Description("时间停止")]
        TimeStop = 11,
        [Description("不明")]
        Unknown = 12
    }

    public enum AilmentType
    {
        [Description("击退")]
        KnockBack = 3,
        [Description("动作")]
        Action = 8,
        [Description("持续伤害")]
        Dot = 9,
        [Description("魅惑")]
        Charm = 11,
        [Description("失明")]
        Darken = 12,
        [Description("沉默")]
        Silence = 13,
        [Description("混乱")]
        Confuse = 19,
        [Description("即死")]
        InstantDeath = 30,
        [Description("千里眼")]
        CountBlind = 56,
        [Description("回复阻碍")]
        InhibitHeal = 59,
        [Description("刻印赋予")]
        AttackSeal = 60,
        [Description("恐慌")]
        Fear = 61,
        [Description("畏怖")]
        Awe = 62,
        [Description("驯鹿化")]
        Toad = 69,
        [Description("HP变化")]
        MaxHP = 70,
        [Description("HP回复量減少")]
        HPRegenerationDown = 76,
        [Description("受到伤害增加")]
        DamageTakenIncreased = 78,
        [Description("行动时伤害")]
        DamageByBehaviour = 79,
        [Description("不明")]
        Unknown = 80,
    }
}