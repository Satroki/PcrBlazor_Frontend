using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PcrBlazor.Shared
{
    public class SkillModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double CastTime { get; set; }
        public double UbCoolTime { get; internal set; }
        public int Icon { get; set; }
        public List<SkillActionModel> Actions { get; set; }
        public int SkillId { get; set; }
        public int SkillLevel { get; set; }
        [JsonIgnore]
        public List<(int actionId, int dependActionId)> ActionIds { get; internal set; }
    }
}
