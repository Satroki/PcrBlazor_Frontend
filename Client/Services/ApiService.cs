using Blazored.LocalStorage;
using Microsoft.JSInterop;
using PcrBlazor.Shared;
using Radzen.Blazor.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TG.Blazor.IndexedDB;
using System.Net.Http.Headers;

namespace PcrBlazor.Client.Services
{
    public class ApiService
    {
        private readonly HttpClient hc;
        private readonly IJSRuntime js;
        private readonly ILocalStorageService storage;
        private readonly DbService db;
        private readonly CacheService cache;
        private readonly HttpClient phc;
        private UnitsTraceSetting traceSetting = new();
        private List<UserBoxLine> box = new List<UserBoxLine>();
        private List<GroupRecord> boxGroups = new List<GroupRecord>();
        private List<UserTeamGroup> teams;
        private string server;
        private Dictionary<string, UnitServerInfo> serverInfo = new Dictionary<string, UnitServerInfo>();
        private Dictionary<string, List<UnitData>> unitDatasDict = new Dictionary<string, List<UnitData>>();
        private Dictionary<string, List<EquipmentData>> equipDatasDict = new Dictionary<string, List<EquipmentData>>();
        private Dictionary<string, List<EquipmentCraft>> equipCraftsDict = new Dictionary<string, List<EquipmentCraft>>();
        private Dictionary<string, List<AreaInfo>> areaInfoDict = new Dictionary<string, List<AreaInfo>>();
        private Dictionary<string, List<ShioriEvent>> shioriDict = new Dictionary<string, List<ShioriEvent>>();
        private Dictionary<string, List<UnitPromotion>> unitPromotionsDict = new Dictionary<string, List<UnitPromotion>>();
        Dictionary<int, int> equipUnitDict;
        private Dictionary<string, UnitStatusCalculator> unitStatusCalculatorDict = new Dictionary<string, UnitStatusCalculator>();
        private readonly SemaphoreSlim calculatorSemaphoreSlim = new SemaphoreSlim(1, 1);

        public event Action BoxChanged;
        public FavExportSetting FavExportSetting { get; } = new FavExportSetting();
        public ClientSettings Settings { get; private set; }
        public EquipmentStatusSet EquipmentStatusSet { get; private set; } = new EquipmentStatusSet(new List<EquipmentStatus>());

        public ApiService(HttpClient hc, IHttpClientFactory hcf, IJSRuntime js, ILocalStorageService storage, DbService db, CacheService cache)
        {
            this.hc = hc;
            this.js = js;
            this.storage = storage;
            this.db = db;
            this.cache = cache;
            this.phc = hcf.CreateClient("PcrBlazor");
        }

        public void ClearCache()
        {
            box.Clear();
        }

        public async ValueTask InitSettings()
        {
            if (Settings == null)
            {
                var s = await storage.GetItemAsync<ClientSettings>("ClientSettings");
                if (s != null)
                    Settings = s;
                else
                    Settings = new ClientSettings();
                Settings.Check();
            }
        }
        public async Task SaveSettings()
        {
            await storage.SetItemAsync("ClientSettings", Settings);
        }

        public async ValueTask<UnitServerInfo> GetServerInfoAsync(string s)
        {
            if (!serverInfo.TryGetValue(s, out var info))
            {
                info = await phc.GetFromJsonAsync<UnitServerInfo>("api/Unit/GetUnitServerInfo?s=" + s);
                serverInfo.Add(s, info);
            }
            return info;
        }

        public async ValueTask<List<UserBoxLine>> GetUserBox(string s, bool refresh = false)
        {
            if (s != server || box.IsNullOrEmpty() || refresh)
            {
                server = s;
                var boxResult = await hc.GetFromJsonAsync<UserBoxResult>($"api/Box/GetUserBoxResult?s={s}&{await GetLimitQueryParamAsync(s)}");
                box = boxResult.Box;
                boxGroups = boxResult.Groups;
                traceSetting = boxResult.UnitsTraceSetting;
                var si = await GetServerInfoAsync(s);
                foreach (var b in box)
                {
                    b.SetBoxUnitState(si);
                }
            }
            return box;
        }

        public async Task<List<UserBoxLine>> GetSharedBox(string shareId, string s)
        {
            var sbox = await phc.GetFromJsonAsync<List<UserBoxLine>>($"api/Box/GetSharedBox/{shareId}?s={s}");
            var si = await GetServerInfoAsync(s);
            foreach (var b in sbox)
            {
                b.SetBoxUnitState(si);
            }
            return sbox;
        }

        public async Task<string> GetShareLink(string s)
        {
            var link = await hc.GetFromJsonAsync<string>($"api/Box/GetShareLink?s={s}");
            return link;
        }

        public async Task<int> CopyUserBox(BoxCopySetting setting)
        {
            var r = await hc.PostAsJsonAsync("api/Box/CopyUserBox", setting);
            var cnt = await r.ReadAsAsync<int>();
            if (cnt > 0)
                await GetUserBox(setting.Server, true);
            return cnt;
        }

        public async Task<string> GetBindToken()
        {
            var link = await hc.GetFromJsonAsync<string>($"api/bind-token");
            return link;
        }

        public async Task<List<UserFavorite>> GetUserFavorites()
        {
            var list = await hc.GetFromJsonAsync<List<UserFavorite>>($"api/Box/GetUserFavorites");
            return list;
        }

        public async Task<UserFavorite> AddUserFavorite(UserFavorite f)
        {
            var resp = await hc.PostAsJsonAsync($"api/Box/AddUserFavorite", f);
            if (resp.IsSuccessStatusCode)
            {
                return await resp.Content.ReadFromJsonAsync<UserFavorite>();
            }
            return null;
        }

        public async Task<bool> DeleteUserFavoriteAsync(int id)
        {
            var resp = await hc.DeleteAsync($"api/Box/DeleteUserFavorite/{id}");
            return resp.IsSuccessStatusCode;
        }

