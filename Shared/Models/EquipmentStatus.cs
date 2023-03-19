using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace PcrBlazor.Shared
{
    public class EquipmentStatus
    {
        public int EquipmentId { get; set; }
        public int EquipmentUid { get; set; }
        public string CnName { get; set; }
        public string JpName { get; set; }
        public string Status { get; set; }
        public string StatusCn { get; set; }
        [JsonIgnore]
        public EquipmentData Data { get; set; }
        [JsonIgnore]
        public EquipmentEnhanceRate EnhanceRate { get; set; }
    }

    public class EquipmentStatusSet
    {
        public Dictionary<int, EquipmentStatus> EquipmentStatusDict { get; set; }
        public Dictionary<int, string> StatusDict { get; set; }
        public Dictionary<int, string> StatusCnDict { get; private set; }
        public bool IsEmpty { get; private set; }

        public EquipmentStatusSet(List<EquipmentStatus> statuses)
        {
            IsEmpty = statuses.IsNullOrEmpty();
            EquipmentStatusDict = statuses.ToDictionary(s => s.EquipmentId);
            StatusDict = statuses.Where(e => e.EquipmentId < 11_0000).ToDictionary(e => e.EquipmentUid, e => e.Status);
            StatusCnDict = statuses.Where(e => e.EquipmentId < 11_0000 && !e.StatusCn.IsNullOrEmpty()).ToDictionary(e => e.EquipmentUid, e => e.StatusCn);
        }

        public string TryGetStatus(int eid, string s)
        {
            if (s == "cn" && StatusCnDict.TryGetValue(eid % 10000, out var st))
                return st;
            StatusDict.TryGetValue(eid % 10000, out st);
            return st;
        }

        public string TryGetNameAndStatus(int eid, string s)
        {
            var name = TryGetName(eid, s);
            var status = TryGetStatus(eid, s);
            return $"{name}\n\n{status}".Trim();
        }

        public string TryGetName(int eid, string s)
        {
            EquipmentStatusDict.TryGetValue(eid, out var es);
            if (es != null)
            {
                return s switch
                {
                    "cn" => es.CnName ?? es.JpName,
                    _ => es.JpName
                };
            }
            return string.Empty;
        }

        public bool Contains(int eid)
        {
            return EquipmentStatusDict.ContainsKey(eid);
        }
    }
}
