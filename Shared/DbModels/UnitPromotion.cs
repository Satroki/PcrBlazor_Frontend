using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PcrBlazor.Shared
{
    public class UnitPromotion
    {
        public const int DefaultSlot = 999999;
        public int UnitId { get; set; }
        public int PromotionLevel { get; set; }
        public int EquipSlot_1 { get; set; }
        public int EquipSlot_2 { get; set; }
        public int EquipSlot_3 { get; set; }
        public int EquipSlot_4 { get; set; }
        public int EquipSlot_5 { get; set; }
        public int EquipSlot_6 { get; set; }

        [NotMapped]
        [JsonIgnore]
        public int[] EquipSlots => new[] { EquipSlot_1, EquipSlot_2, EquipSlot_3, EquipSlot_4, EquipSlot_5, EquipSlot_6 };
        [NotMapped]
        [JsonIgnore]
        public int[] EquipSlotsVertical => new[] { EquipSlot_1, EquipSlot_3, EquipSlot_5, EquipSlot_2, EquipSlot_4, EquipSlot_6 };

        public UnitPromotion CheckLimit(int? mr, int? ms)
        {
            if (!mr.HasValue || !ms.HasValue)
                return this;
            if (mr < PromotionLevel)
                return CreateDefault();
            if (mr > PromotionLevel)
                return this;
            if (ms == 6)
                return this;
            var newP = CreateCopy();
            switch (ms)
            {
                case 5: newP.EquipSlot_1 = DefaultSlot; break;
                case 4: newP.EquipSlot_1 = newP.EquipSlot_3 = DefaultSlot; break;
                case 3: newP.EquipSlot_1 = newP.EquipSlot_3 = newP.EquipSlot_5 = DefaultSlot; break;
                case 2: newP.EquipSlot_1 = newP.EquipSlot_3 = newP.EquipSlot_5 = newP.EquipSlot_2 = DefaultSlot; break;
                case 1: newP.EquipSlot_1 = newP.EquipSlot_3 = newP.EquipSlot_5 = newP.EquipSlot_2 = newP.EquipSlot_4 = DefaultSlot; break;
            }
            return newP;
        }

        public UnitPromotion CheckLimit(int? mr, int? ms, bool ma, UserBoxLine line)
        {
            if (ma)
                return CheckLimit(mr, ms);
            else
                return CheckLimit(line.MaxRank ?? mr, line.MaxSlots ?? ms);
        }

        public static UnitPromotion CreateDefault(int unitId, int level)
        {
            return new UnitPromotion
            {
                UnitId = unitId,
                PromotionLevel = level,
                EquipSlot_1 = DefaultSlot,
                EquipSlot_2 = DefaultSlot,
                EquipSlot_3 = DefaultSlot,
                EquipSlot_4 = DefaultSlot,
                EquipSlot_5 = DefaultSlot,
                EquipSlot_6 = DefaultSlot
            };
        }

        public UnitPromotion CreateDefault()
        {
            return CreateDefault(UnitId, PromotionLevel);
        }

        public UnitPromotion CreateCopy()
        {
            return new UnitPromotion
            {
                UnitId = UnitId,
                PromotionLevel = PromotionLevel,
                EquipSlot_1 = EquipSlot_1,
                EquipSlot_2 = EquipSlot_2,
                EquipSlot_3 = EquipSlot_3,
                EquipSlot_4 = EquipSlot_4,
                EquipSlot_5 = EquipSlot_5,
                EquipSlot_6 = EquipSlot_6
            };
        }

        public bool SlotsEquals(UnitPromotion p)
        {
            var s1 = EquipSlots;
            var s2 = p.EquipSlots;
            for (int i = 0; i < 6; i++)
            {
                if (s1[i] == s2[i] || s1[i] == DefaultSlot || s2[i] == DefaultSlot)
                    continue;
                else
                    return false;
            }
            return true;
        }

        public int this[int slot]
        {
            get => slot switch
            {
                1 => EquipSlot_1,
                2 => EquipSlot_2,
                3 => EquipSlot_3,
                4 => EquipSlot_4,
                5 => EquipSlot_5,
                _ => EquipSlot_6,
            };
        }
    }
}
