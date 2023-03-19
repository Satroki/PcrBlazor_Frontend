namespace PcrBlazor.Shared
{
    public class ClanBattleBossGroup
    {
        public int ClanBattleBossGroupId { get; set; }
        public int OrderNum { get; set; }
        public int WaveGroupId { get; set; }
        public double ScoreCoefficient { get; set; }

        public WaveGroupData WaveGroupData { get; set; }
    }
}