        public async Task<ClanRecruitment> AddOrUpdateClanRecruitment(ClanRecruitment r)
        {
            var resp = await hc.PostAsJsonAsync($"api/Clan/AddOrUpdateClanRecruitment", r);
            return await resp.ReadAsAsync<ClanRecruitment>();
        }

        public async Task<List<ClanRecruitment>> GetClanRecruitments(string s)
        {
            var resp = await hc.GetAsync($"api/Clan/GetClanRecruitments?s={s}");
            return await resp.ReadAsAsync<List<ClanRecruitment>>();
        }

        public async Task<bool> DeleteClanRecruitment(int id)
        {
            var resp = await hc.DeleteAsync($"api/Clan/DeleteClanRecruitment/{id}");
            return await resp.ReadAsAsync<bool>();
        }

        public async Task<List<GroupRecord>> GetGroupRecordsAsync(GroupType type, string server)
        {
            var list = await hc.GetFromJsonAsync<List<GroupRecord>>($"api/Box/GetGroupRecords?type={type}&server={server}");
            return list;
        }

        public List<GroupRecord> GetBoxGroupRecords()
        {
            return boxGroups;
        }

        public async Task<GroupRecord> AddOrUpdateGroupRecordAsync(GroupRecord record)
        {
            var resp = await hc.PostAsJsonAsync($"api/Box/AddOrUpdateGroupRecord", record);
            if (resp.IsSuccessStatusCode)
            {
                return await resp.Content.ReadFromJsonAsync<GroupRecord>();
            }
            return null;
        }

        public async Task<bool> DeleteGroupRecord(int id)
        {
            var resp = await hc.DeleteAsync($"api/Box/DeleteGroupRecord/{id}");
            return resp.IsSuccessStatusCode;
        }

        public async Task<UserBoxLine> AddUserBoxLine(UnitData u, string s)
        {
            var cbox = await GetUserBox(s);
            var line = new UserBoxLine(u, s);
            var resp = await hc.PostAsJsonAsync("api/Box/AddUserBoxLine", line);
            if (resp.IsSuccessStatusCode)
            {
                line = await resp.Content.ReadFromJsonAsync<UserBoxLine>();
                cbox.Add(line);
                u.InBox = true;
                line.SetBoxUnitState(await GetServerInfoAsync(s));
            }
            return line;
        }

        public async Task<int?> AddUserBoxLines(List<UnitData> us, string s, IEnumerable<GroupRecord> grs = null)
        {
            var cbox = await GetUserBox(s);
            var unitIds = us.Where(u => !u.InBox).Select(u => u.UnitId).ToList();
            if (unitIds.Count == 0)
                return 0;
            var resp = await hc.PostAsJsonAsync($"api/Box/AddUserBoxLines?s={s}", unitIds);
            if (resp.IsSuccessStatusCode)
            {
                if (resp.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    var lines = await resp.Content.ReadFromJsonAsync<List<UserBoxLine>>();
                    cbox.AddRange(lines);
                    foreach (var item in us)
                    {
                        item.InBox = true;
                    }
                    var si = await GetServerInfoAsync(s);
                    foreach (var line in lines)
                    {
                        line.SetBoxUnitState(si);
                    }
                    if (!grs.IsNullOrEmpty())
                    {
                        foreach (var gr in grs.Where(g => g.Id > 0))
                        {
                            gr.LineIds.AddRange(lines.Select(l => l.Id));
                            await AddOrUpdateGroupRecordAsync(gr);
                        }
                    }
                    return lines.Count;
                }
                return 0;
            }
            return null;
        }

        public async Task<UserBoxLine> EditUserBoxLine(UserBoxLine line)
        {
            var cbox = await GetUserBox(line.Server);
            var resp = await hc.PutAsJsonAsync($"api/Box/EditUserBoxLine?{await GetLimitQueryParamAsync(line.Server)}", line);
            if (resp.IsSuccessStatusCode)
            {
                var newLline = await resp.Content.ReadFromJsonAsync<UserBoxLine>();
                newLline.SetBoxUnitState(await GetServerInfoAsync(line.Server));
                cbox.Remove(cbox.Find(l => l.Id == line.Id));
                cbox.Add(newLline);
                BoxChanged?.Invoke();
                return newLline;
            }
            return null;
        }

        public async Task<List<UserBoxLine>> EditUserBoxLines(MultiBoxLine line)
        {
            var cbox = await GetUserBox(line.Server);
            var resp = await hc.PostAsJsonAsync($"api/Box/EditUserBoxLines?{await GetLimitQueryParamAsync(line.Server)}", line);
            if (resp.IsSuccessStatusCode)
            {
                var newLlines = await resp.Content.ReadFromJsonAsync<List<UserBoxLine>>();
                var si = await GetServerInfoAsync(line.Server);
                foreach (var item in newLlines)
                {
                    item.SetBoxUnitState(si);
                    cbox.Remove(cbox.Find(l => l.Id == item.Id));
                    cbox.Add(item);
                }
                BoxChanged?.Invoke();
                return newLlines;
            }
            return null;
        }

