using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Web;


class Program
{
    static async Task Main(string[] args)
    {
        // OIDC parameters from .env File
        var clientId = File.ReadAllLines(".env").First(l=>l.Contains("CLIENT_ID=")).Split('=')[1];
        var clientSecret = File.ReadAllLines(".env").First(l=>l.Contains("CLIENT_SECRET=")).Split('=')[1];
        var discoveryUrl = File.ReadAllLines(".env").First(l=>l.Contains("DISCOVERY_URL=")).Split('=')[1];

        string redirectUri = "https://12345.code.mlu.dev.neo.onl/";
        string internalRedirectUri = "http://*:12345/";
        string scope = "openid profile roles";
        string responseType = "code";

        // Discover the authorization and token endpoints from the discovery document
        var httpClient = new HttpClient();
        var discoveryResponse = await httpClient.GetAsync(discoveryUrl);
        var discoveryContent = await discoveryResponse.Content.ReadAsStringAsync();
        var discoveryData = JsonConvert.DeserializeObject<DiscoveryDocument>(discoveryContent);
        string authorizationEndpoint = discoveryData.AuthorizationEndpoint;
        string tokenEndpoint = discoveryData.TokenEndpoint;

        // Build the authorization request URL
        string authRequestUrl = $"{authorizationEndpoint}?client_id={HttpUtility.UrlEncode(clientId)}&redirect_uri={HttpUtility.UrlEncode(redirectUri)}&scope={HttpUtility.UrlEncode(scope)}&response_type={HttpUtility.UrlEncode(responseType)}";

        // Launch the user's web browser to the authorization request URL
        System.Console.WriteLine($"Authorize here: {authRequestUrl}");

        // Listen for the redirect on a temporary local port
        using (var listener = new HttpListener())
        {
            listener.Prefixes.Add(internalRedirectUri);
            listener.Start();

            Console.WriteLine($"Waiting for user to authenticate and be redirected to {redirectUri}");
            Console.WriteLine($"Listening on: {internalRedirectUri}");
            var context = await listener.GetContextAsync();

            // Parse the query string parameters from the redirect URI
            var queryParams = context.Request.QueryString;
            string code = queryParams["code"];

            // Exchange the authorization code for an ID token and access token
            var (idToken, accessToken) = await ExchangeAuthorizationCodeForTokens(code, clientId, clientSecret, redirectUri, tokenEndpoint, scope);

            // Verify the ID token signature and decode its payload
            var idTokenPayload = DecodeAndVerifyIdToken(idToken);

            // Print the ID token claims and access token
            Console.WriteLine("ID token claims:");
            foreach (var entry in idTokenPayload)
            {
                Console.WriteLine($"{entry.Key}: {entry.Value}");
            }

            Console.WriteLine($"\nAccess token: {idToken}");

            // Send a response to the browser to close the window
            using (var writer = new StreamWriter(context.Response.OutputStream))
            {
                writer.Write("<html><head><script>window.close();</script></head><body>Close this window.</body></html>");
            }
        }
    }

    static async Task<(string, string)> ExchangeAuthorizationCodeForTokens(string code, string clientId, string clientSecret, string redirectUri, string tokenEndpoint, string scope)
    {
        // Construct the token request
        var tokenRequest = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "grant_type", "authorization_code" },
            { "code", code },
            { "redirect_uri", redirectUri },
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "scope", scope }
        });
        tokenRequest.Content = content;

        // Send the token request and parse the response
        var client = new HttpClient();
        var response = await client.SendAsync(tokenRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);

        return (responseData["id_token"], responseData["access_token"]);
    }

    static Dictionary<string, object> DecodeAndVerifyIdToken(string idToken)
    {
        // Decode the JWT header and payload from base64 URL encoding
        var parts = idToken.Split('.');
        var headerJson = DecodeBase64UrlString(parts[0]);
        var payloadJson = DecodeBase64UrlString(parts[1]);

        // Parse the JWT header and payload as JSON objects
        var header = JsonConvert.DeserializeObject<Dictionary<string, string>>(headerJson);
        var payload = JsonConvert.DeserializeObject<Dictionary<string, object>>(payloadJson);

        // Verify the JWT signature (not implemented in this example)

        return payload;
    }

    static string DecodeBase64UrlString(string base64Url)
    {
        string base64 = base64Url.Replace('-', '+').Replace('_', '/');
        var padLength = 4 - (base64.Length % 4);
        for (int i = 0; i < padLength; i++)
        {
            base64 += '=';
        }
        byte[] bytes = Convert.FromBase64String(base64);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}
