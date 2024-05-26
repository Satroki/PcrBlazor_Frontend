using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PcrBlazor.Shared
{
    public class PcrUserModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserNameAlias { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public string ShareId { get; set; }
        public DateTimeOffset? CreateAt { get; set; }
        public bool NoPassword { get; set; }

        public List<string> Logins { get; set; }
    }
}
