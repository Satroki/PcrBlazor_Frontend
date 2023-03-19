using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using static PcrBlazor.Shared.CampaignType;

namespace PcrBlazor.Shared
{
    public class CampaignSchedule
    {
        [Key]
        public int Id { get; set; }
        public int CampaignCategory { get; set; }
        public double Value { get; set; }
        public int SystemId { get; set; }
        public int IconImage { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public int LevelId { get; set; }
        public int ShioriGroupId { get; set; }

        [NotMapped]
        public string Category { get; set; }
        [NotMapped]
        public string Level { get; set; }
        [NotMapped]
        public string Bonus { get; set; }
        [NotMapped]
        public int BonusType { get; set; }
        public string GetTitle()
        {
            var type = CampaignCategory.ParseEnum(None);
            switch (type)
            {
                case CoinDungeon:
                case ManaDungeon:
                case DropAmountDungeon:
                    Category = "「地下城」"; break;
                case ManaNormal:
                case DropRareNormal:
                case MasterCoinNormal:
                case HalfStaminaNormal:
                case DropAmountNormal:
                    Category = "「普通」"; break;
                case ExpEventNormal:
                case ManaEventNormal:
                case DropRareEventNormal:
                case DropAmountEventNormal:
                case MasterCoinDropShioriNormal:
                case MasterCoinEventNormal:
                    Level = "普通";
                    Category = "「活动」"; break;
                case ExpRevivalEventNormal:
                case ManaRevivalEventNormal:
                case DropRareRevivalEventNormal:
                case DropAmountRevivalEventNormal:
                case MasterCoinRevivalEventNormal:
                    Level = "普通";
                    Category = "「复刻活动」"; break;
                case ManaHard:
                case DropRareHard:
                case MasterCoinHard:
                case HalfStaminaHard:
                case DropAmountHard:
                    Category = "「困难」"; break;
                case ExpEventHard:
                case ManaEventHard:
                case DropRareEventHard:
                case DropAmountEventHard:
                case MasterCoinDropShioriHard:
                case MasterCoinEventHard:
                    Level = "困难";
                    Category = "「活动」"; break;
                case ExpRevivalEventHard:
                case ManaRevivalEventHard:
                case DropRareRevivalEventHard:
                case DropAmountRevivalEventHard:
                case MasterCoinRevivalEventHard:
                    Level = "困难";
                    Category = "「复刻活动」"; break;
                case DropAmountShrine:
                case MasterCoinShrine:
                case HalfStaminaShrine:
                    Category = "「圣迹调查」"; break;
                case ManaTemple:
                case DropAmountTemple:
                case MasterCoinTemple:
                case HalfStaminaTemple:
                    Category = "「神殿调查」"; break;
                case ManaExploration:
                case DropAmountExploration:
                    Category = "「探索」"; break;
                case ManaVeryHard:
                case DropRareVeryHard:
                case DropAmountVeryHard:
                case MasterCoinVeryHard:
                case HalfStaminaVeryHard:
                    Category = "「高难」"; break;
                default:
                    Category = "其他";
                    break;
            }
            var v = Value / 1000d;
            BonusType = CampaignCategory / 10 % 10;
            Bonus = BonusType switch
            {
                3 => $"掉落{v}倍",
                4 => $"玛那{v}倍",
                //5 => $"经验{v}倍",
                9 => $"大师币{v}倍",
                _ => "",
            };
            return Category + Bonus;
        }

        public static IEnumerable<EventCalendar> GetEventCalendars(IEnumerable<CampaignSchedule> cs)
        {
            foreach (var item in cs)
            {
                item.GetTitle();
            }
            foreach (var g in cs.GroupBy(c => new { c.Value, c.EndTime }))
            {
                var mc = g.Where(c => c.BonusType == 9);
                if (mc.Any())
                {
                    yield return new EventCalendar
                    {
                        Title = mc.First().Bonus,
                        StartTime = mc.Min(c => c.StartTime),
                        EndTime = g.Key.EndTime,
                        Type = EventType.Festival
                    };
                }
                foreach (var sg in g.Where(c => c.BonusType != 9 && c.Bonus != string.Empty).GroupBy(c => new { c.StartTime, c.Category }))
                {
                    var bonus = g.OrderBy(c => c.BonusType).First().Bonus;
                    yield return new EventCalendar
                    {
                        Title = sg.Key.Category + bonus,
                        StartTime = sg.Key.StartTime,
                        EndTime = g.Key.EndTime,
                        Type = EventType.Festival
                    };
                }
            }
        }
    }

    public class CampaignScheduleEqualityComparer : IEqualityComparer<CampaignSchedule>
    {
        public bool Equals(CampaignSchedule x, CampaignSchedule y)
        {
            return true;
        }

        public int GetHashCode(CampaignSchedule obj)
        {
            throw new NotImplementedException();
        }
    }

    public enum CampaignType
    {
        None = 0,
        HalfStaminaNormal = 11,
        HalfStaminaHard,
        HalfStaminaBoth,
        HalfStaminaShrine,
        HalfStaminaTemple,
        HalfStaminaVeryHard,
        DropRareNormal = 21,
        DropRareHard,
        DropRareBoth,
        DropRareVeryHard,
        DropAmountNormal = 31,
        DropAmountHard,
        DropAmountBoth,
        DropAmountExploration,
        DropAmountDungeon,
        DropAmountCoop,
        DropAmountShrine,
        DropAmountTemple,
        DropAmountVeryHard,
        ManaNormal = 41,
        ManaHard,
        ManaBoth,
        ManaExploration,
        ManaDungeon,
        ManaCoop,
        ManaTemple = 48,
        ManaVeryHard,
        CoinDungeon = 51,
        CooltimeArena = 61,
        CooltimeGrandArena,
        MasterCoin = 90,
        MasterCoinNormal,
        MasterCoinHard,
        MasterCoinVeryHard,
        MasterCoinShrine,
        MasterCoinTemple,
        MasterCoinEventNormal,
        MasterCoinEventHard,
        MasterCoinRevivalEventNormal,
        MasterCoinRevivalEventHard,
        MasterCoinDropShioriNormal = 100,
        MasterCoinDropShioriHard,
        HalfStaminaEventNormal = 111,
        HalfStaminaEventHard,
        HalfStaminaEventBoth,
        DropRareEventNormal = 121,
        DropRareEventHard,
        DropRareEventBoth,
        DropAmountEventNormal = 131,
        DropAmountEventHard,
        DropAmountEventBoth,
        ManaEventNormal = 141,
        ManaEventHard,
        ManaEventBoth,
        ExpEventNormal = 151,
        ExpEventHard,
        ExpEventBoth,
        HalfStaminaRevivalEventNormal = 211,
        HalfStaminaRevivalEventHard,
        DropRareRevivalEventNormal = 221,
        DropRareRevivalEventHard,
        DropAmountRevivalEventNormal = 231,
        DropAmountRevivalEventHard,
        ManaRevivalEventNormal = 241,
        ManaRevivalEventHard,
        ExpRevivalEventNormal = 251,
        ExpRevivalEventHard
    }
}
