namespace LicenseClient.Data;

using System.Text.Json;
using LicenseServer;

public interface ILicenseHttpClient
{
    Task<IEnumerable<License>> GetAllLicenses();
}

public class LicenseHttpClient : ILicenseHttpClient
{
    private readonly HttpClient httpClient;
    private readonly ILogger<LicenseHttpClient> logger;

    public LicenseHttpClient(ILogger<LicenseHttpClient> logger, HttpClient httpClient)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<License>> GetAllLicenses()
    {
        // TODO: Investigate https a bit more..
        var uri = new Uri("http://localhost:5042/GetLicenses");

        var responseString = await httpClient.GetStringAsync(uri);

        if (responseString == null)
        {
            this.logger.LogWarning("Response from endpoint is null");
            return new List<License>();
        }

        return JsonSerializer.Deserialize<List<License>>(responseString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
    }
}
