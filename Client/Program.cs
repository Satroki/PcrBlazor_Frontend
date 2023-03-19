using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using PcrBlazor.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Radzen;
using TG.Blazor.IndexedDB;
using System.Text.Json;

namespace PcrBlazor.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.Services.AddBlazoredLocalStorage(config =>
            {
                config.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                config.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                config.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
                config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                config.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                config.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                config.JsonSerializerOptions.WriteIndented = false;
            });
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
            builder.Services.AddScoped<DbService>();
            builder.Services.AddScoped<CacheService>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddScoped<ApiService>();
            builder.Services.AddScoped<ArmoryService>();
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddScoped<NotificationService>();
            builder.Services.AddScoped<TooltipService>();
            builder.Services.AddScoped<TooltipHelperService>();
            builder.Services.AddScoped<ContextMenuService>();
            builder.Services.AddScoped<IAccessTokenProvider, AccessTokenProvider>();
            builder.Services.AddScoped<PcrAuthorizationMessageHandler>();

            var baseUrl = Constants.ApiBase ?? builder.HostEnvironment.BaseAddress;

            builder.Services.AddHttpClient("PcrBlazor.Auth", client => client.BaseAddress = new Uri(baseUrl))
                         .AddHttpMessageHandler<PcrAuthorizationMessageHandler>();
            builder.Services.AddHttpClient("PcrBlazor", client => client.BaseAddress = new Uri(baseUrl));
            builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("PcrBlazor.Auth"));

            builder.Services.AddIndexedDB(dbStore =>
            {
                dbStore.DbName = "UnitSourceData";
                dbStore.Version = 6;
                dbStore.Stores.AddRange(DbService.GetStoreSchemas());
            });

            builder.Services.Configure<LoggerFilterOptions>(opt =>
            {
                opt.AddFilter("System", LogLevel.Warning);
                opt.AddFilter("Microsoft", LogLevel.Warning);
            });
            var host = builder.Build();
            JSMethods.ServiceProvider = host.Services;
            await host.RunAsync();
        }
    }
}
