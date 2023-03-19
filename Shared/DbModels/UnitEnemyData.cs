using System.ComponentModel.DataAnnotations;

namespace PcrBlazor.Shared
{
    public class UnitEnemyData
    {
        [Key]
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int SearchAreaWidth { get; set; }
        public int AtkType { get; set; }
        public string Comment { get; set; }
    }
}
