using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PcrBlazor.Shared
{
    public static class StoreNames
    {
        public const string UnitSourceDatas = "UnitSourceDataJP";
        public const string UnitDatas = "UnitDatas";
        public const string EquipmentDatas = "EquipmentDatas";
        public const string EquipmentCrafts = "EquipmentCrafts";
        public const string AreaInfos = "AreaInfos";
        public const string UnitPromotions = "UnitPromotions";
        public const string EquipmentEnhanceRates = "EquipmentEnhanceRates";
        public const string EquipmentStatuses = "EquipmentStatuses";
        public const string ClanBattleInfos = "ClanBattleInfos";

        public static List<string> GetAllNames()
        {
            return typeof(StoreNames).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
                .Select(x => (string)x.GetRawConstantValue())
                .ToList();
        }
    }

    public static class LocalStoreNames
    {
        public const string UserBoxLines = "UserBoxLines";
        public const string GroupRecords = "GroupRecords";
        public const string ClanBattleTeams = "ClanBattleTeams";
    }
}
