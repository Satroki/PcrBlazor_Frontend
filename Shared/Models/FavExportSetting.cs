using System.Collections.Generic;

namespace PcrBlazor.Shared
{
    public class FavExportSetting
    {
        public string Server { get; set; }
        public bool EnableFilters { get; set; }
        public List<UnitFilter> Filters { get; set; }
        public string FiltersType { get; set; }
        public string ExportMode { get; set; }
        public List<string> ExportFields { get; set; }
        public int? GroupId { get; set; }
    }
}
