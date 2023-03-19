using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PcrBlazor.Client
{
    public enum LoginPageMode
    {
        Login,
        Register,
        ResetPwd,
        ChangePwd,
        ChangeEmail,
        ChangeNickName,
        Info,
        None
    }
}
