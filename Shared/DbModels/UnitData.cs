using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PcrBlazor.Shared
{
    public class UnitData : ISearchAreaWidth
    {
        [Key]
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int Rarity { get; set; }
        public int SearchAreaWidth { get; set; }
        public int GuildId { get; set; }
        public string Comment { get; set; }
        public int AtkType { get; set; }
        public int Cutin1_Star6 { get; set; }
        public int IsLimited { get; set; }
        public DateTimeOffset EndTime { get; set; }
        [ForeignKey(nameof(UnitId))]
        public UnitBackground UnitBackground { get; set; }
        [ForeignKey(nameof(UnitId))]
        public UnitProfile Profile { get; set; }
        [NotMapped]
        public string Icon { get; set; }
        [NotMapped]
        [JsonIgnore]
        public bool InBox { get; set; }
        [NotMapped]
        public int[] Cards { get; set; }
        [NotMapped]
        public AttackPattern[] AttackPatterns { get; set; }
        [NotMapped]
        public UnitSkillModel UnitSkillModel { get; set; }
        [NotMapped]
        [JsonIgnore]
        public bool IsSelected { get; set; }
        [NotMapped]
        public List<UnitData> SubUnits { get; set; }
        [NotMapped]
        public UnitData Star6Data { get; set; }
    }

    public class UnitIdEqualityComparer : IEqualityComparer<UnitData>
    {
        public bool Equals(UnitData x, UnitData y)
        {
            return x?.UnitId == y?.UnitId;
        }

        public int GetHashCode(UnitData obj)
        {
            return obj.UnitId.GetHashCode();
        }
    }

    public class UnitUniqueEquip
    {
        [Key]
        public int UnitId { get; set; }
        public int EquipId { get; set; }
    }

    public class UnitUniqueEquipment
    {
        public int UnitId { get; set; }
        public int EquipSlot { get; set; }
        public int EquipId { get; set; }
    }

    public class ExperienceUnit
    {
        [Key]
        public int UnitLevel { get; set; }
        public int TotalExp { get; set; }
    }
    public class ExperienceTeam
    {
        [Key]
        public int TeamLevel { get; set; }
        public int TotalExp { get; set; }
    }

    public class LoveChara
    {
        [Key]
        public int LoveLevel { get; set; }
        public int TotalLove { get; set; }
    }

    public class StoryDetail
    {
        [Key]
        public int StoryId { get; set; }
        public int LoveLevel { get; set; }
    }

    public class UnitBackground
    {
        [Key]
        public int UnitId { get; set; }
        public string UnitName { get; set; }
    }

    public class UnitConversion
    {
        [Key]
        public int OriginalUnitId { get; set; }
        public int UnitId { get; set; }
    }
}
