namespace LicenseClient.Data;

using LicenseServer;

public class LicenseService
{
    private readonly ILicenseHttpClient licenseHttpClient;

    public LicenseService(ILicenseHttpClient licenseHttpClient)
    {
        this.licenseHttpClient = licenseHttpClient;
    }

    public async Task<IEnumerable<License>> GetLicensesAsync2()
    {
        return await this.licenseHttpClient.GetAllLicenses();
    }
    public Task<LicenseServer.License[]> GetLicensesAsync()
    {
        return Task.FromResult(Enumerable.Range(1, 5).Select(index => new LicenseServer.License
        {
            Identifier = index.ToString(),
            RentalInformation = new LicenseServer.RentalInformation() { Renter = $"Client{index}", Status = LicenseServer.LicenseStatus.Rented, RentedTime = DateTime.Now, RentExpirationTime = DateTime.Now.AddSeconds(15) }
        }).ToArray());
    }
}
