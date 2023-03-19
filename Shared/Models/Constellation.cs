using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace PcrBlazor.Shared
{
    public enum Constellation
    {
        [Description("水瓶座")] Aquarius = 1,
        [Description("双鱼座")] Pisces = 2,
        [Description("白羊座")] Aries = 3,
        [Description("金牛座")] Taurus = 4,
        [Description("双子座")] Gemini = 5,
        [Description("巨蟹座")] Cancer = 6,
        [Description("狮子座")] Leo = 7,
        [Description("处女座")] Virgo = 8,
        [Description("天秤座")] Libra = 9,
        [Description("天蝎座")] Scorpio = 10,
        [Description("射手座")] Sagittarius = 11,
        [Description("摩羯座")] Capricorn = 12,
    }
}
