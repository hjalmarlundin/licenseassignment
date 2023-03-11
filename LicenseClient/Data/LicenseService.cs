namespace LicenseClient.Data;

using LicenseServer;

public class LicenseService
{
    private readonly ILicenseHttpClient licenseHttpClient;

    private readonly string clientName;
    private readonly ILogger<LicenseService> logger;

    public LicenseService(ILogger<LicenseService> logger, ILicenseHttpClient licenseHttpClient)
    {
        this.licenseHttpClient = licenseHttpClient;
        var r = new Random();
        int rInt = r.Next(1, 100);
        clientName = $"client{rInt}";
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<License>> GetLicensesAsync()
    {
        return await licenseHttpClient.GetAllLicenses();
    }

    public async Task<HttpResponseMessage> AddLicense(string licenseName)
    {
        return await licenseHttpClient.AddLicense(licenseName);
    }

    public async Task<License> RentLicense()
    {
        await licenseHttpClient.RentLicense(clientName);
        var licenses = await licenseHttpClient.GetAllLicenses();
        var license = licenses.SingleOrDefault(x => x.RentalInformation.Renter == clientName && x.RentalInformation.Status == LicenseStatus.Rented);
        logger.LogInformation($"Rented license: {license?.Identifier}");
        return license;
    }

    public Task<License[]> GetLicensesAsyncTest()
    {
        return Task.FromResult(Enumerable.Range(1, 5).Select(index => new LicenseServer.License
        {
            Identifier = index.ToString(),
            RentalInformation = new LicenseServer.RentalInformation() { Renter = $"Client{index}", Status = LicenseServer.LicenseStatus.Rented, RentedTime = DateTime.Now, RentExpirationTime = DateTime.Now.AddSeconds(15) }
        }).ToArray());
    }
}
