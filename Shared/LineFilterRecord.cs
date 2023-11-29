using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace PcrBlazor.Shared
{
    using FD = Dictionary<string, Func<UserBoxLine, bool>>;
    public class LineFilterRecord : FilterBase<LineFilterRecord>
    {
        private readonly GroupRecord noGroup = new() { Id = -1, Name = "[未分组]" };
        public string TraceKey { get; set; }
        public string FinKey { get; set; }
        public UnitPosition? Position { get; set; }
        public List<int> GroupIds { get; set; }
        public bool? FilterUnique { get; set; }
        public bool? FilterRarity6 { get; set; }
        public int? Priority { get; set; }
        public int? PriorityTo { get; set; }
        public string Name { get; set; }

        public bool ShowSorter { get; set; } = true;
        public bool ShowTrace { get; set; } = true;
        public bool ShowFin { get; set; } = true;
        public bool ShowPos { get; set; } = true;
        public bool ShowSearch { get; set; } = true;
        public bool ShowUnique { get; set; }
        public bool ShowR6 { get; set; }
        public bool ShowStatusSorter { get; set; } = true;
        public bool ShowPriority { get; set; }
        public bool ShowPriorityTo { get; set; }

        [JsonIgnore]
        public List<GroupRecord> Groups { get; set; }

        [JsonIgnore]
        public IEnumerable<GroupRecord> FilterGroups => noGroup.Yield().Concat(Groups);

        [JsonIgnore]
        public FD TraceFilters { get; } = new FD
        {
            ["常用"] = l => l.Trace,
            ["其他"] = l => !l.Trace,
        };

        [JsonIgnore]
        public FD FinFilters { get; } = new FD
        {
            ["已毕业"] = l => l.IsFinished,
            ["已毕业(不含星)"] = l => l.CheckFinished(false),
            ["已毕业(仅装备)"] = l => l.CheckSlotFinished(),
            ["未毕业"] = l => !l.IsFinished,
            ["未毕业(不含星)"] = l => !l.CheckFinished(false),
            ["未毕业(仅装备)"] = l => !l.CheckSlotFinished(),
        };

        [JsonIgnore]
        public Dictionary<string, UnitPosition> PositionDict { get; } = new Dictionary<string, UnitPosition>
        {
            ["前卫"] = UnitPosition.Front,
            ["中卫"] = UnitPosition.Middle,
            ["后卫"] = UnitPosition.Behind,
        };

        [JsonIgnore]
        public Dictionary<string, bool> UniqueDict { get; } = new Dictionary<string, bool>
        {
            ["有专武"] = true,
            ["无专武"] = false
        };

        [JsonIgnore]
        public Dictionary<string, bool> R6Dict { get; } = new Dictionary<string, bool>
        {
            ["有6星"] = true,
            ["无6星"] = false
        };

        public LineFilterRecord() : base(new List<Sorter>
        {
            new Sorter("常用 ↓","Trace desc"),
            new Sorter("Rank ↓","Promotion desc"),
            new Sorter("星级 ↓","Rarity desc"),
            new Sorter("等级 ↓","Level desc"),
            new Sorter("距离 ↑","SearchAreaWidth asc"),
            new Sorter("距离 ↓","SearchAreaWidth desc"),
            new Sorter("专武 ↓","HasUniqueEquip desc, UniqueEquipRank desc"),
            new Sorter("优先级 ↓","Priority desc"),
            new Sorter("UID ↑","UnitId asc"),
            new Sorter("UID ↓","UnitId desc"),
            new Sorter("游戏内","Level desc, Rarity desc, Promotion desc, UnitId desc"),
        }, "LastFilterRecord")
        { }

        public IEnumerable<UserBoxLine> SetFiltersAndOrder(IEnumerable<UserBoxLine> lines)
        {
            if (ShowTrace && TraceFilters.TryGetValueEx(TraceKey, out var tp))
                lines = lines.Where(tp);
            if (ShowFin && FinFilters.TryGetValueEx(FinKey, out var fp))
                lines = lines.Where(fp);
            if (ShowPos && Position.HasValue)
                lines = lines.Where(l => l.Position == Position);
            if (!Groups.IsNullOrEmpty() && !GroupIds.IsNullOrEmpty())
            {
                var lineIds = Groups.Where(g => GroupIds.Contains(g.Id)).SelectMany(g => g.LineIds).ToList();
                List<int> exIds = null;
                if (GroupIds.Any(i => i < 0))
                {
                    exIds = Groups.SelectMany(g => g.LineIds).ToList();
                }

                lines = (lineIds, exIds) switch
                {
                    (null, null) => lines,
                    (_, null) => lines.Where(l => lineIds.Contains(l.Id)),
                    (null, _) => lines.Where(l => !exIds.Contains(l.Id)),
                    (_, _) => lines.Where(l => lineIds.Contains(l.Id) || !exIds.Contains(l.Id)),
                };
            }
            if (ShowUnique && FilterUnique.HasValue)
                lines = lines.Where(l => l.HasUniqueEquip == FilterUnique);
            if (ShowR6 && FilterRarity6.HasValue)
                lines = lines.Where(l => l.HasRarity6 == FilterRarity6);
            if (ShowPriority)
            {
                if (!ShowPriorityTo && Priority.HasValue)
                    lines = lines.Where(l => (l.Priority ?? 0) == Priority);
                else if (ShowPriorityTo)
                {
                    lines = (Priority, PriorityTo) switch
                    {
                        (null, null) => lines,
                        (_, null) => lines.Where(l => (l.Priority ?? 0) >= Priority),
                        (null, _) => lines.Where(l => (l.Priority ?? 0) <= PriorityTo),
                        (_, _) => lines.Where(l => (l.Priority ?? 0) >= Priority && (l.Priority ?? 0) <= PriorityTo),
                    };
                }
            }
            if (!string.IsNullOrWhiteSpace(Name))
                lines = lines.Where(l => l.UnitName.Contains(Name));
            return SetOrder(lines);
        }
    }

    public class SolveFilterRecord : FilterBase<SolveFilterRecord>
    {
        public SolveFilterRecord() : base(new List<Sorter> {
            new Sorter("次数 ↓","Times desc"),
            new Sorter("效率 ↓","Effective desc"),
            new Sorter("推荐 ↓","Recommend desc"),
            new Sorter("需求 ↓","AllRequirements desc"),
            new Sorter("优先度 ↓","Priority desc"),
        }, "LastSolveFilterRecord")
        { }
    }

    public abstract class FilterBase<T>
    {
        public string LocalKey { get; protected set; }
        public string SorterOrder { get; set; }
        [JsonIgnore]
        public List<Sorter> Sorters { get; private set; }

        public FilterBase(List<Sorter> sorters, string key)
        {
            Sorters = sorters;
            LocalKey = key;
        }

        public void InitSorterOrder()
        {
            var order = SorterOrder?.Split(',');
            if (!order.IsNullOrEmpty())
            {
                Sorters = Sorters.OrderBy(s =>
                {
                    var i = Array.IndexOf(order, s.Key);
                    return i >= 0 ? i : int.MaxValue;
                }).ToList();
            }
        }

        public void SetSorterOrder()
        {
            if (!Sorters.IsNullOrEmpty())
                SorterOrder = string.Join(',', Sorters.Select(s => s.Key));
        }

        public void OnOrderKeyChanged(string key)
        {
            var si = Sorters.FindIndex(s => s.Key == key);
            var st = Sorters[si];
            Sorters.RemoveAt(si);
            Sorters.Insert(0, st);
        }

        public IEnumerable<TItem> SetOrder<TItem>(IEnumerable<TItem> lines)
        {
            var ordering = string.Join(",", Sorters.Select(s => s.Ordering));
            return lines.AsQueryable().OrderBy(ordering);
        }

        public void ChangeLocalKey(string k)
        {
            LocalKey = k;
        }
    }

    public class Sorter
    {
        public string Key { get; set; }
        public string Ordering { get; set; }

        public Sorter()
        {

        }

        public Sorter(string key, string order)
        {
            Key = key;
            Ordering = order;
        }
    }
}
