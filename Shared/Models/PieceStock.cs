using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PcrBlazor.Shared
{
    public class PieceStock
    {
        static readonly int[] RarityUp = new[] { 0, 30, 100, 120, 150, 50 };

        public int UnitUid { get; set; }
        public int Stock { get; set; }
        public int LineId { get; set; }
        [JsonIgnore]
        public int Target { get; set; }
        [JsonIgnore]
        public int Heart { get; set; }
        [JsonIgnore]
        public int Beads { get; set; }
        [JsonIgnore]
        public UserBoxLine Line { get; set; }
        [JsonIgnore]
        public bool HideRarity => Line.TargetRarity <= Line.Rarity;
        [JsonIgnore]
        public bool HideUnique => Line.TargetUniqueEquipRank <= Line.UniqueEquipRank;
        [JsonIgnore]
        public bool HideBreak => Line.LevelBreak != false;
        [JsonIgnore]
        public string RarityNote => $"{Line.Rarity} → {Line.TargetRarity} 星";
        [JsonIgnore]
        public string UniqueNote => $"{Line.UniqueEquipRank} → {Line.TargetUniqueEquipRank} 专";
        [JsonIgnore]
        public bool Gap => Stock < Target;

        public PieceStock()
        {

        }

        public PieceStock(UserBoxLine line, bool isSp, List<int[]> ueRankUp)
        {
            UnitUid = line.UnitId / 100;
            Stock = line.Pieces;
            LineId = line.Id;
            Line = line;
            if (Line != null)
                (Target, Heart, Beads) = CalcTarget(Line.Rarity, Line.TargetRarity, Line.UniqueEquipRank, Line.TargetUniqueEquipRank, line.LevelBreak, isSp, ueRankUp);

        }

        public static (int, int, int) CalcTarget(int r, int tr, int u, int tu, bool? lb, bool sp, List<int[]> ueRankUp)
        {
            var target = 0;
            var heart = 0;
            var beads = 0;
            if (tr > r && r < 6)
            {
                for (int i = r; i < 6 && i < tr; i++)
                {
                    target += RarityUp[i];
                }
                if (tr == 6)
                    beads = 100;
            }
            if (tu > u)
            {
                var si = ueRankUp.FindIndex(arr => u <= arr[0]);
                var ei = ueRankUp.FindIndex(arr => tu <= arr[0]);
                if (ei < 0)
                    ei = ueRankUp.Count;
                for (; si < ei; si++)
                {
                    target += ueRankUp[si][1];
                    heart += ueRankUp[si][2];
                }
            }
            if (lb == false)
            {
                target += (sp ? 40 : 120);
            }
            return (target, heart, beads);
        }

        public static (int, int, int) CalcTarget(UserBoxLine oriLine, UserBoxLine newLine, bool sp, List<int[]> ueRankUp)
        {
            bool? lb = null;
            if (newLine.LevelBreak == true && oriLine.LevelBreak != true)
                lb = false;
            return CalcTarget(oriLine.Rarity, newLine.Rarity, oriLine.UniqueEquipRank, newLine.UniqueEquipRank, lb, sp, ueRankUp);
        }
    }
}
