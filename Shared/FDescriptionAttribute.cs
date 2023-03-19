using System;
using System.ComponentModel;

namespace PcrBlazor.Shared
{
    public class FDescriptionAttribute : DescriptionAttribute
    {
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
        public string Description5 { get; set; }
        public FDescriptionAttribute()
        {

        }

        public FDescriptionAttribute(string d, string d2 = null, string d3 = null, string d4 = null, string d5 = null) : base(d)
        {
            Description2 = d2;
            Description3 = d3;
            Description4 = d4;
            Description5 = d5;
        }
    }
}
