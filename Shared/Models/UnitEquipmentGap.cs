using System;
using System.Collections.Generic;
using System.Linq;

namespace PcrBlazor.Shared
{
    public class UnitEquipmentGap
    {
        public int UnitId { get; set; }
        public string Icon { get; set; }
        public List<SimpleEquipmentData> Gap { get; set; }
        public List<SimpleEquipmentData> Consume { get; set; }
        public int? Priority { get; set; }
    }

    public class UnitEquipmentConsume
    {
        public int UnitId { get; set; }
        public string Icon { get; set; }
        public int Consume { get; set; }
        public int Priority { get; set; }
        public string Class { get; set; }

        public UnitEquipmentConsume()
        {

        }

        public UnitEquipmentConsume(UnitEquipmentGap ug, int consume)
        {
            UnitId = ug.UnitId;
            Icon = ug.Icon;
            Consume = consume;
            Priority = Math.Max(ug.Priority ?? 0, 0);
        }
    }

    public class UnitEquipmentConsumeDict
    {
        private readonly Dictionary<int, List<UnitEquipmentConsume>> unitConsumes = new();
        public bool WithPriority { get; }
        public List<UnitEquipmentConsume> this[int eid]
        {
            get
            {
                unitConsumes.TryGetValue(eid, out var list);
                return list;
            }
        }

        public UnitEquipmentConsumeDict(List<UnitEquipmentGap> unitGaps)
        {
            foreach (var ug in unitGaps)
            {
                WithPriority = WithPriority || ug.Priority > 0;
                foreach (var se in ug.Consume)
                {
                    unitConsumes.AddList(se.EquipmentId, new(ug, se.ConsumeNum));
                }
            }
            foreach (var key in unitConsumes.Keys)
            {
                unitConsumes[key] = unitConsumes[key]
                    .OrderByDescending(t => t.Priority)
                    .ThenByDescending(t => t.Consume)
                    .ThenByDescending(t => t.UnitId)
                    .ToList();
            }
        }
    }
}
