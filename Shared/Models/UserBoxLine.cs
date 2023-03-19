using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace PcrBlazor.Shared
{
    public class UserBoxLine : ISearchAreaWidth, IUserId
    {
        public const int EmptyEquipId = 999999;
        private bool trace;
        private int level;
        private int rarity;
        private int promotion;
        private int loveLevel;
        private int uniqueEquipRank;
        private int targetRarity;
        private int targetPromotion;
        private int targetLoveLevel;
        private int targetUniqueEquipRank;
        private string note;
        private bool slot1;
        private bool slot2;
        private bool slot3;
        private bool slot4;
        private bool slot5;
        private bool slot6;
        //private int? targetSolts;
        //private bool enableTargetSolts;

        private int pieces;
        private int? maxRank;
        private int? maxSlots;
        private int? priority;
        private bool enableTargetSolts;
        private int? targetSolts;
        private bool? levelBreak;

        public event Action<string> PropertyChanged;
        private void SetProperty<T>(ref T filed, T value, [CallerMemberName] string prop = null)
        {
            filed = value;
            PropertyChanged?.Invoke(prop);
        }

        public int Id { get; set; }
        public int? SubBoxId { get; set; }
        public int UserId { get; set; }
        public string Server { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public bool Trace { get => trace; set => SetProperty(ref trace, value); }
        public int Level { get => level; set => SetProperty(ref level, value); }
        public int Rarity { get => rarity; set => SetProperty(ref rarity, value); }
        public int Promotion { get => promotion; set => SetProperty(ref promotion, value); }
        public int LoveLevel { get => loveLevel; set => SetProperty(ref loveLevel, value); }
        public int Pieces { get => pieces; set => SetProperty(ref pieces, value); }

        public bool Slot1 { get => slot1; set => SetProperty(ref slot1, value); }
        public bool Slot2 { get => slot2; set => SetProperty(ref slot2, value); }
        public bool Slot3 { get => slot3; set => SetProperty(ref slot3, value); }
        public bool Slot4 { get => slot4; set => SetProperty(ref slot4, value); }
        public bool Slot5 { get => slot5; set => SetProperty(ref slot5, value); }
        public bool Slot6 { get => slot6; set => SetProperty(ref slot6, value); }
        /// <summary>
        /// 1-6
        /// </summary>
        [Display(Name = "目标装备", Order = 6)]
        public int? TargetSolts { get => targetSolts; set => SetProperty(ref targetSolts, value); }
        [Display(Name = "目标装备自定义", Order = 5)]
        public bool EnableTargetSolts { get => enableTargetSolts; set => SetProperty(ref enableTargetSolts, value); }

        [NotMapped]
        [JsonIgnore]
        public bool[] Slots => new[] { Slot1, Slot2, Slot3, Slot4, Slot5, Slot6 };

        public int UniqueEquipRank { get => uniqueEquipRank; set => SetProperty(ref uniqueEquipRank, value); }

        public string Icon { get; set; }

        [Display(Name = "目标星级", Order = 2)]
        public int TargetRarity { get => targetRarity; set => SetProperty(ref targetRarity, value); }
        [Display(Name = "目标Rank", Order = 1)]
        public int TargetPromotion { get => targetPromotion; set => SetProperty(ref targetPromotion, value); }
        [Display(Name = "目标好感", Order = 4)]
        public int TargetLoveLevel { get => targetLoveLevel; set => SetProperty(ref targetLoveLevel, value); }
        [Display(Name = "目标专武", Order = 3)]
        public int TargetUniqueEquipRank { get => targetUniqueEquipRank; set => SetProperty(ref targetUniqueEquipRank, value); }
        public string Note { get => note; set => SetProperty(ref note, value); }
        public int? MaxRank { get => maxRank; set => SetProperty(ref maxRank, value); }
        public int? MaxSlots { get => maxSlots; set => SetProperty(ref maxSlots, value); }
        public int? Priority { get => priority; set => SetProperty(ref priority, value); }
        public bool? LevelBreak { get => levelBreak; set => SetProperty(ref levelBreak, value); }

        [NotMapped]
        public UnitPromotion UnitPromotion { get; set; }
        [NotMapped]
        [JsonIgnore]
        public UnitPromotion TargetUnitPromotion { get; set; }
        [NotMapped]
        public bool IsFinished { get; set; }
        [NotMapped]
        [JsonIgnore]
        public bool Selected { get; set; }
        [NotMapped]
        public UnitPosition Position { get; set; }
        [NotMapped]
        public int SearchAreaWidth { get; set; }
        [NotMapped]
        public bool HasUniqueEquip { get; set; }
        [NotMapped]
        public bool HasRarity6 { get; set; }
        [NotMapped]
        [JsonIgnore]
        public Dictionary<string, double> Status { get; set; }

        public UserBoxLine()
        {

        }

        public UserBoxLine(UnitData unit, string server)
        {
            Server = server;
            UnitId = unit.UnitId;
            UnitName = unit.UnitName;
            Icon = unit.Icon;
            Level = 1;
            Rarity = TargetRarity = unit.Rarity;
            Promotion = TargetPromotion = 1;
            LoveLevel = TargetLoveLevel = 1;
            Trace = true;
        }

        public bool CheckFinished(bool withRarity)
        {
            return LoveLevel >= TargetLoveLevel
                && Promotion >= TargetPromotion
                && UniqueEquipRank >= TargetUniqueEquipRank
                && (!withRarity || Rarity >= TargetRarity)
                && (levelBreak != false)
                && CheckSlot();
        }

        public bool CheckSlotFinished()
        {
            return Promotion >= TargetPromotion
                && CheckSlot();
        }

        public void SetFinished()
        {
            IsFinished = CheckFinished(true);
        }

        private bool CheckSlot()
        {
            if (UnitPromotion == null)
                return true;
            var ps = UnitPromotion.EquipSlots;
            var s = Slots;
            if (EnableTargetSolts)
                return Enumerable.Range(0, 6).All(i => s[i] || ps[i] == EmptyEquipId || !TargetSolts.IsBitSet(i + 1));
            else
                return Enumerable.Range(0, 6).All(i => s[i] || ps[i] == EmptyEquipId);
        }

        public void CheckAllSlots(bool withTarget = false)
        {
            if (UnitPromotion == null)
                return;
            var up = UnitPromotion.EquipSlots;
            Slot1 = up[0] != EmptyEquipId;
            Slot2 = up[1] != EmptyEquipId;
            Slot3 = up[2] != EmptyEquipId;
            Slot4 = up[3] != EmptyEquipId;
            Slot5 = up[4] != EmptyEquipId;
            Slot6 = up[5] != EmptyEquipId;
            if (withTarget && EnableTargetSolts)
            {
                Slot1 = Slot1 && TargetSolts.IsBitSet(1);
                Slot2 = Slot2 && TargetSolts.IsBitSet(2);
                Slot3 = Slot3 && TargetSolts.IsBitSet(3);
                Slot4 = Slot4 && TargetSolts.IsBitSet(4);
                Slot5 = Slot5 && TargetSolts.IsBitSet(5);
                Slot6 = Slot6 && TargetSolts.IsBitSet(6);
            }
        }

        public void CheckAllTargetSlots()
        {
            if (TargetUnitPromotion == null)
                return;
            var up = TargetUnitPromotion.EquipSlots;
            for (int i = 1; i <= 6; i++)
            {
                TargetSolts = TargetSolts.SetBit(i, up[i - 1] != EmptyEquipId);
            }
        }

        public void SetAllSlots(bool s)
        {
            Slot1 = s;
            Slot2 = s;
            Slot3 = s;
            Slot4 = s;
            Slot5 = s;
            Slot6 = s;
        }

        public void CheckEmptySlots()
        {
            if (UnitPromotion == null)
            {
                Slot1 = Slot2 = Slot3 = Slot4 = Slot5 = Slot6 = false;
            }
            else
            {
                var up = UnitPromotion.EquipSlots;
                Slot1 = Slot1 && (up[0] != EmptyEquipId);
                Slot2 = Slot2 && (up[1] != EmptyEquipId);
                Slot3 = Slot3 && (up[2] != EmptyEquipId);
                Slot4 = Slot4 && (up[3] != EmptyEquipId);
                Slot5 = Slot5 && (up[4] != EmptyEquipId);
                Slot6 = Slot6 && (up[5] != EmptyEquipId);
            }
        }

        public void SetBoxUnitState(UnitServerInfo si)
        {
            SetFinished();
            HasUniqueEquip = Array.IndexOf(si.UniqueEquipUnits, UnitId) >= 0;
            HasRarity6 = Array.IndexOf(si.Rarity6Units, UnitId) >= 0;
        }

        public void CheckTarget()
        {
            if (TargetPromotion < Promotion)
                TargetPromotion = Promotion;
            if (TargetLoveLevel < LoveLevel)
                TargetLoveLevel = LoveLevel;
            if (TargetRarity < Rarity)
                TargetRarity = Rarity;
            if (TargetUniqueEquipRank < UniqueEquipRank)
                TargetUniqueEquipRank = UniqueEquipRank;
        }
    }

    public class MultiBoxLine : UserBoxLine
    {
        private List<PropertyInfo> slotProps;

        [JsonIgnore]
        public List<UserBoxLine> Lines { get; set; }
        public List<int> LineIds { get; set; }
        public List<string> Fields { get; set; } = new List<string>();
        public bool FinishMode { get; set; }
        [JsonIgnore]
        public List<int> LineUnitIds { get; set; }

        [JsonIgnore]
        public bool WithUnique { get; set; }
        [JsonIgnore]
        public UnitPromotion DefultPromotion { get; set; } = new UnitPromotion
        {
            EquipSlot_1 = 101011,
            EquipSlot_2 = 101011,
            EquipSlot_3 = 101011,
            EquipSlot_4 = 101011,
            EquipSlot_5 = 101011,
            EquipSlot_6 = 101011,
        };

        public MultiBoxLine()
        {
            slotProps = typeof(MultiBoxLine).GetProperties().Where(p => Regex.IsMatch(p.Name, @"Slot\d")).ToList();
        }

        public MultiBoxLine(IEnumerable<UserBoxLine> lines, string s) : this()
        {
            Lines = lines.ToList();
            LineIds = Lines.Select(l => l.Id).ToList();
            LineUnitIds = Lines.Select(l => l.UnitId).ToList();
            UnitName = $"{Lines.Count}名角色（仅保存有改动的属性）";
            Server = s;
            Icon = "000001";
            Level = Lines.Min(l => l.Level);
            Rarity = Lines.Min(l => l.Rarity);
            TargetRarity = Lines.Min(l => l.TargetRarity);
            Promotion = Lines.Min(l => l.Promotion);
            TargetPromotion = Lines.Min(l => l.TargetPromotion);
            LoveLevel = Lines.Min(l => l.LoveLevel);
            TargetLoveLevel = Lines.Min(l => l.TargetLoveLevel);
            UniqueEquipRank = Lines.Min(l => l.UniqueEquipRank);
            TargetUniqueEquipRank = Lines.Min(l => l.TargetUniqueEquipRank);
            Trace = Lines.Any(l => l.Trace);
            Priority = Lines.Min(lines => lines.Priority);
            UnitPromotion = DefultPromotion;
            TargetUnitPromotion = DefultPromotion;
            EnableTargetSolts = lines.All(e => e.EnableTargetSolts);
            var lb = lines.Select(l => l.LevelBreak).Distinct().ToArray();
            LevelBreak = lb.Length == 1 ? lb[0] : null;
            foreach (var sp in slotProps)
            {
                sp.SetValue(this, Lines.All(l => (bool)sp.GetValue(l)));
            }

            MaxRank = lines.Min(m => m.MaxRank);
            MaxSlots = lines.Min(m => m.MaxSlots);
        }

        public void Update(UserBoxLine line)
        {
            var props = typeof(UserBoxLine).GetProperties().ToDictionary(p => p.Name);
            foreach (var f in Fields.Distinct())
            {
                var p = props[f];
                p.SetValue(line, p.GetValue(this));
            }
        }
    }

    public class BoxEditModel
    {
        public UserBoxLine Line { get; set; }
        public List<UserBoxLine> Lines { get; set; }
        public bool NoConsume { get; set; }
    }
}
