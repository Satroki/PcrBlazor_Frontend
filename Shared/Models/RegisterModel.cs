using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace PcrBlazor.Shared
{

    public class AccountModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string Email { get; set; }
        public string NickName { get; set; }
        public string UserNameAlias { get; set; }
    }

    public class AccountResult
    {
        public bool Successful { get; set; }
        public string Error { get; set; }
        public string Token { get; set; }
        public AccountResult()
        {

        }

        public AccountResult(string error)
        {
            Successful = false;
            Error = error;
        }

        public AccountResult(bool success, string error = null)
        {
            Successful = success;
            Error = error;
        }
    }
}
