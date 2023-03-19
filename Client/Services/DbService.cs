using PcrBlazor.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using TG.Blazor.IndexedDB;

namespace PcrBlazor.Client.Services
{
    public class DbService
    {
        private readonly IndexedDBManager dBManager;

        public PcrDataSet<UnitData> UnitDatas { get; }
        public PcrDataSet<EquipmentData> EquipmentDatas { get; }
        public PcrDataSet<EquipmentCraft> EquipmentCrafts { get; }
        public PcrDataSet<AreaInfo> AreaInfos { get; }
        public PcrDataSet<UnitPromotion> UnitPromotions { get; }
        public PcrDataSet<EquipmentEnhanceRate> EquipmentEnhanceRates { get; }
        public PcrDataSet<EquipmentStatus> EquipmentStatuses { get; }
        public PcrDataSet<ClanBattleInfo> ClanBattleInfos { get; }
        public PcrDataSet<ClanBattleTeam> ClanBattleTeams { get; }
        public UserDataSet<UserBoxLine, int, int> UserBoxLines { get; }
        public UserDataSet<GroupRecord, int, int> GroupRecords { get; }

        public DbService(IndexedDBManager dBManager)
        {
            this.dBManager = dBManager;

            UnitDatas = new PcrDataSet<UnitData>(dBManager, StoreNames.UnitDatas);
            EquipmentDatas = new PcrDataSet<EquipmentData>(dBManager, StoreNames.EquipmentDatas);
            EquipmentCrafts = new PcrDataSet<EquipmentCraft>(dBManager, StoreNames.EquipmentCrafts);
            AreaInfos = new PcrDataSet<AreaInfo>(dBManager, StoreNames.AreaInfos);
            UnitPromotions = new PcrDataSet<UnitPromotion>(dBManager, StoreNames.UnitPromotions);
            EquipmentEnhanceRates = new PcrDataSet<EquipmentEnhanceRate>(dBManager, StoreNames.EquipmentEnhanceRates);
            EquipmentStatuses = new PcrDataSet<EquipmentStatus>(dBManager, StoreNames.EquipmentStatuses);
            ClanBattleInfos = new PcrDataSet<ClanBattleInfo>(dBManager, StoreNames.ClanBattleInfos);
            ClanBattleTeams = new PcrDataSet<ClanBattleTeam>(dBManager, LocalStoreNames.ClanBattleTeams);

            UserBoxLines = new UserDataSet<UserBoxLine, int, int>(dBManager, LocalStoreNames.UserBoxLines);
            GroupRecords = new UserDataSet<GroupRecord, int, int>(dBManager, LocalStoreNames.GroupRecords);
        }

        public async Task<(UnitSourceData, bool)> GetAndCheckUnitSourceDataAsync(int uid, UnitServerInfo si)
        {
            if (si.LastModifyDict == null || !si.LastModifyDict.TryGetValue(StoreNames.UnitSourceDatas, out var date))
                date = si.LastModified;
            var sd = await dBManager.GetRecordById<int, UnitSourceData>(StoreNames.UnitSourceDatas, uid);
            bool needUpdate = false;
            if (sd == null || !sd.LastModified.HasValue || date > sd.LastModified)
                needUpdate = true;
            return (sd, needUpdate);
        }

        public async Task<UnitSourceData> AddOrUpdateUnitSourceData(UnitSourceData oldSd, UnitSourceData newSd)
        {
            if (oldSd != null)
                await dBManager.UpdateRecord(new StoreRecord<UnitSourceData> { Data = newSd, Storename = StoreNames.UnitSourceDatas });
            else
                await dBManager.AddRecord(new StoreRecord<UnitSourceData> { Data = newSd, Storename = StoreNames.UnitSourceDatas });
            return newSd;
        }

        public static IEnumerable<StoreSchema> GetStoreSchemas()
        {
            yield return new StoreSchema
            {
                Name = StoreNames.UnitSourceDatas,
                PrimaryKey = new IndexSpec { Name = "unitId", KeyPath = "unitId", Auto = true }
            };

            yield return new StoreSchema
            {
                Name = StoreNames.UnitDatas,
                PrimaryKey = new IndexSpec { Name = "server", KeyPath = "server", Auto = true }
            };

            yield return new StoreSchema
            {
                Name = StoreNames.EquipmentDatas,
                PrimaryKey = new IndexSpec { Name = "server", KeyPath = "server", Auto = true }
            };

            yield return new StoreSchema
            {
                Name = StoreNames.EquipmentCrafts,
                PrimaryKey = new IndexSpec { Name = "server", KeyPath = "server", Auto = true }
            };

            yield return new StoreSchema
            {
                Name = StoreNames.AreaInfos,
                PrimaryKey = new IndexSpec { Name = "server", KeyPath = "server", Auto = true }
            };

            yield return new StoreSchema
            {
                Name = StoreNames.UnitPromotions,
                PrimaryKey = new IndexSpec { Name = "server", KeyPath = "server", Auto = true }
            };

            yield return new StoreSchema
            {
                Name = StoreNames.EquipmentEnhanceRates,
                PrimaryKey = new IndexSpec { Name = "server", KeyPath = "server", Auto = true }
            };

            yield return new StoreSchema
            {
                Name = StoreNames.EquipmentStatuses,
                PrimaryKey = new IndexSpec { Name = "server", KeyPath = "server", Auto = true }
            };

            yield return new StoreSchema
            {
                Name = StoreNames.ClanBattleInfos,
                PrimaryKey = new IndexSpec { Name = "server", KeyPath = "server", Auto = true }
            };

            //本地数据
            yield return new StoreSchema
            {
                Name = LocalStoreNames.ClanBattleTeams,
                PrimaryKey = new IndexSpec { Name = "server", KeyPath = "server", Auto = true }
            };

            yield return new StoreSchema
            {
                Name = LocalStoreNames.UserBoxLines,
                PrimaryKey = new IndexSpec { Name = "id", KeyPath = "id", Auto = false, Unique = true },
                Indexes = new()
                {
                    new IndexSpec { Name = "userId", KeyPath = "userId", Auto = false, Unique = false }
                }
            };

            yield return new StoreSchema
            {
                Name = LocalStoreNames.GroupRecords,
                PrimaryKey = new IndexSpec { Name = "id", KeyPath = "id", Auto = false, Unique = true },
                Indexes = new()
                {
                    new IndexSpec { Name = "userId", KeyPath = "userId", Auto = false, Unique = false }
                }
            };
        }
    }

