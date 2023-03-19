using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using PcrBlazor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PcrBlazor.Client.Services
{
    public class UserService
    {
        private readonly HttpClient phc;
        private readonly HttpClient hc;
        private readonly ApiAuthenticationStateProvider authenticationStateProvider;

        public string Token => authenticationStateProvider?.Token;

        public UserService(IHttpClientFactory httpClientFactory,
            AuthenticationStateProvider authenticationStateProvider)
        {
            this.phc = httpClientFactory.CreateClient("PcrBlazor");
            this.hc = httpClientFactory.CreateClient("PcrBlazor.Auth");
            this.authenticationStateProvider = (ApiAuthenticationStateProvider)authenticationStateProvider;
        }

        public async Task<AccountResult> Register(AccountModel registerModel)
        {
            var resp = await phc.PostAsJsonAsync("api/Accounts", registerModel);
            return await resp.ReadAsAsync<AccountResult>(true);
        }

        public async Task<AccountResult> Login(AccountModel loginModel)
        {
            var response = await phc.PostAsJsonAsync("api/Login", loginModel);
            var loginResult = await response.ReadAsAsync<AccountResult>(true);
            if (loginResult.Successful)
                await authenticationStateProvider.MarkUserAsAuthenticatedAsync(loginResult.Token);
            return loginResult;
        }

        public async Task<AccountResult> Login(string token)
        {
            var response = await phc.GetAsync($"api/Login?token={token}");
            var loginResult = await response.ReadAsAsync<AccountResult>(true);
            if (loginResult.Successful)
                await authenticationStateProvider.MarkUserAsAuthenticatedAsync(loginResult.Token);
            return loginResult;
        }

        public async Task Logout()
        {
            await authenticationStateProvider.MarkUserAsLoggedOutAsync();
        }

        public async Task<PcrUserModel> GetUserInfo()
        {
            var r = await hc.GetFromJsonAsync<PcrUserModel>("api/Accounts/GetUserInfo");
            return r;
        }

        public async Task<AccountResult> UpdateUserInfo(string nickName)
        {
            var r = await hc.PostAsJsonAsync("api/Accounts/UpdateUserInfo", new AccountModel { NickName = nickName });
            return await r.ReadAsAsync<AccountResult>();
        }

        public async Task<AccountResult> ChangeEmail(string email, string pwd)
        {
            var r = await hc.PostAsJsonAsync("api/Accounts/ChangeEmail", new AccountModel { Password = pwd, Email = email });
            return await r.ReadAsAsync<AccountResult>();
        }

        public async Task<AccountResult> ChangePassword(string pwd, string newPwd)
        {
            var r = await hc.PostAsJsonAsync("api/Accounts/ChangePassword", new AccountModel { Password = pwd, NewPassword = newPwd });
            return await r.ReadAsAsync<AccountResult>();
        }

        public async Task<AccountResult> UnBind(string p)
        {
            var r = await hc.GetFromJsonAsync<AccountResult>($"api/Accounts/UnBind?p={p}");
            return r;
        }

        public async Task<AccountResult> DeleteUser()
        {
            var r = await hc.GetFromJsonAsync<AccountResult>($"api/Accounts/DeleteUser");
            if (r.Successful)
            {
                await Logout();
            }
            return r;
        }

        public async Task<AccountResult> GetResetPasswordToken(string userName, string email)
        {
            var r = await phc.PostAsJsonAsync("api/Accounts/GetResetPasswordToken", new AccountModel { UserName = userName, Email = email });
            return await r.ReadAsAsync<AccountResult>();
        }

        public async Task<AccountResult> ResetPasswordByEmail(string userName, string token, string newPwd)
        {
            var r = await phc.PostAsJsonAsync("api/Accounts/ResetPasswordByEmail", new AccountModel { UserName = userName, Password = token, NewPassword = newPwd });
            return await r.ReadAsAsync<AccountResult>();
        }

        public async Task<AccountResult> SetUserEmail(string user, string email)
        {
            var r = await hc.PostAsJsonAsync("api/Accounts/SetUserEmail", new AccountModel { UserName = user, Email = email });
            return await r.ReadAsAsync<AccountResult>();
        }

        public async Task<string> GeneratePasswordResetToken(string user)
        {
            var s = await hc.GetStringAsync($"/api/Accounts/GeneratePasswordResetToken?user={user}");
            return s;
        }
    }
}
