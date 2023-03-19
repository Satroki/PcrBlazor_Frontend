using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PcrBlazor.Shared
{
    internal class AilmentAction : SkillActionParameter
    {
        private Ailment ailment;

        public override string GetDescription()
        {
            ailment = new Ailment(a.ActionType, a.ActionDetail_1);
            actionValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            chanceValues.Add(new ActionValue(a.ActionValue_3, a.ActionValue_4, null));
            durationValues = chanceValues;

            var t = targetParameter.BuildTargetClause();
            var dur = $"，持续【{BuildExpression(durationValues)}】秒";
            var dur2 = $"，持续【{BuildExpression()}】秒";
            string desp = UnknownExpression();
            switch (ailment.AilmentType)
            {
                case AilmentType.Action:
                    var aad = (AilmentActionDetail)ailment.AilmentDetail;
                    switch (aad)
                    {
                        case AilmentActionDetail.Haste:
                        case AilmentActionDetail.Slow:
                            desp = $"使{t}的攻击速度变为原本的【{BuildExpression()}】倍{dur}"; break;
                        case AilmentActionDetail.Sleep:
                        case AilmentActionDetail.Faint:
                        case AilmentActionDetail.TimeStop:
                            desp = $"使{t}进入{aad.GetDescription()}状态{dur}"; break;
                        default:
                            desp = $"使{t}{aad.GetDescription()}{dur}"; break;
                    }
                    break;
                case AilmentType.Dot:
                    var dd = (DotDetail)ailment.AilmentDetail;
                    switch (dd)
                    {
                        case DotDetail.Poison:
                            desp = $"使{t}中毒，每秒造成【{BuildExpression()}】伤害{dur}";
                            break;
                        case DotDetail.ViolentPoison:
                            desp = $"使{t}进入猛毒状态，每秒造成【{BuildExpression()}】伤害{dur}";
                            break;
                        default:
                            desp = $"使{t}{dd.GetDescription()}，每秒造成【{BuildExpression()}】伤害{dur}";
                            break;
                    }
                    if (a.ActionValue_5 > 0)
                    {
                        desp += $"，伤害每秒增加初始值的{a.ActionValue_5}%";
                    }
                    break;
                case AilmentType.Silence:
                    desp = $"以【{BuildExpression(chanceValues)}%】的概率使{t}进入沉默状态{dur2}";
                    break;
                case AilmentType.Darken:
                    desp = $"以【{BuildExpression(chanceValues)}%】的概率使{t}进入失明状态{dur2}，对象进行物理攻击时有【{100 - a.ActionDetail_1}%】的概率miss";
                    break;
            }
            return desp;
        }
    }
}