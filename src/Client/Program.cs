

using IdentityModel.Client;
using System.Text.Json.Nodes;


// discover endpoints from metadata
var client = new HttpClient();

var discoveryDocument = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
if (discoveryDocument.IsError)
{
    Console.WriteLine(discoveryDocument.Error);
    Console.ReadKey();
    return;
}

// request token
var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = discoveryDocument.TokenEndpoint,

    ClientId = "client",
    ClientSecret = "secret",
    Scope = "api1"
});

if (tokenResponse.IsError)
{
    Console.WriteLine(tokenResponse.Error);
    Console.ReadKey();
    return;
}

Console.WriteLine(tokenResponse.Json);

// call api
var apiClient = new HttpClient();
apiClient.SetBearerToken(tokenResponse.AccessToken);

var response = await apiClient.GetAsync("https://localhost:6001/identity");
if (!response.IsSuccessStatusCode)
{
    Console.WriteLine(response.StatusCode);
    Console.ReadKey();
}
else
{
    string content = await response.Content.ReadAsStringAsync();
    Console.WriteLine(JsonArray.Parse(content));
}

Console.ReadKey();