namespace LicenseClient.Data;

using LicenseServer;

public interface ILicenseHttpClient
{
    Task<IEnumerable<License>> GetAllLicenses();

    Task<HttpResponseMessage> AddLicense(string licenseName);

    Task<HttpResponseMessage> RentLicense(string clientName);

}
