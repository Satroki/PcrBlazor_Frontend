using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PcrBlazor.Shared
{
    public static class Ext
    {
        public static IEnumerable<T> Yield<T>(this T obj)
        {
            yield return obj;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }
        public static int AsInt(this string s)
        {
            int.TryParse(s, out var i);
            return i;
        }
        public static double AsDouble(this string s)
        {
            double.TryParse(s, out var i);
            return i;
        }

        public static bool TryGetValueEx<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, out TValue value)
        {
            value = default;
            if (key is null)
                return false;
            return dict.TryGetValue(key, out value);
        }

        public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default)
        {
            if (key is null)
                return defaultValue;
            if (dict.ContainsKey(key))
                return dict[key];
            return defaultValue;
        }

        public static TValue GetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default)
        {
            if (key is null)
                return defaultValue;
            if (dict.ContainsKey(key))
                return dict[key];
            return defaultValue;
        }

        public static string AppendNewLine(this string src, string value)
        {
            if (src.IsNullOrEmpty())
                return value;
            else
                return src + "\n" + value;
        }

        public static bool AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value) where TValue : IEquatable<TValue>
        {
            if (dict.ContainsKey(key))
            {
                if (!dict[key].Equals(value))
                {
                    dict[key] = value;
                    return true;
                }
            }
            else
            {
                dict.Add(key, value);
                return true;
            }
            return false;
        }

        public static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key))
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
        }

        public static void AddList<TKey, TValue>(this Dictionary<TKey, List<TValue>> dict, TKey key, TValue value)
        {
            if (dict.TryGetValue(key, out var list))
            {
                if (list == null)
                    dict[key] = new() { value };
                else
                    list.Add(value);
            }
            else
            {
                dict.Add(key, new() { value });
            }
        }

        public static string GetDescription(this Enum e)
        {
            var memInfo = e.GetType().GetMember(e.ToString());
            var attr = memInfo[0].GetCustomAttribute<DescriptionAttribute>(false);
            return attr?.Description;
        }
        public static FDescriptionAttribute GetFDescriptionAttr(this Enum e)
        {
            var memInfo = e.GetType().GetMember(e.ToString());
            var attr = memInfo[0].GetCustomAttribute<FDescriptionAttribute>(false);
            return attr;
        }

        public static T ParseEnum<T>(this int v, T defaultValue) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), v))
                return (T)(object)v;
            return defaultValue;
        }

        public static string Join(this IEnumerable<string> values, string separator)
        {
            return string.Join(separator, values);
        }

        public static string Join(this string value, string value2, string separator)
        {
            return (value, value2) switch
            {
                (null, null) => null,
                (null, _) => value2,
                (_, null) => value,
                (_, _) => $"{value}{separator}{value2}"
            };
        }

        public static T JsonCopy<T>(this T obj)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(obj);
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }

        public static DateTimeOffset Truncate(this DateTimeOffset dt)
        {
            return dt.AddTicks(-(dt.Ticks % TimeSpan.TicksPerSecond));
        }

        public static bool IsBitSet(this int? b, int index)
        {
            return b.HasValue && (b & (1 << index)) > 0;
        }

        public static int SetBit(this int? b, int index, bool v)
        {
            var r = b ?? 0;
            if (v)
                return r | (1 << index);
            else
                return r & ~(1 << index);
        }
    }

    public static class PcrExt
    {
        public static int GetEquipmentEnhanceLevel(this int promotionLevel)
        {
            return promotionLevel switch
            {
                < 2 => 0,
                2 => 1,
                3 => 3,
                > 3 => 5,
            };
        }

        public static int GetEquipmentPromotionLevel(this int equipmentId)
        {
            return equipmentId / 1000 % 10;
        }

        public static Dictionary<int, int> GetStockData(this IEnumerable<string> lines, out Dictionary<int, int> exDict)
        {
            exDict = null;
            if (!lines.IsNullOrEmpty())
            {
                var dict = new Dictionary<int, int>();
                exDict = new();
                foreach (var s in lines)
                {
                    var vs = s.Trim().Split(' ');
                    if (vs.Length >= 2 && int.TryParse(vs[0], out var id))
                    {
                        if (int.TryParse(vs[1], out var num) && num > 0)
                            dict.AddOrUpdate(id, num);
                        if (vs.Length > 2 && int.TryParse(vs[2], out var exn) && exn > 0)
                            exDict.AddOrUpdate(id, exn);
                    };
                }
                return dict;
            }
            return null;
        }

        public static Dictionary<int, int> GetStockData(this string lines, out Dictionary<int, int> exDict)
        {
            if (!lines.IsNullOrEmpty())
            {
                var ss = lines.Split("\n");
                return GetStockData(ss, out exDict);
            }
            exDict = null;
            return null;
        }

        public static string ExportFieldName(this string field)
        {
            return field switch
            {
                nameof(UserBoxLine.Level) => "等级",
                nameof(UserBoxLine.Rarity) => "星级",
                nameof(UserBoxLine.Promotion) => "Rank",
                nameof(UserBoxLine.UniqueEquipRank) => "专武",
                nameof(UserBoxLine.LoveLevel) => "好感度",
                _ => string.Empty
            };
        }

        public static UnitPosition GetUnitPosition(this int searchAreaWidth)
        {
            if (searchAreaWidth < 300)
                return UnitPosition.Front;
            if (searchAreaWidth >= 600)
                return UnitPosition.Behind;
            return UnitPosition.Middle;
        }

        public static bool StatusEquals(this IUnitStatus s, IUnitStatus t)
        {
            return s.Accuracy == t.Accuracy
                && s.Atk == t.Atk
                && s.Def == t.Def
                && s.Dodge == t.Dodge
                && s.EnergyRecoveryRate == t.EnergyRecoveryRate
                && s.EnergyReduceRate == t.EnergyReduceRate
                && s.Hp == t.Hp
                && s.HpRecoveryRate == t.HpRecoveryRate
                && s.LifeSteal == t.LifeSteal
                && s.MagicCritical == t.MagicCritical
                && s.MagicDef == t.MagicDef
                && s.MagicPenetrate == t.MagicPenetrate
                && s.MagicStr == t.MagicStr
                && s.PhysicalCritical == t.PhysicalCritical
                && s.PhysicalPenetrate == t.PhysicalPenetrate
                && s.WaveEnergyRecovery == t.WaveEnergyRecovery
                && s.WaveHpRecovery == t.WaveHpRecovery;
        }

        public static int GetUnitsHashCode(this IEnumerable<ISearchAreaWidth> units)
        {
            var hash = new HashCode();
            foreach (var item in units)
            {
                hash.Add(item.UnitId);
            }
            return hash.ToHashCode();
        }
    }
}
