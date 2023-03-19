using PcrBlazor.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcrBlazor.Client
{
    public class ClientSettings : IRankSlotSetting, IAreaSetting
    {
        public string ExportType { get; set; } = "v-c";
        public List<string> ExportFields { get; set; } = new List<string> { nameof(UserBoxLine.Level), nameof(UserBoxLine.Rarity), nameof(UserBoxLine.Promotion) };
        public int SolveNormal { get; set; } = 2;
        public int SolveHard { get; set; }
        public int SolveHardTimes { get; set; }
        public int SolveAreaLimit { get; set; }
        public int SolveForecast { get; set; }
        public bool ShowForecast { get; set; } = true;
        public int MinLevel { get; set; } = 4;
        public AreaSetting AreaSettingJp { get; set; } = new();
        public bool ShowByproduct { get; set; }
        public bool FilterByproduct { get; set; } = true;
        public bool AutoStatusCalc { get; set; } = true;
        public bool ShowAllStatus { get; set; }
        public bool AllowAllRank { get; set; }
        public bool ApplyToAll { get; set; } = true;
        public int MaxRank { get; set; } = 10;
        public int Slots { get; set; } = 4;
        public bool AllowAllUnique { get; set; }
        public bool UseJpUniqueRank { get; set; }
        public bool UseJpRarity6 { get; set; }
        public RankSlotSetting RankSlotSettingJp { get; set; } = new RankSlotSetting { ApplyToAll = true, MaxRank = 18, Slots = 3 };
        public bool AutoConsume { get; set; } = true;
        public bool ShowStockAdd { get; set; } = false;
        public bool ShowEquipsStock { get; set; } = false;
        public bool EnableTargetSlots { get; set; } = false;
        public bool EnableQuestEfficiency { get; set; } = true;
        public bool ShowSkillValueNo { get; set; } = true;
        public bool RemoteSolve { get; set; }
        public bool EnableExtraStock { get; set; }
        public bool IncludeExtraDrops { get; set; }
        public bool IgnoreDonation { get; set; }
        public bool HighlightUnique { get; set; } = true;
        public bool HighlightPriority { get; set; }
        public bool UseJpCraft { get; set; }
        public bool UseCnCraft { get; set; }
        public bool UseOldCraft { get; set; }

        public void Check()
        {
            if (ExportType == "v")
                ExportType = "v-c";
            if (ExportType == "h")
                ExportType = "h-x";
            if (ExportFields == null)
                ExportFields = new List<string> { nameof(UserBoxLine.Level), nameof(UserBoxLine.Rarity), nameof(UserBoxLine.Promotion) };
        }

        public IRankSlotSetting GetRankSetting(string s) => s switch
        {
            "jp" => RankSlotSettingJp,
            _ => this
        };

        public IAreaSetting GetAreaSetting(string s) => s switch
        {
            "jp" => AreaSettingJp,
            _ => this
        };
    }

    public class RankSlotSetting : IRankSlotSetting
    {
        public bool AllowAllRank { get; set; }
        public bool ApplyToAll { get; set; }
        public int MaxRank { get; set; }
        public int Slots { get; set; }
    }

    public interface IRankSlotSetting
    {
        bool AllowAllRank { get; set; }
        bool ApplyToAll { get; set; }
        int MaxRank { get; set; }
        int Slots { get; set; }
    }

    public class AreaSetting : IAreaSetting
    {
        public int SolveNormal { get; set; } = 2;
        public int SolveHard { get; set; }
        public int SolveHardTimes { get; set; }
        public int SolveAreaLimit { get; set; }
        public int SolveForecast { get; set; }
        public bool ShowForecast { get; set; } = true;
        public int MinLevel { get; set; } = 4;
    }

    public interface IAreaSetting
    {
        int SolveNormal { get; set; }
        int SolveHard { get; set; }
        int SolveHardTimes { get; set; }
        int SolveAreaLimit { get; set; }
        int SolveForecast { get; set; }
        int MinLevel { get; set; }
        bool ShowForecast { get; set; }
    }

    public class GroupingSettings
    {
        public int Rank { get; set; }
        public int Level { get; set; }
        public int Rarity { get; set; }
        public int Limit { get; set; } = 20;
        public int MLimit { get; set; } = 3;
        public int MagicLimit { get; set; } = 3;
        public int TeamCnt { get; set; } = 3;
        public bool PointMode { get; set; }
        public bool WithSame { get; set; }
        public List<int> GroupIds { get; set; }
        public List<int> ExceptUnits { get; set; } = new();

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Rank > 0)
                sb.Append($"Rank≥{Rank}；");
            if (Level > 0)
                sb.Append($"等级≥{Level}；");
            if (Rarity > 0)
                sb.Append($"星级≥{Rarity}；");
            if (!GroupIds.IsNullOrEmpty())
                sb.Append($"{GroupIds.Count}组；");
            if (ExceptUnits.Any())
                sb.Append($"排除{ExceptUnits.Count}位角色；");
            if (sb.Length == 0)
                return "无";
            return sb.ToString().Trim('；');
        }
    }
}
