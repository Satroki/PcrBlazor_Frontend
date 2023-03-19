using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PcrBlazor.Shared
{
    public class SimpleEquipmentData
    {
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public int ConsumeNum { get; set; }
        public int StockNum { get; set; }
        public int? StockDelta { get; set; }
        public int PromotionLevel { get; set; }
        public bool CraftFlg { get; set; }
        public List<SimpleEquipmentData> ConsumedEquips { get; set; }
        public List<SimpleEquipmentData> ConsumeByEquips { get; set; }
        public int Gap => ConsumeNum - StockNum;
        public bool IsRaw => EquipmentId > 110000;
        public int? ExtraStock { get; set; }
        [JsonIgnore]
        public HashSet<string> UnitIcons { get; set; }
        public bool IsVisible { get; set; }
        public bool EnableDonation { get; }

        public SimpleEquipmentData()
        {

        }

        public SimpleEquipmentData(EquipmentData e)
        {
            EquipmentId = e.EquipmentId;
            EquipmentName = e.EquipmentName;
            PromotionLevel = e.PromotionLevel;
            CraftFlg = e.CraftFlg;
            IsVisible = e.IsVisible;
            EnableDonation = e.EnableDonation > 0;
        }

        public SimpleEquipmentData(EquipmentData e, int c) : this(e)
        {
            ConsumeNum = c;
            ConsumedEquips = e.ConsumedEquips;
            ConsumeByEquips = e.ConsumeByEquips;
        }

        public IEnumerable<int> GetConsumeTreeIds()
        {
            yield return EquipmentId;
            if (!ConsumedEquips.IsNullOrEmpty())
            {
                foreach (var child in ConsumedEquips)
                {
                    foreach (var t in child.GetConsumeTreeIds())
                    {
                        yield return t;
                    }
                }
            }
        }

        public IEnumerable<(int, int)> GetEquipmentConsumeList()
        {
            yield return (EquipmentId, ConsumeNum);
            if (!ConsumedEquips.IsNullOrEmpty())
            {
                foreach (var child in ConsumedEquips)
                {
                    foreach (var t in child.GetEquipmentConsumeList())
                    {
                        yield return t;
                    }
                }
            }
        }
    }
}
