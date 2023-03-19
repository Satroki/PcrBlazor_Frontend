using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace PcrBlazor.Shared
{
    public class UserTeamGroup : IUserId
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Server { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public bool UnitReusable { get; set; }
        public bool WithSupport { get; set; }
        public bool AllowShare { get; set; }
        public List<UserTeam> Teams { get; set; } = new List<UserTeam>();
        [NotMapped]
        [JsonIgnore]
        public double TotalDamage { get; set; }
        [NotMapped]
        [JsonIgnore]
        public int Index { get; set; }
    }

    public class UserTeam
    {
        [Key]
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public List<int> UnitIds { get; set; } = new List<int>();
        public int? SupporterId { get; set; }
        public string SupporterNote { get; set; }
        public string Axis { get; set; }
        public List<string[]> Links { get; set; }

        [NotMapped]
        [JsonIgnore]
        public List<ISearchAreaWidth> Units { get; set; } = new List<ISearchAreaWidth>();
        [NotMapped]
        [JsonIgnore]
        public bool Selected { get; set; }
        [NotMapped]
        public int UnitsHash { get; set; }
        [NotMapped]
        [JsonIgnore]
        public string PhaseName { get; set; }
        [NotMapped]
        [JsonIgnore]
        public int CbTeamId { get; set; }
        public void SortUnits()
        {
            Units = Units.OrderByDescending(u => u.SearchAreaWidth).ToList();
            UnitsHash = Units.GetUnitsHashCode();
        }

        public void Clear()
        {
            UnitIds.Clear();
            Units.Clear();
            SupporterId = null;
        }
    }

    public class ClanBattleTeam
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Server { get; set; }
        public string Name { get; set; }
        public string Phase { get; set; }
        public int Boss { get; set; }
        public string Note { get; set; }
        public double Damage { get; set; }
        public double PointRate { get; set; }
        public List<int> UnitIds { get; set; } = new List<int>();
        public string Axis { get; set; }
        public bool Auto { get; set; }
        [NotMapped]
        public bool AutoH { get; set; }
        [NotMapped]
        public bool Tail { get; set; }
        [NotMapped]
        [JsonIgnore]
        public string TypeDesp => GetTypeDesp();
        public bool FromDoc { get; set; }
        public int Like { get; set; }
        public List<string[]> Links { get; set; }

        [NotMapped]
        public bool Liked { get; set; }

        [NotMapped]
        [JsonIgnore]
        public List<UnitData> Units { get; set; } = new List<UnitData>();
        [NotMapped]
        [JsonIgnore]
        public string PhaseName => $"{Phase}{Boss}";
        [NotMapped]
        [JsonIgnore]
        public bool ShowAxis { get; set; }
        [NotMapped]
        [JsonIgnore]
        public bool Enabled { get; set; } = true;
        [NotMapped]
        [JsonIgnore]
        public bool Checked { get; set; }
        [NotMapped]
        [JsonIgnore]
        public int Supporter { get; set; }
        [NotMapped]
        public int? Limit { get; set; }
        [NotMapped]
        [JsonIgnore]
        public bool NoSupporter { get; set; }
        [NotMapped]
        public int AtkType { get; set; }
        [NotMapped]
        public int UnitsHash { get; set; }
        public void SortUnits()
        {
            Units = Units.OrderByDescending(u => u.SearchAreaWidth).ToList();
            UnitsHash = Units.GetUnitsHashCode();
        }

        public bool ParseBoss()
        {
            if (Phase.IsNullOrEmpty() || Boss == 0)
            {
                if (!Name.IsNullOrEmpty())
                {
                    var m = System.Text.RegularExpressions.Regex.Match(Name, @"([A-F])([1-6])");
                    if (m.Success)
                    {
                        Phase = m.Groups[1].Value;
                        Boss = m.Groups[2].Value.AsInt();
                        return true;
                    }
                    else
                    {
                        Phase = "A";
                        Boss = 1;
                        return false;
                    }
                }
                return false;
            }
            return true;
        }

        public bool TeamEquals(ClanBattleTeam t2)
        {
            return Phase == t2.Phase
                && Boss == t2.Boss
                && Name == t2.Name
                && Axis == t2.Axis
                && Note == t2.Note
                && !UnitIds.Except(t2.UnitIds).Any();
        }
        private string GetTypeDesp()
        {
            if (Tail)
                return "尾";
            if (AutoH)
                return "半A";
            if (Auto)
                return "A";
            return null;
        }
    }

    public class UserTeamGroupPointComparer : IComparer<UserTeamGroup>
    {
        public int Compare(UserTeamGroup x, UserTeamGroup y)
        {
            var c = x.TotalDamage.CompareTo(y.TotalDamage);
            if (c == 0)
                c = x.Index.CompareTo(y.Index);
            return c;
        }
    }
    public class CbTeamUserLike
    {
        [Key]
        public long Id { get; set; }
        public int UserId { get; set; }
        public int TeamId { get; set; }
        [ForeignKey(nameof(TeamId))]
        public ClanBattleTeam Team { get; set; }
    }

    public class ClanBattleTeamGroup
    {
        public string Name { get; set; }
        public bool Selected { get; set; } = true;
        public bool IsVisible { get; set; } = true;
        public double Rate { get; set; }
        public int? Limit { get; set; }
        public int MinLimit { get; set; }
        public bool NoSupporter { get; set; }
        public string Return { get; set; }
        public int Skip { get; set; }
        public List<ClanBattleTeam> Teams { get; set; }
        public ClanBattleTeamGroup()
        {

        }

        public ClanBattleTeamGroup(ClanBattleTeamGroup c)
        {
            Name = c.Name;
            MinLimit = c.MinLimit;
            Limit = c.Limit;
            Selected = c.Selected;
            NoSupporter = c.NoSupporter;
            IsVisible = c.IsVisible;
            Return = c.Return;
        }

        public ClanBattleTeamGroup(IGrouping<string, ClanBattleTeam> g)
        {
            Name = g.Key;
            Teams = g.OrderByDescending(g => g.Like).ThenByDescending(g => g.Damage).ToList();
            var r = g.Max(t => t.PointRate);
            Rate = r >= 1 ? Math.Round(r, 1) : 1;
            Limit = g.Max(t => t.Limit);
        }
    }
}
