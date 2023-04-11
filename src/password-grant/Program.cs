using Newtonsoft.Json;

var clientId = File.ReadAllLines(".env").First(l=>l.Contains("CLIENT_ID=")).Split('=')[1];
var clientSecret = File.ReadAllLines(".env").First(l=>l.Contains("CLIENT_SECRET=")).Split('=')[1];
var discoveryEndpoint = File.ReadAllLines(".env").First(l=>l.Contains("DISCOVERY_URL=")).Split('=')[1];

// OIDC configuration
string username = args[0];
string password = args[1];
string scope = "openid profile";

// Get the OIDC discovery document
using (HttpClient httpClient = new HttpClient())
{
    var discoveryResponse = await httpClient.GetAsync(discoveryEndpoint);
    var discoveryContent = await discoveryResponse.Content.ReadAsStringAsync();
    var discoveryDocument = JsonConvert.DeserializeObject<DiscoveryDocument>(discoveryContent);

    // Use the discovery document to build the token endpoint URL
    var tokenEndpoint = $"{discoveryDocument.TokenEndpoint}";

    // Build the token request payload
    Dictionary<string, string> payload = new Dictionary<string, string>();
    payload.Add("grant_type", "password");
    payload.Add("client_id", clientId);
    payload.Add("client_secret", clientSecret);
    payload.Add("username", username);
    payload.Add("password", password);
    payload.Add("scope", scope);

    // Send the token request
    using (HttpClient client = new HttpClient())
    {
        var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
        request.Content = new FormUrlEncodedContent(payload);
        var response = await client.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Parse the token response
        var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

        if (!string.IsNullOrEmpty(tokenResponse.Error))
        {
            Console.WriteLine($"Error: {tokenResponse.Error} - {tokenResponse.ErrorDescription}");
        }
        else
        {
            Console.WriteLine($"ID Token: {tokenResponse.IdToken}");
            File.WriteAllText("idToken.txt", tokenResponse.IdToken);
        }
    }
}