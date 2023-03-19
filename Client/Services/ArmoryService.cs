using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using PcrBlazor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcrBlazor.Client.Services
{
    public class ArmoryService
    {
        private readonly ApiService api;
        private readonly ILocalStorageService localStorage;
        private readonly ApiAuthenticationStateProvider authenticationState;
        private List<UserBoxLine> trackedLines;
        private Dictionary<int, int> StockDictCn { get; set; }
        private Dictionary<int, int> StockDictJp { get; set; }
        private Dictionary<int, int> ExStockDictCn { get; set; }
        private Dictionary<int, int> ExStockDictJp { get; set; }
        private Dictionary<string, Dictionary<int, int>> DemandDict = new Dictionary<string, Dictionary<int, int>>();
        private PcrSolveHelper solveHelper;

        public ArmoryService(ApiService api, ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationState)
        {
            this.api = api;
            this.localStorage = localStorage;
            this.authenticationState = (ApiAuthenticationStateProvider)authenticationState;
            this.authenticationState.AuthenticationStateChanged += AuthenticationState_AuthenticationStateChanged;
        }

        private async void AuthenticationState_AuthenticationStateChanged(Task<AuthenticationState> task)
        {
            var s = await task;
            if (!s.User.Identity.IsAuthenticated)
                StockDictCn = StockDictJp = null;
        }

        public async Task<(List<SimpleEquipmentData> equips, List<SimpleEquipmentData> consume, UnitEquipmentConsumeDict dict)>
            CalcBoxGap(IEnumerable<UserBoxLine> box, string s)
        {
            await InitUserStockDict(s);
            var datas = await api.GetEquipmentDatasAsync(s);
            var crafts = await api.GetEquipmentCraftsAsync(s);
            var ups = await api.GetUnitPromotionsAsync();
            var (mr, ms, ma) = await api.GetLimitParamAsync(s);
            var equips = PcrDataHelper.CalcBoxEquipmentsGap(box, datas, ups, mr, ms, ma, out var unitGaps);
            var consume = PcrDataHelper.CalcEquipmentGapConsume(equips, datas, crafts);
            foreach (var unitGap in unitGaps)
            {
                unitGap.Consume = PcrDataHelper.CalcEquipmentGapConsume(unitGap.Gap, datas, crafts);
            }
            var ueDict = new UnitEquipmentConsumeDict(unitGaps);
            var dict = GetStockDict(s);
            foreach (var item in consume)
            {
                dict.TryGetValue(item.EquipmentId, out var stock);
                item.StockNum = stock;
                if (item.CraftFlg)
                    item.ConsumedEquips = datas.First(d => d.EquipmentId == item.EquipmentId).ConsumedEquips;
            }
            foreach (var item in consume)
            {
                ApplyEquipStockToConsume(item, consume, 0, item.StockNum);
            }
            return (equips, consume, ueDict);
        }

        public async Task CheckExConsume(List<SimpleEquipmentData> consume, Dictionary<int, SimpleEquipmentData> consumeDict, string s)
        {
            if (api.Settings.EnableExtraStock)
            {
                var exDict = await InitUserExStockDict(s);
                var dict = await InitUserStockDict(s);
                if (!exDict.IsNullOrEmpty())
                {
                    var datas = await api.GetEquipmentDatasAsync(s);
                    foreach (var (eid, num) in exDict)
                    {
                        if (consumeDict.ContainsKey(eid))
                            consumeDict[eid].ConsumeNum += num;
                        else
                        {
                            var eq = datas.FirstOrDefault(e => e.EquipmentId == eid);
                            if (eq != null)
                            {
                                dict.TryGetValue(eid, out var stock);
                                var se = new SimpleEquipmentData(eq, num)
                                {
                                    StockNum = stock,
                                    ConsumeByEquips = null,
                                    ConsumedEquips = null
                                };
                                consume.Add(se);
                                consumeDict.Add(eid, se);
                            }
                        }
                    }
                }
            }
        }

        public void ApplyEquipStockToConsume(SimpleEquipmentData equip, List<SimpleEquipmentData> consume, int oldStock, int newStock)
        {
            if (equip.CraftFlg)
            {
                var stockChange = newStock - oldStock;
                if (stockChange > 0)
                    stockChange = Math.Max(0, Math.Min(stockChange, equip.ConsumeNum - oldStock));
                else if (stockChange < 0 && oldStock > equip.ConsumeNum)
                    stockChange = Math.Min(0, oldStock - equip.ConsumeNum + stockChange);

                if (stockChange != 0)
                {
                    foreach (var child in equip.ConsumedEquips)
                    {
                        var c = consume.FirstOrDefault(e => e.EquipmentId == child.EquipmentId);
                        if (c != null)
                        {
                            var ns = child.ConsumeNum * stockChange;
                            if (c.CraftFlg)
                            {
                                if (ns > 0)
                                    ApplyEquipStockToConsume(c, consume, 0, ns);
                                else
                                    ApplyEquipStockToConsume(c, consume, c.StockNum, c.StockNum + ns);
                            }
                            c.ConsumeNum = Math.Max(0, c.ConsumeNum - child.ConsumeNum * stockChange);
                        }
                    }
                }
            }
        }

        public async ValueTask<List<SimpleEquipmentData>> GetAllEquipments(string s)
        {
            var dict = await InitUserStockDict(s);
            var exDict = await InitUserExStockDict(s);
            var datas = await api.GetEquipmentDatasAsync(s);
            var list = new List<SimpleEquipmentData>();
            foreach (var d in datas)
            {
                var sp = new SimpleEquipmentData(d);
                dict.TryGetValue(sp.EquipmentId, out var stock);
                sp.StockNum = stock;
                if (exDict.TryGetValue(sp.EquipmentId, out var exs))
                {
                    sp.ExtraStock = exs;
                }
                list.Add(sp);
            }
            return list;
        }

        public Dictionary<int, int> GetStockDict(string s)
        {
            return s == "cn" ? StockDictCn : StockDictJp;
        }

        public Dictionary<int, int> GetExStockDict(string s)
        {
            return s == "cn" ? ExStockDictCn : ExStockDictJp;
        }

        public void UpdateSimpleEquipmentsNum(IEnumerable<SimpleEquipmentData> datas, string s)
        {
            var dict = GetStockDict(s);
            var exDict = GetExStockDict(s);
            foreach (var sp in datas)
            {
                dict.TryGetValue(sp.EquipmentId, out var stock);
                sp.StockNum = stock;
                if (exDict != null && exDict.TryGetValue(sp.EquipmentId, out var exs))
                    sp.ExtraStock = exs;
            }
        }

        public async Task UpdateStockNum(SimpleEquipmentData data, string s)
        {
            var dict = GetStockDict(s);
            if (dict.AddOrUpdate(data.EquipmentId, data.StockNum))
                await SaveStockToLocal(s);
        }

        public async ValueTask UpdateStockNums(IEnumerable<SimpleEquipmentData> datas, string s)
        {
            var dict = GetStockDict(s);
            var flag = false;
            foreach (var data in datas)
            {
                if (dict.AddOrUpdate(data.EquipmentId, data.StockNum))
                    flag = true;
            }
            if (flag)
                await SaveStockToLocal(s);
        }

        public async Task UpdateExStockNum(SimpleEquipmentData data, string s)
        {
            var dict = GetExStockDict(s);
            if (data.ExtraStock > 0)
            {
                if (dict.AddOrUpdate(data.EquipmentId, data.ExtraStock.Value))
                    await SaveExStockToLocal(s);
            }
            else
            {
                if (dict.Remove(data.EquipmentId))
                    await SaveExStockToLocal(s);
            }
        }

        public async Task UpdateStockNum(Dictionary<int, int> data, string s, Dictionary<int, int> exData)
        {
            if (!data.IsNullOrEmpty())
            {
                var dict = GetStockDict(s);
                foreach (var kv in data)
                {
                    dict.AddOrUpdate(kv.Key, kv.Value);
                }
                await SaveStockToLocal(s);
            }
            if (!exData.IsNullOrEmpty())
            {
                var dict = await InitUserExStockDict(s);
                foreach (var kv in exData)
                {
                    dict.AddOrUpdate(kv.Key, kv.Value);
                }
                await SaveExStockToLocal(s);
            }
        }

        public async ValueTask<Dictionary<int, int>> InitUserStockDict(string s)
        {
            var dict = GetStockDict(s);
            if (dict != null)
                return dict;
            switch (s)
            {
                case "cn":
                    StockDictCn = new Dictionary<int, int>();
                    dict = StockDictCn;
                    break;
                case "jp":
                    StockDictJp = new Dictionary<int, int>();
                    dict = StockDictJp;
                    break;
            }
            var userKey = $"stocks_{s}_{authenticationState.UserId}";
            var arrs = await localStorage.GetItemAsync<int[][]>(userKey);
            if (arrs == null)
            {
                var ues = await api.GetUserEquipStockAsync(s);
                if (ues != null)
                {
                    foreach (var es in ues.Stocks)
                    {
                        if (es.Stock > 0)
                            dict.Add(es.EId, es.Stock);
                    }
                    if (dict.Any())
                        await SaveStockToLocal(s);
                }
                return dict;
            }
            foreach (var arr in arrs)
            {
                if (arr[1] > 0)
                    dict.Add(arr[0], arr[1]);
            }
            return dict;
        }

        public async ValueTask<Dictionary<int, int>> InitUserExStockDict(string s)
        {
            var dict = GetExStockDict(s);
            if (dict != null)
                return dict;
            switch (s)
            {
                case "cn":
                    ExStockDictCn = new Dictionary<int, int>();
                    dict = ExStockDictCn;
                    break;
                case "jp":
                    ExStockDictJp = new Dictionary<int, int>();
                    dict = ExStockDictJp;
                    break;
            }
            var userKey = $"ex_stocks_{s}_{authenticationState.UserId}";
            var arrs = await localStorage.GetItemAsync<int[][]>(userKey);
            if (arrs == null)
            {
                return dict;
            }
            foreach (var arr in arrs)
            {
                if (arr[1] > 0)
                    dict.Add(arr[0], arr[1]);
            }
            return dict;
        }

        public async Task SaveStockToLocal(string s)
        {
            var d = GetStockDict(s);
            var userKey = $"stocks_{s}_{authenticationState.UserId}";
            await localStorage.SetItemAsync(userKey, d.Select(kv => new[] { kv.Key, kv.Value }));
        }

        public async Task SaveExStockToLocal(string s)
        {
            var d = GetExStockDict(s);
            var userKey = $"ex_stocks_{s}_{authenticationState.UserId}";
            await localStorage.SetItemAsync(userKey, d.Select(kv => new[] { kv.Key, kv.Value }));
        }

        private async Task<Dictionary<int, int[]>> GetStockUnionDict(string s)
        {
            var d = GetStockDict(s);
            var exd = await InitUserExStockDict(s);
            var keys = d.Keys.Union(exd.Keys);
            var dict = keys.ToDictionary(k => k, k => new[] { d.GetValueOrDefault(k), exd.GetValueOrDefault(k) });
            return dict;
        }

        public async Task<bool> SaveStockToServer(string s)
        {
            var d = await GetStockUnionDict(s);
            var us = new UserEquipStock { Server = s };
            us.Stocks = d.Select(kv => new EquipmentStock(kv.Key, kv.Value[0], kv.Value[1])).ToList();
            return await api.SaveUserEquipStockAsync(us, s);
        }

        public async Task ExportStock(string s)
        {
            var dict = await GetStockUnionDict(s);
            var str = string.Join("\n", dict.Select(kv => $"{kv.Key} {kv.Value[0]} {kv.Value[1]}"));
            await api.SaveStringAsFile($"库存导出{DateTime.Now:yyMMdd}.txt", str);
        }

        public async Task<bool> GetStockFromServer(string s)
        {
            var dict = GetStockDict(s);
            if (dict == null)
            {
                switch (s)
                {
                    case "cn":
                        StockDictCn = new Dictionary<int, int>();
                        dict = StockDictCn;
                        break;
                    case "jp":
                        StockDictJp = new Dictionary<int, int>();
                        dict = StockDictJp;
                        break;
                }
            }
            dict.Clear();
            var exDict = await InitUserExStockDict(s);
            exDict.Clear();

            var ues = await api.GetUserEquipStockAsync(s);
            if (ues == null)
                return false;
            foreach (var es in ues.Stocks)
            {
                if (es.Stock > 0)
                    dict.Add(es.EId, es.Stock);
                if (es.Ex > 0)
                    exDict.Add(es.EId, es.Ex);
            }
            await SaveStockToLocal(s);
            await SaveExStockToLocal(s);
            return true;
        }

        public async Task ClearStock(string s)
        {
            var d = await InitUserStockDict(s);
            d.Clear();
            await SaveStockToLocal(s);

            d = await InitUserExStockDict(s);
            d.Clear();
            await SaveExStockToLocal(s);
        }

        public void StartTrackBoxLines(List<UserBoxLine> lines)
        {
            trackedLines = lines;
        }

        public async ValueTask StopTrackBoxLines(List<UserBoxLine> lines, bool noConsume)
        {
            if (!api.Settings.AutoConsume)
                return;
            if (noConsume)
                return;
            if (trackedLines == null || lines == null)
                return;
            var q = from ol in trackedLines
                    join nl in lines
                    on ol.Id equals nl.Id
                    select (ol, nl);
            var lineGroups = q.ToList();
            if (lineGroups.IsNullOrEmpty())
                return;
            var s = lines[0].Server;
            var dict = await InitUserStockDict(s);
            if (!dict.Values.Any(v => v > 0))
                return;
            var datas = await api.GetEquipmentDatasAsync(s);
            var crafts = await api.GetEquipmentCraftsAsync(s);
            var ups = await api.GetUnitPromotionsAsync();
            var (mr, ms, ma) = await api.GetLimitParamAsync(s);
            var equips = PcrDataHelper.CalcBoxLineDiff(lineGroups, datas, ups, mr, ms, ma);
            var consume = PcrDataHelper.CalcEquipmentGapConsume(equips, datas, crafts);
            foreach (var item in consume)
            {
                dict.TryGetValue(item.EquipmentId, out var stock);
                item.StockNum = stock;
                if (item.CraftFlg)
                    item.ConsumedEquips = datas.First(d => d.EquipmentId == item.EquipmentId).ConsumedEquips;
            }
            foreach (var item in consume)
            {
                ApplyEquipStockToConsume(item, consume, 0, item.StockNum);
            }
            foreach (var eq in consume)
            {
                if (eq.ConsumeNum > 0 && dict.TryGetValue(eq.EquipmentId, out var snum) && snum > 0)
                {
                    var min = Math.Min(snum, eq.ConsumeNum);
                    dict[eq.EquipmentId] -= min;
                    Console.WriteLine($"消耗 {eq.EquipmentName} {min}");
                }
            }
            await SaveStockToLocal(s);
        }

        public async ValueTask<Dictionary<int, int>> CalcEquipmentDemand(string s, bool withBox, bool withStock, bool useJp)
        {
            List<UserBoxLine> box = null;
            UnitsTraceSetting traceSetting = null;
            if (withBox)
            {
                await InitUserStockDict(s);
                box = await api.GetUserBox(s);
                traceSetting = api.GetUnitsTraceSetting();
            }
            var ups = await api.GetUnitPromotionsAsync();

            var eqs = await api.GetEquipmentDatasAsync(useJp ? "jp" : s);
            var crafts = await api.GetEquipmentCraftsAsync(useJp ? "jp" : s);
            var result = PcrDataHelper.CalcEquipmentDemand(box, eqs, crafts, ups, traceSetting);
            if (withStock)
            {
                var stock = GetStockDict(s);
                foreach (var (qid, num) in stock)
                {
                    if (result.ContainsKey(qid))
                    {
                        result[qid] = Math.Max(0, result[qid] - num);
                    }
                }
            }
            DemandDict.AddOrReplace(s, result);
            return result;
        }

        public async ValueTask<Dictionary<int, int>> GetEquipmentDemand(string s)
        {
            if (DemandDict.TryGetValue(s, out var dict))
                return dict;
            return await CalcEquipmentDemand(s, true, true, false);
        }

        public async ValueTask CalcQuestEfficiency(IEnumerable<QuestInfo> quests, string s)
        {
            var dict = await GetEquipmentDemand(s);
            foreach (var qi in quests)
            {
                var p = 0d;
                foreach (var item in qi.DropItems)
                {
                    dict.TryGetValue(item.Id, out var d);
                    item.Demand = d;
                    p += d * item.Odds / 100d / qi.Stamina;
                }
                qi.Efficiency = p;
            }
        }

        public async Task<List<SolveResult>> GetSolveResultsAsync(SolveRequest r)
        {
            if (!api.Settings.RemoteSolve)
            {
                if (solveHelper == null || solveHelper.Server != r.Server)
                {
                    var eds = await api.GetEquipmentDatasAsync(r.Server);
                    var areas = await api.GetAreaInfos(r.Server);
                    await CalcQuestEfficiency(areas.SelectMany(a => a.Quests), r.Server);
                    solveHelper = new PcrSolveHelper(r.Server, eds, areas, api.GetSolveTask());
                }
                solveHelper.UpdateIncludeExtraDrops(api.Settings.IncludeExtraDrops);
                solveHelper.UpdateDemands(await GetEquipmentDemand(r.Server));
                return await solveHelper.GetJsSolveResult(r);
            }
            else
            {
                return await api.GetSolveResultsAsync(r);
            }
        }
    }
}
