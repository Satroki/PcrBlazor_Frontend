using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PcrBlazor.Shared
{
    public class SolveResult
    {
        public int QuestId { get; set; }
        public string QuestName { get; set; }
        public int AllRequirements { get; set; }
        public int Effective { get; set; }
        public int Recommend { get; set; }
        public int Times { get; set; }
        public int Stamina { get; set; }
        public List<int> EquipmentIds { get; set; }
        public List<int> Expected { get; set; }
        public int Priority { get; set; }
        public List<int> UniqueIds { get; set; }

        public List<int> NoDrop { get; set; }
        [JsonIgnore]
        public List<SimpleEquipmentData> Equipments { get; set; }
        [JsonIgnore]
        public List<SimpleEquipmentData> RestEquipments { get; set; }

        public void SortEquipment()
        {
            var zip = EquipmentIds.Zip(Expected).OrderByDescending(d => d.Second).ThenByDescending(d => d.First % 10000);
            EquipmentIds = zip.Select(t => t.First).ToList();
            Expected = zip.Select(t => t.Second).ToList();
        }
    }
}
