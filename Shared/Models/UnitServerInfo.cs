using System;
using System.Collections.Generic;
using System.Text;

namespace PcrBlazor.Shared
{
    public class UnitServerInfo
    {
        public string Server { get; set; }
        public int MaxLevel { get; set; }
        public int MaxRank { get; set; }
        public int MaxLoveLevel { get; set; }
        public int MaxUniqueLevel { get; set; }
        public int[] Rarity6Units { get; set; }
        public int[] UniqueEquipUnits { get; set; }
        public int[] UniqueEquip2Units { get; set; }
        public int[] LimitedUnits { get; set; }

        public DateTimeOffset LastModified { get; set; }
        public int MaxSlots { get; set; }

        public Dictionary<string, DateTimeOffset> LastModifyDict { get; set; }
        public List<int[]> UniqueEquipmentRankup { get; set; }
    }
}
