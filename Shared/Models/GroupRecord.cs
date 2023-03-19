using System.Collections.Generic;

namespace PcrBlazor.Shared
{
    public class GroupRecord : IUserId
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Server { get; set; }
        public string Name { get; set; }
        public GroupType Type { get; set; }
        public List<int> LineIds { get; set; }
    }

    public enum GroupType
    {
        Box = 0,
        Favorite = 1
    }
}
