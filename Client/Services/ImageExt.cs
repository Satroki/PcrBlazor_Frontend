using PcrBlazor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcrBlazor.Client.Services
{
    public static class ImageExt
    {
#if DEBUG
        private const string BaseUri = "https://pcr-1252403488.file.myqcloud.com";
#else
        private const string BaseUri = "";
#endif

        public static bool IsIos { get; set; }

        private static string Format => IsIos ? "?imageMogr2/format/jpg" : null;

        public static string GetSkillIcon(this int id)
        {
            return $"{BaseUri}/icon/skill/{id:d4}.webp{Format}";
        }
        public static string GetUnitIcon(this string id, char min = '0')
        {
            if (id[^2] < min)
            {
                var arr = id.ToCharArray();
                arr[^2] = min;
                id = new(arr);
            }
            return $"{BaseUri}/icon/unit/{id}.webp{Format}";
        }
        public static string GetUnitIcon(this int id)
        {
            return $"{BaseUri}/icon/unit/{id}.webp{Format}";
        }

        public static string GetUnitCard(this int id)
        {
            return $"{BaseUri}/card/full/{id:d6}.webp{Format}";
        }
        public static string GetEquipIcon(this int id)
        {
            return $"{BaseUri}/icon/equipment/{id:d6}.webp{Format}";
        }
        public static string GetItemIcon(this int id)
        {
            return $"{BaseUri}/icon/item/{id:d5}.webp{Format}";
        }
        public static string GetIcon(this QuestDropItem item)
        {
            return item.IsItem ? item.Id.GetItemIcon() : item.Id.GetEquipIcon();
        }
    }
}