    public class PcrDataRecord<T>
    {
        public string Server { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public List<T> Datas { get; set; }
    }

    public class PcrDataSet<T>
    {
        private readonly IndexedDBManager dBManager;
        public string StoreName { get; }
        public PcrDataSet(IndexedDBManager dBManager, string store)
        {
            this.dBManager = dBManager;
            StoreName = store;
        }

        public async Task<PcrDataRecord<T>> GetRecordAsync(string server)
        {
            var r = await dBManager.GetRecordById<string, PcrDataRecord<T>>(StoreName, server);
            return r;
        }

        public async Task<(PcrDataRecord<T>, bool)> GetAndCheckRecordAsync(string server, UnitServerInfo si)
        {
            var r = await dBManager.GetRecordById<string, PcrDataRecord<T>>(StoreName, server);
            bool needUpdate;
            if (r == null)
                needUpdate = true;
            else if (si.LastModifyDict != null && si.LastModifyDict.TryGetValue(StoreName, out var date))
            {
                needUpdate = r.LastModified < date;
            }
            else
                needUpdate = false;
            return (r, needUpdate);
        }

        public async Task<PcrDataRecord<T>> AddOrUpdateRecord(PcrDataRecord<T> oldRecord, string server, List<T> datas, UnitServerInfo si)
        {
            DateTimeOffset date = default;
            si.LastModifyDict?.TryGetValue(StoreName, out date);
            var dr = new PcrDataRecord<T>
            {
                Datas = datas,
                Server = server,
                LastModified = date
            };
            var sr = new StoreRecord<PcrDataRecord<T>>
            {
                Data = dr,
                Storename = StoreName
            };
            if (oldRecord == null)
                await dBManager.AddRecord(sr);
            else
                await dBManager.UpdateRecord(sr);
            return dr;
        }

        public async Task<PcrDataRecord<T>> AddOrUpdateRecord(string server, List<T> datas, bool add)
        {
            var dr = new PcrDataRecord<T>
            {
                Datas = datas,
                Server = server,
                LastModified = DateTimeOffset.Now
            };
            var sr = new StoreRecord<PcrDataRecord<T>>
            {
                Data = dr,
                Storename = StoreName
            };
            if (add)
                await dBManager.AddRecord(sr);
            else
                await dBManager.UpdateRecord(sr);
            return dr;
        }
    }

    public class UserDataSet<T, TKey, TIndex> where TIndex : IEquatable<TIndex>
    {
        private readonly IndexedDBManager dBManager;
        private readonly string indexName;
        private readonly StoreIndexQuery<int> indexSearch;

        public string StoreName { get; }
        public UserDataSet(IndexedDBManager dBManager, string store, string indexName = "userId")
        {
            this.dBManager = dBManager;
            StoreName = store;
            this.indexName = indexName;

            indexSearch = new StoreIndexQuery<int>
            {
                Storename = store,
                IndexName = indexName
            };
        }

        private StoreIndexQuery<TIndex> CreateIndexQuery(TIndex v)
        {
            return new StoreIndexQuery<TIndex>
            {
                Storename = StoreName,
                IndexName = indexName,
                QueryValue = v
            };
        }

        public async Task<List<T>> GetRecordsByIndexAsync(TIndex value, Func<T, bool> subQuery = null)
        {
            var r = await dBManager.GetAllRecordsByIndex<TIndex, T>(CreateIndexQuery(value));
            if (subQuery != null)
                return r.Where(subQuery).ToList();
            else
                return r.ToList();
        }

        public async Task<T> AddOrUpdateRecordAsync(T data, TKey key)
        {
            var sr = new StoreRecord<T>
            {
                Data = data,
                Storename = StoreName
            };
            await dBManager.AddRecord(sr);
            return data;
        }

        public async Task DeleteRecordAsync(TKey key)
        {
            await dBManager.DeleteRecord(StoreName, key);
        }

        public async Task<List<T>> SyncRecordsByIndexAsync(List<T> datas, TIndex value, Func<T, TKey> keySelector, Func<T, bool> subQuery = null)
        {
            var ori = await GetRecordsByIndexAsync(value, subQuery);
            var removeKeys = ori.Select(keySelector).Except(datas.Select(keySelector)).ToArray();
            foreach (var rk in removeKeys)
            {
                await DeleteRecordAsync(rk);
            }
            foreach (var d in datas)
            {
                await AddOrUpdateRecordAsync(d, keySelector(d));
            }
            return datas;
        }
    }
}
