using System;
using System.Collections.Generic;

namespace PcrBlazor.Shared
{
    public class ClanBattleInfo
    {
        public int ClanBattleId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string Title { get; set; }
        public DateTimeOffset? StartTime { get; set; }
        public DateTimeOffset? EndTime { get; set; }
        public List<ClanBattlePhase> Phases { get; set; }
        public bool Started { get; set; }
    }
}
