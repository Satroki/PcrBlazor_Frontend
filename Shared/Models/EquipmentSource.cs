using System.Collections.Generic;
using System.Linq;

namespace PcrBlazor.Shared
{
    public class EquipmentSource
    {
        private Dictionary<int, double> questOdds;
        public int EquipmentId { get; set; }
        public int[] QuestIds { get; set; }
        public double[] Odds { get; set; }
        public EquipmentSource()
        {

        }
        public EquipmentSource(int equipmentId)
        {
            EquipmentId = equipmentId;
            questOdds = new();
        }

        public void AddSource(int questId, double odds)
        {
            if (questOdds.ContainsKey(questId))
                questOdds[questId] += odds;
            else
                questOdds[questId] = odds;
        }

        public void FinishAdd()
        {
            if (questOdds != null)
            {
                var kvs = questOdds.OrderBy(kv => kv.Key);
                QuestIds = kvs.Select(kv => kv.Key).ToArray();
                Odds = kvs.Select(kv => kv.Value).ToArray();
                questOdds.Clear();
                questOdds = null;
            }
        }
    }
}