        public async Task<bool> UpdateUserBoxPiecesAsync(List<PieceStock> ps)
        {
            var resp = await hc.PostAsJsonAsync($"api/Box/UpdateUserBoxPieces", ps);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUserBoxLine(UserBoxLine line)
        {
            var cbox = await GetUserBox(line.Server);
            var resp = await hc.DeleteAsync($"api/Box/DeleteUserBoxLine/{line.Id}");
            if (resp.IsSuccessStatusCode)
            {
                cbox.Remove(cbox.Find(l => l.Id == line.Id));
                var uds = await GetUnitDatasAsync(line.Server);
                ResetInBox(uds, line.UnitId);
                BoxChanged?.Invoke();
                return true;
            }
            return false;
        }
        public async Task<bool> DeleteUserBoxLines(IEnumerable<int> ids, string s)
        {
            var cbox = await GetUserBox(s);
            var resp = await hc.PostAsJsonAsync($"api/Box/DeleteUserBoxLines", ids);
            if (resp.IsSuccessStatusCode)
            {
                var rids = await resp.Content.ReadFromJsonAsync<int[]>();
                var uds = await GetUnitDatasAsync(s);
                foreach (var id in rids)
                {
                    var line = cbox.Find(l => l.Id == id);
                    ResetInBox(uds, line.UnitId);
                    cbox.Remove(line);
                }
                BoxChanged?.Invoke();
                return true;
            }
            return false;
        }

        private static void ResetInBox(List<UnitData> uds, int unitId)
        {
            var u = uds.FirstOrDefault(v => v.UnitId == unitId);
            if (u != null)
                u.InBox = false;
        }

        public async ValueTask CheckInBoxAsync(IEnumerable<UnitData> units, string s)
        {
            var cbox = await GetUserBox(s);
            var uids = cbox.Select(b => b.UnitId).ToArray();
            foreach (var unit in units)
            {
                unit.InBox = Array.IndexOf(uids, unit.UnitId) >= 0;
            }
        }

        public async ValueTask<UnitPromotion> GetUnitPromotionAsync(UserBoxLine line, int? promotion = null)
        {
            UnitPromotion up;
            promotion ??= line.Promotion;
            if (!unitPromotionsDict.ContainsKey("jp"))
                await GetUnitPromotionsAsync();
            if (unitPromotionsDict.TryGetValue("jp", out var unitPromotions))
            {
                up = unitPromotions.FirstOrDefault(p => p.UnitId == line.UnitId && p.PromotionLevel == promotion);
            }
            else
            {
                up = await phc.GetFromJsonAsync<UnitPromotion>($"api/Unit/GetUnitPromotion/{line.UnitId}/{promotion}");
            }
            var (mr, ms, ma) = await GetLimitParamAsync(line.Server);
            return up?.CheckLimit(mr, ms, ma, line);
        }

        public async ValueTask<List<UnitPromotion>> GetUnitPromotionsAsync()
        {
            return await GetPcrDataAsync(unitPromotionsDict, "jp", db.UnitPromotions, "api/Unit/GetUnitPromotions");
        }

        public async ValueTask<Dictionary<int, int>> GetEquipUnitDictAsync()
        {
            if (equipUnitDict == null)
            {
                var ups = await GetUnitPromotionsAsync();
                equipUnitDict = new Dictionary<int, int>();
                foreach (var up in ups)
                {
                    foreach (var slot in up.EquipSlots)
                    {
                        if (equipUnitDict.ContainsKey(slot))
                            equipUnitDict[slot]++;
                        else
                            equipUnitDict.Add(slot, 1);
                    }
                }
            }
            return equipUnitDict;
        }

        public async ValueTask<List<EquipmentData>> GetEquipmentDatasAsync(string s)
        {
            return await GetPcrDataAsync(equipDatasDict, s, db.EquipmentDatas, "api/Equipment/GetEquipmentDatas");
        }

        public async ValueTask<EquipmentStatusSet> GetEquipmentStatusSetAsync()
        {
            if (EquipmentStatusSet.IsEmpty)
            {
                var list = await GetPcrDataAsync(null, "jp", db.EquipmentStatuses, "api/Equipment/GetEquipmentStatuses");
                EquipmentStatusSet = new EquipmentStatusSet(list);
            }
            return EquipmentStatusSet;
        }

        public async ValueTask<List<EquipmentCraft>> GetEquipmentCraftsAsync(string s)
        {
            await InitSettings();
            if (Settings.UseJpCraft)
                s = "jp";
            if (Settings.UseCnCraft)
                s = "cn";
            var first = !equipCraftsDict.ContainsKey(s);
            var list = await GetPcrDataAsync(equipCraftsDict, s, db.EquipmentCrafts, "api/Equipment/GetEquipmentCrafts");
            if (first && s == "cn" && Settings.UseOldCraft)
            {
                list = PcrDataHelper.GetEquipmentCrafts(await GetEquipmentDatasAsync(s), list);
            }
            return list;
        }

        public async Task<UserEquipStock> GetUserEquipStockAsync(string s)
        {
            var r = await hc.GetAsync($"api/Equipment/GetUserEquipStock?s={s}");
            return await r.ReadAsAsync<UserEquipStock>();
        }

        public async Task<bool> SaveUserEquipStockAsync(UserEquipStock stock, string s)
        {
            var resp = await hc.PostAsJsonAsync($"api/Equipment/SaveUserEquipStock?s={s}", stock);
            return resp.IsSuccessStatusCode;
        }

        public async Task<UnitData> GetUnitInfoAsync(int unitId, string s, bool withNo)
        {
            var u = await phc.GetFromJsonAsync<UnitData>($"api/Unit/GetUnitInfo/{unitId}?s={s}&withNo={withNo}");
            return u;
        }

        public async Task<ClanBattleBoss> GetClanBattleBossInfo(int eid, string s)
        {
            var resp = await phc.GetAsync($"api/Unit/GetClanBattleBossInfo/{eid}?s={s}");
            return await resp.ReadAsAsync<ClanBattleBoss>();
        }

        public async ValueTask<List<UnitData>> GetUnitDatasAsync(string s)
        {
            return await GetPcrDataAsync(unitDatasDict, s, db.UnitDatas, "api/Unit/GetUnitDatas");
        }

        public async ValueTask<List<UnitData>> GetAllUnitDatasAsync(string s)
        {
            var jp = await GetUnitDatasAsync("jp");
            if (s == "jp")
                return jp;
            var cn = await GetUnitDatasAsync("cn");
            return cn.Union(jp, new UnitIdEqualityComparer()).ToList();
        }

        private async ValueTask<List<T>> GetPcrDataAsync<T>(Dictionary<string, List<T>> cacheDict, string s, PcrDataSet<T> set, string url)
        {
            if (cacheDict == null || !cacheDict.TryGetValue(s, out var list))
            {
                if (set == null)
                {
                    list = await phc.GetFromJsonAsync<List<T>>($"{url}?s={s}");
                    cacheDict?.TryAdd(s, list);
                    return list;
                }
                else
                {
                    var si = await GetServerInfoAsync(s);
                    var (dr, update) = await set.GetAndCheckRecordAsync(s, si);
                    if (update)
                    {
                        list = await phc.GetFromJsonAsync<List<T>>($"{url}?s={s}");
                        dr = await set.AddOrUpdateRecord(dr, s, list, si);
                    }
                    cacheDict?.TryAdd(s, dr.Datas);
                    return dr.Datas;
                }
            }
            return list;
        }

        public async Task<Dictionary<int, UnitNickNames>> GetUnitNickNamesAsync()
        {
            if (!cache.TryGet(CacheService.NickNames, out Dictionary<int, UnitNickNames> u))
            {
                var unns = await phc.GetFromJsonAsync<List<UnitNickNames>>($"api/Unit/GetUnitNickNames");
                u = unns.ToDictionary(v => v.UnitId);
                var local = await storage.GetItemAsync<List<UnitNickNames>>(CacheService.LocalNickNames);
                local ??= new();
                cache.Set(CacheService.LocalNickNames, local);
                if (!local.IsNullOrEmpty())
                {
                    foreach (var ln in local)
                    {
                        if (!u.TryGetValue(ln.UnitId, out var ext))
                        {
                            u.Add(ln.UnitId, ln);
                        }
                        else
                        {
                            foreach (var name in ln.NickNames)
                            {
                                ext.AddNewNickName(name);
                            }
                        }
                    }
                }
                cache.Set(CacheService.NickNames, u);
            }
            return u;
        }

        public async Task<UnitNickNames> AddNickNames(int uid, string nicks, bool clear = false, bool isAdmin = false)
        {
            var r = await hc.GetAsync($"/api/Unit/AddNickName?uid={uid}&nick={nicks}&clear={clear}");
            var n = await r.ReadAsAsync<UnitNickNames>();
            if (!isAdmin)
            {
                var names = nicks.Split(',', ' ', '，');
                var dict = cache.Get<Dictionary<int, UnitNickNames>>(CacheService.NickNames);
                if (!dict.TryGetValue(uid, out var ext))
                {
                    dict.Add(uid, n);
                }
                n = ext;
                n.AddNewNickNames(names);

                var local = cache.Get<List<UnitNickNames>>(CacheService.LocalNickNames);
                var ln = local.FirstOrDefault(u => u.UnitId == uid);
                if (ln == null)
                {
                    ln = new UnitNickNames { UnitId = uid };
                    local.Add(ln);
                }
                ln.AddNewNickNames(names);
                await storage.SetItemAsync(CacheService.LocalNickNames, local);
            }
            return n;
        }

        public async Task<bool> AddUpdateLog(UpdateLog log)
        {
            var resp = await hc.PostAsJsonAsync($"/api/Manage/AddUpdateLog", log);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteUpdateLog(int id)
        {
            var resp = await hc.DeleteAsync($"/api/Manage/DeleteUpdateLog/{id}");
            return resp.IsSuccessStatusCode;
        }

        public async Task<List<UpdateLog>> GetUpdateLogsAsync(int cnt = 3)
        {
            try
            {
                if (!cache.TryGet(CacheService.UpdateLogs, out List<UpdateLog> logs))
                {
                    logs = await phc.GetFromJsonAsync<List<UpdateLog>>($"/api/Manage/GetUpdateLogs?limit={cnt}");
                    cache.Set(CacheService.UpdateLogs, logs);
                }
                return logs;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<EventCalendar>> GetEvents(string s, bool refresh = false)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Manage/GetEvents?s={s}");
            if (refresh)
                request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
            var resp = await phc.SendAsync(request);
            return await resp.ReadAsAsync<List<EventCalendar>>();
        }

        public async Task<EventCalendar> AddOrUpdateEvent(EventCalendar e)
        {
            var resp = await hc.PostAsJsonAsync($"/api/Manage/AddOrUpdateEvent", e);
            return await resp.Content.ReadFromJsonAsync<EventCalendar>();
        }

        public async Task<bool> DeleteEvent(int id)
        {
            var resp = await hc.DeleteAsync($"/api/Manage/DeleteEvent/{id}");
            return resp.IsSuccessStatusCode;
        }

        public async Task<List<UnitFilterResult>> FilterUserFavoriteAsync(IEnumerable<UnitFilter> filters, string s, string type)
        {
            var resp = await hc.PostAsJsonAsync($"api/Box/FilterUserFavorite?s={s}&type={type}", filters);
            return await resp.ReadAsAsync<List<UnitFilterResult>>();
        }

        public async Task<string> ExportUserFavorites()
        {
            await InitSettings();
            FavExportSetting.ExportMode = Settings.ExportType;
            FavExportSetting.ExportFields = Settings.ExportFields;
            var resp = await hc.PostAsJsonAsync("api/Box/ExportUserFavorites", FavExportSetting);
            return await resp.ReadAsAsync<string>();
        }

        public async Task ExportUserBox(IEnumerable<UserBoxLine> box)
        {
            var user = await storage.GetItemAsync<string>("UserName");
            var fav = new UserFavorite { Note = user, Box = box.ToList() };
            var data = PcrDataHelper.ExportBoxData(new[] { fav }, Settings.ExportFields);
            await SaveStringAsFile("Box导出.csv", data);
        }

        public async Task ExportUserBox(IEnumerable<UnitFilterResult> rs)
        {
            var favs = rs.Select(f => new UserFavorite { Note = f.Note, Box = f.Lines });
            var data = PcrDataHelper.ExportBoxData(favs, Settings.ExportFields);
            await SaveStringAsFile("收藏筛选导出.csv", data);
        }

        public async Task SaveFileLink(string filename, string link)
        {
            await js.InvokeVoidAsync("saveFileLink", filename, link);
        }

        public async Task SaveAsFile(string filename, string base64Data)
        {
            await js.InvokeVoidAsync("saveAsFile", filename, base64Data);
        }

        //public async Task SaveStringAsFile(string filename, string str)
        //{
        //    var bytes = new UTF8Encoding(false).GetBytes(str);
        //    var b64 = Convert.ToBase64String(bytes);
        //    await SaveAsFile(filename, b64);
        //}

        public async Task SaveStringAsFile(string filename, string content)
        {
            var mimetype = System.IO.Path.GetExtension(filename) switch
            {
                ".csv" => "text/csv",
                _ => "text/plain"
            };
            await js.InvokeVoidAsync("saveString", filename, "\ufeff" + content, mimetype);
        }

        public async Task<List<AreaInfo>> GetAreaInfos(string s)
        {
            var areas = await GetPcrDataAsync(areaInfoDict, s, db.AreaInfos, "api/Quest/GetAreaInfos");
            if (!areas.Any(a => a.Quests[0].Section == 0))
            {
                var drops = await GetAreaExtraDrops(s);
                foreach (var d in drops)
                {
                    var odds = 100d / d.DropItems.Count;
                    d.DropItems.ForEach(di => di.DoubleOdds = odds);
                    var area = areas.FirstOrDefault(a => a.AreaId == d.AreaId);
                    if (area != null && area.Quests[0].Section > 0)
                        area.Quests.Insert(0, d);
                }
            }
            return areas;
        }

        public async Task<List<QuestInfo>> GetAreaExtraDrops(string s)
        {
            var resp = await hc.GetAsync($"api/Quest/GetAreaExtraDrops?s={s}");
            return await resp.ReadAsAsync<List<QuestInfo>>();
        }

        public async Task<List<ShioriEvent>> GetShioriEvents(string s)
        {
            var areas = await GetPcrDataAsync(shioriDict, s, null, "api/Quest/GetShioriEvents");
            return areas;
        }

        public async Task<List<ClanBattleInfo>> GetClanBattleInfos(string s)
        {
            return await GetPcrDataAsync(null, s, db.ClanBattleInfos, "api/Quest/GetClanBattleInfos");
        }

        public async Task<Dictionary<string, double>> GetClanBattleScores(string s)
        {
            var cbs = await GetClanBattleInfos(s);
            var last = cbs.Last(c => c.Started);
            return PcrDataHelper.GetClanBattleScores(last);
        }

        public async Task<List<SolveResult>> GetSolveResultsAsync(SolveRequest r)
        {
            var resp = await hc.PostAsJsonAsync($"api/Quest/GetSolveResults", r);
            return await resp.ReadAsAsync<List<SolveResult>>();
        }

        public async Task ExportSolveResults(List<SolveResult> results)
        {
            if (!results.IsNullOrEmpty())
            {
                var sb = new StringBuilder();
                sb.AppendLine("关卡,次数,推荐,掉落期望");
                foreach (var r in results)
                {
                    sb.Append($"{r.QuestName},{r.Times},{r.Recommend}");
                    for (int i = 0; i < r.Equipments.Count; i++)
                    {
                        var se = r.Equipments[i];
                        sb.Append(',').Append($"{se.EquipmentName} +{r.Expected[i]}");
                    }
                    sb.AppendLine();
                }
                await SaveStringAsFile($"规划结果 {DateTime.Now:yyMMddHHmm}.csv", sb.ToString());
            }
        }

        public async ValueTask<UnitStatusCalculator> GetStatusCalculator(string s)
        {
            if (unitStatusCalculatorDict.ContainsKey(s))
                return unitStatusCalculatorDict[s];
            try
            {
                await calculatorSemaphoreSlim.WaitAsync();
                if (!unitStatusCalculatorDict.TryGetValue(s, out var c))
                {
                    var eers = await GetPcrDataAsync(null, "jp", db.EquipmentEnhanceRates, "api/Equipment/GetEquipmentEnhanceRates");
                    var eds = await GetEquipmentDatasAsync(s);
                    c = new UnitStatusCalculator(eers, eds, s);
                    unitStatusCalculatorDict.Add(s, c);
                }
                return c;
            }
            finally
            {
                calculatorSemaphoreSlim.Release();
            }
        }

        public async Task<UnitSourceData> GetUnitSourceDataAsync(int id)
        {
            var si = await GetServerInfoAsync("jp");
            var (sd, update) = await db.GetAndCheckUnitSourceDataAsync(id, si);
            if (update)
            {
                var newSd = await hc.GetFromJsonAsync<UnitSourceData>($"api/Unit/GetUnitSourceData/{id}");
                sd = await db.AddOrUpdateUnitSourceData(sd, newSd);
            }
            return sd;
        }

        private async ValueTask<string> GetLimitQueryParamAsync(string s)
        {
            var (mr, ms, ma) = await GetLimitParamAsync(s);
            return $"mr={mr}&ms={ms}&ma={ma}";
        }

        public async ValueTask<(int?, int?, bool)> GetLimitParamAsync(string s)
        {
            await InitSettings();
            var settings = Settings.GetRankSetting(s);
            if (settings.AllowAllRank)
            {
                if (settings.ApplyToAll)
                    return (settings.MaxRank, settings.Slots, true);
                else
                {
                    var ss = await GetServerInfoAsync(s);
                    return (ss.MaxRank, ss.MaxSlots, false);
                }
            }
            else
            {
                var ss = await GetServerInfoAsync(s);
                return (ss.MaxRank, ss.MaxSlots, true);
            }
        }

        #region 队伍
        public async ValueTask<List<UserTeamGroup>> GetTeamGroupsAsync(string s, bool refresh = false)
        {
            if (s != server || teams == null || refresh)
            {
                await GetUserBox(s);
                teams = await hc.GetFromJsonAsync<List<UserTeamGroup>>($"api/Teams/GetUserTeamGroups/{s}");
            }
            await SetTeamUnitAsync(s, teams);
            return teams;
        }

        public async Task<UserTeamGroup> AddUserTeamGroupAsync(UserTeamGroup g)
        {
            var resp = await hc.PostAsJsonAsync("api/Teams/CreateUserTeamGroup", g);
            if (resp.IsSuccessStatusCode)
            {
                g = await resp.Content.ReadFromJsonAsync<UserTeamGroup>();
                await SetTeamUnitAsync(g.Server, g.Yield());
                teams.Add(g);
            }
            return g;
        }

        public async Task<UserTeamGroup> EditUserTeamGroupAsync(UserTeamGroup g)
        {
            var resp = await hc.PutAsJsonAsync($"api/Teams/UpdateUserTeamGroup", g);
            if (resp.IsSuccessStatusCode)
            {
                var ng = await resp.Content.ReadFromJsonAsync<UserTeamGroup>();
                await SetTeamUnitAsync(g.Server, ng.Yield());
                var i = teams.IndexOf(g);
                teams[i] = ng;
                return ng;
            }
            return null;
        }

        public async Task<bool> DeletUserTeamGroupAsync(int gid)
        {
            var resp = await hc.DeleteAsync($"api/Teams/DeleteUserTeamGroup/{gid}");
            if (resp.IsSuccessStatusCode)
            {
                teams.Remove(teams.Find(l => l.Id == gid));
                return true;
            }
            return false;
        }

        public async Task<bool> DeletUserTeamGroupsAsync(string s, List<int> gids)
        {
            var resp = await hc.PostAsJsonAsync($"api/Teams/DeleteUserTeamGroups?s={s}", gids);
            if (resp.IsSuccessStatusCode)
            {
                teams.Clear();
                return true;
            }
            return false;
        }

        public async Task<List<ClanBattleTeam>> SearchClanBattleTeamsAsync(string s, string phase, int? boss)
        {
            var teams = await hc.GetFromJsonAsync<List<ClanBattleTeam>>($"api/Teams/SearchTeams?s={s}&phase={phase}&boss={boss}");
            await SetUnitsAsync(teams, s);
            return teams;
        }

        public async Task<List<ClanBattleTeam>> GetBattleTeamsFromDoc(string phase, int? boss)
        {
            var teams = await hc.GetFromJsonAsync<List<ClanBattleTeam>>($"api/Teams/GetBattleTeamsFromDoc?&phase={phase}&boss={boss}");
            await SetUnitsAsync(teams, "cn");
            return teams;
        }

        public async Task<int> ShareClanBattleTeam(ClanBattleTeam t)
        {
            var resp = await hc.PostAsJsonAsync("api/Teams/ShareTeam", t);
            if (resp.IsSuccessStatusCode)
            {
                t = await resp.Content.ReadFromJsonAsync<ClanBattleTeam>();
            }
            return t.Id;
        }

        public async Task<bool> DeleteClanBattleTeamAsync(int id)
        {
            var resp = await hc.DeleteAsync($"api/Teams/DeleteTeam/{id}");
            return resp.IsSuccessStatusCode;
        }

        public async Task<int> LikeClanBattleTeam(int id, bool? like)
        {
            var cnt = await hc.GetFromJsonAsync<int>($"api/Teams/Like/{id}?like={like}");
            return cnt;
        }

        public async ValueTask SetUnitsAsync(IEnumerable<ClanBattleTeam> teams, string s)
        {
            var units = await GetUnitDatasAsync(s);
            SetUnits(teams, units);
        }

        public void SetUnits(IEnumerable<ClanBattleTeam> teams, List<UnitData> units)
        {
            foreach (var t in teams)
            {
                t.Units = units.Where(u => t.UnitIds.Contains(u.UnitId)).ToList();
                t.SortUnits();
                t.AtkType = t.Units.Count(u => u.AtkType == 2) >= 3 ? 2 : 1;
            }
        }

        private async ValueTask SetTeamUnitAsync(string s, IEnumerable<UserTeamGroup> groups)
        {
            if (groups.IsNullOrEmpty())
                return;
            var box = await GetUserBox(s);
            var units = await GetAllUnitDatasAsync(s);
            foreach (var g in groups)
            {
                foreach (var t in g.Teams)
                {
                    t.Units = new List<ISearchAreaWidth>();
                    if (t.SupporterId.HasValue)
                    {
                        var sp = units.First(u => u.UnitId == t.SupporterId);
                        t.Units.Add(sp);
                    }
                    var lines = box.Where(b => t.UnitIds.Contains(b.UnitId)).ToList();
                    t.Units.AddRange(lines);
                    t.SortUnits();
                }
            }
        }
        public async Task<List<UserTeamGroup>> CalcTeamGroupingAsync(IEnumerable<ClanBattleTeamGroup> cbTeamGroup, List<int> boxIds, string s, GroupingSettings gs)
        {
            await Task.Yield();
            var used = cbTeamGroup.SelectMany(t => t.Teams).Where(t => t.Checked).ToList();
            var tms = new List<ClanBattleTeam>();
            var tmsr = new Dictionary<string, List<ClanBattleTeam>>();
            var rtDict = cbTeamGroup.Where(g => g.Selected && !g.Return.IsNullOrEmpty()).ToDictionary(g => g.Name, g => g.Return);
            foreach (var cbtg in cbTeamGroup)
            {
                var spCnt = cbtg.NoSupporter ? 0 : 1;
                cbtg.Teams.ForEach(t => t.NoSupporter = cbtg.NoSupporter);
                if (cbtg.Selected || rtDict.ContainsValue(cbtg.Name))
                {
                    var teams = cbtg.Teams.Where(t => t.Enabled && t.UnitIds.Except(boxIds).Count() <= spCnt).ToList();
                    if (cbtg.Selected)
                        tms.AddRange(teams);
                    if (rtDict.ContainsValue(cbtg.Name))
                        tmsr.Add(cbtg.Name, teams);
                }
            }

            var limits = cbTeamGroup.ToDictionary(g => g.Name, g => g.Limit ?? 3);
            var minlimits = cbTeamGroup.Where(g => g.MinLimit > 0).ToDictionary(g => g.Name, g => g.MinLimit);
            if (minlimits.Values.Sum() > gs.TeamCnt)
                return new();
            var set = new SortedSet<UserTeamGroup>(new UserTeamGroupPointComparer());
            double? minDamage = null;
            var index = 0;
            var emptyCnt = 3 - gs.TeamCnt;
            for (int t = 0; t < emptyCnt; t++)
            {
                used.Insert(0, null);
            }
            foreach (var cbts in GetTeams(tms, used))
            {
                var tts = emptyCnt > 0 ? cbts[emptyCnt..] : cbts;
                var totalDamage = tts.Sum(t => t.Damage * (gs.PointMode ? t.PointRate : 1));
                if (totalDamage <= minDamage)
                    continue;
                if (tts.Count(t => !t.Auto) > gs.MLimit)
                    continue;
                if (tts.Count(t => t.AtkType == 2) > gs.MagicLimit)
                    continue;

                if (tts.GroupBy(tt => tt.PhaseName).Any(sg => sg.Count() > limits[sg.Key]))
                    continue;

                var ml = from kv in minlimits
                         join sg in tts.GroupBy(tt => tt.PhaseName)
                         on kv.Key equals sg.Key into joined
                         from j in joined.DefaultIfEmpty()
                         select j == null || j.Count() < kv.Value;
                if (ml.Any(m => m))
                    continue;
                if (!CalcSupporters(tts, boxIds, out var teams))
                    continue;

                var utg = new UserTeamGroup
                {
                    Name = string.Join(", ", tts.Select(t => $"{t.Phase}{t.Boss}")),
                    TotalDamage = totalDamage,
                    Server = s,
                    UnitReusable = false,
                    WithSupport = true,
                    Teams = teams,
                    Index = index++,
                };
                utg.Note = utg.TotalDamage.ToString("0.00");
                if (utg.Teams.Count == gs.TeamCnt)
                {
                    if (set.Count < gs.Limit)
                    {
                        set.Add(utg);
                        if (set.Count == gs.Limit)
                            minDamage = set.Min.TotalDamage;
                    }
                    else
                    {
                        var min = set.Min;
                        set.Remove(min);
                        set.Add(utg);
                        minDamage = set.Min.TotalDamage;
                    }
                }
            };
            var list = set.Reverse().ToList();

            if (list.Any())
            {
                //补偿刀
                if (rtDict.Any())
                {
                    foreach (var tg in list)
                    {
                        var rteamsArr = new List<ClanBattleTeam>[tg.Teams.Count];
                        var leftBoxIds = boxIds.Except(tg.Teams.SelectMany(t => t.UnitIds)).ToList();
                        for (int ti = 0; ti < tg.Teams.Count; ti++)
                        {
                            var t = tg.Teams[ti];
                            var tUnits = t.UnitIds.ToArray();
                            if (rtDict.TryGetValue(t.PhaseName, out var rBoss))
                            {
                                if (tmsr.TryGetValue(rBoss, out var rTeams))
                                {
                                    var box = leftBoxIds.Concat(t.UnitIds);
                                    rteamsArr[ti] = rTeams.Where(t => t.UnitIds.Except(box).Count() <= 1).ToList();
                                }
                                else
                                    rteamsArr[ti] = new();
                            }
                        }
                        if (rteamsArr.Any(t => t != null && !t.Any()))
                            continue;
                        var rindexTeam = rteamsArr.Select((a, i) => a.IsNullOrEmpty() ? null : tg.Teams[i]).Where(i => i != null).ToArray();
                        foreach (var rts in GetTeams(rteamsArr))
                        {
                            if (!CalcSupporters(rts, leftBoxIds, out var teams, rindexTeam))
                                continue;
                            for (int rti = 0; rti < teams.Count; rti++)
                            {
                                var t = teams[rti];
                                t.Name = "补偿-" + t.Name;
                                tg.Teams.Insert(tg.Teams.IndexOf(rindexTeam[rti]) + 1, t);
                            }
                            break;
                        }
                    }
                }

                await SetTeamUnitAsync(s, list);

                //通用刀
                if (gs.WithSame)
                {
                    var gdict = tms.GroupBy(t => t.PhaseName).ToDictionary(t => t.Key, t => t.OrderByDescending(t => t.Damage).ToList());
                    foreach (var tg in list)
                    {
                        var newTeams = new List<UserTeam>();
                        foreach (var t in tg.Teams.ToArray())
                        {
                            newTeams.Add(t);
                            if (t.Name.StartsWith("补偿"))
                                continue;
                            foreach (var (k, v) in gdict)
                            {
                                if (k != t.PhaseName)
                                {
                                    var tt = v.FirstOrDefault(c => c.UnitsHash == t.UnitsHash);
                                    if (tt != null)
                                    {
                                        var st = new UserTeam
                                        {
                                            Name = $"——{tt.Name} {tt.Damage}",
                                            Note = tt.Note,
                                            Axis = tt.Axis,
                                            UnitIds = t.UnitIds.ToList(),
                                            SupporterId = t.SupporterId,
                                            Links = tt.Links,
                                            PhaseName = tt.PhaseName,
                                            Units = t.Units,
                                            CbTeamId = tt.Id
                                        };
                                        newTeams.Add(st);
                                    }
                                }
                            }
                        }
                        tg.Teams = newTeams;
                    }
                }
            }
            return list;
        }

        private bool CalcSupporters(ClanBattleTeam[] cbts, List<int> boxIds, out List<UserTeam> teams, UserTeam[] rtnTeams = null)
        {
            teams = null;
            var boxBak = boxIds.ToList();
            var sps = new int?[3];
            var leftTs = new List<ClanBattleTeam>();
            var cbts2 = cbts.ToArray();
            var cbtsBak = cbts;
            if (rtnTeams != null)
            {
                for (int ri = 0; ri < cbts.Length; ri++)
                {
                    var cbt = cbts[ri];
                    var cp = cbt.JsonCopy();
                    cp.UnitIds = cbt.UnitIds.Except(rtnTeams[ri].UnitIds).ToList();
                    cbts2[ri] = cp;
                    leftTs.Add(cp);
                }
            }
            else
            {
                for (int i = 0; i < cbts.Length; i++)
                {
                    var t = cbts[i];
                    if (t.NoSupporter)
                    {
                        if (!ExceptBox(boxBak, t.UnitIds))
                            return false;
                    }
                    else if (t.Checked && t.Supporter > 0)
                    {
                        boxBak.Add(t.Supporter);
                        sps[i] = t.Supporter;
                        if (!ExceptBox(boxBak, t.UnitIds))
                            return false;
                    }
                    else
                    {
                        leftTs.Add(t);
                    }
                }
            }
            if (leftTs.Any())
            {
                var uids = leftTs.SelectMany(t => t.UnitIds).ToList();
                ExceptBox(uids, boxBak);
                if (uids.Count > leftTs.Count)
                    return false;

                var dict = new Dictionary<int, List<int>>();
                for (int i = 0; i < cbts2.Length; i++)
                {
                    var t = cbts2[i];
                    if (!leftTs.Contains(t))
                        continue;

                    var inter = t.UnitIds.Intersect(uids).ToList();
                    if (inter.Any())
                        dict.Add(i, inter);
                }
                if (uids.Count > dict.Count)
                    return false;

                while (dict.Any())
                {
                    var (i, ids) = dict.OrderBy(d => d.Value.Count).First();
                    dict.Remove(i);

                    if (ids.IsNullOrEmpty())
                        continue;

                    var sp = ids[0];
                    sps[i] = sp;
                    uids.Remove(sp);

                    if (!uids.Contains(sp))
                    {
                        foreach (var list in dict.Values)
                        {
                            list.Remove(sp);
                        }
                    }
                }
            }
            teams = cbtsBak.Select((tt, i) =>
            {
                var t = new UserTeam
                {
                    Name = $"{tt.Name} {tt.Damage}",
                    Note = tt.Note,
                    Axis = tt.Axis,
                    UnitIds = tt.UnitIds.ToList(),
                    Links = tt.Links,
                    PhaseName = tt.PhaseName,
                    CbTeamId = tt.Id
                };
                if (sps[i].HasValue)
                {
                    t.SupporterId = sps[i].Value;
                    t.UnitIds.Remove(sps[i].Value);
                }
                return t;
            }).ToList();
            return true;
        }

        private IEnumerable<ClanBattleTeam[]> GetTeams(List<ClanBattleTeam>[] all)
        {
            var arr = all.Where(a => !a.IsNullOrEmpty()).ToArray();
            switch (arr.Length)
            {
                case 1:
                    return arr[0].Select(a => new[] { a });
                case 2:
                    return from t0 in arr[0]
                           from t1 in arr[1]
                           select new[] { t0, t1 };
                case 3:
                    return from t0 in arr[0]
                           from t1 in arr[1]
                           from t2 in arr[2]
                           select new[] { t0, t1, t2 };
                default:
                    return Enumerable.Empty<ClanBattleTeam[]>();
            }
        }

        private IEnumerable<ClanBattleTeam[]> GetTeams(List<ClanBattleTeam> all, List<ClanBattleTeam> used)
        {
            switch (used.Count)
            {
                case 0:
                    {
                        for (int i = 0; i < all.Count; i++)
                            for (int j = i + 1; j < all.Count; j++)
                                for (int k = j + 1; k < all.Count; k++)
                                    yield return new[] { all[i], all[j], all[k] };
                    }
                    break;
                case 1:
                    {
                        var ex = all.Except(used).ToList();
                        for (int i = 0; i < ex.Count; i++)
                            for (int j = i + 1; j < ex.Count; j++)
                                yield return new[] { used[0], ex[i], ex[j] };
                    }
                    break;
                case 2:
                    {
                        var ex = all.Except(used).ToList();
                        for (int i = 0; i < ex.Count; i++)
                            yield return new[] { used[0], used[1], ex[i] };
                    }
                    break;
                default:
                    yield break;
            }
        }

        private bool ExceptBox(List<int> all, List<int> ids)
        {
            var r = true;
            foreach (var id in ids)
            {
                r = all.Remove(id) && r;
            }
            return r;
        }

        public async Task SaveLocalTeamGroups(List<ClanBattleTeamGroup> teamGroups, string s)
        {
            var key = $"ClanBattleTeamGroup_{s}";
            if (teamGroups.IsNullOrEmpty())
                await storage.RemoveItemAsync(key);
            else
            {
                var cache = teamGroups.Select(c => new ClanBattleTeamGroup(c)).ToList();
                await storage.SetItemAsync(key, cache);
            }
        }

        public async Task<List<ClanBattleTeamGroup>> GetLocalTeamGroups(string s)
        {
            return await storage.GetItemAsync<List<ClanBattleTeamGroup>>($"ClanBattleTeamGroup_{s}");
        }

        public async Task SaveGroupingSettings(GroupingSettings gs, string s)
        {
            await storage.SetItemAsync($"GroupingSettings_{s}", gs);
        }

        public async Task<GroupingSettings> GetGroupingSettings(string s)
        {
            return await storage.GetItemAsync<GroupingSettings>($"GroupingSettings_{s}");
        }

        public UnitsTraceSetting GetUnitsTraceSetting()
        {
            return traceSetting;
        }

        public async Task<UnitsTraceSetting> SaveUnitsTraceSetting(UnitsTraceSetting setting)
        {
            var resp = await hc.PostAsJsonAsync("api/Box/SaveUnitsTraceSetting", setting);
            traceSetting = await resp.ReadAsAsync<UnitsTraceSetting>();
            return setting;
        }
        #endregion

        public Func<PcrSolveHelper.JsSolveModel, Task<JsonElement>> GetSolveTask() => async m => await js.InvokeAsync<JsonElement>("getSolveResult", m);

        public async Task<JdUnionAd> GetAd()
        {
            try
            {
                var r = await phc.GetAsync("api/JdUnion");
                return await r.ReadAsAsync<JdUnionAd>();
            }
            catch
            {
                return null;
            }
        }
    }
}
