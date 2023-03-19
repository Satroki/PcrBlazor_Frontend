using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcrBlazor.Shared
{
    public class ChangeEnergyByDamageAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            actionValues.Add(new(a.ActionValue_1, a.ActionValue_2, null));
            return a.ActionDetail_1 switch
            {
                1 => $"对{targetParameter.BuildTargetClause()}追加【{a.ActionValue_3.ToString(withNo)}】层标记【ID: {a.ActionDetail_2}】（最大【{a.ActionValue_4.ToString(withNo)}】层），持续时间【{a.ActionValue_5.ToString(withNo)}】秒。当受到伤害时，移除 1 层标记并恢复【{BuildExpression(actionValues)}】TP。",
                _ => UnknownExpression()
            };
        }
    }
}
