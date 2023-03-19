using System.ComponentModel.DataAnnotations;

namespace PcrBlazor.Shared
{
    public class ItemData
    {
        [Key]
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int PromotionLevel { get; set; }
        public int ItemType { get; set; }
        public int Value { get; set; }
        public int Price { get; set; }
        public int LimitNum { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }

}
