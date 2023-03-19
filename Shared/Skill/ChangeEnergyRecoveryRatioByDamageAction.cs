using System.Linq;
using System.Text;

namespace PcrBlazor.Shared
{
    internal class ChangeEnergyRecoveryRatioByDamageAction : SkillActionParameter
    {
        public override string GetDescription()
        {
            return $"包括通过受伤获得的TP奖励值在内，使{targetParameter.BuildTargetClause()}通过【动作{GetChildrenActionString()}】获得的所有TP恢复效果值变为【原本的值 * {a.ActionValue_1.ToString(withNo)}】";
        }

        protected string GetChildrenActionString()
        {
            var ids = sm?.ActionIds?.Where(t => t.dependActionId == a.ActionId).Select(t => t.actionId % 10).ToArray();
            if (ids.IsNullOrEmpty())
                return null;
            return string.Join(", ", ids);
        }
    }
}
