using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using PcrBlazor.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PcrBlazor.Client
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private const string key = "authToken";
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService localStorage;

        public int UserId { get; private set; }
        public string UserName { get; private set; }
        public string Token { get; private set; }
        public ApiAuthenticationStateProvider(HttpClient _httpClient, ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
            this._httpClient = _httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var resp = await _httpClient.GetAsync("api/Accounts");
            if (resp.IsSuccessStatusCode)
            {
                var token = await localStorage.GetItemAsync<string>(key);
                var authState = await GetAuthenticationStateFromJwtAsync(token);
                SetUser(authState);
                await localStorage.SetItemAsync("UserName", authState.User.GetUserName());
                return authState;
            }
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public async Task MarkUserAsAuthenticatedAsync(string token)
        {
            var authState = await GetAuthenticationStateFromJwtAsync(token);
            SetUser(authState);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task MarkUserAsLoggedOutAsync()
        {
            await localStorage.RemoveItemAsync(key);
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            SetUser();
            AccessTokenProvider.Instance.RemoveAccessToken();
            NotifyAuthenticationStateChanged(authState);
        }

        private async Task<AuthenticationState> GetAuthenticationStateFromJwtAsync(string token)
        {
            var jwt = new JwtToken(token);
            Token = token;
            await localStorage.SetItemAsync(key, token);
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims, "apiauth"));
            var state = new AuthenticationState(authenticatedUser);
            return state;
        }

        private void SetUser(AuthenticationState authState)
        {
            UserId = authState.User.FindFirst(ClaimTypes.NameIdentifier).Value.AsInt();
            UserName = authState.User.FindFirst(ClaimTypes.Name).Value;
        }
        private void SetUser()
        {
            UserId = 0;
            UserName = string.Empty;
            Token = string.Empty;
        }
    }

    public class JwtToken
    {
        public List<Claim> Claims { get; set; }
        public long Exp { get; set; }
        public DateTimeOffset Expires { get; set; }
        public string Iss { get; set; }
        public string Aud { get; set; }
        public string Raw { get; set; }
        public JwtToken(string token)
        {
            Raw = token;
            var payload = token.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var dict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonBytes);
            Aud = dict["aud"].GetString();
            Iss = dict["iss"].GetString();
            Exp = dict["exp"].GetInt64();
            Expires = DateTimeOffset.FromUnixTimeSeconds(Exp);
            Claims = new List<Claim>();
            if (dict.TryGetValue(ClaimTypes.Name, out var jname))
                Claims.Add(new Claim(ClaimTypes.Name, jname.GetString()));
            if (dict.TryGetValue(ClaimTypes.NameIdentifier, out var jid))
                Claims.Add(new Claim(ClaimTypes.NameIdentifier, jid.GetString()));
            if (dict.TryGetValue("NickName", out var nn))
                Claims.Add(new Claim("NickName", nn.GetString()));
            if (dict.TryGetValue(ClaimTypes.Role, out var jroles))
            {
                switch (jroles.ValueKind)
                {
                    case JsonValueKind.Array:
                        {
                            foreach (var jr in jroles.EnumerateArray())
                            {
                                Claims.Add(new Claim(ClaimTypes.Role, jr.GetString()));
                            }
                            break;
                        }
                    case JsonValueKind.String:
                        {
                            Claims.Add(new Claim(ClaimTypes.Role, jroles.GetString()));
                            break;
                        }
                }
            }
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            base64 = base64.Replace('-', '+').Replace('_', '/');
            return Convert.FromBase64String(base64);
        }
    }

    public class AccessTokenProvider : IAccessTokenProvider
    {
        private const string key = "authToken";
        private readonly ILocalStorageService localStorage;
        public event Action OnRemoveToken;
        public static AccessTokenProvider Instance;

        public AccessTokenProvider(ILocalStorageService localStorage)
        {
            this.localStorage = localStorage;
            Instance = this;
        }

        public void RemoveAccessToken()
        {
            Instance.OnRemoveToken?.Invoke();
        }

        public async ValueTask<AccessTokenResult> RequestAccessToken()
        {
            var token = await localStorage.GetItemAsync<string>(key);
            if (string.IsNullOrWhiteSpace(token))
                return new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null, "/login");
            var accessToken = new AccessToken() { Value = token, Expires = DateTimeOffset.Now.AddMinutes(30) };
            return new AccessTokenResult(AccessTokenResultStatus.Success, accessToken, null);
        }

        public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options)
        {
            return RequestAccessToken();
        }
    }

    public class PcrAuthorizationMessageHandler : DelegatingHandler
    {
        private readonly AccessTokenProvider _provider;
        private readonly NavigationManager _navigation;
        private AccessToken _lastToken;
        private AuthenticationHeaderValue _cachedHeader;
        private readonly Uri[] _authorizedUris;

        public PcrAuthorizationMessageHandler(
            IAccessTokenProvider provider,
            NavigationManager navigation)
        {
            _provider = (AccessTokenProvider)provider;
            _provider.OnRemoveToken += _provider_OnRemoveToken;
            _navigation = navigation;
            _authorizedUris = new[] { navigation.BaseUri, Constants.ApiBase }.Where(u => !string.IsNullOrEmpty(u)).Distinct()
                .Select(u => new Uri(u, UriKind.Absolute)).ToArray();
        }

        private void _provider_OnRemoveToken()
        {
            _lastToken = null;
            _cachedHeader = null;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.Now;
            if (_authorizedUris.Any(uri => uri.IsBaseOf(request.RequestUri)))
            {
                if (_lastToken == null || now >= _lastToken.Expires.AddMinutes(-5))
                {
                    var tokenResult = await _provider.RequestAccessToken();

                    if (tokenResult.TryGetToken(out var token))
                    {
                        _lastToken = token;
                        _cachedHeader = new AuthenticationHeaderValue("Bearer", _lastToken.Value);
                    }
                    else
                    {
                        _cachedHeader = null;
                    }
                }
                request.Headers.Authorization = _cachedHeader;
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
