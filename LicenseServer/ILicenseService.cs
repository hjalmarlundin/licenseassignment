namespace LicenseServer;

public interface ILicenseService
{
    Task AddLicense(string licenseName);
    IEnumerable<License> ListAllLicenses();
    Task<License> RentLicenseAsync(string renter);
}
