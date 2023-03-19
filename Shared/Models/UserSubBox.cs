using System.Collections.Generic;

namespace PcrBlazor.Shared
{
    public class UserSubBox:IUserId
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Server { get; set; }
        public string Name { get; set; }
        public List<UserBoxLine> Lines { get; set; }
    }
}
