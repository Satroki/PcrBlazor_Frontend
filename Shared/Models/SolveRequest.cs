namespace PcrBlazor.Shared
{
    public class SolveRequest
    {
        public string Server { get; set; }
        public int[] EquipmentIds { get; set; }
        public int[] Requirements { get; set; }
        public int Normal { get; set; }
        public int Hard { get; set; }
        public int HardTimes { get; set; }
        public int AreaLimit { get; set; }
        public int Forecast { get; set; }
        public int MinLevel { get; set; }

        public double GetOdds(int qid, double odds)
        {
            if (qid < 12_000_000)
                return odds * Normal;
            else
                return odds * Hard;
        }
    }
}
