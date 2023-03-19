using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using PcrBlazor.Client.Services;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcrBlazor.Client
{
    public static class JSMethods
    {
        public static IServiceProvider ServiceProvider;

        [JSInvokable]
        public static Task CloseDialog()
        {
            var ds = ServiceProvider?.GetRequiredService<DialogService>();
            ds?.Close();
            return Task.CompletedTask;
        }

        [JSInvokable]
        public static Task SetIsIos()
        {
            ImageExt.IsIos = true;
            return Task.CompletedTask;
        }
    }
}
