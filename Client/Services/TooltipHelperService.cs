using Microsoft.AspNetCore.Components;
using PcrBlazor.Shared;
using Radzen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcrBlazor.Client.Services
{
    public class TooltipHelperService
    {
        private readonly ApiService api;
        private readonly TooltipService tooltip;
        private bool opened = false;
        readonly string style = @"padding: 0.5rem;
    font-size: 0.8rem;
    line-height: 1rem;
    background: rgba(0,0,0,0.8);";

        public TooltipHelperService(ApiService api, TooltipService tooltip)
        {
            this.api = api;
            this.tooltip = tooltip;
        }

        public void ShowEquipmentStatus(ElementReference ele, int id, string s)
        {
            var t = api.EquipmentStatusSet.TryGetNameAndStatus(id, s);
            if (!t.IsNullOrEmpty())
            {
                try
                {
                    tooltip.Open(ele, CreateDynamicComponent(t), new TooltipOptions { Duration = null, Style = style, Position = TooltipPosition.Right });
                    opened = true;
                }
                catch { }
            }
        }

        public void ShowString(ElementReference ele, string s, TooltipPosition? p = null)
        {
            if (!string.IsNullOrWhiteSpace(s))
            {
                try
                {
                    var opt = new TooltipOptions { Duration = null, Style = style };
                    if (p.HasValue)
                        opt.Position = p.Value;
                    tooltip.Open(ele, CreateDynamicComponent(s), opt);
                    opened = true;
                }
                catch { }
            }
        }

        private static RenderFragment<TooltipService> CreateDynamicComponent(string s) => ts =>
        {
            return builder =>
            {
                builder.OpenElement(0, "p");
                builder.AddAttribute(1, "class", "tooltip-p");
                builder.AddContent(2, s);
                builder.CloseElement();
            };
        };

        public void Close()
        {
            try
            {
                if (opened)
                {
                    opened = false;
                    tooltip.Close();
                }
            }
            catch { }
        }
    }
}
