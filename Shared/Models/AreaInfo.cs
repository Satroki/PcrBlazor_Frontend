using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace PcrBlazor.Shared
{
    public class AreaInfo
    {
        public int AreaId { get; set; }
        public int AreaType { get; set; }
        public int AreaLevel { get; set; }
        public string TypeName { get; set; }
        public string AreaName { get; set; }
        public List<QuestInfo> Quests { get; set; }
        public AreaInfo()
        {

        }
        public AreaInfo(int areaId, string areaName, IEnumerable<QuestInfo> quests)
        {
            AreaId = areaId;
            AreaName = areaName;
            Quests = quests.ToList();
            AreaType = areaId / 1000;
            AreaLevel = areaId % 100;
            TypeName = GetTypeName(AreaType);
        }

        public static string GetTypeName(int areaType)
        {
            return areaType switch
            {
                11 => "普通",
                12 => "困难",
                13 => "高难",
                18 => "圣迹调查",
                19 => "神殿调查",
                21 => "探索",
                _ => "其他"
            };
        }
    }

    public class QuestInfo
    {
        public int QuestId { get; set; }
        public string QuestName { get; set; }
        public int QuestLevel { get; set; }
        public int Section { get; set; }
        public int AreaType { get; set; }
        public int Stamina { get; set; }
        public int AreaId { get; set; }
        public List<QuestDropItem> DropItems { get; set; }
        public double? Efficiency { get; set; }

        public QuestInfo()
        {

        }

        public QuestInfo(QuestData qd)
        {
            QuestId = qd.QuestId;
            QuestName = qd.QuestName;
            Stamina = qd.Stamina;
            AreaId = qd.AreaId;
            QuestLevel = QuestId / 1000 % 100;
            Section = QuestId % 100;
            AreaType = AreaId / 1000;
        }
    }

    public class ShioriEvent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Unit1 { get; set; }
        public int Unit2 { get; set; }

        public List<QuestPlan> Plans1 { get; set; }
        public List<QuestPlan> Plans2 { get; set; }

        public IEnumerable<(int unit, List<QuestPlan> plans)> GetUnitPlans()
        {
            yield return (Unit1, Plans1);
            yield return (Unit2, Plans2);
        }

        public IEnumerable<QuestPlan> GetAllPlans()
        {
            yield return Plans1[0];
            yield return Plans2[0];
            yield return Plans1[1];
            yield return Plans2[1];
            yield return Plans1[2];
        }
    }

    public class QuestDropItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Odds { get; set; }
        public bool IsItem { get; set; }
        public int? Demand { get; set; }
        public double? DoubleOdds { get; set; }
        public QuestDropItem()
        {

        }
        public QuestDropItem(int rewardId, int odds, string name)
        {
            Id = rewardId;
            IsItem = Id < 100_000;
            Odds = odds;
            Name = name;
        }
    }

    public class DropItemIdEqualityComparer : IEqualityComparer<QuestDropItem>
    {
        public bool Equals(QuestDropItem x, QuestDropItem y)
        {
            return x?.Id == y?.Id;
        }

        public int GetHashCode([DisallowNull] QuestDropItem obj)
        {
            return obj.Id.GetHashCode();
        }
    }

    public class AreaInfoIdEqualityComparer : IEqualityComparer<AreaInfo>
    {
        public bool Equals(AreaInfo x, AreaInfo y)
        {
            return x?.AreaId == y?.AreaId;
        }

        public int GetHashCode([DisallowNull] AreaInfo obj)
        {
            return obj.AreaId.GetHashCode();
        }
    }
}
