using System.Collections.Generic;

namespace PcrBlazor.Shared
{
    public class UserBoxResult
    {
        public List<UserBoxLine> Box { get; set; }
        public List<UserSubBox> SubBoxes { get; set; }
        public List<GroupRecord> Groups { get; set; }
        public UnitsTraceSetting UnitsTraceSetting { get; set; }
    }
}
