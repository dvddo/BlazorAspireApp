using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BlazorAspireApp.Web
{
    public class AuthorizationApiClient
    {

        private readonly HttpClient _httpClient;

        public static async Task<HttpClient> SetHeaderToken(IJSRuntime jSRuntime, HttpClient httpClient) {
            
            if(httpClient.DefaultRequestHeaders.Authorization==null)
            {
                var accessToken = await jSRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");
                httpClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", accessToken);
            }
            return httpClient;
        }

        public AuthorizationApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Set Bearer token for authenticated requests
        public void SetBearerToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // User Registration
        public async Task<RegisterResponse> RegisterAsync(string email, string password, CancellationToken cancellationToken = default)
        {

            try
            {
                var payload = new { email, password };
                var regResponse = (await _httpClient.PostAsJsonAsync("/api/Users/Register", payload, cancellationToken)).Content.ReadFromJsonAsync<RegisterResponse>(cancellationToken).Result;
                return regResponse;

            }
            catch (Exception ex)
            {
                return new RegisterResponse { status = -800,  errors = null };
            }
        }

        public record RegisterErrors
        {
            public List<string> additionalProp1 { get; set; }
            public List<string> additionalProp2 { get; set; }
            public List<string> additionalProp3 { get; set; }
        }

        public record RegisterResponse
        {
            public string type { get; set; }
            public string title { get; set; }
            public int status { get; set; }
            public string detail { get; set; }
            public string instance { get; set; }
            public RegisterErrors? errors { get; set; }
            public string? additionalProp1 { get; set; }
            public string? additionalProp2 { get; set; }
            public string? additionalProp3 { get; set; }
        }

        // User Login
        public async Task<LoginResponse> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            try
            {
                var payload = new { email, password, twoFactorCode = "", twoFactorRecoveryCode = "" };
                var loginResponse = (await _httpClient.PostAsJsonAsync("/api/Users/Login?useCookies=false&useSessionCookies=false", payload, cancellationToken)).Content.ReadFromJsonAsync<LoginResponse>(cancellationToken).Result;
                return loginResponse;

            }
            catch (Exception ex)
            {
                return new LoginResponse { tokenType = "", accessToken = "", expiresIn = 0, refreshToken = "" };
            }
        }

        public record LoginResponse
        {
            public string tokenType { get; set; }
            public string accessToken { get; set; }
            public int expiresIn { get; set; }
            public string refreshToken { get; set; }
        }

        // Refresh Token
        public async Task<HttpResponseMessage> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
        {
            var payload = new { refreshToken };
            return await _httpClient.PostAsJsonAsync("/api/Users/Refresh", payload, cancellationToken);
        }
    }
}
