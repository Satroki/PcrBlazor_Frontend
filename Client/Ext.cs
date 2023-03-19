using Blazored.LocalStorage;
using PcrBlazor.Client.Shared;
using PcrBlazor.Shared;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PcrBlazor.Client
{
    public static class Ext
    {
        public static async Task<bool> OpenConfirmAsync(this DialogService dialog, string title, string content = null)
        {
            var cr = await dialog.OpenAsync<Confirm>(title, new Dictionary<string, object> { ["Content"] = content },
                new DialogOptions { Width = "400px" });
            if (cr is bool b)
                return b;
            return false;
        }

        public static async Task<List<UnitData>> OpenUnitPickerAsync(this DialogService dialog,
            List<int> filter = null, List<int> preset = null, List<int> selected = null)
        {
            var cr = await dialog.OpenAsync<UnitPicker>("选取角色", new Dictionary<string, object>
            {
                ["Filter"] = filter,
                ["Preset"] = preset,
                ["Selected"] = selected,
            }, new DialogOptions { Width = "760px" });
            if (cr is List<UnitData> list)
                return list;
            return null;
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirst("NickName")?.Value ?? user.Identity.Name;
        }

        public static int GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value.AsInt() ?? 0;
        }

        public static async Task<T> GetFilterRecord<T>(this ILocalStorageService storage, T defaultRecord, string server) where T : FilterBase<T>
        {
            var lastFR = await storage.GetItemAsync<T>(defaultRecord.LocalKey + server?.ToUpper());
            lastFR?.InitSorterOrder();
            var result = lastFR ?? defaultRecord;
            result.ChangeLocalKey(defaultRecord.LocalKey);
            return result;
        }

        public static async Task SaveFilterRecord<T>(this ILocalStorageService storage, T record, string server) where T : FilterBase<T>
        {
            if (record != null)
            {
                record.SetSorterOrder();
                await storage.SetItemAsync(record.LocalKey + server?.ToUpper(), record);
            }
        }

        public static async Task<T> ReadAsAsync<T>(this HttpResponseMessage r, bool ignoreStatusCode = false)
        {
            if ((ignoreStatusCode || r.IsSuccessStatusCode) && r.StatusCode != System.Net.HttpStatusCode.NoContent)
            {
                return await r.Content.ReadFromJsonAsync<T>();
            }
            return default;
        }
    }
}
