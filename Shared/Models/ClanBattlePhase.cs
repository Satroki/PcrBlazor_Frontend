using System.Collections.Generic;
using System.Linq;

namespace PcrBlazor.Shared
{
    public class ClanBattlePhase
    {
        public int LapFrom { get; set; }
        public int LapTo { get; set; }
        public List<ClanBattleBoss> Bosses { get; set; }
        public ClanBattlePhase()
        {

        }

        public ClanBattlePhase(ClanBattleMapData m)
        {
            LapFrom = m.LapNumFrom;
            LapTo = m.LapNumTo;
            Bosses = m.BossGroups.OrderBy(g => g.OrderNum).Select(b => new ClanBattleBoss(b)).ToList();
        }

        public ClanBattlePhase(ClanBattle2MapData m)
        {
            LapFrom = m.LapNumFrom;
            LapTo = m.LapNumTo;
            Bosses = m.WaveGroupDatas.OrderBy(g => g.WaveGroupId).Select((w, i) => new ClanBattleBoss(w, m.ScoreCoefficients[i])).ToList();
        }
    }
}
