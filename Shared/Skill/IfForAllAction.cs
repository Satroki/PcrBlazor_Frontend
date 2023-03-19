namespace PcrBlazor.Shared
{
    internal class IfForAllAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            string trueClause = null;
            string falseClause = null;
            var ifType = a.ActionDetail_1.ParseEnum(IfType.Unknown);
            var ad2 = a.ActionDetail_2;
            var ad1 = a.ActionDetail_1;
            var ad3 = a.ActionDetail_3;
            var tf = targetParameter.BuildTargetClause();
            var tt = targetParameter.BuildTargetClause(true);
            if (ad2 != 0)
            {
                trueClause = ad1 switch
                {
                    710 or 100 or 1700 or 1601 => ifType != IfType.Unknown ? $"当{tt}在{ifType.GetDescription()}的场合，使用【动作{ad2 % 100}】" : null,
                    >= 0 and < 100 => $"以【{ad1}%】的概率使用【动作{ad2 % 10}】",
                    >= 500 and < 512 => $"{tf}在{ifType.GetDescription()}的场合，使用【动作{ad2 % 10}】",
                    599 => $"{tf}身上挂有Dot的场合，使用【动作{ad2 % 10}】",
                    >= 600 and < 700 => $"{tt}的标记【ID：{ad1 - 600}】层数在【{a.ActionValue_3.ToString(withNo)}】以上的场合，使用【动作{ad2 % 10}】",
                    700 => $"{tt}是单独的场合，使用【动作{ad2 % 10}】",
                    >= 701 and < 710 => $"排除潜伏状态的单位，{tf}的数量是【{ad1 - 700}】的场合，使用【动作{ad2 % 10}】",
                    720 => $"排除潜伏状态的单位，{tf}中存在【ID：{a.ActionValue_3.ToString(withNo)}】的单位的场合，使用【动作{ad2 % 10}】",
                    721 => $"{tt}持有标记【ID：{a.ActionValue_3.ToString(withNo)}】的场合，使用【动作{ad2 % 10}】",
                    >= 901 and < 1000 => $"{tt}的HP在【{ad1 - 900}%】以下的场合，使用【动作{ad2 % 10}】",
                    1000 => $"上一个动作击杀了单位的场合，使用【动作{ad2 % 10}】",
                    1001 => $"技能暴击的场合，使用【动作{ad2 % 10}】",
                    >= 1200 and < 1300 => $"计数器的数量在【{ad1 % 10}】以上的场合，使用【动作{ad2 % 10}】",
                    >= 6000 and < 7000 => a.ActionValue_3 == 0
                        ? $"{tt}持有标记【ID：{ad1 - 6000}】的场合，使用【动作{ad2 % 10}】"
                        : $"{tt}的标记【ID：{ad1 - 6000}】层数在【{a.ActionValue_3.ToString(withNo)}】以上的场合，使用【动作{ad2 % 10}】",
                    _ => null
                };
            }
            if (ad3 != 0)
            {
                falseClause = ad1 switch
                {
                    710 or 100 or 1700 or 1601 => ifType != IfType.Unknown ? $"{tt}不在{ifType.GetDescription()}的场合，使用【动作{ad3 % 100}】" : null,
                    >= 0 and < 100 => $"以【{100 - ad1}%】的概率使用【动作{ad3 % 10}】",
                    >= 500 and < 512 => $"{tf}不在{ifType.GetDescription()}的场合，使用【动作{ad3 % 10}】",
                    599 => $"{tf}身上没有Dot的场合，使用【动作{ad3 % 10}】",
                    >= 600 and < 700 => $"{tt}的标记【ID：{ad1 - 600}】层数不足【{a.ActionValue_3.ToString(withNo)}】的场合，使用【动作{ad3 % 10}】",
                    700 => $"{tt}不是单独的场合，使用【动作{ad3 % 10}】",
                    >= 701 and < 710 => $"排除潜伏状态的单位，{tf}的数量不是【{ad1 - 700}】的场合，使用【动作{ad3 % 10}】",
                    720 => $"排除潜伏状态的单位，{tf}中不存在【ID：{a.ActionValue_3.ToString(withNo)}】的单位的场合，使用【动作{ad3 % 10}】",
                    721 => $"{tt}未持有标记【ID：{a.ActionValue_3.ToString(withNo)}】的场合，使用【动作{ad3 % 10}】",
                    >= 901 and < 1000 => $"{tt}的HP不在【{ad1 - 900}%】以下的场合，使用【动作{ad3 % 10}】",
                    1000 => $"上一个动作没有击杀单位的场合，使用【动作{ad3 % 10}】",
                    1001 => $"技能未暴击的场合，使用【动作{ad3 % 10}】",
                    >= 1200 and < 1300 => $"计数器的数量不足【{ad1 % 10}】的场合，使用【动作{ad3 % 10}】",
                    >= 6000 and < 7000 => a.ActionValue_3 == 0
                       ? $"{tt}未持有标记【ID：{ad1 - 6000}】的场合，使用【动作{ad3 % 10}】"
                       : $"{tt}的标记【ID：{ad1 - 6000}】层数不足【{a.ActionValue_3.ToString(withNo)}】的场合，使用【动作{ad3 % 10}】",
                    _ => null
                };
            }

            if (trueClause != null || falseClause != null)
                return $"全体条件分歧：{trueClause.Join(falseClause, "；")}";
            else
                return UnknownExpression();
        }
    }
}