using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PcrBlazor.Shared
{
    public class ClanBattleMapData
    {
        [Key]
        public int Id { get; set; }
        public int ClanBattleId { get; set; }
        public int LapNumFrom { get; set; }
        public int LapNumTo { get; set; }
        public int ClanBattleBossGroupId { get; set; }
        public int Phase { get; set; }

        public List<ClanBattleBossGroup> BossGroups { get; set; }
    }
}
