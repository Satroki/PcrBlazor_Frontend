using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PcrBlazor.Shared
{
    // \((.*)\)

    public enum DispelType
    {
        Unknown = 0,
        [Description("增益状态")]
        Buff = 1,
        [Description("减益状态")]
        Debuff = 2
    }
    public enum Condition
    {
        Unknown = -1,
        Damage = 1,
        Target = 2,
        Hit = 3,
        CriticalHit = 4
    }

    public enum HPtype
    {
        Unknown = 0,
        Max = 1,
        Current = 2,
        OriginalMax
    }
    public enum RegenerationType
    {
        Unknown = -1,
        [Description("HP")]
        Hp = 1,
        [Description("TP")]
        Tp = 2
    }

    public enum TriggerType
    {
        Unknown = 0,
        Dodge = 1,
        Damaged = 2,
        Hp = 3,
        Dead = 4,
        CriticalDamaged = 5,
        GetCriticalDamagedWithSummon = 6
    }

    public enum BarrierType
    {
        Unknown = 0,
        PhysicalGuard = 1,
        MagicalGuard = 2,
        PhysicalDrain = 3,
        MagicalDrain = 4,
        BothGuard = 5,
        BothDrain = 6
    }
    public enum FieldType
    {
        Normal,
        Repeat
    }
    public enum AuraType
    {
        Atk = 1,
        Def = 2,
        MagicStr = 3,
        MagicDef = 4,
        Dodge = 5,
        PhysicalCritical = 6,
        MagicCritical = 7,
        EnergyRecoveryRate = 8,
        LifeSteal = 9,
        [Description("移动速度")]
        MoveSpeed = 10,
        [Description("物理爆伤")]
        PhysicalCriticalDamage = 11,
        [Description("魔法爆伤")]
        MagicalCriticalDamage = 12,
        Accuracy = 13,
        [Description("爆伤抗性")]
        CriticalDef = 14,
        [Description("最大HP")]
        MaxHP = 100,
        None = 999
    }


    public enum AuraActionType
    {
        [Description("提高")]
        Raise,
        [Description("降低")]
        Reduce = 1
    }

    public enum BreakType
    {
        Unknown = -1,
        Normal = 1,
        Break = 2
    }

    public enum PassiveTiming
    {
        Unknown = -1,
        [Description("增益效果")]
        Buff = 1
    }

    public enum SealTarget
    {
        Unknown = -1,
        [Description("自身")]
        Self = 0
    }

    public enum CountType
    {
        Unknown = -1,
        [Description("减益効果")]
        Debuff = 1
    }

    public enum EffectType
    {
        Unknown = -1,
        [Description("+")]
        Add = 1,
        [Description("-")]
        Subtract = 2
    }
}
