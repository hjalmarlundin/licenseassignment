namespace LicenseClient.Data;

using System.Text.Json;
using LicenseServer;

public class LicenseHttpClient : ILicenseHttpClient
{
    private readonly HttpClient httpClient;
    private readonly ILogger<LicenseHttpClient> logger;

    // TODO: Investigate https a bit more..
    private readonly Uri serverEndpoint = new Uri("http://localhost:5042");

    public LicenseHttpClient(ILogger<LicenseHttpClient> logger, HttpClient httpClient)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<License>> GetAllLicenses()
    {
        var uri = new Uri(serverEndpoint, "GetLicenses");
        var responseString = await httpClient.GetStringAsync(uri);

        if (responseString == null)
        {
            this.logger.LogWarning("Response from endpoint is null");
            return new List<License>();
        }

        var licenses = JsonSerializer.Deserialize<List<License>>(responseString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        if (licenses == null)
        {
            this.logger.LogError("Could not deserialize response to license model");
        }

        return licenses ?? new List<License>();
    }

    public async Task<HttpResponseMessage> AddLicense(string licenseName)
    {
        this.logger.LogDebug($"Adding license with name {licenseName}");
        var uri = new Uri(serverEndpoint, $"License?licenseName={licenseName}");

        var response = await httpClient.PostAsync(uri, null);
        this.logger.LogDebug($"Response: {response.StatusCode}");
        return response;
    }

    public async Task<HttpResponseMessage> RentLicense(string clientName)
    {
        this.logger.LogDebug($"Rentning license for client {clientName}");
        var uri = new Uri(serverEndpoint, $"RentLicense?renter={clientName}");

        var response = await httpClient.GetAsync(uri);
        this.logger.LogDebug($"Received {response.StatusCode}");

        return response;
    }
}
