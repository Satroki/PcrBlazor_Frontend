using System.ComponentModel;

namespace PcrBlazor.Shared
{
    internal class IfForChildrenAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            var ifType = a.ActionDetail_1.ParseEnum(IfType.Unknown);
            var ad1 = a.ActionDetail_1;
            string trueClause = null;
            string falseClause = null;
            var t = targetParameter.BuildTargetClause(true);
            var isSp = a.ActionId == 104001201;
            if (a.ActionDetail_2 != 0)
            {
                if (ifType != IfType.Unknown)
                {
                    var ac = isSp ? a.ActionDetail_3 : a.ActionDetail_2;
                    trueClause = $"当{t}在{ifType.GetDescription()}的场合，使用【动作{ac % 100}】";
                }
                else
                {
                    trueClause = ad1 switch
                    {
                        (>= 600 and < 700) or 710 => $"{t}持有标记【ID：{ad1 - 600}】的场合，使用【动作{a.ActionDetail_2 % 10}】",
                        700 => $"{t}是单独的场合，使用【动作{a.ActionDetail_2 % 10}】",
                        >= 901 and < 1000 => $"{t}的HP在【{ad1 - 900}%】以下的场合，使用【动作{a.ActionDetail_2 % 10}】",
                        1300 => $"{t}是使用魔法攻击对象的场合，使用【动作{a.ActionDetail_3 % 10}】",
                        1800 => $"{t}是多目标单位的场合，对其使用【动作{a.ActionDetail_2 % 10}】",
                        1900 => $"{t}拥有护盾时，对其使用【动作{a.ActionDetail_2 % 10}】",
                        >= 6000 and < 7000 => $"{t}持有标记【ID：{ad1 - 6000}】的场合，使用【动作{a.ActionDetail_2 % 10}】",
                        _ => null
                    };
                }
            }
            if (a.ActionDetail_3 != 0)
            {
                if (ifType != IfType.Unknown)
                {
                    var ac = isSp ? a.ActionDetail_2 : a.ActionDetail_3;
                    falseClause = $"{t}不在{ifType.GetDescription()}的场合，使用【动作{ac % 100}】";
                }
                else
                {
                    falseClause = ad1 switch
                    {
                        (>= 600 and < 700) or 710 => $"{t}未持有标记【ID：{ad1 - 600}】的场合，使用【动作{a.ActionDetail_3 % 10}】",
                        700 => $"{t}不是单独的场合，使用【动作{a.ActionDetail_3 % 10}】",
                        >= 901 and < 1000 => $"{t}的HP不在【{ad1 - 900}%】以下的场合，使用【动作{a.ActionDetail_3 % 10}】",
                        1300 => $"{t}不是使用魔法攻击对象的场合，使用【动作{a.ActionDetail_2 % 10}】",
                        1800 => $"{t}不是多目标单位的场合，对其使用【动作{a.ActionDetail_3 % 10}】",
                        1900 => $"{t}未拥有护盾时，对其使用【动作{a.ActionDetail_3 % 10}】",
                        >= 6000 and < 7000 => $"{t}未持有标记【ID：{ad1 - 6000}】的场合，使用【动作{a.ActionDetail_3 % 10}】",
                        _ => null,
                    };
                }
            }

            if (ad1 == 100 || ad1 == 101 | ad1 == 200 || ad1 == 300 || (ad1 >= 500 && ad1 <= 512)
               || (ad1 >= 600 && ad1 < 900) || (ad1 >= 901 && ad1 < 1000)
               || ad1 == 1300 || ad1 == 1400 || ad1 == 1600 || ad1 == 1800 || ad1 == 1900 || (ad1 >= 6000 && ad1 < 7000))
            {
                if (trueClause != null || falseClause != null)
                    return $"条件分歧：{trueClause.Join(falseClause, "；")}";
            }
            else if (ad1 >= 0 && ad1 < 100)
            {
                if (a.ActionDetail_2 != 0 && a.ActionDetail_3 != 0)
                    return $"随机事件：【{ad1}%】的概率使用【动作{a.ActionDetail_2 % 10}】，否则使用【动作{a.ActionDetail_3 % 10}】";
                else if (a.ActionDetail_2 != 0)
                    return $"随机事件：【{ad1}%】的概率使用【动作{a.ActionDetail_2 % 10}】";
                else if (a.ActionDetail_3 != 0)
                    return $"随机事件：【{100 - ad1}%】的概率使用【动作{a.ActionDetail_3 % 10}】";
            }
            return UnknownExpression();
        }

    }
    internal enum IfType
    {
        [Description("不明")]
        Unknown,
        [Description("无法行动状态中")]
        Controllered = 100,
        [Description("加速状态中")]
        Hastened = 101,
        [Description("失明状态中")]
        Blind = 200,
        [Description("魅惑或混乱状态中")]
        Convert = 300,
        [Description("挑衅状态中")]
        Decoy = 400,
        [Description("火伤状态中")]
        Burn = 500,
        [Description("诅咒状态中")]
        Curse = 501,
        [Description("中毒状态中")]
        Poison = 502,
        [Description("猛毒状态中")]
        Venom = 503,
        [Description("恶咒状态中")]
        Hex = 504,
        [Description("诅咒或恶咒状态中")]
        CurseOrHex = 511,
        [Description("中毒或猛毒状态中")]
        PoisonOrVenom = 512,
        [Description("Break状态中")]
        Break = 710,
        [Description("变身状态中")]
        Polymorph = 1400,
        [Description("恐慌状态中")]
        Fear = 1600,
        [Description("隐身状态中")]
        Spy = 1601,
        [Description("魔防降低状态中")]
        MagicDefDecreased = 1700
    }
}