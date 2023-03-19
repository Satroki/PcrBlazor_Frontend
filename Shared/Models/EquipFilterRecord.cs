using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic;

namespace PcrBlazor.Shared
{
    public class EquipFilterRecord : FilterBase<EquipFilterRecord>
    {
        public int MinLevel { get; set; } = 4;
        public int? MaxLevel { get; set; }
        public string Key { get; set; } = null;
        public int StockFilter { get; set; } = 0;
        public int TypeFilter { get; set; } = 2;
        public bool ShowAll { get; set; }
        public string CustomFilter { get; set; }

        public bool ShowStockFilter { get; set; } = true;
        public bool ShowTypeFilter { get; set; } = true;
        public bool ShowLevelFilter { get; set; } = true;

        public EquipFilterRecord() : base(new(), "LastEquipFilterRecord")
        {

        }

        [JsonIgnore]
        public Dictionary<string, int> StockDict { get; } = new Dictionary<string, int>
        {
            ["库存无关"] = 0,
            ["有库存"] = 1,
            ["无库存"] = 2,
            ["需囤积"] = 3,
        };

        [JsonIgnore]
        public Dictionary<string, int> TypeDict { get; } = new Dictionary<string, int>
        {
            ["所有类型"] = 0,
            ["成品"] = 1,
            ["材料"] = 2,
        };

        public IEnumerable<SimpleEquipmentData> SetFiltersAndOrder(IEnumerable<SimpleEquipmentData> lines, bool withStock)
        {
            var q = ShowAll ? lines : lines.Where(l => l.IsVisible);
            q = Key switch
            {
                null => q,
                "" => q,
                _ => q.Where(e => e.EquipmentName.Contains(Key))
            };
            q = TypeFilter switch
            {
                1 => q.Where(e => e.CraftFlg),
                2 => q.Where(e => !e.CraftFlg),
                _ => q
            };
            if (withStock)
            {
                q = StockFilter switch
                {
                    1 => q.Where(e => e.StockNum > 0),
                    2 => q.Where(e => e.StockNum <= 0),
                    3 => q.Where(e => e.ExtraStock > 0),
                    _ => q
                };
            }
            if (MaxLevel.HasValue)
                q = q.Where(e => e.PromotionLevel <= MaxLevel);
            q = q.Where(e => e.PromotionLevel >= MinLevel);
            q = q.OrderByDescending(d => d.PromotionLevel).ThenByDescending(d => d.EquipmentId % 10000).ThenByDescending(d => d.EquipmentId);
            if (!string.IsNullOrWhiteSpace(CustomFilter))
            {
                var fs = CustomFilter.Replace("库存", nameof(SimpleEquipmentData.StockNum)).Replace("装等", nameof(SimpleEquipmentData.PromotionLevel));
                return q.AsQueryable().Where(fs).ToList();
            }
            return q.ToList();
        }
    }
}
