using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcrBlazor.Shared
{
    public class DamageByBehaviourAction : SkillActionParameter
    {
        private Ailment ailment;

        public override string GetDescription()
        {
            ailment = new Ailment(a.ActionType, a.ActionDetail_1);
            actionValues.Add(new ActionValue(a.ActionValue_1, a.ActionValue_2, null));
            durationValues.Add(new ActionValue(a.ActionValue_3, a.ActionValue_4, null));
            return $"每当{targetParameter.BuildTargetClause()}行动时，使其进入{ailment.GetDescription()}状态，每次行动受到【{BuildExpression()}】伤害，持续【{BuildExpression(durationValues)}】秒";
        }
    }
}
