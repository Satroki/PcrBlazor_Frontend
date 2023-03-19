using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PcrBlazor.Shared
{
    public abstract class SkillActionParameter
    {
        protected SkillAction a;
        protected SkillAction da;
        protected SkillModel sm;
        protected bool withNo;
        protected SkillTargetParameter targetParameter;
        protected List<ActionValue> actionValues = new List<ActionValue>();
        protected List<ActionValue> durationValues = new List<ActionValue>();
        protected List<ActionValue> chanceValues = new List<ActionValue>();
        protected List<ActionValue> percentValues = new List<ActionValue>();

        public static SkillActionParameter Create(SkillAction a, SkillAction da, SkillModel sm, bool withNo)
        {
            SkillActionParameter ap = (ActionType)a.ActionType switch
            {
                ActionType.Damage => new DamageAction(),
                ActionType.Move => new MoveAction(),
                ActionType.Knock => new KnockAction(),
                ActionType.Heal => new HealAction(),
                ActionType.Cure => new CureAction(),
                ActionType.Guard => new BarrierAction(),
                ActionType.ChooseArea => new ReflexiveAction(),
                ActionType.Ailment => new AilmentAction(),
                ActionType.Dot => new AilmentAction(),
                ActionType.Aura => new AuraAction(),
                ActionType.Charm => new CharmAction(),
                ActionType.Blind => new AilmentAction(),
                ActionType.Silence => new AilmentAction(),
                ActionType.ChangeMode => new ModeChangeAction(),
                ActionType.Summon => new SummonAction(),
                ActionType.ChangeEnergy => new ChangeEnergyAction(),
                ActionType.Trigger => new TriggerAction(),
                ActionType.DamageCharge => new DamageChargeAction(),
                ActionType.Charge => new ChargeAction(),
                ActionType.Taunt => new DecoyAction(),
                ActionType.Invulnerable => new NoDamageAction(),
                ActionType.ChangePattern => new ChangePatternAction(),
                ActionType.IfForChildren => new IfForChildrenAction(),
                ActionType.Revival => new RevivalAction(),
                ActionType.ContinuousAttack => new ContinuousAttackAction(),
                ActionType.Additive => new AdditiveAction(),
                ActionType.Multiple => new MultipleAction(),
                ActionType.IfForAll => new IfForAllAction(),
                ActionType.ChangeSearchArea => new SearchAreaChangeAction(),
                ActionType.InstantDeath => new DestroyAction(),
                ActionType.ContinuousAttackNearby => new ContinuousAttackNearby(),
                ActionType.EnhanceLifeSteal => new EnchantLifeStealAction(),
                ActionType.EnhanceStrikeBack => new EnchantStrikeBackAction(),
                ActionType.AccumulativeDamage => new AccumulativeDamageAction(),
                ActionType.Seal => new SealAction(),
                ActionType.AttackField => new AttackFieldAction(),
                ActionType.HealField => new HealFieldAction(),
                ActionType.ChangeParameterField => new ChangeParameterFieldAction(),
                ActionType.DotField => new AbnormalStateFieldAction(),
                ActionType.AilmentField => new ChangeSpeedFieldAction(),
                ActionType.ChangeUBTime => new UBChangeTimeAction(),
                ActionType.LoopTrigger => new LoopTriggerAction(),
                ActionType.IfHasTarget => new IfHasTargetAction(),
                ActionType.WaveStartIdle => new WaveStartIdleAction(),
                ActionType.SkillCount => new SkillExecCountAction(),
                ActionType.Gravity => new RatioDamageAction(),
                ActionType.UpperLimitAttack => new UpperLimitAttackAction(),
                ActionType.Hot => new RegenerationAction(),
                ActionType.Dispel => new DispelAction(),
                ActionType.Channel => new ChannelAction(),
                ActionType.Division => new UnknownAction(),
                ActionType.ChangeWidth => new ChangeBodyWidthAction(),
                ActionType.IfExistsFieldForAll => new IFExistsFieldForAllAction(),
                ActionType.Stealth => new StealthAction(),
                ActionType.MoveParts => new MovePartsAction(),
                ActionType.CountBlind => new CountBlindAction(),
                ActionType.CountDown => new CountDownAction(),
                ActionType.StopFieldAction => new StopFieldAction(),
                ActionType.InhibitHealAction => new InhibitHealAction(),
                ActionType.AttackSeal => new AttackSealAction(),
                ActionType.Fear => new FearAction(),
                ActionType.Awe => new AweAction(),
                ActionType.Loop => new LoopMotionRepeatAction(),
                ActionType.Toad => new ToadAction(),
                ActionType.KnightGuard => new KnightGuardAction(),
                ActionType.DamageCut => new DamageCutAction(),
                ActionType.LogBarrier => new LogBarrierAction(),
                ActionType.Divide => new DivideAction(),
                ActionType.ActionByHitCount => new ActionByHitCountAction(),
                ActionType.HealDown => new HealDownAction(),
                ActionType.PassiveSeal => new PassiveSealAction(),
                ActionType.PassiveDamageUp => new PassiveDamageUpAction(),
                ActionType.DamageByBehaviourAction => new DamageByBehaviourAction(),
                ActionType.Ex => new PassiveAction(),
                ActionType.ExPlus => new PassiveInermittentAction(),
                ActionType.ChangeEnergyRecoveryRatioByDamage => new ChangeEnergyRecoveryRatioByDamageAction(),
                ActionType.IgnoreDecoyAction => new IgnoreDecoyAction(),
                ActionType.EffectAction => new EffectAction(),
                ActionType.SpyAction => new SpyAction(),
                ActionType.ChangeEnergyFieldAction => new ChangeEnergyFieldAction(),
                ActionType.ChangeEnergyByDamageAction => new ChangeEnergyByDamageAction(),
                _ => new UnknownAction(),
            };
            ap.a = a;
            ap.da = da;
            ap.sm = sm;
            ap.withNo = withNo;
            ap.targetParameter = new SkillTargetParameter(a, da);
            return ap;
        }

        public abstract string GetDescription();

        public int? RelatedUnit { get; set; }

        protected string BuildExpression(List<ActionValue> actionValues = null, bool hasBracesIfNeeded = false)
        {
            actionValues ??= this.actionValues;
            var expression = actionValues.Select(val =>
            {
                var t = (val.Initial.Value, val.PerLevel.Value);
                var part = string.Empty;
                switch (t)
                {
                    case (0, 0): break;
                    case (0, _): part = $"{val.PerLevel.ToString(withNo)} * 技能等级"; break;
                    case (_, 0): part = $"{val.Initial.ToString(withNo)}"; break;
                    case (_, _): part = $"{val.Initial.ToString(withNo)} + {val.PerLevel.ToString(withNo)} * 技能等级"; break;
                }
                if (!val.StatusKey.IsNullOrEmpty())
                {
                    switch (t)
                    {
                        case (0, 0): break;
                        case (0, _):
                        case (_, 0): part += $" * {UnitStatus.Names[val.StatusKey]}"; break;
                        case (_, _): part = $"({part}) * {UnitStatus.Names[val.StatusKey]}"; break;
                    }
                }
                return part;
            }).Where(s => !s.IsNullOrEmpty()).Join(" + ");
            if (expression.IsNullOrEmpty())
            {
                return "0";
            }
            else
            {
                return hasBracesIfNeeded ? BracesIfNeeded(expression) : expression;
            }
        }

        private static string BracesIfNeeded(string content)
        {
            if (content.Contains("+"))
                return $"({content})";
            else
                return content;
        }

        protected string UnknownExpression()
        {
            return $"{a.ActionId} 效果不明 {a.ActionType}";
        }

        protected static string GetPercent(double v)
        {
            return (int)v == 2 ? "%" : string.Empty;
        }
    }

    public enum ClassModifier
    {
        [Description("未知")]
        Unknown = 0,
        [Description("物理")]
        Physical = 1,
        [Description("魔法")]
        Magical = 2,
        [Description("必中物理")]
        InevitablePhysical = 3
    }

    public enum CriticalModifier
    {
        Normal = 0,
        Critical = 1
    }
}
